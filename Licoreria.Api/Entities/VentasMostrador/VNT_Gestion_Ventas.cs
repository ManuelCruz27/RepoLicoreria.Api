using Licoreria.Api.Entities.Clientes;
using Licoreria.Api.Entities.RecursosHumanos.UsuarioEntities;
using Licoreria.Api.Entities.Sucursales;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.VentasMostrador
{
    [Table("VNT_Gestion_Ventas")]
    public class VNT_Gestion_Ventas
    {
        [Key]
        public int VentaId { get; set; }
        public int SucursalId { get; set; }
        public int UsuarioId {  get; set; }
        public int ClienteId { get; set; }
        public DateTime FechaVenta {  get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public int Estatus { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int CajaId { get; set; }

        [ForeignKey(nameof(SucursalId))]
        public virtual Info_Sucursal Info_Sucursal { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual GestionUsuario GestionUsuario { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public virtual Gestion_Clientes Gestion_Clientes { get; set; }

        [ForeignKey(nameof(Estatus))]
        public virtual VNT_Status_Ventas VNT_Status_Ventas { get; set; }

        [ForeignKey(nameof(CajaId))]
        public virtual VNT_Gestion_CajasMostrador VNT_Gestion_CajasMostrador { get; set; }
    }
}
