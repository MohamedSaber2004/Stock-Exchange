<#
.SYNOPSIS
    Generates the Bruno collection from the OpenAPI document.

.DESCRIPTION
    Replaces the Postman publishing step with a Bruno collection that lives in the repository
    as plain .bru files. No API key, no cloud workspace, no paid plan: the collection is
    generated from openapi/v1.json, committed, and every team member opens it from the repo.

    The collection is generated with the official importer
    (@usebruno/cli, pinned so an upgrade cannot silently rewrite every request file), so the
    output is byte-identical for the same document and the deploy job only produces a commit
    when the API surface really changed.

    Two things the importer does not do, so the script handles them:
      * the collection auth is switched from "none" to bearer {{token}} for the whole collection
      * environment files are created on first run and then left untouched

.EXAMPLE
    .\scripts\Update-BrunoCollection.ps1
#>
[CmdletBinding()]
param(
    [string]$SpecFile = '',
    [string]$OutputRoot = '',
    [string]$CollectionName = 'StockExchange',
    [string]$LocalBaseUrl = 'https://localhost:44308',
    [string]$TestBaseUrl = 'https://stock-exchange.runasp.net',
    [string]$CliVersion = '4.2.0'
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
if (-not $SpecFile) { $SpecFile = Join-Path $repoRoot 'openapi\v1.json' }
if (-not $OutputRoot) { $OutputRoot = Join-Path $repoRoot 'bruno' }

if (-not (Test-Path -LiteralPath $SpecFile)) {
    throw "OpenAPI document not found: $SpecFile. Run .\scripts\Save-OpenApiDocument.ps1 first."
}

$spec = [System.IO.File]::ReadAllText($SpecFile, [System.Text.Encoding]::UTF8) | ConvertFrom-Json

$operations = 0
foreach ($pathProperty in $spec.paths.PSObject.Properties) {
    foreach ($property in $pathProperty.Value.PSObject.Properties) {
        if ($property.Name.ToUpper() -in @('GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'HEAD', 'OPTIONS', 'TRACE')) {
            $operations++
        }
    }
}
Write-Host "The document declares $operations operation(s)."

# The importer refuses a non-empty output directory, and deleted endpoints would otherwise be
# left behind as stale .bru files, so the generated collection is always rebuilt from scratch.
# Environment files are hand-owned, so they are carried across the rebuild untouched.
$collectionDir = Join-Path $OutputRoot $CollectionName
$envDir = Join-Path $collectionDir 'environments'
$preservedEnv = @{}
if (Test-Path -LiteralPath $envDir) {
    foreach ($file in Get-ChildItem -LiteralPath $envDir -Filter '*.bru' -File) {
        $preservedEnv[$file.Name] = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    }
}

if (Test-Path -LiteralPath $collectionDir) {
    Remove-Item -LiteralPath $collectionDir -Recurse -Force
}
if (-not (Test-Path -LiteralPath $OutputRoot)) {
    New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null
}

Write-Host "Importing $SpecFile into $collectionDir (Bruno CLI $CliVersion)..."
& npx --yes "@usebruno/cli@$CliVersion" import openapi `
    --source $SpecFile `
    --output $OutputRoot `
    --collection-name $CollectionName `
    --collection-format bru `
    --group-by tags

if ($LASTEXITCODE -ne 0) {
    throw "The Bruno importer failed with exit code $LASTEXITCODE."
}

# Whole-collection bearer auth: the importer leaves it as "none" because the document describes
# the scheme per operation. Requests inherit it, so no per-request edits are ever needed.
$collectionFile = Join-Path $collectionDir 'collection.bru'
$collectionText = [System.IO.File]::ReadAllText($collectionFile, [System.Text.Encoding]::UTF8)
$bearerAuth = "auth {`n  mode: bearer`n}`n`nauth:bearer {`n  token: {{token}}`n}"
if ($collectionText -match '(?s)auth \{.*?\}') {
    $collectionText = [regex]::Replace($collectionText, '(?s)auth \{.*?\}', $bearerAuth, 1)
} else {
    $collectionText = $bearerAuth + "`n`n" + $collectionText
}
[System.IO.File]::WriteAllText($collectionFile, $collectionText, (New-Object System.Text.UTF8Encoding($false)))

# Environments are created once and then left alone, so a developer can keep a personal
# environment file with their own baseUrl and a locally stored secret token without the next
# deploy overwriting it.
$environments = @{
    'test.bru'  = "vars {`n  baseUrl: $TestBaseUrl`n  token:`n}`n"
    'local.bru' = "vars {`n  baseUrl: $LocalBaseUrl`n  token:`n}`n"
}
foreach ($name in $preservedEnv.Keys) { $environments[$name] = $preservedEnv[$name] }

foreach ($name in ($environments.Keys | Sort-Object)) {
    $path = Join-Path $envDir $name
    $existed = $preservedEnv.ContainsKey($name)
    if (-not (Test-Path -LiteralPath $envDir)) { New-Item -ItemType Directory -Path $envDir -Force | Out-Null }
    [System.IO.File]::WriteAllText($path, $environments[$name], (New-Object System.Text.UTF8Encoding($false)))
    $state = if ($existed) { 'kept' } else { 'created' }
    Write-Host "$state bruno\$CollectionName\environments\$name"
}

$requestFiles = @(Get-ChildItem -LiteralPath $collectionDir -Recurse -Filter '*.bru' -File |
    Where-Object { $_.Name -notin @('folder.bru', 'collection.bru') -and $_.Directory.Name -ne 'environments' })
Write-Host "Generated $($requestFiles.Count) request file(s) for $operations operation(s)."

if ($requestFiles.Count -ne $operations) {
    foreach ($file in $requestFiles) { Write-Host "  $($file.FullName.Substring($collectionDir.Length + 1))" }
    throw "The Bruno collection has $($requestFiles.Count) request(s) but the document declares $operations operation(s)."
}

Write-Host "Open bruno\$CollectionName in the Bruno app, select the 'test' environment, and run the collection."
