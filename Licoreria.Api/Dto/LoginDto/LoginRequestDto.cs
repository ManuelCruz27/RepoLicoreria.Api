namespace Licoreria.Api.Dto.LoginDto
{
    public class LoginRequestDto
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public int SucursalID { get; set; }
    }
}
