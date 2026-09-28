$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "../frontend/campusfix-web")
if (!(Test-Path "node_modules")) {
    npm ci
    if ($LASTEXITCODE -ne 0) { throw "No se pudieron instalar las dependencias." }
}
npm start
