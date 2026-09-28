$ErrorActionPreference = "Stop"
$projectRoot = Split-Path $PSScriptRoot -Parent
Set-Location (Join-Path $projectRoot "frontend/campusfix-web")
npm ci
if ($LASTEXITCODE -ne 0) { throw "Fallo npm ci" }
npm run build
if ($LASTEXITCODE -ne 0) { throw "Fallo la compilacion Angular" }
$webroot = Join-Path $projectRoot "backend/CampusFix.Api/wwwroot"
New-Item -ItemType Directory -Force -Path $webroot | Out-Null
Copy-Item "dist/campusfix-web/browser/*" $webroot -Recurse -Force
Set-Location (Join-Path $projectRoot "backend/CampusFix.Api")
dotnet build
if ($LASTEXITCODE -ne 0) { throw "Fallo la compilacion del backend" }
Write-Host "Listo. Ejecute scripts/Iniciar-Backend.ps1 y abra http://localhost:5137"
