using System.Text;
using CampusFix.Api.Configuration;
using CampusFix.Api.Data;
using CampusFix.Api.Middlewares;
using CampusFix.Api.Models;
using CampusFix.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString(
        "CampusFixConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Configure ConnectionStrings:CampusFixConnection " +
        "mediante dotnet user-secrets.");
}

builder.Services.AddDbContext<CampusFixDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddIdentity<Usuario, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredUniqueChars = 1;

        options.User.RequireUniqueEmail = true;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<CampusFixDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(
        JwtSettings.SectionName));

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "La clave JWT no está configurada. " +
        "Configure Jwt:Key mediante dotnet user-secrets.");
}

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? "CampusFix.Api";

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? "CampusFix.Angular";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ReporteService>();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet(
    "/api/health",
    async (CampusFixDbContext db) =>
        await db.Database.CanConnectAsync()
            ? Results.Ok(new
            {
                estado = "Conectado",
                baseDatos = "PostgreSQL"
            })
            : Results.StatusCode(503));

app.MapFallbackToFile("index.html");

// Crear los roles necesarios de Identity.
// No asigna el rol Administrador a usuarios normales.
await IdentitySeeder.InicializarAsync(app.Services);

// Bootstrap controlado del primer administrador.
// Solo se ejecuta si BootstrapAdmin está configurado
// mediante user-secrets.
await AdminSeeder.InicializarAsync(
    app.Services,
    app.Configuration);

app.Run();