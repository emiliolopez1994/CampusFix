using CampusFix.Api.DTOs;
using CampusFix.Api.Models;

namespace CampusFix.Api.Mappings;

public static class ReporteMapping
{
    public static ReporteDto ToDto(this Reporte r) =>
        new(
            r.Id,
            r.Problema,
            r.Ubicacion,
            r.Descripcion,
            r.Prioridad,
            r.Estado,
            r.ReportadoPorId,
            r.ReportadoPor?.Nombre ?? "",
            r.FechaReporte,
            r.FechaActualizacion,
            r.Solucion,
            r.FechaSolucion,
            r.HistorialEstados
                .OrderBy(h => h.FechaCambio)
                .Select(h => new HistorialDto(
                    h.Id,
                    h.EstadoAnterior,
                    h.EstadoNuevo,
                    h.FechaCambio))
                .ToArray()
        );

    public static void Aplicar(this Reporte r, ReporteEntrada d)
    {
        r.Problema = d.Problema.Trim();
        r.Ubicacion = d.Ubicacion.Trim();
        r.Descripcion = d.Descripcion?.Trim();
        r.Prioridad = d.Prioridad;
        r.ReportadoPorId = d.ReportadoPorId;
        r.FechaActualizacion = DateTime.UtcNow;
    }
}