using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Licoreria.Api.Dto.UsuarioDto;
using Licoreria.Api.Data.RecursosHumanosDATA;

namespace Licoreria.Api.Controllers.RecursosHumanosControllers
{
    [Authorize(Roles = "Rh,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioControllers : ControllerBase
    {
        private readonly LicoreriaDbContext _db;

        public UsuarioControllers(LicoreriaDbContext db)
        {
            _db = db;

        }

        [HttpGet("ConsultarDatosUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerDatos()
        {
            try
            {
                var Usuarios = await _db.GestionUsuario
                    .AsNoTracking()
                    .OrderBy(e => e.UsuarioId)
                    .Select(e => new
                    {
                        e.UsuarioId,
                        e.Usuario,
                        e.EmpleadoId,
                        e.Status,
                        e.PerfilId,
                        e.FechaCreacion


                    })
                    .ToListAsync();
                return Ok(Usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "\tError API \nError al consultar los datos de los usuarios *UsuarioControllers/ConsultarDatosEmpleados*", detail = ex.Message });

            }
        }

        [HttpGet("BuscarUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarUsuario([FromQuery] string q)
        {
            q = (q ?? "").Trim();

            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Debes mandar parámetro q." });

            var esNumero = int.TryParse(q, out var empleadoId);

            var query = _db.GestionUsuario.AsNoTracking().AsQueryable();

            if (esNumero)
                query = query.Where(u => u.EmpleadoId == empleadoId);
            else
                query = query.Where(u => u.Usuario.Contains(q));

            var data = await query
                .Select(u => new {
                    u.UsuarioId,
                    u.Usuario,
                    u.EmpleadoId,
                    u.Status,
                    u.PerfilId,
                    u.FechaCreacion
                })
                .ToListAsync();

            return Ok(data);
        }


        [HttpGet("ObtenerUsuarioDatosPorID/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerDatosPorID(int id)
        {
            var datos = await _db.GestionUsuario
                .AsNoTracking()
                .Where(e => e.EmpleadoId == id)
                .Select(e => new { 
                    e.UsuarioId,
                    e.EmpleadoId,
                    e.Usuario,
                    e.PerfilId,
                    //e.Status

                }).FirstOrDefaultAsync();

            if (datos == null)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(datos);
        }


        [HttpGet("ObtenerPerfiles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerPerfiles()
        {
            try
            {
                var datos = await _db.GestionSeguridadPerfile
                    .AsNoTracking()
                    .OrderBy(e => e.PerfilId)
                    .Select(e => new
                    {
                        e.PerfilId,
                        e.Descripcion

                    }).ToListAsync();
                return Ok(datos);
                    
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "\tError API \nError al consultar los datos de los UsuarioDto/ObtenerPerfiles", detail = ex.Message });

            }
        }

        [HttpPut("EditarDatosUsuario/{empleadoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarDatosUsuario([FromRoute]int empleadoId, [FromBody] UsuarioIUpdateDto dto) {

            //Buscar el empleado en la base de datos
            var empleado = await _db.GestionUsuario.FirstOrDefaultAsync(e => e.EmpleadoId == empleadoId);
            if (empleado == null)
                return NotFound(new { message = "f no encontrado." });

            empleado.PerfilId = dto.PerfilId;

            _db.SaveChanges();
            return Ok(new { message = "Empleado actualizado correctamente", empleadoId = empleado.EmpleadoId });


        }




    }
}
