using CampusFix.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Api.Data;

public class CampusFixDbContext : DbContext
{
    public CampusFixDbContext(DbContextOptions<CampusFixDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Reporte> Reportes => Set<Reporte>();
    public DbSet<HistorialEstado> HistorialEstados => Set<HistorialEstado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.ReportadoPor)
            .WithMany(u => u.Reportes)
            .HasForeignKey(r => r.ReportadoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HistorialEstado>()
            .HasOne(h => h.Reporte)
            .WithMany(r => r.HistorialEstados)
            .HasForeignKey(h => h.ReporteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
