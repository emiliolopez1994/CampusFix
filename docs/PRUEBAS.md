# Verificación de la entrega

## Ejecutado en el entorno de preparación

| Comprobación | Resultado |
|---|---|
| Compilación ASP.NET Core 10 | Correcta, 0 errores y 0 advertencias |
| Compilación Angular de producción | Correcta |
| Test Angular de navegación principal | 1 prueba aprobada |
| Validador real C# | 28 casos aprobados más una entrada válida |
| Arranque real del backend | Correcto en Development |
| Presentación con un servidor | HTML, archivos JS/CSS y ruta /reportes servidos correctamente por ASP.NET |
| OpenAPI | HTTP 200, contratos generados |
| POST reporte sin campos obligatorios | HTTP 400 |
| POST persona con correo inválido | HTTP 400 |
| Health con PostgreSQL ausente | HTTP 503, sin informar una conexión ficticia |
| Navegador Chromium | Crear, editar, avanzar los tres estados, historial, filtrar, registrar persona y navegar al tablero: aprobado con API simulada |
| Consola del navegador | Sin errores JavaScript en el recorrido probado |
| Vista adaptable | Revisada en 1440 px y 390 px; tabla con desplazamiento interno |
| SQL | Tablas y migración coinciden con modelos originales; tablas de negocio vacías |

## Límite de estas pruebas

**No se ejecutó la integración contra PostgreSQL 18 real en este entorno.** Las pruebas del navegador usan respuestas simuladas para comprobar la interfaz, no prueban persistencia. La compilación y los 28 casos C# sí se ejecutaron sobre el código entregado. Los scripts PowerShell se entregan para Windows; su ejecución requiere tu equipo.

## Verificación final en el equipo del estudiante

1. Configurar la conexión siguiendo README.md y encender PostgreSQL 18.
2. Iniciar la API y comprobar `/api/health`.
3. Ejecutar `scripts/Prueba-Integracion.ps1`. Debe crear registros QA, rechazar un salto con 409 y guardar tres cambios válidos.
4. Reiniciar el backend y volver a consultar el ID QA indicado por el script. Debe mantener estado Solucionado e historial.
5. En Angular, crear un reporte, editar ubicación, filtrar y avanzar cada estado. Recargar.
6. Enviar dos PATCH simultáneos al mismo reporte y mismo siguiente estado: uno debe avanzar y el otro devolver 409; no debe duplicarse el historial.
7. Revisar la consulta SQL descrita en EVIDENCIAS.md.

No se afirma que estas comprobaciones pendientes hayan sido superadas.

## Repetir las pruebas de navegador

Con Angular iniciado en localhost:4200, desde `tests/browser` ejecuta `npm install`, `npx playwright install chromium` y `npm test`. La prueba intercepta `/api/**`, utiliza personas/reportes sintéticos y guarda capturas en `artifacts/`. Estos registros no se envían a la base. Las capturas sirven para comprobar diseño, no como evidencia de persistencia.
