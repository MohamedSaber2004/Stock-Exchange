<#
.SYNOPSIS
    Verifies that a Postman collection covers every operation in the OpenAPI document.

.DESCRIPTION
    Turns "is Postman up to date?" into a check instead of a guess. Every path+method in the
    OpenAPI document is looked up in the collection by method and URL (path parameters are
    compared as {name} on both sides), and any missing operation is reported.

    Two modes:
      (default)     compare the document with the generated file postman\StockExchange.postman_collection.json
      -FromPostman  compare the document with the collection stored in Postman (needs
                    POSTMAN_API_KEY and POSTMAN_COLLECTION_ID)

.EXAMPLE
    .\scripts\Verify-PostmanCollection.ps1
.EXAMPLE
    .\scripts\Verify-PostmanCollection.ps1 -FromPostman
#>
[CmdletBinding()]
param(
    [string]$SpecFile = '',
    [string]$CollectionFile = '',
    [switch]$FromPostman,
    [string]$ApiBaseUrl = 'https://api.getpostman.com'
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
if (-not $SpecFile) { $SpecFile = Join-Path $repoRoot 'openapi\v1.json' }
if (-not $CollectionFile) { $CollectionFile = Join-Path $repoRoot 'postman\StockExchange.postman_collection.json' }

if (-not (Test-Path -LiteralPath $SpecFile)) { throw "OpenAPI document not found: $SpecFile" }

$spec = Get-Content -LiteralPath $SpecFile -Raw | ConvertFrom-Json
$expected = @()
foreach ($pathProperty in $spec.paths.PSObject.Properties) {
    foreach ($property in $pathProperty.Value.PSObject.Properties) {
        $method = $property.Name.ToUpper()
        if ($method -notin @('GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'HEAD', 'OPTIONS', 'TRACE')) { continue }
        $expected += [PSCustomObject]@{
            Key     = "$method $($pathProperty.Name)"
            Summary = [string]$property.Value.summary
        }
    }
}

if ($FromPostman) {
    $apiKey = $env:POSTMAN_API_KEY
    $collectionId = $env:POSTMAN_COLLECTION_ID
    if (-not $apiKey -or -not $collectionId) {
        throw 'POSTMAN_API_KEY and POSTMAN_COLLECTION_ID are required with -FromPostman.'
    }
    Write-Host "Reading collection $collectionId from Postman..."
    $response = Invoke-RestMethod -Method Get -Uri "$ApiBaseUrl/collections/$collectionId" `
        -Headers @{ 'X-Api-Key' = $apiKey }
    $collection = $response.collection
    $source = "Postman collection '$($collection.name)'"
} else {
    if (-not (Test-Path -LiteralPath $CollectionFile)) { throw "Collection file not found: $CollectionFile" }
    $collection = Get-Content -LiteralPath $CollectionFile -Raw | ConvertFrom-Json
    $source = $CollectionFile
}

function Get-Requests {
    param($Items)

    $requests = @()
    foreach ($item in $Items) {
        if ($null -ne $item.PSObject.Properties['item'] -and $item.item) {
            $requests += Get-Requests $item.item
        } elseif ($null -ne $item.PSObject.Properties['request']) {
            $url = [string]$item.request.url.raw
            $url = $url -replace '^\{\{[^}]+\}\}', ''                       # drop the baseUrl variable
            $url = $url -replace '\{\{([^}]+)\}\}', '{$1}'                  # path variables back to {name}
            $requests += [PSCustomObject]@{
                Key      = "$($item.request.method.ToUpper()) $url"
                Summary  = [string]$item.name
                RawUrl   = [string]$item.request.url.raw
            }
        }
    }
    return $requests
}

$actual = Get-Requests $collection.item
Write-Host "$source holds $($actual.Count) requests; the document declares $($expected.Count) operations."

$missing = @($expected | Where-Object { $_.Key -notin $actual.Key })
$unexpected = @($actual | Where-Object { $_.Key -notin $expected.Key })

foreach ($item in $unexpected) {
    Write-Host "note: $source has $($item.Key) which the document does not declare ($($item.Summary))"
}

if ($missing.Count -gt 0) {
    foreach ($item in $missing) {
        Write-Host "::error::missing from ${source}: $($item.Key) - $($item.Summary)"
    }
    throw "$($missing.Count) operation(s) from the OpenAPI document are missing."
}

Write-Host "All $($expected.Count) operations are present."
