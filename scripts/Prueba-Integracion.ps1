param([string]$BaseUrl = "http://localhost:5137")
$ErrorActionPreference = "Stop"
# Crea registros identificables de prueba; no borra datos existentes.
function Send-Json($Method, $Path, $Body) {
    Invoke-RestMethod -Method $Method -Uri "$BaseUrl$Path" -ContentType "application/json; charset=utf-8" -Body ([Text.Encoding]::UTF8.GetBytes(($Body | ConvertTo-Json)))
}
$health = Invoke-RestMethod "$BaseUrl/api/health"
if ($health.estado -ne "Conectado") { throw "PostgreSQL no esta disponible" }
$stamp = [Guid]::NewGuid().ToString("N").Substring(0,12)
$person = Send-Json POST "/api/usuarios" @{nombre="Prueba QA $stamp";correo="qa-$stamp@example.test"}
$ticket = Send-Json POST "/api/reportes" @{problema="QA - Proyector no funciona";ubicacion="Aula QA";descripcion="Registro creado por la prueba de integracion";prioridad=3;reportadoPorId=$person.id}
if ($ticket.estado -ne 1) { throw "El reporte no inicio en Reportado" }
try {
    Send-Json PATCH "/api/reportes/$($ticket.id)/estado" @{estado=4} | Out-Null
    throw "Se permitio saltar estados"
} catch {
    if (!$_.Exception.Response -or [int]$_.Exception.Response.StatusCode -ne 409) { throw }
}
foreach ($next in 2,3,4) {
    $ticket = Send-Json PATCH "/api/reportes/$($ticket.id)/estado" @{estado=$next}
    if ($ticket.estado -ne $next) { throw "Transicion incorrecta" }
}
$stored = Invoke-RestMethod "$BaseUrl/api/reportes/$($ticket.id)"
if ($stored.estado -ne 4 -or $stored.historial.Count -ne 3) { throw "Historial o persistencia incorrectos" }
Write-Host "PASS: PostgreSQL, creacion, rechazo de salto, tres transiciones y lectura del historial. Reporte QA: $($ticket.id)"
