namespace Licoreria.Api.Entities.Sucursales
{
    public class IVA
    {
        public int IVAId { get; set; }
        public long Porcentaje {  get; set; }
        public string Descripcion { get; set; }
        public bool Status { get; set; }
    }
}
