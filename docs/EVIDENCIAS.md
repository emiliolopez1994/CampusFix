# Evidencias para el informe — por pasos

Estas son instrucciones de captura del código real. Abre CampusFix en VS Code. No muestres contraseñas ni comandos de user-secrets con valores reales.

## 1. Estructura Angular
Expande `frontend/campusfix-web/src/app/features/reportes`. Deben verse `pages`, `components`, `services`, `models`. Abre `app.routes.ts` y muestra las cuatro rutas. Terminal opcional:
```powershell
Get-ChildItem frontend/campusfix-web/src/app/features/reportes -Recurse -File | Select-Object FullName
```

## 2. Capas del backend
Expande `backend/CampusFix.Api` para mostrar Controllers, Services, DTOs, Models, Data, Validators, Mappings, Middlewares y Migrations.

## 3. Conexión PostgreSQL
Abre `backend/CampusFix.Api/Program.cs`. Busca `CampusFixConnection` con Ctrl+F. Captura desde la lectura de la cadena hasta `options.UseNpgsql(connectionString)`; allí se aprecia `AddDbContext`. Terminal para encontrar el fragmento:
```powershell
Select-String -Path backend/CampusFix.Api/Program.cs -Pattern 'CampusFixConnection|AddDbContext|UseNpgsql' -Context 1,2
```

## 4. Modelo
Abre `Models/Reporte.cs`; captura las propiedades Ubicacion, Problema, Prioridad, Estado y ReportadoPorId con la carpeta Models visible. No necesitas mostrar todo el archivo ni buscar un commit diferente.

## 5. Regla principal
Abre `Validators/ReporteValidator.cs` y muestra `ValidarTransicion`. Abre `Services/ReporteService.cs` y muestra `Cambiar`: transacción, bloqueo, validación e inserción de HistorialEstado.

## 6. Interfaz real
Después de conectar PostgreSQL, registra Carlos y el reporte “Proyector no funciona”, Aula 204, prioridad Alta. Captura el listado y detalle. Avanza a En revisión, En reparación y Solucionado, tomando una captura del historial final. No presentes las vistas vacías como evidencia de persistencia.

## 7. Prueba del salto prohibido
Con un reporte nuevo en estado Reportado, cambia la petición PATCH de `CampusFix.Api.http` a estado 4 y captura el HTTP 409 junto al mensaje de secuencia obligatoria. Si usas el archivo HTTP, reemplaza el ID por el del reporte nuevo.

## 8. Persistencia
Recarga el navegador. En pgAdmin consulta:
```sql
SELECT * FROM public."Reportes" ORDER BY "Id" DESC;
SELECT * FROM public."HistorialEstados" ORDER BY "Id" DESC;
```
Captura únicamente el reporte de ejemplo y sus tres cambios. Esta evidencia debe generarse en tu PostgreSQL.

## 9. Git real
```powershell
git log --oneline -5
git status
git diff --stat
```
Los commits originales son 7952965 y de0a384. El código nuevo se entrega sin commit; no atribuyas las nuevas funciones al commit de conexión anterior. Después de revisar y registrar tus cambios, captura el nuevo log.
