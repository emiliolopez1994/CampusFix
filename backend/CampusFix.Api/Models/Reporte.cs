namespace CampusFix.Api.Models;

public class Reporte
{
    public int Id { get; set; }

    public string Ubicacion { get; set; } = string.Empty;

    public string Problema { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public PrioridadReporte Prioridad { get; set; }

    public EstadoReporte Estado { get; set; } = EstadoReporte.Reportado;

    public int ReportadoPorId { get; set; }

    public Usuario? ReportadoPor { get; set; }

    public DateTime FechaReporte { get; set; } = DateTime.UtcNow;

    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    public ICollection<HistorialEstado> HistorialEstados { get; set; }
        = new List<HistorialEstado>();
}
