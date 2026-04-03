using Licoreria.Api.Entities.RecursosHumanos.EmpleadoEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.Almacen
{
    [Table("Productos")]
    public class Productos
    {
        [Key]
        public int ProductoId { get; set; }

        public string CodigoBarras { get; set; }
        public string SkucodigoInterno { get; set; }

        public string Nombre { get; set; }
        public int MarcaId { get; set; }
        public string Descripcion { get; set; }

        public int ContenidoMl { get; set; }
        public decimal GraduacionAlcohol { get; set; }
        public string Presentacion { get; set; }

        public int CategoriaId { get; set; }

        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Iva { get; set; }
        public bool Status { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        //[ForeignKey(nameof(CategoriaId))]
        public virtual CategoriasProductos Categoria { get; set; }

        public virtual ICollection<ProductoImagenes> ProductoImagenes { get; set; } =  null;
    }
}

