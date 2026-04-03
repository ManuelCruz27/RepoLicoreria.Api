using Licoreria.Api.Data.AlmacenDATA;
using Licoreria.Api.Entities.VentasMostrador;
using Microsoft.EntityFrameworkCore;

namespace Licoreria.Api.Data.VentasDATA
{
    public class LicoreriaVentasDbContext : DbContext
    {
        public LicoreriaVentasDbContext(DbContextOptions<LicoreriaVentasDbContext> options)
            : base(options) { }

        public DbSet<VNT_Gestion_Ventas> VNT_Gestion_Ventas => Set<VNT_Gestion_Ventas>();
        public DbSet<VNT_Status_Ventas> VNT_Status_Ventas => Set<VNT_Status_Ventas>();
        public DbSet<VNT_Gestion_CajasMostrador> VNT_Gestion_CajasMostradors => Set<VNT_Gestion_CajasMostrador>();
        public DbSet<VNT_Gestion_VentasDetalle> VNT_Gestion_VentasDetalles => Set<VNT_Gestion_VentasDetalle>();
        public DbSet<VNT_Gestion_VentasPagosDetalles> VNT_Gestion_VentasPagosDetalles => Set<VNT_Gestion_VentasPagosDetalles>();
        public DbSet<VNT_MetodoPago> VNT_MetodoPagos => Set<VNT_MetodoPago>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VNT_Gestion_Ventas>()
                .HasOne(i => i.Info_Sucursal)
                .WithMany()
                .HasForeignKey(i => i.SucursalId);
            modelBuilder.Entity<VNT_Gestion_Ventas>()
                .HasOne(u => u.GestionUsuario)
                .WithMany()
                .HasForeignKey(u => u.UsuarioId);
            modelBuilder.Entity<VNT_Gestion_Ventas>()
                .HasOne(e => e.Gestion_Clientes)
                .WithMany()
                .HasForeignKey(e => e.ClienteId);
            modelBuilder.Entity<VNT_Gestion_Ventas>()
                .HasOne(s => s.VNT_Status_Ventas)
                .WithMany()
                .HasForeignKey(s => s.Estatus);
            modelBuilder.Entity<VNT_Gestion_Ventas>()
                .HasOne(c => c.VNT_Gestion_CajasMostrador)
                .WithMany()
                .HasForeignKey(c => c.CajaId);


            modelBuilder.Entity<VNT_Gestion_CajasMostrador>()
                .HasOne(i => i.Info_Sucursal)
                .WithMany()
                .HasForeignKey(i => i.SucursalId);


            modelBuilder.Entity<VNT_Gestion_VentasDetalle>()
                .HasOne(gv => gv.VNT_Gestion_Ventas)
                .WithMany()
                .HasForeignKey(v => v.VentaId);
            modelBuilder.Entity<VNT_Gestion_VentasDetalle>()
                .HasOne(p => p.Productos)
                .WithMany()
                .HasForeignKey(p => p.ProductoId);

            modelBuilder.Entity<VNT_Gestion_VentasPagosDetalles>()
               .HasOne(v => v.VNT_Gestion_Ventas)
               .WithMany()
               .HasForeignKey(v => v.VentaId);
            modelBuilder.Entity<VNT_Gestion_VentasPagosDetalles>()
               .HasOne(p => p.VNT_MetodoPago)
               .WithMany()
               .HasForeignKey(p => p.MetodoPagoId);


        }


    }
}
