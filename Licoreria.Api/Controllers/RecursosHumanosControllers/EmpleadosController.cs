using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Licoreria.Api.Dto.EmpleadoDto;
using Licoreria.Api.Entities.RecursosHumanos.EmpleadoEntities;
using Licoreria.Api.Entities.RecursosHumanos.UsuarioEntities;
using Licoreria.Api.Data.RecursosHumanosDATA;


namespace Licoreria.Api.Controllers.RecursosHumanosControllers
{
    [Authorize(Roles = "RH,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly LicoreriaDbContext _db;

        public EmpleadosController(LicoreriaDbContext db)
        {
            _db = db;
        }

        //----------------------------- Agregar Empleados ---------------------------------------

        //********** validaciones para crear el usuario *************************************
        private static string QuitaAcentos(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        private static string NormalizaUser(string s)
        {
            s = QuitaAcentos(s ?? "").Trim().ToLowerInvariant();
            s = s.Replace(" ", "");
            return s;
        }
        private static string PrimerNombre(string nombreCompleto)
        {
            var parts = (nombreCompleto ?? "")
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : "";
        }


        //Api para agregar empleados 
        [HttpPost("AgregarEmpleados")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CrearEmpleado([FromBody] EmpleadoCreateDto dto)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {

                // 1) Validación rápida (mínimos)
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest(new { message = "El nombre es obligatorio." });

                if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                    return BadRequest(new { message = "El apellido paterno es obligatorio." });

                if (string.IsNullOrWhiteSpace(dto.ApellidoMaterno))
                    return BadRequest(new { message = "El apellido materno es obligatorio." });

                // 2) Convertir celular (tu dto trae string)
                long? celular = null;
                if (!string.IsNullOrWhiteSpace(dto.NumeroCelular))
                {
                    if (!long.TryParse(dto.NumeroCelular, out var celParsed))
                        return BadRequest(new { message = "Número de celular inválido (solo números)." });

                    celular = celParsed;
                }

                // 3) (Opcional) Evitar duplicados por correo
                if (!string.IsNullOrWhiteSpace(dto.CorreoElectronico))
                {
                    var existeCorreo = await _db.Empleados
                        .AnyAsync(e => e.Correo == dto.CorreoElectronico);

                    if (existeCorreo)
                        return Conflict(new { message = "Ya existe un empleado con ese correo." });
                }

                static string? Clean(string? s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return null;

                    s = s.Trim();
                    if (string.Equals(s, "null", StringComparison.OrdinalIgnoreCase)) return null;

                    return s;
                }

                // 4) Mapear DTO -> Entity
                var empleado = new Empleado
                {
                    Nombre = dto.Nombre.Trim(),
                    Apellido_Paterno = dto.ApellidoPaterno.Trim(),
                    Apellido_Materno = Clean(dto.ApellidoMaterno),
                    Sexo = dto.Sexo.Trim(),
                    Correo = Clean(dto.CorreoElectronico),
                    CURP = Clean(dto.CURP),
                    RFC = Clean(dto.RFC),
                    Direccion = Clean(dto.Direccion),
                    NExterior = Clean(dto.NExterior),
                    NInterior = Clean(dto.NInterior),
                    Colonia = Clean(dto.Colonia),
                    Municipio = Clean(dto.Municipio),
                    Estado = Clean(dto.Estado),
                    CodigoPostal = Clean(dto.CodigoPostal),
                    Celular = string.IsNullOrWhiteSpace(dto.NumeroCelular) ? null : long.Parse(dto.NumeroCelular)

                };



                // 5) Guardar en SQL Server
                _db.Empleados.Add(empleado);
                await _db.SaveChangesAsync(); // ya tenemos EmpleadoID

                //generar username base: primerNombre.primerApellido
                var nombre = NormalizaUser(PrimerNombre(dto.Nombre));
                var apParteno = NormalizaUser(dto.ApellidoPaterno);

                var baseUser = $"{nombre}.{apParteno}";
                var userFinal = baseUser;

                //Evitar duplicados: juan.cruz2, juan.cruz3...
                var i = 2;
                while (await _db.GestionUsuario.AnyAsync(u => u.Usuario == userFinal))
                {
                    userFinal = $"{baseUser}{1}";
                    i++;
                }
                //var passwordInicial = empleado.EmpleadoID.ToString();
                //var hasher = new PasswordHasher<GestionUsuario>();
                var hasher = BCrypt.Net.BCrypt.HashPassword(empleado.EmpleadoID.ToString());



                //Crear usuario ligado al empleadoID
                var usuario = new GestionUsuario
                {
                    EmpleadoId = empleado.EmpleadoID,
                    Usuario = userFinal,
                    Status = true,
                    PerfilId = 1,
                    PasswordHash = hasher,
                    FechaCreacion = DateTime.Now

                };

                //usuario.PasswordHash = hasher.HashPassword(usuario, passwordInicial);
                _db.GestionUsuario.Add(usuario);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();


                //  Respuesta (Created)
                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = empleado.EmpleadoID },
                    new
                    {
                        message = "Empleado creado correctamente",
                        empleadoId = empleado.EmpleadoID,
                        usuario = userFinal
                    });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { message = "Error al crear empleado/usuario", detail = ex.Message });

            }

        }

        // Para que CreatedAtAction tenga a dónde apuntar
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId([FromRoute] int id)
        {
            var empleado = await _db.Empleados.FirstOrDefaultAsync(e => e.EmpleadoID == id);
            if (empleado == null) return NotFound(new { message = "Empleado no encontrado" });

            return Ok(empleado);
        }

        //****************************************************************************************


        //-------------------- Consultar Datos --------------------------------------------------

        //API para consultar informacion de los empleados
        [HttpGet("ConsultarDatosEmpleados")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var empleados = await _db.Empleados
                .AsNoTracking()
                .OrderBy(e => e.EmpleadoID)
                .Select(e => new
                {
                    e.EmpleadoID,
                    e.Nombre,
                    e.Apellido_Paterno,
                    e.Apellido_Materno,
                    e.Sexo,
                    e.Correo,
                    e.Celular,
                    e.CURP,
                    e.RFC,
                    e.Direccion,
                    e.NExterior,
                    e.NInterior,
                    e.Colonia,
                    e.Municipio,
                    e.Estado,
                    e.CodigoPostal
                })
                .ToListAsync();

                return Ok(empleados);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "\tError API \nError al consultar los datos de los empleado/ConsultarDatosEmpleados", detail = ex.Message });

            }





        }

        [HttpGet("BuscarEmpleado")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Buscar([FromQuery] string q)
        {
            q = (q ?? "").Trim();

            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Debes mandar un parámetro q." });

            // 1) Si es número, busca por EmpleadoID 
            if (int.TryParse(q, out var id))
            {
                var BuscarID = await _db.Empleados
                    .AsNoTracking()
                    .Where(e => e.EmpleadoID == id)
                    .Select(e => new
                    {
                        empleadoId = e.EmpleadoID,
                        e.Nombre,
                        e.Apellido_Paterno,
                        e.Apellido_Materno,
                        e.Sexo,
                        e.Celular,
                        e.Correo,
                        e.CURP,
                        e.RFC,
                        e.Direccion,
                        e.NExterior,
                        e.NInterior,
                        e.Colonia,
                        e.Municipio,
                        e.Estado,
                        e.CodigoPostal


                    }).ToListAsync();
                return Ok(BuscarID);
            }

            // 2) Si trae varias palabras, permitir buscar "Nombre Completo"

            var tokens = q
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(t => t.Length > 0)
                .ToArray();

            var query = _db.Empleados.AsNoTracking().AsQueryable();

            // 3) Filtro base: campos individuales y "nombre completo"
            query = query.Where(e =>
                (e.Nombre ?? "").Contains(q) ||
                (e.Apellido_Paterno ?? "").Contains(q) ||
                (e.Apellido_Materno ?? "").Contains(q) ||
                (e.Correo ?? "").Contains(q) ||
                (e.CURP ?? "").Contains(q) ||
                (e.RFC ?? "").Contains(q) ||
                ((e.Nombre ?? "") + " " + (e.Apellido_Paterno ?? "") + " " + (e.Apellido_Materno ?? "")).Contains(q)

                );

            // 4) Bonus: si escriben "juan perez" (tokens), que encuentre aunque no sea el string exacto
            // Esto hace que cada palabra tenga que aparecer en algún lugar del nombre completo.
            if (tokens.Length > 1)
            {
                foreach (var t in tokens)
                {
                    var token = t; // evitar capture raro
                    query = query.Where(e =>
                        ((e.Nombre ?? "") + " " + (e.Apellido_Paterno ?? "") + " " + (e.Apellido_Materno ?? "")).Contains(token)
                    );
                }
            }

            var empleados = await query
           .OrderBy(e => e.EmpleadoID)
           .Select(e => new
           {
               empleadoId = e.EmpleadoID,
               e.Nombre,
               e.Apellido_Paterno,
               e.Apellido_Materno,
               e.Sexo,
               e.Celular,
               e.Correo,
               e.CURP,
               e.RFC,
               e.Direccion,
               e.NExterior,
               e.NInterior,
               e.Colonia,
               e.Municipio,
               e.Estado,
               e.CodigoPostal

           }).ToListAsync();

            return Ok(empleados);

        }

        //****************************************************************************************


        [HttpGet("ObtenerDatosPorId/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerDatosPorId(int id)
        {
            var empleado = await _db.Empleados
                .AsNoTracking()
                .Where(e => e.EmpleadoID == id)
                .Select(e => new Empleado {
                    EmpleadoID = e.EmpleadoID,
                    Nombre = e.Nombre,
                    Apellido_Paterno = e.Apellido_Paterno,
                    Apellido_Materno = e.Apellido_Materno,
                    Sexo = e.Sexo,
                    Correo = e.Correo,
                    Celular = e.Celular,
                    CURP = e.CURP,
                    RFC = e.RFC,
                    Direccion = e.Direccion,
                    NExterior = e.NExterior,
                    NInterior = e.NInterior,
                    Municipio = e.Municipio,
                    Colonia = e.Colonia,
                    Estado = e.Estado,
                    CodigoPostal = e.CodigoPostal

                }).FirstOrDefaultAsync();

            if (empleado == null)
            {
                return NotFound(new { mesage = "Empleado no encontrado" });
            }

            return Ok(empleado);
        }

        [HttpPut("EditarDatosPorId/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditarDatosPorId([FromRoute] int id, [FromBody] EmpleadoCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "El nombre es obligatorio." });

            if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                return BadRequest(new { message = "El apellido paterno es obligatorio." });

            if (string.IsNullOrWhiteSpace(dto.ApellidoMaterno))
                return BadRequest(new { message = "El apellido materno es obligatorio." });

            //Buscar el empleado en la base de datos
            var empleado = await _db.Empleados.FirstOrDefaultAsync(e => e.EmpleadoID == id);
            if (empleado == null)
                return NotFound(new { message = "Empleado no encontrado." });

            //convertir celular a int (DTO trae string)
            long? celular = null;
            if (!string.IsNullOrWhiteSpace(dto.NumeroCelular))
            {
                if (!long.TryParse(dto.NumeroCelular, out var celParsed))
                    return BadRequest(new { message = "Número de celular inválido (solo números)." });
                celular = celParsed;
            }

            //Evitar duplicado de correo
            if(!string.IsNullOrWhiteSpace(dto.CorreoElectronico))
            {
                var existeCorreo = await _db.Empleados.AnyAsync(e =>
                e.Correo == dto.CorreoElectronico && e.EmpleadoID != id);

                if (existeCorreo)
                    return Conflict(new { message = "Ya existe un empleado con ese correo." });
            }


            // 5) Mapear DTO -> Entity
            empleado.Nombre = dto.Nombre;
            empleado.Apellido_Paterno = dto.ApellidoPaterno;
            empleado.Apellido_Materno = dto.ApellidoMaterno;
            empleado.Sexo = dto.Sexo;
            empleado.Correo = dto.CorreoElectronico;
            empleado.Celular = celular;

            empleado.CURP = dto.CURP;
            empleado.RFC = dto.RFC;
            empleado.Direccion = dto.Direccion;
            empleado.NExterior = dto.NExterior;
            empleado.NInterior = dto.NInterior;
            empleado.Colonia = dto.Colonia;
            empleado.Municipio = dto.Municipio;
            empleado.Estado = dto.Estado;
            empleado.CodigoPostal = dto.CodigoPostal; 

            //Guardar
            await _db.SaveChangesAsync();

            return Ok(new { message = "Empleado actualizado correctamente", empleadoId = empleado.EmpleadoID });

        }


        //****************************************************************************************



    }
}
