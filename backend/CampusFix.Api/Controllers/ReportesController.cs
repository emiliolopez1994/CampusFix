using CampusFix.Api.DTOs;
using CampusFix.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusFix.Api.Controllers;

[ApiController]
[Route("api/reportes")]
public class ReportesController(ReporteService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await service.Listar(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id, CancellationToken ct) =>
        Ok(await service.Obtener(id, ct));

    [HttpPost]
    public async Task<IActionResult> Crear(
        ReporteEntrada d,
        CancellationToken ct)
    {
        var r = await service.Guardar(null, d, ct);

        return CreatedAtAction(
            nameof(Obtener),
            new { id = r.Id },
            r);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(
        int id,
        ReporteEntrada d,
        CancellationToken ct) =>
        Ok(await service.Guardar(id, d, ct));

    // Registra o actualiza la solución documentada del incidente.
    [HttpPut("{id:int}/solucion")]
    public async Task<IActionResult> RegistrarSolucion(
        int id,
        SolucionReporteEntrada d,
        CancellationToken ct) =>
        Ok(await service.RegistrarSolucion(id, d, ct));

    // Soft Delete:
    // El reporte se archiva, pero permanece almacenado en PostgreSQL.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Archivar(
        int id,
        CancellationToken ct)
    {
        await service.Archivar(id, ct);

        return NoContent();
    }

    [HttpPatch("{id:int}/estado")]
    public async Task<IActionResult> Cambiar(
        int id,
        CambioEstadoEntrada d,
        CancellationToken ct) =>
        Ok(await service.Cambiar(id, d.Estado, ct));
}