using Licoreria.Api.Data.AlmacenDATA;
using Licoreria.Api.Data.RecursosHumanosDATA;
using Licoreria.Api.Dto.Almacen;
using Licoreria.Api.Dto.Almacen.Inventario;
using Licoreria.Api.Entities.Almacen;
using Licoreria.Api.Entities.Almacen.Inventatio;
using Licoreria.Api.Entities.Sucursales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;

namespace Licoreria.Api.Controllers.AlmacenYmasControllers
{
    [Authorize(Roles = "ALMACEN,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class AlmacenControllers : Controller
    {
        private readonly LicoreriaAlmacenDbContext _db;
        private readonly LicoreriaDbContext _dbRH;

        public AlmacenControllers(LicoreriaAlmacenDbContext db, LicoreriaDbContext dbRH)
        {
            _db = db;
            _dbRH = dbRH;
        }

        [HttpPost("AgregarProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AgregarProducto([FromBody] ProductosInsertDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Body requerido." });

            using var tx = await _db.Database.BeginTransactionAsync();
            bool status = true;

            try
            {

                var producto = new Productos
                {
                 
                    CodigoBarras = dto.CodigoBarras,
                    SkucodigoInterno = dto.SkucodigoInterno,
                    Nombre = dto.Nombre,
                    MarcaId = dto.MarcaId,
                    Descripcion = dto.Descripcion,
                    ContenidoMl = dto.ContenidoML,
                    GraduacionAlcohol = dto.GraduacionAlcohol,
                    Presentacion = dto.Presentacion,
                    CategoriaId = dto.CategoriaId,
                    PrecioCompra = dto.PrecioCompra,
                    PrecioVenta = dto.PrecioVenta,
                    Iva = dto.IVA,
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow,
                    Status = status
                    
                };

                _db.Productos.Add(producto);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                return Created("", new
                {
                    message = "Producto creado correctamente",
                    productoId = producto.ProductoId // cambia si tu PK se llama diferente
                });

            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                var detail = ex.InnerException?.Message ?? ex.Message;

                return StatusCode(500, new
                {
                    message = "Error al crear producto",
                    detail
                });
            }


        }

        [HttpGet("CargarCategoriasProductos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargarCategoriasProductos()
        {
            try
            {
                var datos = await _db.CategoriasProductos
                    .AsNoTracking()
                    .OrderBy(x => x.CategoriaId)
                    .Select(x => new
                    {
                        x.CategoriaId,
                        x.Nombre,
                        //x.Status
                    }).ToListAsync();
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error API \nError al consultar los datos de los AlmacenDto/CargarCategoriasProductos", detail = ex.Message });

            }
        }


        [HttpGet("CargarMarcaProductos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargarMarcaProductos()
        {
            try
            {
                var datos = await _db.MarcaProductos
                    .AsNoTracking()
                    .OrderBy(e => e.MarcaId)
                    .Select(e => new
                    {
                        e.MarcaId,
                        e.Marca

                    }).ToListAsync();
                return Ok(datos);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error API \nError al consultar los datos de los AlmacenDto/CargarMarcaProductos", detail = ex.Message });

            }
        }


        [HttpGet("CargarIVA/{SucursalId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargarIVA(int SucursalId)
        {
            try
            {
                var iva = await _dbRH.Info_Sucursal
                    .AsNoTracking()
                    .Where(s => s.SucursalId == SucursalId)
                    .Select(s => new
                    {
                        s.IVA.IVAId,
                        s.IVA.Porcentaje
                    })
                    .FirstOrDefaultAsync();

                if (iva == null)
                {
                    return NotFound(new { message = "Sucursal o IVA no encontrado." });
                }

                return Ok(iva);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error API\nError al consultar el IVA de la sucursal.",
                    detail = ex.Message
                });
            }
        }
       
        [HttpGet("VerProductos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerProductos()
        {
            try
            {
               
                var datos = await _db.Productos
                    .AsNoTracking()
                    .OrderBy(e => e.ProductoId)
                    .Where(e => e.Status == true)
                    .Select(e => new
                    {
                        e.ProductoId,
                        e.CodigoBarras,
                        e.SkucodigoInterno,
                        e.Nombre,
                        e.MarcaId,
                        e.Descripcion,
                        e.ContenidoMl,
                        e.GraduacionAlcohol,
                        e.Presentacion,
                        e.CategoriaId,
                        e.PrecioCompra,
                        e.PrecioVenta,
                        e.Iva,
                        e.Status,
                        e.FechaCreacion,
                        e.FechaActualizacion

                    }).ToListAsync();
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al obtener productos",
                    detalle = ex.Message
                });

            }
        }

        [HttpPost("Eliminar/{productoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EliminarProducto(int productoId, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                var producto = await _db.Productos.FirstOrDefaultAsync(e => e.ProductoId == productoId);
                
                if(producto == null)
                    return NotFound(new { message = "Producto no encontrado." });

                var imagen = await _db.ProductoImagenes
                    .Where(i => i.ProductoId == productoId)
                    .ToListAsync();

                producto.Status = dto.Status;
                producto.FechaActualizacion = DateTime.Now;

                if (imagen.Any())
                {
                    _db.ProductoImagenes.RemoveRange(imagen);
                }

                await _db.SaveChangesAsync();
                return Ok(
                    new 
                    {
                        message = "Producto actualizado (baja lógica) e imágenes eliminadas.",
                        productoId,
                        status = producto.Status,
                        imagenesEliminadas = imagen.Count
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error API.\n Error al eliminar el producto y sus imágenes.",
                    detail = ex.Message });

            }

        }

        [HttpGet("EditarProducto/{productoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarProducto(int productoId)
        {
            try
            {
                var datos = await _db.Productos
                    .AsNoTracking()
                    .Where(e => e.ProductoId == productoId)
                    .Select(e => new
                    {
                        e.Nombre,
                        e.MarcaId,
                        e.Descripcion,
                        e.ContenidoMl,
                        e.GraduacionAlcohol,
                        e.Presentacion,
                        e.CategoriaId,
                        e.PrecioCompra,
                        e.PrecioVenta,
                        e.Iva,
                        e.FechaActualizacion

                    }).FirstOrDefaultAsync();

                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error API \nError al consultar los datos de los AlmacenDto/CargarCategoriasProductos", detail = ex.Message });
            }
        }

        [HttpGet("BuscarProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarProducto([FromQuery] string q,[FromQuery] string tipo)
        {
            q = (q ?? "").Trim();
            tipo = (tipo ?? "").Trim().ToLower();

            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Debes mandar parámetro q." });

            if (string.IsNullOrWhiteSpace(tipo))
                return BadRequest(new { message = "Debes mandar un tipo de busqueda" });


            var query = _db.Productos.AsNoTracking()
                .AsNoTracking()
                 .Where(e => e.Status == true)
                 .AsQueryable();


            switch (tipo)
            {
                case "productoid":
                    if (!int.TryParse(q, out var ProductoID))
                        return BadRequest(new { message = "El ProductoId debe ser numérico." });

                    query = query.Where(p => p.ProductoId == ProductoID);
                    break;

                case "codigobarras":
                    query = query.Where(p => p.CodigoBarras == q);
                    break;
                case "skucodigo":
                    query = query.Where(p => p.SkucodigoInterno == q);
                    break;
                case "nombre":
                    query = query.Where(p => p.Nombre.Contains(q));
                    break;
                default:
                    return BadRequest(new { message = "Tipo de búsqueda no válida." });
            }

            var datos = await query
                .Select(e => new
                {
                    e.ProductoId,
                    e.CodigoBarras,
                    e.SkucodigoInterno,
                    e.Nombre,
                    e.MarcaId,
                    e.Descripcion,
                    e.ContenidoMl,
                    e.GraduacionAlcohol,
                    e.Presentacion,
                    e.CategoriaId,
                    e.PrecioCompra,
                    e.PrecioVenta,
                    e.Iva,
                    e.Status,
                    e.FechaCreacion,
                    e.FechaActualizacion

                }).ToListAsync();
            return Ok(datos);


        }

        [HttpGet("ObtenerDatosPorId/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObtenerDatosPorId(int id)
        {
            try
            {
                var datos = await _db.Productos
                    .AsNoTracking()
                    .Where(e => e.ProductoId == id)
                    .Select(e => new
                    {
                        e.ProductoId,
                        e.CodigoBarras,
                        e.SkucodigoInterno,
                        e.Nombre,
                        e.MarcaId,
                        e.Descripcion,
                        e.ContenidoMl,
                        e.GraduacionAlcohol,
                        e.Presentacion,
                        e.CategoriaId,
                        e.PrecioCompra,
                        e.PrecioVenta,
                        e.Iva,
                        e.Status,
                        e.FechaCreacion,
                        e.FechaActualizacion

                    }).FirstOrDefaultAsync();
                return Ok(datos);
                

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error Api \nError al consultar los datos de los AlmacenDto/CargarCategoriasProductos", detail = ex.Message });
            }
    
        }

        [HttpPut("EditarProducto/{productoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditarProducto([FromRoute] int productoId, [FromBody] ProductosUpDateDto dto)
        {
            try
            {
                var productos = await _db.Productos.FirstOrDefaultAsync(e => e.ProductoId == productoId);
                if (productos == null)
                    return NotFound(new { message = "Producto no encontrado." });

                productos.Nombre = dto.Nombre;
                productos.MarcaId = dto.MarcaId;
                productos.Descripcion = dto.Descripcion;
                productos.ContenidoMl = dto.ContenidoML;
                productos.GraduacionAlcohol = dto.GraduacionAlcohol;
                productos.Presentacion = dto.Presentacion;
                productos.CategoriaId = dto.CategoriaId;
                productos.PrecioCompra = dto.PrecioCompra;
                productos.PrecioVenta = dto.PrecioVenta;
                productos.Iva = dto.IVA;

                productos.FechaActualizacion = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Ok(new { message = "Producto actualizado correctamente", productoId = productos.ProductoId});

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al obtener productos",
                    detalle = ex.Message
                });
            }
        }



        //************************ Inventario *****************************************+

        [HttpGet("CargarDatos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargarDatos([FromQuery] string q)
        {
            try
            {
                q = (q ?? "").Trim();

                if (string.IsNullOrWhiteSpace(q))
                    return BadRequest(new { message = "Debes mandar un parámetro q." });

                var baseQuery = _db.Productos
                    .AsNoTracking()
                    .Where(p => p.Status == true);

                var porCodigoBarras = await baseQuery
                    .Where(p => p.CodigoBarras == q)
                    .Select(p => new
                    {
                        p.ProductoId,
                        p.Nombre,
                        Inventario = _db.ALM_InventarioProductos
                            .AsNoTracking()
                            .Where(i => i.ProductoId == p.ProductoId)
                            .Select(i => new
                            {
                                i.StockActual,
                                i.StockMinimo
                            })
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                if (porCodigoBarras.Any())
                    return Ok(porCodigoBarras);

                if (int.TryParse(q, out var productoId))
                {
                    var porId = await baseQuery
                        .Where(p => p.ProductoId == productoId)
                        .Select(p => new
                        {
                            p.ProductoId,
                            p.Nombre,
                            Inventario = _db.ALM_InventarioProductos
                                .AsNoTracking()
                                .Where(i => i.ProductoId == p.ProductoId)
                                .Select(i => new
                                {
                                    i.StockActual,
                                    i.StockMinimo
                                })
                                .FirstOrDefault()
                        })
                        .ToListAsync();

                    if (porId.Any())
                        return Ok(porId);
                }

                return NotFound(new { message = "No se encontraron productos con ese criterio" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error API\nError al cargar los datos",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("MovimientoInventario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MovimientoInventario([FromBody] MovimientoInventarioInsertDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Body requerido." });

            if (dto.ProductoId <= 0)
                return BadRequest(new { message = "ProductoId inválido." });

            if (dto.SucursalId <= 0)
                return BadRequest(new { message = "SucursalId inválido." });

            if (string.IsNullOrWhiteSpace(dto.TipoMovimiento))
                return BadRequest(new { message = "El tipo de movimiento es requerido." });

            if (dto.Cantidad <= 0)
                return BadRequest(new { message = "La cantidad debe ser mayor a 0." });

            if (string.IsNullOrWhiteSpace(dto.Usuario))
                return BadRequest(new { message = "El usuario es requerido." });

            try
            {
                var usuario = await _dbRH.GestionUsuario
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Usuario == dto.Usuario);

                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado." });

                var producto = await _db.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductoId == dto.ProductoId && p.Status == true);

                if (producto == null)
                    return NotFound(new { message = "Producto no encontrado o inactivo." });

                var tipoMovimiento = dto.TipoMovimiento.Trim().ToUpper();

                if (tipoMovimiento != "ENTRADA" && tipoMovimiento != "SALIDA" && tipoMovimiento != "AJUSTE")
                    return BadRequest(new { message = "TipoMovimiento inválido. Usa 'ENTRADA' o 'SALIDA'." });

                await using var transaction = await _db.Database.BeginTransactionAsync();

                var inventario = await _db.ALM_InventarioProductos
                    .FirstOrDefaultAsync(i =>
                        i.ProductoId == dto.ProductoId &&
                        i.SucursalId == dto.SucursalId);

                if (inventario == null)
                {
                    inventario = new ALM_InventarioProductos
                    {
                        ProductoId = dto.ProductoId,
                        SucursalId = dto.SucursalId,
                        StockActual = 0,
                        StockMinimo = dto.StockMinimo,
                        FechaActualizacion = DateTime.UtcNow
                    };

                    _db.ALM_InventarioProductos.Add(inventario);
                }

                int nuevoStock;
                int diferenciaAjuste = 0;
                if (tipoMovimiento == "ENTRADA")
                {
                    nuevoStock = inventario.StockActual + dto.Cantidad;
                }
                else if (tipoMovimiento == "SALIDA")
                {
                    if (inventario.StockActual < dto.Cantidad)
                    {
                        return BadRequest(new
                        {
                            message = "No hay stock suficiente para realizar la salida.",
                            stockActual = inventario.StockActual,
                            cantidadSolicitada = dto.Cantidad
                        });
                    }

                    nuevoStock = inventario.StockActual - dto.Cantidad;
                }
                else if (tipoMovimiento == "AJUSTE")
                {
                    diferenciaAjuste = dto.Cantidad - inventario.StockActual;
                    nuevoStock = inventario.StockActual + dto.Cantidad;
                   
                    if (nuevoStock < 0)
                    {
                        return BadRequest(new
                        {
                            message = "El ajuste no puede dejar el stock en negativo.",
                            stockActual = inventario.StockActual,
                            ajuste = dto.Cantidad
                        });
                    }

                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Tipo de movimiento inválido. Usa ENTRADA, SALIDA o AJUSTE."
                    });
                }


                var movimientoInventario = new ALM_MovimientosInventario
                {
                    ProductoId = dto.ProductoId,
                    SucursalId = dto.SucursalId,
                    TipoMovimiento = tipoMovimiento,
                    Cantidad = dto.Cantidad,
                    Referencia = dto.Referencia,
                    UsuarioID = usuario.UsuarioId,
                    FechaMovimiento = DateTime.UtcNow
                };

                _db.ALM_MovimientosInventarios.Add(movimientoInventario);

                inventario.StockActual = nuevoStock;
                inventario.StockMinimo = dto.StockMinimo;
                inventario.FechaActualizacion = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Movimiento de inventario registrado correctamente.",
                    productoId = inventario.ProductoId,
                    sucursalId = inventario.SucursalId,
                    tipoMovimiento,
                    cantidad = dto.Cantidad,
                    stockActual = inventario.StockActual,
                    stockMinimo = inventario.StockMinimo
                });

            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al guardar en base de datos.",
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error general en API.",
                    detail = ex.Message
                });
            }
        }
        //************************* Imagenes ********************************************
        [HttpPost("AgregarImagen")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AgregarImagen([FromBody] ProductoImagenInsertDto dto)
        {
            try
            {
                var imagen = new ProductoImagenes
                {
                    ProductoId = dto.ProductoId,
                    Ruta = dto.Ruta,
                    Orden = dto.Orden ,
                    EsPrincipal = dto.EsPrincipal,
                    FechaCreacion = DateTime.UtcNow
                };

                _db.ProductoImagenes.Add(imagen);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Imagen registrada", imagenId = imagen.ImagenId });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar imagen", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("ProductosConImagenPrincipal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProductosConImagenPrincipal()
        {
            try
            {
                var datos = await _db.Productos
                    .AsNoTracking()
                    .Where(p => p.Status)
                    .OrderBy(p => p.ProductoId)
                    .Select(p => new
                    {
                        p.ProductoId,
                        p.Nombre,
                        p.Descripcion,
                        Ruta = _db.ProductoImagenes
                            .AsNoTracking()
                            .Where(i => i.ProductoId == p.ProductoId && i.EsPrincipal)
                            .Select(i => i.Ruta)
                            .FirstOrDefault() // ✅ IMPORTANTÍSIMO
                    })
                    .ToListAsync();

                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error API\nError al cargar los datos.", detail = ex.Message });
            }
        }


        [HttpGet("ProductoDetalleImagenes/{productoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProductoDetalleImagenes(int productoId)
        {
            try
            {
               var producto = await _db.Productos
               .AsNoTracking()
               .Where(p => p.ProductoId == productoId && p.Status == true)
               .Select(p => new ProductoDetalleDto
               {
                   ProductoId = p.ProductoId,
                   Nombre = p.Nombre,
                   CodigoBarras = p.CodigoBarras,
                   SKUCodigoInterno = p.SkucodigoInterno,
                   Descripcion = p.Descripcion,
                   ContenidoML = p.ContenidoMl,
                   GraduacionAlcohol = p.GraduacionAlcohol,
                   Presentacion = p.Presentacion,
                   PrecioVenta = p.PrecioVenta,
                   FechaActualizacion = p.FechaActualizacion,
               })
               .FirstOrDefaultAsync();

                    if (producto == null)
                    {
                        return NotFound(new
                        {
                            message = "Producto no encontrado o inactivo."
                        });
                    }

                producto.Imagenes = await _db.ProductoImagenes
                    .AsNoTracking()
                    .Where(i => i.ProductoId == productoId)
                    .OrderBy(i => i.Orden)
                    .Select(i => new ProductoImagenDto
                    {
                        ImagenId = i.ImagenId,
                        Ruta = i.Ruta,
                        Orden = i.Orden, 
                        EsPrincipal = i.EsPrincipal
                    })
                    .ToListAsync();

                return Ok(producto);


            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error API al obtener el detalle del producto.",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("BuscarImagenProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarImagenProducto([FromQuery] string q)
        {
            try
            {
                q = (q ?? "").Trim();

                if (string.IsNullOrWhiteSpace(q))
                    return BadRequest(new { message = "Debes mandar un parámetro q." });

                var BaseQuery = _db.Productos
                    .AsNoTracking()
                    .Where(e => e.Status == true);

                var porCodigoBarras = await BaseQuery
                    .Where(p => p.CodigoBarras == q)
                    .Select(e => new
                    {
                        e.ProductoId,
                        e.Nombre,
                        e.Descripcion,
                        Ruta = _db.ProductoImagenes
                        .AsNoTracking()
                        .Where(i => i.ProductoId == e.ProductoId && i.EsPrincipal)
                        .Select(i => i.Ruta)
                        .FirstOrDefault()
                    }).ToListAsync();

                if (porCodigoBarras.Any())
                    return Ok(porCodigoBarras);

                
                if(int.TryParse(q,out var productoId))
                {
                    var porId = await BaseQuery
                        .Where(p => p.ProductoId == productoId)
                        .Select(e => new
                        {
                            e.ProductoId,
                            e.Nombre,
                            e.Descripcion,
                            Ruta = _db.ProductoImagenes
                                   .AsNoTracking()
                                   .Where(i => i.ProductoId == e.ProductoId && i.EsPrincipal)
                                   .Select(i => i.Ruta)
                                   .FirstOrDefault()

                        }).ToListAsync();

                    if (porId.Any())
                        return Ok(porId);
                }

                return NotFound(new { message = "No se encontraron productos con ese criterio." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error API\nError al cargar los datos.", detail = ex.Message });
            }

        }




    }
}
