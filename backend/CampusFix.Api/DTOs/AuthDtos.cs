namespace CampusFix.Api.DTOs;

public record RegistroRequest(
    string Nombre,
    string Correo,
    string Password
);

public record LoginRequest(
    string Correo,
    string Password
);

public record AuthResponse(
    int Id,
    string Nombre,
    string Correo,
    string[] Roles,
    string Token
);