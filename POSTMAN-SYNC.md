# Swagger → Postman sync

Any change to the API surface shows up in the team's Postman collection without anyone
re-importing anything.

## How it works

```
API code → openapi/v1.json → Postman (Connect Repository) → team collection
                ↓
     postman/StockExchange.postman_collection.json  (import fallback + CI drift check)
```

1. `scripts/Update-OpenApi.ps1` starts the API, downloads `/swagger/v1/swagger.json` and writes
   it to the tracked file `openapi/v1.json`.
2. `openapi/v1.json` is committed. Postman's **Connect Repository** watches that file on the
   default branch and rebuilds the collection in the team workspace on every merge.
3. `scripts/Update-PostmanCollection.ps1` converts the same document into
   `postman/StockExchange.postman_collection.json` (via `scripts/Convert-OpenApiToPostman.ps1`),
   for members who do not use the team workspace and for CI.
4. The `OpenAPI / Postman Sync` workflow regenerates both artifacts on every push and pull
   request that touches the API and **fails** if the committed files differ, so the collection
   can never be silently stale.

`postman/StockExchange.postman_collection.json` is a **generated artifact**. Hand edits are
lost on the next regeneration. Edit the collection inside Postman, not in the repository.

## Developer loop

After changing a controller, a DTO, an `[ApiResponse]` shape, or anything under
`Stock Exchange/Swagger`:

```powershell
.\scripts\Update-OpenApi.ps1 -Launch
.\scripts\Update-PostmanCollection.ps1
git add openapi postman
git commit -m "feat: ..."
```

Both scripts are deterministic: running them twice on unchanged code produces no diff.

To regenerate against a running instance or the deployed site instead of launching a new one:

```powershell
.\scripts\Update-OpenApi.ps1 -BaseUrl https://stock-exchange.runasp.net
```

## Setting up the Postman side (one-off, workspace admin)

1. Create a **Team Workspace** (Connect Repository is a Team-plan feature).
2. **APIs → Create API → Connect Repository → GitHub**, authorize
   `MohamedSaber2004/Stock-Exchange`.
3. Branch `master`, file `openapi/v1.json`, folder strategy **Tags**.
4. Add environment variables once: `baseUrl` (e.g. `https://localhost:7059` locally,
   `https://stock-exchange.runasp.net` deployed) and `token` for the JWT. The collection
   inherits bearer auth from `{{token}}`.

Verify the automatic sync: change one `<summary>` in a controller, regenerate, push, then press
**Sync** in Postman. The new text appears without any import.

## Without a Postman team seat

```powershell
git pull
# regenerate only if you changed the API
.\scripts\Update-OpenApi.ps1 -Launch
.\scripts\Update-PostmanCollection.ps1
```

Then in Postman: **Import** → `postman/StockExchange.postman_collection.json`.

## Adding a new API version

The API uses `Asp.Versioning` with documents served at `/swagger/{group}/swagger.json`.

```powershell
.\scripts\Update-OpenApi.ps1 -Launch -Version v2
.\scripts\Update-PostmanCollection.ps1 -Version v2
```

Commit the new `openapi/v2.json` (and `postman/StockExchange.postman_collection_v2.json`,
after renaming the output), then add a second API in the Postman workspace pointing at the new
file. The drift workflow regenerates `v1` only; extend its steps when `v2` ships.

## Troubleshooting

| Symptom | Cause and fix |
| --- | --- |
| `Timed out after N seconds waiting for http://localhost:5074/...` | The API did not start, or another process owns port 5074. Check the log in `%TEMP%\stock-exchange-openapi\api.err.log`, and make sure you are on the `http` launch profile — the `https` and IIS Express profiles bind an HTTPS port, and the HTTPS redirect middleware then answers with a 307. |
| `OpenAPI document not found: ...\openapi\v1.json` | Run `scripts\Update-OpenApi.ps1` first. |
| CI fails with `Generated artifacts are out of date` | Run the two scripts locally and commit `openapi/` and `postman/`. The workflow prints the diff. |
| An endpoint is missing from the collection | The action has no XML `<summary>`/`<remarks>`, so it is named `GET /path`, or the action is missing `[ApiVersion("1.0")]` and is therefore not part of any versioned document. |
| Request bodies contain `"string"` placeholders | Expected: bodies are generated from the JSON schema. Only properties with `example`, `default` or `enum` get real values. |
| `scripts/tests/Convert-OpenApiToPostman.Tests.ps1` fails after editing the converter | The tests assert folder grouping, `$ref` resolution, example generation, bearer auth and byte-identical output. Fix the converter, not the test. |

## Files

| File | Role |
| --- | --- |
| `scripts/Update-OpenApi.ps1` | Downloads the served document into `openapi/<version>.json`. |
| `scripts/Convert-OpenApiToPostman.ps1` | OpenAPI 3 → Postman collection v2.1 converter (no npm dependency). |
| `scripts/Update-PostmanCollection.ps1` | Entry point that converts `openapi/v1.json` into the committed collection. |
| `scripts/tests/Convert-OpenApiToPostman.Tests.ps1` | Converter unit tests, run by CI. |
| `openapi/v1.json` | Generated, committed. The single source of truth Postman syncs from. |
| `postman/StockExchange.postman_collection.json` | Generated, committed. Import fallback. |
| `.github/workflows/openapi-sync.yml` | Regenerates both artifacts in CI and fails on drift. |
