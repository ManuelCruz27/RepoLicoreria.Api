namespace Licoreria.Api.Dto.Almacen
{
    public class ProductoDetalleDto
    {

        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string CodigoBarras { get; set; }
        public string SKUCodigoInterno { get; set; }
        public string Descripcion { get; set; }
        public int ContenidoML { get; set; }
        public decimal GraduacionAlcohol { get; set; }
        public string Presentacion { get; set; }
        public decimal PrecioVenta { get; set; }
        public DateTime FechaActualizacion { get; set; }


        public List<ProductoImagenDto> Imagenes { get; set; } = new();
    }
}
