using Licoreria.Api.Data.RecursosHumanosDATA;
using Licoreria.Api.Dto.ClienteDto;
using Licoreria.Api.Entities.Clientes;
using Licoreria.Api.Entities.RecursosHumanos.EmpleadoEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Licoreria.Api.Controllers.RecursosHumanosControllers
{
    [Authorize(Roles = "ALMACEN,RH,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesControllers : Controller
    {
        private readonly LicoreriaDbContext _db;

        public ClientesControllers(LicoreriaDbContext db)
        {
            _db = db;
        }

        [HttpPost("AgregarCliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AgregarCliente([FromBody] CrearClientesDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Body requerido." });

            using var tx = await _db.Database.BeginTransactionAsync();
            bool status = true;

            try
            {
                var datos = new Gestion_Clientes
                {
                    Nombre = dto.Nombre,
                    ApellidoPaterno = dto.ApellidoPaterno,
                    ApellidoMaterno = dto.ApellidoMaterno,
                    Celular = dto.Celular,
                    Email = dto.Email,
                    FechaNacimiento = dto.FechaNacimiento,
                    Direccion = dto.Direccion,
                    NumeroExterior = dto.NumeroExterior,
                    NumeroInterior = dto.NumeroInterior,
                    Colonia = dto.Colonia,
                    Municipio = dto.Municipio,
                    CodigoPostal = dto.CodigoPostal,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow,
                    SucursalId = dto.SucursalId,

                };
                _db.Gestion_Clientes.Add(datos);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                return Created("", new
                {
                    message = "Cliente creado correctamente",
                    ClienteId = datos.ClienteId
                });

            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                var detail = ex.InnerException?.Message ?? ex.Message;

                return StatusCode(500, new
                {
                    message = "Error al crear al cliente",
                    detail
                });
            } 
        }

        [HttpGet("optenerClientes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OptenerClientes()
        {
            try
            {
                var Datos = await _db.Gestion_Clientes
                    .AsNoTracking()
                    .OrderBy(c => c.ClienteId)
                    .Where(c => c.Activo == true)
                    .Select(c => new
                    {
                        c.ClienteId,
                        c.Nombre,
                        c.ApellidoPaterno,
                        c.ApellidoMaterno,
                        c.Celular,
                        c.Email,
                        c.FechaNacimiento,
                        c.Direccion,
                        c.NumeroExterior,
                        c.NumeroInterior,
                        c.Colonia,
                        c.Municipio,
                        c.Estado,
                        c.CodigoPostal,
                        c.FechaCreacion,
                        c.FechaActualizacion,
                        c.SucursalId

                    })
                    .ToArrayAsync();
                return Ok(Datos);


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error al obtener a los clientes.",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("ObtenerDatosPorID/{clienteId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerDatosPorID(int clienteId)
        {
            try
            {
                var cliente = await _db.Gestion_Clientes
                    .AsNoTracking()
                    .Where(c => c.ClienteId == clienteId)
                    .Select(c => new Gestion_Clientes
                    {
                        ClienteId = clienteId,
                        Nombre = c.Nombre,
                        ApellidoPaterno = c.ApellidoPaterno,
                        ApellidoMaterno = c.ApellidoMaterno,
                        Celular = c.Celular,
                        Email = c.Email,
                        FechaNacimiento = c.FechaNacimiento,
                        Direccion = c.Direccion,
                        NumeroExterior = c.NumeroExterior,
                        NumeroInterior = c.NumeroInterior,
                        Colonia = c.Colonia,
                        Municipio = c.Municipio,
                        Estado = c.Estado,
                        CodigoPostal = c.CodigoPostal,
                        FechaCreacion = c.FechaCreacion,
                        FechaActualizacion = c.FechaActualizacion,
                        SucursalId = c.SucursalId

                    })
                    .FirstOrDefaultAsync();
                
                if (cliente == null)
                {
                    return NotFound(new { mesage = "Cliente no encontrado" });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error al obtener al cliente.",
                    detalle = ex.Message
                });
            }
        }

        [HttpPut("EditarDatosCliente/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarDatosCliente([FromRoute] int id, [FromBody] UpdateClientesDto dto)
        {
            var cliente = await _db.Gestion_Clientes.FirstOrDefaultAsync(e => e.ClienteId == id);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado." });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "El correo es obligatorio." });

            var existeCorreo = await _db.Gestion_Clientes.AnyAsync(e =>
                e.Email == dto.Email && e.ClienteId != id && e.Activo == true);

            if (existeCorreo)
                return Conflict(new { message = "Ya existe un cliente con ese correo." });
           

            cliente.Nombre = dto.Nombre;
            cliente.ApellidoPaterno = dto.ApellidoPaterno;
            cliente.ApellidoMaterno = dto.ApellidoMaterno;
            cliente.Email = dto.Email;
            cliente.Celular = dto.Celular;
            cliente.FechaNacimiento = dto.FechaNacimiento;

            cliente.Direccion = dto.Direccion;
            cliente.NumeroExterior = dto.NumeroExterior;
            cliente.NumeroInterior = dto.NumeroInterior;
            cliente.Colonia = dto.Colonia;
            cliente.Municipio = dto.Municipio;
            cliente.Estado = dto.Estado;
            cliente.CodigoPostal = dto.CodigoPostal;
            cliente.FechaActualizacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Cliente actualizado correctamente", clienteId = cliente.ClienteId });
        }
    }
}
