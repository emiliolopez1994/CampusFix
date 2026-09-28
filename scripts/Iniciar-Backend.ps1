$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "../backend/CampusFix.Api")
dotnet run --launch-profile http
if ($LASTEXITCODE -ne 0) { throw "No se pudo iniciar el backend. Revise la conexion y el SDK .NET 10." }
