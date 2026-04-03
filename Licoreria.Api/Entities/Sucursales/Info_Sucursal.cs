using Licoreria.Api.Entities.RecursosHumanos.UsuarioEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.Sucursales
{
    [Table("Info_Sucursal")]
    public class Info_Sucursal
    {
        [Key]
        public int SucursalId { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int Encargado { get; set; }
        public string Direcion { get; set; }
        public string NumeroExterior { get; set; }
        public string NumeroInterno { get; set; }
        public string Colonia { get; set; }
        public string Referencia { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string CodigoPostal { get; set; }
        public string Numero { get; set; }
        public string Email { get; set; }
        public string Horario { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }

        public int CodigoSucursalId { get; set; }

        public int IVAId { get; set; }


        [ForeignKey(nameof(Encargado))]
        public virtual GestionUsuario GestionUsuario { get; set; } = null!;

        [ForeignKey(nameof(IVAId))]
        public virtual IVA IVA { get; set; }


    }
}
