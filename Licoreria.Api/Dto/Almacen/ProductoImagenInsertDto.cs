namespace Licoreria.Api.Dto.Almacen
{
    public class ProductoImagenInsertDto
    {
        public int ProductoId { get; set; }
        public string Ruta { get; set; }
        public byte Orden { get; set; }
        public bool EsPrincipal { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
