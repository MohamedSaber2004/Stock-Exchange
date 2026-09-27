# Postman ↔ Swagger sync

The team's Postman collection is generated from the API and published by the pipeline. Nobody
imports anything, and nobody runs a script by hand.

## The chain

```
Stock Exchange/**                you change a controller, a DTO, or an XML comment
        │
        ▼  push / pull request
.github/workflows/openapi-postman-sync.yml
        │
        ├─ 1. builds and boots the API in the runner, reads /swagger/v1/swagger.json
        ├─ 2. writes openapi/v1.json                      (scripts/Update-OpenApi.ps1)
        ├─ 3. regenerates postman/StockExchange…json      (scripts/Update-PostmanCollection.ps1)
        ├─ 4. verifies every operation in the document is in the collection
        ├─ 5. commits both back to the branch
        │        "chore(api): sync OpenAPI document and Postman collection [skip ci]"
        └─ 6. PUTs the collection to Postman              (scripts/Publish-PostmanCollection.ps1)
                 and reads it back to prove the publish landed (scripts/Verify-PostmanCollection.ps1)
        │
        ▼
Postman team workspace            the collection your team already uses, updated in place
```

Both artifacts are committed on purpose: the diff of a bot commit is the reviewed record of
exactly which endpoints a change added, removed, or altered. `[skip ci]` keeps that commit from
starting another run.

**No manual import, no API key in your own machine, no re-import after every deploy.** The
collection is *replaced in place* through the Postman API, so its UID never changes and monitors,
mock servers, and published documentation that point at it keep working.

## What you do

1. Add the collection to your Postman workspace **once**, from the team workspace, as normal.
2. Set the two collection variables in that workspace:

   | Variable | Value |
   |---|---|
   | `baseUrl` | `https://stock-exchange.runasp.net` (or `https://localhost:44308` for IIS Express) |
   | `token` | empty; paste the access token from a sign-in response, or use Postman secrets |

3. Work normally. The collection refreshes itself on the next pipeline run.

Do **not** re-import the collection on every deploy, and do not import it into a personal
workspace if you want the automatic updates — the pipeline only knows the collection identified by
`POSTMAN_COLLECTION_ID`. A second copy in another workspace will silently go stale.

### One-time repository setup

The pipeline needs two repository secrets:

| Secret | Where to get it |
|---|---|
| `POSTMAN_API_KEY` | Postman → Settings → API keys → Generate API Key |
| `POSTMAN_COLLECTION_ID` | Open the collection in the team workspace; the UID is in the URL and in the collection's settings |

Optional repository **variable** `SITE_URL` sets the host baked into the generated collection. It
defaults to `https://stock-exchange.runasp.net`.

## Why the document is built in the runner

The workflow starts the API and reads `/swagger/v1/swagger.json` from the code it just built,
rather than downloading the document from the deployed site. Two reasons:

* **No lag.** A deployed-site document describes whatever was last deployed, so a collection
  generated from it can be several commits behind the branch — a route that was commented out keeps
  showing up as a request that answers `404`. This is not hypothetical: the collection generated
  this way kept advertising `/api/v1/authentication/login-with-google` long after the action was
  commented out.
* **No configuration needed.** `Program.cs:51-53` loads every `appsettings*.json` as optional, and
  the whole set of them is git-ignored. The API boots and serves the document with no configuration
  and no secrets at all, so the job needs nothing from the repository secrets. The generated
  document is byte-identical to one produced with a full local configuration.

The deploy workflow therefore only builds, deploys, and smoke-tests; it no longer refreshes the
collection.

## The collection is a generated artifact

`Update-PostmanCollection.ps1` overwrites `postman/StockExchange.postman_collection.json` on every
run, and `Publish-PostmanCollection.ps1` replaces the collection in Postman with that file. So:

* Do not hand-edit requests in Postman — the next publish overwrites them. Change the controller,
  the DTO, or the XML comment and let the pipeline regenerate.
* The **only** things that survive are the collection and environment variables, plus anything
  Postman attaches to the collection rather than storing in it (monitors, mock servers,
  published docs, examples you add at the request level are *not* safe).
* Keep scratch work in a separate collection if you need it.

## Recovering locally

Only needed to reproduce or repair a pipeline run by hand; it is never part of the normal flow.

```powershell
# Refresh the document from the API built in this working tree.
.\scripts\Update-OpenApi.ps1 -Launch
# Rebuild the collection from it.
.\scripts\Update-PostmanCollection.ps1 -BaseUrl https://stock-exchange.runasp.net
# Check that nothing in the document is missing from the collection.
.\scripts\Verify-PostmanCollection.ps1
```

Conversion is deterministic: the same document always produces a byte-identical file, so
regenerating twice leaves the tree clean and a pipeline run only commits when the API surface
really changed.

## Troubleshooting

**The publish step fails with 401 or 403**
`POSTMAN_API_KEY` is wrong, expired, or lacks write access to the collection's workspace. Regenerate
the key in Postman and update the secret. Note the key is a *secret* and is unavailable to pull
requests from forks, which is why the publish steps are skipped there.

**The publish step fails with "collection not found"**
`POSTMAN_COLLECTION_ID` does not match a collection the key can see. Copy the UID from the
collection's own page, not the workspace ID.

**The job fails in the verify step after a successful publish**
The collection in Postman does not match the document this run generated. Usually a manual edit
inside Postman: the publish ran, the verify read back something else. Re-run the workflow.

**The collection is missing an endpoint that exists in the code**
Check the `Sync OpenAPI document and Postman collection` run on the push. The usual causes are a
missing `[ApiVersion("1.0")]` on the action, or a commented-out route attribute — `openapi/v1.json`
never described it, so the converter had nothing to generate. The verify step fails the job when
the counts disagree, so a genuine omission surfaces there rather than passing silently.

**A request returns `404` for a route the collection has**
The server is older than your checkout, or you are running a build that predates the change. The
collection is always correct for the branch; the server is not necessarily current.

**Every request fails with a connection error**
`baseUrl` in the Postman environment does not match the port your API is listening on. IIS Express
binds `https://localhost:44308`; the `dotnet run` profiles bind different ports (`http` → 5074,
`https` → 7059 + 5074, IIS Express → 32032 + 44308).

**The generation step times out waiting for the document**
The API did not start in the runner. The script writes its output to
`<temp>/stock-exchange-openapi/api.out.log` and `api.err.log`; those two files hold the answer.
