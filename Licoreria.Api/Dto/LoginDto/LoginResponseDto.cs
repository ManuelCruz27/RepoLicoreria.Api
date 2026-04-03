namespace Licoreria.Api.Dto.LoginDto
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = "";
        public int EmpleadoId { get; set; }
        public int PerfilId { get; set; }
        public string Usuario { get; set; } = "";
        public int SucursalId { get; set; }
    }
}
