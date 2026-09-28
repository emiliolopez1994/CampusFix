# Guía académica — CampusFix

## Objetivo y problema
Centralizar incidencias de espacios y equipos institucionales. Sustituye reportes dispersos por registros identificables, consultables y con una secuencia de atención comprobable.

## Arquitectura real
Angular 22 envía solicitudes JSON a controladores ASP.NET Core 10. ReporteService aplica la lógica de negocio y EF Core usa CampusFixDbContext y Npgsql para persistir en PostgreSQL 18. El frontend es una SPA: no necesita renderizado del servidor para este panel local. Se conserva el código SSR original, pero no se activa en la compilación del panel.

## Capas
| Capa | Archivo/carpeta | Responsabilidad |
|---|---|---|
| Páginas | features/reportes/pages | Panel, tabla, tablero, directorio, formularios y detalle |
| Componentes | components/estado-badge.ts | Presentación coherente del estado |
| Servicios Angular | services/reportes.service.ts | Solicitudes HTTP, sin lógica SQL |
| Interfaces | models/reporte.ts | Contratos TypeScript y etiquetas |
| Rutas | app.routes.ts | Carga diferida por vista |
| Controllers | ReportesController, UsuariosController | Entrada HTTP y respuestas |
| Services | ReporteService | Crear, editar, leer y avanzar estado |
| DTOs | ReportesDtos.cs | Contratos separados de entidades EF |
| Validators | ReporteValidator | Campos y regla de transición |
| Mappings | ReporteMapping | Entidad → DTO y actualización de campos |
| Models | Modelos originales | Reporte, Usuario e HistorialEstado |
| Data | CampusFixDbContext | Relaciones, índice único de correo y acceso EF |
| Middlewares | ExceptionMiddleware | Errores de negocio y errores internos |
| Migrations | InitialCreate original | Esquema versionado sin alteraciones |

## Datos conservados
Reporte: Id, Ubicacion, Problema, Descripcion, Prioridad, Estado, ReportadoPorId, FechaReporte, FechaActualizacion. Usuario: Id, Nombre, Correo. HistorialEstado: Id, ReporteId, EstadoAnterior, EstadoNuevo, FechaCambio. Relación 1:N de usuario a reportes y de reporte a cambios. Prioridades: Baja=1, Media=2, Alta=3, Critica=4. Estados: Reportado=1, EnRevision=2, EnReparacion=3, Solucionado=4.

## Regla de negocio
La API solo admite el estado inmediatamente siguiente. Rechaza saltos, retrocesos, repetir el mismo estado y cambios posteriores a Solucionado con HTTP 409. El DTO de creación/edición no acepta un campo para modificar el estado. Cada avance guarda el historial y actualiza la fecha dentro de una transacción; un bloqueo de fila PostgreSQL serializa transiciones concurrentes del mismo reporte. Las fechas se generan en UTC y Angular las presenta en la hora local del navegador.

## API REST
| Método | Ruta | Resultado |
|---|---|---|
| GET | /api/health | 200 si conecta con PostgreSQL; 503 si no |
| GET | /api/reportes | Lista de reportes con reportante e historial |
| GET | /api/reportes/{id} | Detalle; 404 si no existe |
| POST | /api/reportes | Crea en Reportado, devuelve 201 |
| PUT | /api/reportes/{id} | Edita datos sin saltar estados |
| PATCH | /api/reportes/{id}/estado | Avanza secuencialmente |
| GET | /api/usuarios | Directorio |
| POST | /api/usuarios | Registra persona con correo único |
| GET | /openapi/v1.json | Especificación en entorno Development |

Entradas inválidas: 400. Conflictos de regla o base: 409. Fallos imprevistos: 500 con mensaje general; el detalle se registra en el servidor. No se devuelve una entidad EF con ciclos de navegación al frontend.

## Matriz de cumplimiento
| Requisito recibido | Implementación |
|---|---|
| Reportar un problema de una institución | Formulario y POST persistente |
| Aula/ubicación, problema, prioridad, reportante | Campos del modelo original y formulario |
| Cuatro estados en secuencia | Validador, servicio y botón del siguiente paso |
| Evitar salto de Reportado a Solucionado | Validación del backend y pruebas de reglas |
| Angular organizado por features | features/reportes con pages, components, services, models |
| ASP.NET con las capas indicadas | Carpetas con responsabilidades reales |
| PostgreSQL 18 | Npgsql, contexto y migración existentes conservados |
| Historial | Consulta del historial en detalle y transacción por cambio |

Mejoras de usabilidad incorporadas: panel de indicadores calculados, búsqueda, filtros por prioridad/estado, tablero visual, diseño adaptable, directorio, mensajes de error, bloqueo mientras se guarda y estados vacíos.

## Alcance
No se añadieron categorías, técnicos asignados, autenticación, eliminación ni adjuntos: no figuraban como requisitos obligatorios en el material recibido. La búsqueda y filtros se aplican en el navegador sobre el conjunto cargado; para una operación institucional masiva se necesitaría paginación y filtros en servidor. Es un sistema académico local, no un despliegue productivo con roles.

## Conclusión
La implementación conecta las capas del sistema de incidencias y centraliza en el backend la regla principal exigida. El funcionamiento completo con la base de datos del usuario debe confirmarse mediante la prueba de integración documentada, sin confundir compilación con persistencia comprobada.
