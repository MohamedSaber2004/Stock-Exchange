<#
.SYNOPSIS
    Downloads the OpenAPI document served by the Stock Exchange API into a tracked git file.

.DESCRIPTION
    Writes a normalized copy of /swagger/<version>/swagger.json to openapi/<version>.json.
    The file is committed to the repository so that Postman (Connect Repository), Newman
    and any OpenAPI tooling consume the exact same document the API serves.

.EXAMPLE
    .\scripts\Update-OpenApi.ps1 -Launch
    Starts the API on the http launch profile, downloads the document, stops the API.

.EXAMPLE
    .\scripts\Update-OpenApi.ps1 -BaseUrl https://stock-exchange.runasp.net
    Downloads the document from an already running or deployed instance.
#>
[CmdletBinding()]
param(
    [string]$BaseUrl = 'http://localhost:5074',
    [switch]$Launch,
    [string]$Version = 'v1',
    [int]$TimeoutSeconds = 120
)

$ErrorActionPreference = 'Stop'

. (Join-Path $PSScriptRoot 'StableJson.ps1')

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot 'Stock Exchange\Stock-Exchange.API.csproj'
$specUri = "$($BaseUrl.TrimEnd('/'))/swagger/$Version/swagger.json"

function Wait-ForSpec {
    param([string]$Uri, [int]$TimeoutSeconds)

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        try {
            $response = Invoke-WebRequest -Uri $Uri -UseBasicParsing -TimeoutSec 15
            if ($response.StatusCode -eq 200 -and $response.Content -match '"openapi"') {
                return $response.Content
            }
            Write-Host "  $($response.StatusCode) without an OpenAPI document, retrying..."
        } catch {
            Write-Host "  not ready yet ($($_.Exception.Message)), retrying..."
        }
        Start-Sleep -Seconds 3
    }

    throw "Timed out after $TimeoutSeconds seconds waiting for $Uri."
}

$appProcess = $null
$logDir = Join-Path $env:TEMP 'stock-exchange-openapi'
if (-not (Test-Path -LiteralPath $logDir)) {
    New-Item -ItemType Directory -Path $logDir | Out-Null
}

try {
    if ($Launch) {
        Write-Host "Starting API (http launch profile)..."
        $appProcess = Start-Process -FilePath 'dotnet' -WorkingDirectory $repoRoot -PassThru `
            -ArgumentList @(
                'run', '--project', "`"$projectPath`"", '--launch-profile', 'http'
            ) `
            -RedirectStandardOutput (Join-Path $logDir 'api.out.log') `
            -RedirectStandardError (Join-Path $logDir 'api.err.log')
    } else {
        Write-Host "Using running API at $BaseUrl"
    }

    Write-Host "Reading $specUri"
    $content = Wait-ForSpec -Uri $specUri -TimeoutSeconds $TimeoutSeconds

    $tempFile = Join-Path $env:TEMP "openapi-$Version.json"
    [System.IO.File]::WriteAllText($tempFile, $content, (New-Object System.Text.UTF8Encoding($false)))

    & (Join-Path $PSScriptRoot 'Save-OpenApiDocument.ps1') -InputPath $tempFile -Version $Version
} finally {
    if ($appProcess) {
        Write-Host "Stopping API..."
        taskkill /PID $appProcess.Id /T /F | Out-Null
    }
}
