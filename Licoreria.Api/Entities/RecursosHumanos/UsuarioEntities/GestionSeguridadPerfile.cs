using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.RecursosHumanos.UsuarioEntities
{
    [Table("gestion_SeguridadPerfiles")]
    public class GestionSeguridadPerfile
    {
        [Key]
        public int PerfilId { get; set; }
        public string? Descripcion { get; set; } = null!;

        public virtual ICollection<GestionUsuario> GestionUsuarios { get; set; } = new List<GestionUsuario>();
    }
}
