using CampusFix.Api.Data;
using CampusFix.Api.Services;
using CampusFix.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("CampusFixConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Configure ConnectionStrings:CampusFixConnection mediante dotnet user-secrets. Consulte README.md.");
builder.Services.AddDbContext<CampusFixDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<ReporteService>();
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/api/health", async (CampusFixDbContext db) =>
    await db.Database.CanConnectAsync() ? Results.Ok(new { estado = "Conectado", baseDatos = "PostgreSQL" }) : Results.StatusCode(503));
app.MapFallbackToFile("index.html");
app.Run();
