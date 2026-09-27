# Bruno ↔ Swagger sync

The Bruno collection is a **generated artifact of the OpenAPI document**, and the pipeline
generates it. You do not import anything, and you do not run any script by hand.

## The chain

```
Stock Exchange/**            you change a controller, a DTO, or an XML comment
        │
        ▼  push / pull request
.github/workflows/openapi-bruno-sync.yml
        │
        ├─ 1. builds and boots the API in the runner, downloads /swagger/v1/swagger.json
        ├─ 2. writes openapi/v1.json            (scripts/Update-OpenApi.ps1)
        ├─ 3. regenerates bruno/StockExchange  (scripts/Update-BrunoCollection.ps1)
        └─ 4. commits both back to the branch
               "chore(api): sync OpenAPI document and Bruno collection [skip ci]"
        │
        ▼
bruno/StockExchange/*.bru     plain files in git — you open the folder, nothing to publish
```

Both artifacts are committed on purpose: the diff of a bot commit is the reviewed record of
exactly which endpoints a change added, removed, or altered. `[skip ci]` keeps that commit from
starting another run.

**No API keys, no cloud workspace, no Postman plan, no manual step.** Bruno stores a collection
as plain `.bru` files, so the repository *is* the distribution channel.

## What you do

1. Open the Bruno app.
2. **Open Collection** → pick `bruno/StockExchange` from this repository.
3. Choose the `local` or `test` environment.
4. `git pull` whenever you want the latest API. That is the entire update procedure.

Do **not** import the collection into the Bruno app from `swagger.json`. An imported copy lives in
the app's local storage, is not tracked by git, and is never refreshed by the pipeline — it will
silently rot. The pipeline can only update the copy in the repository.

### Environments

| Environment | `baseUrl` | Used for |
|---|---|---|
| `local` | `https://localhost:44308` | IIS Express (`Properties/launchSettings.json`, `iisSettings.sslPort`) |
| `test` | `https://stock-exchange.runasp.net` | The deployed MonsterASP site |

`token` is empty in git and stays empty. Sign in, then paste the access token from the response
into `token`, or keep it in the Bruno secrets store. The collection uses bearer auth with
`{{token}}` and every request inherits it, so no request file ever needs editing.

The pipeline never overwrites your environment files: `Update-BrunoCollection.ps1` carries
`bruno/StockExchange/environments/*.bru` across each rebuild untouched, so a personal `baseUrl` or
a locally stored token survives every regeneration.

### Running the whole collection

```powershell
cd bruno\StockExchange
npx --yes "@usebruno/cli@4.2.0" run . -r --env local
```

`run . -r` must be executed from the collection root, and the path must be `.` — passing
`bruno\StockExchange` fails with *"You can run only at the root of a collection"*.

The collection contains no assertions, so a run reports a pass for any response the server
returned, including `400` and `401`. The status line next to each request is the real result.

## Why the document is built in the runner

The workflow starts the API and reads `/swagger/v1/swagger.json` from the code it just built,
rather than downloading the document from the deployed site. Two reasons:

* **No lag.** A deployed-site document describes whatever was last deployed, so a collection
  generated from it can be several commits behind the branch — a route that was commented out
  keeps showing up as a request that answers `404`.
* **No configuration needed.** `Program.cs:51-53` loads every `appsettings*.json` as optional, and
  the whole tree of them is git-ignored. The API boots and serves the document with no
  configuration and no secrets at all, so the job needs nothing from the repository secrets. The
  generated document is byte-identical to one produced with a full local configuration.

The old post-deploy job in `deploy-stock-exchange.yml` did the deployed-site download; it is gone,
and the deploy workflow now only builds, deploys, and smoke-tests.

## Hand edits to the collection are lost

`Update-BrunoCollection.ps1` deletes and recreates `bruno/StockExchange` on every run. That is
what makes the output deterministic, so the pipeline's commit is empty when nothing changed. It
means a request you edited by hand is overwritten by the next push.

If a request is wrong, change the source — the controller action, the DTO, or the XML comment — and
push. Never patch the `.bru` file.

Two things survive a rebuild, and only these two:

* `collection.bru` — the name comes from the document's `info.title`, and the script re-applies the
  collection-wide bearer auth.
* `environments/*.bru` — carried across untouched.

## Troubleshooting

**The collection is missing an endpoint that exists in the code**
Check the `OpenAPI document + Bruno collection` run on the push. The usual causes are a missing
`[ApiVersion("1.0")]` on the action, or a commented-out route attribute — `openapi/v1.json` never
described it, so the importer had nothing to generate. The script prints both counts and fails if
they disagree, so a mismatch shows up in the job log rather than as a quiet omission.

**The collection is out of date after `git pull`**
`git status`. If `openapi/v1.json` or `bruno/` shows local modifications, someone edited generated
files; discard them with `git checkout -- openapi bruno` and pull again. If the tree is clean, the
sync job has not finished yet — check its run on the latest commit.

**A request returns `404` for a route the collection has**
The deployed site is older than your checkout, or the local server is running a build that predates
the change. Restart or redeploy. The collection is always correct for the branch; the server is not
necessarily current.

**Every request fails with a connection error**
The environment's `baseUrl` does not match the port the API is listening on. `local` targets IIS
Express on `https://localhost:44308`; the `dotnet run` profiles bind different ports
(`http` → 5074, `https` → 7059 + 5074, IIS Express → 32032 + 44308).

**Recovering locally**

Only needed to reproduce or repair a pipeline run by hand; it is never part of the normal flow.

```powershell
# Refresh the document from the API built in this working tree.
.\scripts\Update-OpenApi.ps1 -Launch
# Rebuild the collection from it.
.\scripts\Update-BrunoCollection.ps1
```

`Update-OpenApi.ps1 -Launch` starts the API on the `http` profile, waits for the document, and
stops the API again. It works with or without local `appsettings*.json` present.
