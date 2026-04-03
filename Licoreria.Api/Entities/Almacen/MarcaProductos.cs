using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Licoreria.Api.Entities.Almacen;

namespace Licoreria.Api.Entities.Almacen
{
    [Table("MarcaProductos")]
    public class MarcaProductos
    {
        [Key]
        public int MarcaId { get; set; }
        public string Marca { get; set; }
        public int ProveedorId { get; set; }
        public bool Status { get; set; }
        public DateTime FechaCreacion { get; set; }

        public virtual Gestion_Proveedores Gestion_Proveedores { get; set; } = null;


    }
}
