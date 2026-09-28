using System.ComponentModel.DataAnnotations;
using CampusFix.Api.Models;

namespace CampusFix.Api.DTOs;

public record ReporteEntrada(
    [Required, StringLength(160)] string Problema,
    [Required, StringLength(120)] string Ubicacion,
    [StringLength(4000)] string? Descripcion,
    PrioridadReporte Prioridad,
    int ReportadoPorId);

public record CambioEstadoEntrada(
    EstadoReporte Estado);

// Se utiliza cuando un incidente es resuelto.
// La solución queda almacenada para consulta y reutilización posterior.
public record SolucionReporteEntrada(
    [Required, StringLength(4000)] string Solucion);

public record UsuarioEntrada(
    [Required, StringLength(120)] string Nombre,
    [Required, EmailAddress, StringLength(254)] string Correo);

public record UsuarioDto(
    int Id,
    string Nombre,
    string Correo);

public record HistorialDto(
    int Id,
    EstadoReporte EstadoAnterior,
    EstadoReporte EstadoNuevo,
    DateTime FechaCambio);

public record ReporteDto(
    int Id,
    string Problema,
    string Ubicacion,
    string? Descripcion,
    PrioridadReporte Prioridad,
    EstadoReporte Estado,
    int ReportadoPorId,
    string ReportadoPor,
    DateTime FechaReporte,
    DateTime FechaActualizacion,
    string? Solucion,
    DateTime? FechaSolucion,
    IEnumerable<HistorialDto> Historial);