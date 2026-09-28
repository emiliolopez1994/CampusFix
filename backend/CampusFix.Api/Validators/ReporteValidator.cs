using CampusFix.Api.DTOs;
using CampusFix.Api.Models;
namespace CampusFix.Api.Validators;
public static class ReporteValidator
{
    public static void Validar(ReporteEntrada d)
    {
        if (string.IsNullOrWhiteSpace(d.Problema) || string.IsNullOrWhiteSpace(d.Ubicacion))
            throw new ArgumentException("El problema y la ubicación son obligatorios.");
        if (!Enum.IsDefined(d.Prioridad) || d.ReportadoPorId <= 0)
            throw new ArgumentException("Seleccione una prioridad y un reportante válidos.");
    }
    public static void ValidarTransicion(EstadoReporte actual, EstadoReporte siguiente)
    {
        if (!Enum.IsDefined(siguiente) || actual == EstadoReporte.Solucionado || (int)siguiente != (int)actual + 1)
            throw new InvalidOperationException("Solo puede avanzar al siguiente estado: Reportado → En revisión → En reparación → Solucionado.");
    }
}
