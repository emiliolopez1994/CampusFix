using CampusFix.Api.Data;
using CampusFix.Api.DTOs;
using CampusFix.Api.Models;
using CampusFix.Api.Mappings;
using CampusFix.Api.Validators;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Api.Services;

public class ReporteService(CampusFixDbContext db)
{
    private IQueryable<Reporte> Consulta =>
        db.Reportes
            .Include(r => r.ReportadoPor)
            .Include(r => r.HistorialEstados);

    public async Task<ReporteDto[]> Listar(CancellationToken ct) =>
        (await Consulta
            .AsNoTracking()
            .OrderByDescending(r => r.FechaReporte)
            .ToArrayAsync(ct))
        .Select(r => r.ToDto())
        .ToArray();

    public async Task<ReporteDto> Obtener(int id, CancellationToken ct) =>
        (await Consulta
            .AsNoTracking()
            .SingleOrDefaultAsync(r => r.Id == id, ct)
        ?? throw new KeyNotFoundException("Reporte no encontrado."))
        .ToDto();

    public async Task<ReporteDto> Guardar(
        int? id,
        ReporteEntrada d,
        CancellationToken ct)
    {
        ReporteValidator.Validar(d);

        var usuario = await db.Usuarios.FindAsync([d.ReportadoPorId], ct)
            ?? throw new ArgumentException("El reportante no existe.");

        var r = id.HasValue
            ? await Consulta.SingleOrDefaultAsync(r => r.Id == id, ct)
                ?? throw new KeyNotFoundException("Reporte no encontrado.")
            : new Reporte();

        r.Aplicar(d);
        r.ReportadoPor = usuario;

        if (!id.HasValue)
            db.Reportes.Add(r);

        await db.SaveChangesAsync(ct);

        return r.ToDto();
    }

    public async Task<ReporteDto> Cambiar(
        int id,
        EstadoReporte siguiente,
        CancellationToken ct)
    {
        // El bloqueo de fila serializa transiciones simultáneas
        // del mismo reporte.
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var filas = await db.Reportes
            .FromSqlInterpolated(
                $"SELECT * FROM \"Reportes\" WHERE \"Id\" = {id} FOR UPDATE")
            .ToListAsync(ct);

        var r = filas.SingleOrDefault()
            ?? throw new KeyNotFoundException("Reporte no encontrado.");

        ReporteValidator.ValidarTransicion(r.Estado, siguiente);

        db.HistorialEstados.Add(new HistorialEstado
        {
            ReporteId = id,
            EstadoAnterior = r.Estado,
            EstadoNuevo = siguiente
        });

        r.Estado = siguiente;
        r.FechaActualizacion = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return await Obtener(id, ct);
    }

    // Solución documentada:
    // Registra qué acción permitió resolver el incidente.
    // Esta información podrá reutilizarse posteriormente
    // en CampusFix Knowledge.
    public async Task<ReporteDto> RegistrarSolucion(
        int id,
        SolucionReporteEntrada d,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(d.Solucion))
            throw new ArgumentException("La solución es obligatoria.");

        var r = await Consulta
            .SingleOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new KeyNotFoundException("Reporte no encontrado.");

        r.Solucion = d.Solucion.Trim();
        r.FechaSolucion = DateTime.UtcNow;
        r.FechaActualizacion = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return r.ToDto();
    }

    // Soft Delete:
    // El reporte no se elimina físicamente de PostgreSQL.
    // Se marca como archivado y deja de aparecer en las consultas normales.
    public async Task Archivar(int id, CancellationToken ct)
    {
        var r = await db.Reportes
            .SingleOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new KeyNotFoundException("Reporte no encontrado.");

        r.Eliminado = true;
        r.FechaEliminacion = DateTime.UtcNow;
        r.FechaActualizacion = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}