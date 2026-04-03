namespace Licoreria.Api.Dto.UsuarioDto
{
    public class UsuarioIUpdateDto
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; } = "";
        public int EmpleadoId { get; set; }
        public bool Status { get; set; }
        public int PerfilId { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
