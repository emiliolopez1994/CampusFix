using CampusFix.Api.Data;
using CampusFix.Api.DTOs;
using CampusFix.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CampusFix.Api.Controllers;
[ApiController, Route("api/usuarios")]
public class UsuariosController(CampusFixDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Listar(CancellationToken ct) => Ok(await db.Usuarios.AsNoTracking().OrderBy(u => u.Nombre).Select(u => new UsuarioDto(u.Id, u.Nombre, u.Correo)).ToArrayAsync(ct));
    [HttpPost] public async Task<IActionResult> Crear(UsuarioEntrada d, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(d.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
        var correo = d.Correo.Trim().ToLowerInvariant();
        if (await db.Usuarios.AnyAsync(u => u.Correo.ToLower() == correo, ct))
            throw new InvalidOperationException("Ya existe una persona con ese correo.");
        var u = new Usuario { Nombre = d.Nombre.Trim(), Correo = correo };
        db.Usuarios.Add(u); await db.SaveChangesAsync(ct);
        return StatusCode(201, new UsuarioDto(u.Id, u.Nombre, u.Correo));
    }
}
