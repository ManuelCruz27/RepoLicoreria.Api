using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.Almacen
{
    [Table("Gestion_Proveedores")]
    public class Gestion_Proveedores
    {
        [Key]
        public int ProveedorId { get; set; }

        public string Nombre { get; set; }
        public string Contacto { get; set; }
        public int Numero { get; set; }
        public bool Status { get; set; }

    }
}
