using Licoreria.Api.Entities.RecursosHumanos.EmpleadoEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.RecursosHumanos.UsuarioEntities
{
    [Table("gestion_Usuarios")]
    public class GestionUsuario
    {
        [Key]
        public int UsuarioId { get; set; }

        public int EmpleadoId { get; set; }

        public string? Usuario {  get; set; }
        public bool? Status { get; set; }
        public int PerfilId { get; set; }
        public string? PasswordHash {  get; set; }
        public DateTime? FechaCreacion {  get; set; }   

        public virtual Empleado Empleado { get; set; } = null!;

        public virtual GestionSeguridadPerfile Perfil { get; set; } = null!;

    }


}
