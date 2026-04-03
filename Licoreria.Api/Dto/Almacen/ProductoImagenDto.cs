namespace Licoreria.Api.Dto.Almacen
{
    public class ProductoImagenDto
    {
        public int ImagenId { get; set; }
        public string Ruta { get; set; }
        public byte Orden { get; set; }
        public bool EsPrincipal { get; set; }
    }
}
