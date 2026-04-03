using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.Almacen
{
    [Table("ProductoImagenes")]
    public class ProductoImagenes
    {
        [Key]
        public int ImagenId { get; set; }
        public int ProductoId { get; set; }
        public string Ruta { get; set; }
        public byte Orden { get; set; }
        public bool EsPrincipal { get; set; }
        public DateTime FechaCreacion { get; set; }

        //navegación correcta: una imagen -> un producto
        public virtual Productos Productos { get; set; }

    }
}
