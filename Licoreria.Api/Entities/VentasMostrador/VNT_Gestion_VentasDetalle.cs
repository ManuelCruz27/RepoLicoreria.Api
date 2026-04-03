using Licoreria.Api.Dto.Almacen;
using Licoreria.Api.Entities.Almacen;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.VentasMostrador
{
    [Table("VNT_Gestion_VentasDetalle")]
    public class VNT_Gestion_VentasDetalle
    {
        [Key]
        public int DetalleVentaId { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public string Producto {  get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal PorcentajeIVA { get; set; }
        public decimal MontoIVA { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalLinea { get; set; }

        [ForeignKey(nameof(VentaId))]
        public virtual VNT_Gestion_Ventas VNT_Gestion_Ventas { get; set; }
        [ForeignKey(nameof(ProductoId))]
        public virtual Productos Productos { get; set; }
    }
}
