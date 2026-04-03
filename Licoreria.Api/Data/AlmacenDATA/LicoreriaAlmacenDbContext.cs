using Licoreria.Api.Entities.Almacen;
using Licoreria.Api.Entities.Almacen.Inventatio;
using Microsoft.EntityFrameworkCore;

namespace Licoreria.Api.Data.AlmacenDATA
{
    public class LicoreriaAlmacenDbContext : DbContext
    {
        public LicoreriaAlmacenDbContext(DbContextOptions<LicoreriaAlmacenDbContext> options) 
            : base(options) { }

        public DbSet<CategoriasProductos> CategoriasProductos => Set<CategoriasProductos>();
        public DbSet<Productos> Productos => Set<Productos>();
        public DbSet<MarcaProductos> MarcaProductos => Set<MarcaProductos>();
        public DbSet<Gestion_Proveedores> Gestion_Proveedores => Set<Gestion_Proveedores>();
        public DbSet<ProductoImagenes> ProductoImagenes => Set<ProductoImagenes>();
        public DbSet<ALM_InventarioProductos> ALM_InventarioProductos => Set<ALM_InventarioProductos>();
        public DbSet<ALM_MovimientosInventario> ALM_MovimientosInventarios => Set<ALM_MovimientosInventario>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductoImagenes>()
                .HasOne(pi => pi.Productos)
                .WithMany(p => p.ProductoImagenes)
                .HasForeignKey(pi => pi.ProductoId);

            modelBuilder.Entity<ALM_InventarioProductos>()
                .HasOne(i => i.Productos)
                .WithMany()
                .HasForeignKey(i => i.ProductoId);

            modelBuilder.Entity<ALM_InventarioProductos>()
                .HasOne(i => i.Info_Sucursal)
                .WithMany()
                .HasForeignKey(i => i.SucursalId);

            modelBuilder.Entity<ALM_MovimientosInventario>()
                .HasOne(i => i.Productos)
                .WithMany()
                .HasForeignKey(i => i.ProductoId);

            modelBuilder.Entity<ALM_MovimientosInventario>()
                .HasOne(i => i.Info_Sucursal)
                .WithMany()
                .HasForeignKey(i => i.SucursalId);

            modelBuilder.Entity<ALM_MovimientosInventario>()
                .HasOne(i => i.GestionUsuario)
                .WithMany()
                .HasForeignKey(i => i.UsuarioID);
        }

    }
}
