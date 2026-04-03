using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Licoreria.Api.Dto.LoginDto;
using Licoreria.Api.Data.RecursosHumanosDATA;
using Licoreria.Api.Entities.Sucursales;

namespace Licoreria.Api.Controllers.LoginControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LicoreriaDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(LicoreriaDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {

            dto.Usuario = (dto.Usuario ?? "").Trim();

            if (string.IsNullOrWhiteSpace(dto.Usuario) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Usuario y contraseña son obligatorios." });
            
            var user = await _db.GestionUsuario
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Usuario == dto.Usuario && u.Status == true);

            if (user == null)
                return Unauthorized(new { message = "Credenciales invalidas" });

            //verificar contraseña (BCrypt recomendado)
            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok)
                return Unauthorized(new { message = "Credenciales inválidas." });


            var token = CrearJwt(user.Usuario, user.EmpleadoId, user.PerfilId, dto.SucursalID);
            
            return Ok(new LoginResponseDto
            {
                Token = token,
                EmpleadoId = user.EmpleadoId,
                PerfilId = user.PerfilId,
                Usuario = user.Usuario,
                SucursalId = dto.SucursalID

            });

        }

        // Ejemplo: PerfilId 1=RH, 2=Almacen, 3=Mostrador
        static string MapPerfilToRole(int perfilId) => perfilId switch
        {
            1 => "RH",
            2 => "ALMACEN",
            3 => "MOSTRADOR",
            4 => "ADMIN",
            _ => "SIN_PERFIL"
        };

        private string CrearJwt(string usuario, int empleadoId, int perfilId, int sucursalID)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var role = MapPerfilToRole(perfilId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario),
                new Claim("empleadoId", empleadoId.ToString()),
                new Claim("perfilId", perfilId.ToString()),
                new Claim("sucursalId", sucursalID.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("OptenerSucursales")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OptenerSucursales()
        {
            try
            {
                var datos = await _db.Info_Sucursal
                    .AsNoTracking()
                    .Where(s => s.Activo == true)
                    .OrderBy(s => s.SucursalId)
                    .Select(s => new
                    {
                        s.SucursalId,
                        s.Nombre
                    }).ToListAsync();

                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error \nError al consultar los datos de las sucursales.", detail = ex.Message });
            }
        }




    }
}
