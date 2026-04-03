using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.VentasMostrador
{
    [Table("VNT_MetodoPago")]
    public class VNT_MetodoPago
    {
        [Key]
        public int MetodoPagoId { get; set; }
        public string Descripcion { get; set; }
        public bool Status { get; set; }
    }
}
