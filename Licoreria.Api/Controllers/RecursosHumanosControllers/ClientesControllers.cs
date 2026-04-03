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

        [HttpGet("obtenerClientes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OptenerClientes()
        {
            try
            {
                var datos = await _db.Gestion_Clientes
                    .AsNoTracking()
                    .Where(c => c.Activo)
                    .OrderBy(c => c.ClienteId)
                    .Select(c => new
                    {
                        c.ClienteId,
                        Nombre = c.Nombre ?? "",
                        ApellidoPaterno = c.ApellidoPaterno ?? "",
                        ApellidoMaterno = c.ApellidoMaterno ?? "",
                        c.Celular,
                        Email = c.Email ?? "",
                        c.FechaNacimiento,
                        Direccion = c.Direccion ?? "",
                        NumeroExterior = c.NumeroExterior ?? "",
                        NumeroInterior = c.NumeroInterior ?? "",
                        Colonia = c.Colonia ?? "",
                        Municipio = c.Municipio ?? "",
                        Estado = c.Estado ?? "",
                        CodigoPostal = c.CodigoPostal ?? "",
                        c.FechaCreacion,
                        c.FechaActualizacion,
                        c.SucursalId
                    })
                    .ToArrayAsync();

                return Ok(datos);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error al obtener a los clientes.",
                    detalle = ex.Message,
                    inner = ex.InnerException?.Message
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

        [HttpGet("BuscarEmpleado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarEmpleado([FromQuery] string q)
        {
            try
            {
                q = (q ?? "").Trim();

                if (string.IsNullOrWhiteSpace(q))
                    return BadRequest(new { message = "Debes mandar un parámetro q" });

                if(int.TryParse(q, out int id))
                {
                    var buscarID = await _db.Gestion_Clientes
                        .AsNoTracking()
                        .Where(e => e.ClienteId == id && e.Activo == true)
                        .Select(c => new
                        {
                            c.ClienteId,
                            Nombre = c.Nombre ?? "",
                            ApellidoPaterno = c.ApellidoPaterno ?? "",
                            ApellidoMaterno = c.ApellidoMaterno ?? "",
                            c.Celular,
                            Email = c.Email ?? "",
                            c.FechaNacimiento,
                            Direccion = c.Direccion ?? "",
                            NumeroExterior = c.NumeroExterior ?? "",
                            NumeroInterior = c.NumeroInterior ?? "",
                            Colonia = c.Colonia ?? "",
                            Municipio = c.Municipio ?? "",
                            Estado = c.Estado ?? "",
                            CodigoPostal = c.CodigoPostal ?? "",
                            c.FechaCreacion,
                            c.FechaActualizacion,
                            c.SucursalId


                        }).ToListAsync();
                    return Ok(buscarID);
                }

                var tokens = q
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(t => t.Length > 0)
                    .ToArray();

                var query = _db.Gestion_Clientes.AsNoTracking().AsQueryable();

                query = query.Where(e =>
                (e.Nombre ?? "").Contains(q) ||
                (e.ApellidoPaterno ?? "").Contains(q) ||
                (e.ApellidoMaterno ?? "").Contains(q) ||
                (e.Email ?? "").Contains(q) ||
                ((e.Nombre ?? "") + " " + (e.ApellidoPaterno ?? "") + " " + (e.ApellidoMaterno ?? "")).Contains(q)
                );

                if(tokens.Length > 1)
                {
                    foreach(var t in tokens)
                    {
                        var token = t;
                        query = query.Where(e =>
                            ((e.Nombre ?? "") + " " + (e.ApellidoPaterno ?? "") + " " + (e.ApellidoMaterno ?? "")).Contains(token)
                        );
                    }
                }

                var clientes = await query
                    .OrderBy(e => e.ClienteId)
                    .Select(c => new
                    {
                        clienteID = c.ClienteId,
                        Nombre = c.Nombre ?? "",
                        ApellidoPaterno = c.ApellidoPaterno ?? "",
                        ApellidoMaterno = c.ApellidoMaterno ?? "",
                        c.Celular,
                        Email = c.Email ?? "",
                        c.FechaNacimiento,
                        Direccion = c.Direccion ?? "",
                        NumeroExterior = c.NumeroExterior ?? "",
                        NumeroInterior = c.NumeroInterior ?? "",
                        Colonia = c.Colonia ?? "",
                        Municipio = c.Municipio ?? "",
                        Estado = c.Estado ?? "",
                        CodigoPostal = c.CodigoPostal ?? "",
                        c.FechaCreacion,
                        c.FechaActualizacion,
                        c.SucursalId
                    }).ToArrayAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error al obtener al cliente.",
                    detalle = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
            


        }
    }
}
