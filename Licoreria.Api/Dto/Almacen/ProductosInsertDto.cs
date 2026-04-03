using Licoreria.Api.Entities.Almacen;

namespace Licoreria.Api.Dto.Almacen
{
    public class ProductosInsertDto
    {
        public string CodigoBarras { get; set; } = "";
        public string SkucodigoInterno { get; set; } = "";
        public string Nombre { get; set; } = "";
        public int MarcaId { get; set; }
        public string Descripcion { get; set; } = "";
        
        public int ContenidoML { get; set; }
        public decimal GraduacionAlcohol { get; set; }
        public string Presentacion { get; set; } = "";
        
        public int CategoriaId { get; set; }
       
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal IVA { get; set; }
        public bool Status { get; set; }

    }
}
