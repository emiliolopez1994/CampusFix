using Microsoft.AspNetCore.Identity;

namespace CampusFix.Api.Models;

public class Usuario : IdentityUser<int>
{
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Reporte> Reportes { get; set; }
        = new List<Reporte>();
}