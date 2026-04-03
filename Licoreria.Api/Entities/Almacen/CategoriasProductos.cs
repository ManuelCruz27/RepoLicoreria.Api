using Licoreria.Api.Entities.RecursosHumanos.UsuarioEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.Almacen
{
    [Table("CategoriasProductos")]
    public class CategoriasProductos
    {
        [Key]
        public int CategoriaId { get; set; }
        public string Nombre { get; set; }
        public bool Status { get; set; }


        public virtual ICollection<Productos> Productos { get; set; } = new List<Productos>();
    }
}
