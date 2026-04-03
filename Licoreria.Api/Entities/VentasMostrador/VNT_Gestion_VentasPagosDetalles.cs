using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.VentasMostrador
{
    [Table("VNT_Gestion_VentasPagosDetalles")]
    public class VNT_Gestion_VentasPagosDetalles
    {
        [Key]
        public int VentaMetodoPagoId { get; set; }
        public int VentaId { get; set; }
        public int MetodoPagoId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaMovimiento { get; set; }

        [ForeignKey(nameof(VentaId))]
        public virtual VNT_Gestion_Ventas VNT_Gestion_Ventas { get; set; }

        [ForeignKey(nameof(MetodoPagoId))]
        public virtual VNT_MetodoPago VNT_MetodoPago { get; set; }
    }
}
