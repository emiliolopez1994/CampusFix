using CampusFix.Api.Authorization;
using CampusFix.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace CampusFix.Api.Data;

public static class AdminSeeder
{
    public static async Task InicializarAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var correo = configuration["BootstrapAdmin:Email"];
        var password = configuration["BootstrapAdmin:Password"];
        var nombre = configuration["BootstrapAdmin:Nombre"];

        if (string.IsNullOrWhiteSpace(correo) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        using var scope = services.CreateScope();

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<Usuario>>();

        correo = correo.Trim().ToLowerInvariant();

        var usuario =
            await userManager.FindByEmailAsync(correo);

        if (usuario is null)
        {
            usuario = new Usuario
            {
                Nombre = string.IsNullOrWhiteSpace(nombre)
                    ? "Administrador"
                    : nombre.Trim(),

                Email = correo,
                UserName = correo,
                EmailConfirmed = true
            };

            var resultadoCreacion =
                await userManager.CreateAsync(
                    usuario,
                    password);

            if (!resultadoCreacion.Succeeded)
            {
                var errores = string.Join(
                    " | ",
                    resultadoCreacion.Errors.Select(
                        e => e.Description));

                throw new InvalidOperationException(
                    "No se pudo crear el administrador. " +
                    errores);
            }
        }

        var esAdministrador =
            await userManager.IsInRoleAsync(
                usuario,
                Roles.Administrador);

        if (!esAdministrador)
        {
            var resultadoRol =
                await userManager.AddToRoleAsync(
                    usuario,
                    Roles.Administrador);

            if (!resultadoRol.Succeeded)
            {
                var errores = string.Join(
                    " | ",
                    resultadoRol.Errors.Select(
                        e => e.Description));

                throw new InvalidOperationException(
                    "No se pudo asignar el rol Administrador. " +
                    errores);
            }
        }
    }
}