using CampusFix.Api.Authorization;
using CampusFix.Api.DTOs;
using CampusFix.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;

    public UsuariosController(
        UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        CancellationToken ct)
    {
        var usuarios = await _userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.Nombre)
            .Select(u => new UsuarioDto(
                u.Id,
                u.Nombre,
                u.Email ?? string.Empty))
            .ToArrayAsync(ct);

        return Ok(usuarios);
    }
}