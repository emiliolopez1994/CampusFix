namespace CampusFix.Api.Models;

public class HistorialEstado
{
    public int Id { get; set; }

    public int ReporteId { get; set; }

    public Reporte? Reporte { get; set; }

    public EstadoReporte EstadoAnterior { get; set; }

    public EstadoReporte EstadoNuevo { get; set; }

    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
}
