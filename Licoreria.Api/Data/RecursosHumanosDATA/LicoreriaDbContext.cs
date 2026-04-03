using Licoreria.Api.Entities.RecursosHumanos.EmpleadoEntities;
using Licoreria.Api.Entities.RecursosHumanos.UsuarioEntities;
using Microsoft.EntityFrameworkCore;
using Licoreria.Api.Entities.Sucursales;
using Licoreria.Api.Entities.Clientes;

namespace Licoreria.Api.Data.RecursosHumanosDATA
{
    public class LicoreriaDbContext : DbContext
    {
        public LicoreriaDbContext(DbContextOptions<LicoreriaDbContext> options)
            : base(options) { }

        public DbSet<Empleado> Empleados => Set<Empleado>();
        public DbSet<GestionUsuario> GestionUsuario => Set<GestionUsuario>();
        public DbSet<GestionSeguridadPerfile> GestionSeguridadPerfile => Set<GestionSeguridadPerfile>();
        public DbSet<Info_Sucursal> Info_Sucursal => Set<Info_Sucursal>();
        public DbSet<Gestion_Clientes> Gestion_Clientes => Set<Gestion_Clientes>();
        public DbSet<IVA> IVA => Set<IVA>();
        public DbSet<CodigoSucursal> codigoSucursals => Set<CodigoSucursal>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Info_Sucursal>(entity =>
            {
                entity.ToTable("Info_Sucursal");
                entity.HasKey(e => e.SucursalId);

                entity.HasOne(e => e.GestionUsuario)
                      .WithMany()
                      .HasForeignKey(e => e.Encargado)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Gestion_Clientes>(entity =>
            {
                entity.ToTable("Gestion_Clientes");
                entity.HasKey(e => e.ClienteId);

                entity.HasOne(e => e.Info_Sucursal)
                      .WithMany()
                      .HasForeignKey(e => e.SucursalId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
