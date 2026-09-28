namespace CampusFix.Api.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}
