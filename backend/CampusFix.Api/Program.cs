using CampusFix.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar controladores
builder.Services.AddControllers();

// Configuración de OpenAPI
builder.Services.AddOpenApi();

// Obtener la conexión a PostgreSQL
var connectionString =
    builder.Configuration.GetConnectionString("CampusFixConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión CampusFixConnection.");
}

// Configurar Entity Framework Core con PostgreSQL
builder.Services.AddDbContext<CampusFixDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// OpenAPI disponible en desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Habilitar controladores de la API
app.MapControllers();

// Ruta sencilla para comprobar que CampusFix funciona
app.MapGet("/", () => new
{
    aplicacion = "CampusFix API",
    estado = "Backend funcionando"
});

app.Run();