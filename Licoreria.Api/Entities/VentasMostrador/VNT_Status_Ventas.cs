using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.VentasMostrador
{
    [Table("VNT_Status_Ventas")]
    public class VNT_Status_Ventas
    {
        [Key]
        public int StatusId { get; set; }
        public string Descripcion { get; set; }
    }
}
