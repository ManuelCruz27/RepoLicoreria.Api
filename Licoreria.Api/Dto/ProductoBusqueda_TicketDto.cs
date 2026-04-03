namespace Licoreria.Api.Dto
{
    public class ProductoBusqueda_TicketDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string CodigoBarras { get; set; }
        public string SKUCodigoInterno { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Existencia { get; set; }
        public int SucursalId { get; set; }
    }
}
