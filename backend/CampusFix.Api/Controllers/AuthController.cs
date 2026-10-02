using CampusFix.Api.Authorization;
using CampusFix.Api.DTOs;
using CampusFix.Api.Models;
using CampusFix.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CampusFix.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly JwtService _jwtService;

    public AuthController(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        JwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    // =====================================================
    // REGISTRO
    // POST /api/auth/registro
    // =====================================================

    [HttpPost("registro")]
    public async Task<IActionResult> Registro(
        RegistroRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new
            {
                mensaje = "El nombre es obligatorio."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Correo))
        {
            return BadRequest(new
            {
                mensaje = "El correo es obligatorio."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                mensaje = "La contraseña es obligatoria."
            });
        }

        var correo = request.Correo
            .Trim()
            .ToLowerInvariant();

        var usuarioExistente =
            await _userManager.FindByEmailAsync(correo);

        if (usuarioExistente is not null)
        {
            return Conflict(new
            {
                mensaje = "Ya existe un usuario con ese correo."
            });
        }

        var usuario = new Usuario
        {
            Nombre = request.Nombre.Trim(),
            Email = correo,
            UserName = correo
        };

        // Identity genera automáticamente el PasswordHash.
        // La contraseña nunca se guarda en texto plano.
        var resultado =
            await _userManager.CreateAsync(
                usuario,
                request.Password);

        if (!resultado.Succeeded)
        {
            return BadRequest(new
            {
                mensaje = "No se pudo crear el usuario.",
                errores = resultado.Errors
                    .Select(e => e.Description)
                    .ToArray()
            });
        }

        // IMPORTANTE:
        // Todo registro público recibe SIEMPRE
        // el rol Usuario.
        var resultadoRol =
            await _userManager.AddToRoleAsync(
                usuario,
                Roles.Usuario);

        if (!resultadoRol.Succeeded)
        {
            // Evitamos dejar un usuario creado sin rol.
            await _userManager.DeleteAsync(usuario);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    mensaje =
                        "No se pudo asignar el rol al usuario."
                });
        }

        var roles =
            await _userManager.GetRolesAsync(usuario);

        var token =
            await _jwtService.GenerarTokenAsync(usuario);

        return StatusCode(
            StatusCodes.Status201Created,
            new AuthResponse(
                usuario.Id,
                usuario.Nombre,
                usuario.Email ?? string.Empty,
                roles.ToArray(),
                token));
    }


    // =====================================================
    // LOGIN
    // POST /api/auth/login
    // =====================================================

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Correo) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        var correo = request.Correo
            .Trim()
            .ToLowerInvariant();

        var usuario =
            await _userManager.FindByEmailAsync(correo);

        // No revelamos si el correo existe o no.
        if (usuario is null)
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        // PasswordSignInAsync verifica el PasswordHash.
        // lockoutOnFailure = true hace que los intentos
        // incorrectos cuenten para el bloqueo configurado.
        var resultado =
            await _signInManager.CheckPasswordSignInAsync(
                usuario,
                request.Password,
                lockoutOnFailure: true);

        if (resultado.IsLockedOut)
        {
            return StatusCode(
                StatusCodes.Status423Locked,
                new
                {
                    mensaje =
                        "La cuenta está temporalmente bloqueada " +
                        "por varios intentos fallidos."
                });
        }

        if (!resultado.Succeeded)
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        var roles =
            await _userManager.GetRolesAsync(usuario);

        var token =
            await _jwtService.GenerarTokenAsync(usuario);

        return Ok(
            new AuthResponse(
                usuario.Id,
                usuario.Nombre,
                usuario.Email ?? string.Empty,
                roles.ToArray(),
                token));
    }
}