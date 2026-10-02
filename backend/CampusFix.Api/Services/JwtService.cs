using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CampusFix.Api.Configuration;
using CampusFix.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CampusFix.Api.Services;

public class JwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<Usuario> _userManager;

    public JwtService(
        IOptions<JwtSettings> jwtOptions,
        UserManager<Usuario> userManager)
    {
        _jwtSettings = jwtOptions.Value;
        _userManager = userManager;
    }

    public async Task<string> GenerarTokenAsync(Usuario usuario)
    {
        var roles = await _userManager.GetRolesAsync(usuario);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                usuario.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                usuario.Email ?? string.Empty),

            new(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new(
                ClaimTypes.Name,
                usuario.Nombre),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        foreach (var rol in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}