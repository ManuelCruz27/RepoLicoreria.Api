using Licoreria.Api.Entities.RecursosHumanos.UsuarioEntities;
using Licoreria.Api.Entities.Sucursales;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.Almacen.Inventatio
{
    [Table("ALM_MovimientosInventario")]
    public class ALM_MovimientosInventario
    {
        [Key]
        public int MovimientoId { get; set; }
        public int ProductoId {get; set; }
        public int SucursalId {get; set; }
        public string TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public string Referencia { get; set; }
        public int UsuarioID { get; set; }

        public DateTime FechaMovimiento { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Productos Productos { get; set; }

        [ForeignKey(nameof(SucursalId))]
        public Info_Sucursal Info_Sucursal { get; set; }
        
        [ForeignKey(nameof(UsuarioID))]
        public virtual GestionUsuario GestionUsuario { get; set; }
    }
}
