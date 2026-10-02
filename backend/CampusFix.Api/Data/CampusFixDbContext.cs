using CampusFix.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Api.Data;

public class CampusFixDbContext
    : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public CampusFixDbContext(DbContextOptions<CampusFixDbContext> options)
        : base(options)
    {
    }

    public DbSet<Reporte> Reportes => Set<Reporte>();

    public DbSet<HistorialEstado> HistorialEstados => Set<HistorialEstado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relación entre Usuario y Reporte.
        // Un usuario puede tener varios reportes.
        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.ReportadoPor)
            .WithMany(u => u.Reportes)
            .HasForeignKey(r => r.ReportadoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft Delete:
        // Los reportes archivados permanecen en PostgreSQL,
        // pero no aparecen en las consultas normales del sistema.
        modelBuilder.Entity<Reporte>()
            .HasQueryFilter(r => !r.Eliminado);

        // Relación entre Reporte e HistorialEstado.
        modelBuilder.Entity<HistorialEstado>()
            .HasOne(h => h.Reporte)
            .WithMany(r => r.HistorialEstados)
            .HasForeignKey(h => h.ReporteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}