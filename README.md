# CampusFix — Sistema de gestión de incidencias

Aplicación académica basada en el proyecto original: **Angular 22 + ASP.NET Core 10 + Entity Framework Core + PostgreSQL 18**. Conserva los modelos, tablas, migración inicial e historial Git recibidos.

## 1. Abrir el proyecto

Extrae el ZIP en una carpeta nueva (no encima de tu copia anterior). Abre la carpeta `CampusFix` en VS Code. Abre Terminal → Nueva terminal. Los comandos siguientes son para PowerShell y parten de esa carpeta.

Requisitos: SDK .NET 10, Node.js 24 y npm, PostgreSQL 18 encendido. Comprueba:

```powershell
dotnet --version
node --version
npm --version
```

## 2. Configurar TU conexión

Si tu base original ya existe, **úsala directamente**. No vuelvas a importar el SQL encima de una base con tablas.

Sustituye `CampusFix`, `postgres` y `TU_CLAVE` por el nombre real de tu base, usuario y contraseña. El puerto habitual es 5432.

```powershell
cd backend/CampusFix.Api
dotnet user-secrets set "ConnectionStrings:CampusFixConnection" "Host=localhost;Port=5432;Database=CampusFix;Username=postgres;Password=TU_CLAVE"
cd ../..
```

La configuración queda fuera del repositorio. El proyecto conserva el UserSecretsId original, por lo que una conexión ya configurada en tu PC puede seguir disponible. Si la contraseña contiene caracteres especiales de PowerShell, usa comillas simples alrededor de la cadena y duplica las comillas simples internas.

### Solo si necesitas una base NUEVA

Elige **una** opción:

**A. Restaurar el SQL entregado:** crea una base vacía en pgAdmin y ejecuta desde PowerShell, cambiando el nombre de base si corresponde:

```powershell
& "C:/Program Files/PostgreSQL/18/bin/psql.exe" -U postgres -d CampusFix -v ON_ERROR_STOP=1 -f "database/CampusFix.sql"
```

El archivo es un dump Plain de PostgreSQL 18.4, incluye instrucciones de psql como COPY y `\restrict`; se restaura con **psql**, no pegándolo entero en Query Tool. Su propietario original es `postgres`. Incluye las tablas y la migración inicial, pero no contiene personas ni reportes.

**B. Crear las tablas mediante la migración existente:** con la conexión del paso 2 configurada y un usuario autorizado para crear la base/tablas:

```powershell
dotnet tool restore
dotnet ef database update --project backend/CampusFix.Api
```

No se modificó el esquema; no necesitas una nueva migración para esta entrega.

## 3. Iniciar el backend

En una terminal desde `CampusFix`:

```powershell
cd backend/CampusFix.Api
dotnet run --launch-profile http
```

Abre `http://localhost:5137/api/health`. Debe mostrar `Conectado`. Si devuelve 503, revisa PostgreSQL, el nombre de la base y las credenciales.

El ZIP incluye también la interfaz ya compilada: al iniciar el backend puedes abrir **http://localhost:5137** directamente. Para desarrollar/modificar Angular, continúa con el paso 4.

## 4. Iniciar Angular

Deja la terminal del backend abierta. En **otra terminal**, desde `CampusFix`:

```powershell
cd frontend/campusfix-web
npm ci
npm start
```

Abre **http://localhost:4200**. El proxy Angular envía `/api` al backend en el puerto 5137. No necesitas cambiar CORS ni URLs en el servicio.

## 5. Primer uso

1. Entra en **Personas** y registra al reportante (ejemplo: Carlos y su correo).
2. Pulsa **Nuevo reporte**. Ingresa el problema, ubicación, prioridad y reportante; descripción opcional.
3. Abre el detalle y avanza con el botón del siguiente estado.
4. Comprueba: **Reportado → En revisión → En reparación → Solucionado**.
5. Usa Reportes para buscar y filtrar; Tablero de estados para ver la distribución.
6. Edita los datos desde el detalle. El estado solo cambia mediante su acción dedicada.
7. Recarga la página para comprobar que los registros permanecen en PostgreSQL.

No hay datos ficticios insertados al iniciar. Registrar una persona no crea un login. La autenticación, asignación a técnicos y categorías no fueron requisitos del enunciado recibido y no se añadieron como requisitos inventados. La aplicación está preparada para demostración local; no dispone de control de acceso para exposición pública.

## 6. Presentación con un solo servidor

Desde `CampusFix`:

```powershell
./scripts/Preparar-Presentacion.ps1
./scripts/Iniciar-Backend.ps1
```

Luego abre **http://localhost:5137**. El primer script compila Angular y copia la salida a `wwwroot`, que sirve ASP.NET Core. No requiere dejar `ng serve` ejecutándose. Si tu equipo bloquea scripts PowerShell, ejecuta manualmente los comandos de los pasos 3–4; no necesitas cambiar políticas del sistema.

## 7. Pruebas

```powershell
dotnet build backend/CampusFix.Api
dotnet run --project tests/CampusFix.Rules.Tests
cd frontend/campusfix-web
npm test -- --watch=false
npm run build
cd ../..
```

Con el backend y PostgreSQL funcionando:

```powershell
./scripts/Prueba-Integracion.ps1
```

Esta última prueba **crea una persona y un reporte QA**, comprueba el rechazo del salto a Solucionado y el historial de tres transiciones. Conserva esos registros para poder inspeccionarlos.

Consulta `docs/PRUEBAS.md` para distinguir las comprobaciones ejecutadas aquí de las pendientes en tu PostgreSQL.

## 8. Estructura y documentación

- `frontend/campusfix-web/src/app/features/reportes/`: pages, components, services y models.
- `backend/CampusFix.Api/`: Controllers, Services, DTOs, Models, Data, Validators, Mappings, Middlewares y Migrations.
- `database/CampusFix.sql`: copia sin cambios del SQL recibido.
- `docs/GUIA-ACADEMICA.md`: arquitectura, API, requisitos y alcance.
- `docs/EVIDENCIAS.md`: capturas, archivos y comandos exactos.
- `docs/PRUEBAS.md`: resultado y protocolo de validación.
- `scripts/`: arranque, presentación y prueba de integración.

## 9. Git

Se conserva `.git` y los commits originales `7952965` y `de0a384`. Los cambios de esta entrega se dejan **sin commit** para que puedas revisarlos antes de registrarlos; no se ha realizado push a GitHub.

```powershell
git status
git diff --stat
git diff --ignore-space-at-eol
```

Propuestas, cuando hayas revisado los cambios:

- `feat: implementar API de reportes y flujo de estados`
- `feat: crear interfaz Angular para gestion de incidencias`
- `docs: agregar arranque pruebas y evidencias academicas`

No confundir estas propuestas con commits ya realizados.

El ZIP original ya mostraba diferencias de fin de línea en archivos sin cambios de contenido. Usa `git diff --ignore-space-at-eol` para separar esos cambios de formato de la implementación nueva.
