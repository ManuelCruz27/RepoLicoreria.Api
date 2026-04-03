namespace Licoreria.Api.Dto.Almacen.Inventario
{
    public class MovimientoInventarioInsertDto
    {

        public int ProductoId { get; set; }
        public int SucursalId { get; set; }
        public string TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public string Referencia { get; set; }
        public int UsuarioID { get; set; }
        public string Usuario {  get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
    }
}
