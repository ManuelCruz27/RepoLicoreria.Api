using Licoreria.Api.Entities.Sucursales;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.Almacen.Inventatio
{
    [Table("ALM_InventarioProductos")]
    public class ALM_InventarioProductos
    {
        [Key]
        public int InventarioId { get; set; }
        public int ProductoId {  get; set; }
        public int SucursalId { get; set; }
        public int StockActual {  get; set; }
        public int StockMinimo { get; set; }
        public DateTime FechaActualizacion { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Productos Productos { get; set; }

        [ForeignKey(nameof(SucursalId))]
        public Info_Sucursal Info_Sucursal { get; set; }
    }
}
