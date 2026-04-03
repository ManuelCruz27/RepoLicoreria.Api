using Licoreria.Api.Entities.Sucursales;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.Clientes
{
    [Table("Gestion_Clientes")]
    public class Gestion_Clientes
    {
        [Key]
        public int ClienteId { get; set; }

        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string? ApellidoMaterno { get; set; }

        public long? Celular { get; set; }
        public string? Email { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        public string? Direccion { get; set; }
        public string? NumeroExterior { get; set; }
        public string? NumeroInterior { get; set; }
        public string? Colonia { get; set; }
        public string? Municipio { get; set; }
        public string? Estado { get; set; }
        public string? CodigoPostal { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int SucursalId { get; set; }

        [ForeignKey(nameof(SucursalId))]
        public virtual Info_Sucursal Info_Sucursal { get; set; } = null!;
    }
}