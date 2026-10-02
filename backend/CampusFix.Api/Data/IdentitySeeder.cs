using CampusFix.Api.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CampusFix.Api.Data;

public static class IdentitySeeder
{
    public static async Task InicializarAsync(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<int>>>();

        string[] roles =
        {
            Roles.Usuario,
            Roles.Administrador
        };

        foreach (var rol in roles)
        {
            var existe =
                await roleManager.RoleExistsAsync(rol);

            if (!existe)
            {
                var resultado =
                    await roleManager.CreateAsync(
                        new IdentityRole<int>(rol));

                if (!resultado.Succeeded)
                {
                    var errores = string.Join(
                        " | ",
                        resultado.Errors.Select(
                            e => e.Description));

                    throw new InvalidOperationException(
                        $"No se pudo crear el rol '{rol}'. " +
                        errores);
                }
            }
        }
    }
}