using Licoreria.Api.Entities.Sucursales;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.VentasMostrador
{
    [Table("VNT_Gestion_CajasMostrador")]
    public class VNT_Gestion_CajasMostrador
    {
        [Key]
        public int CajaId { get; set; }
        public int SucursalId { get; set; }
        public string Nombre { get; set; }
        public int NumeroCaja { get; set; }
        public string Descripcion { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaCreacion { get; set; }

        [ForeignKey(nameof(SucursalId))]
        public virtual Info_Sucursal Info_Sucursal { get; set; }
    }
}
