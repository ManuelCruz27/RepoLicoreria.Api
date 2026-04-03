using Licoreria.Api.Data.AlmacenDATA;
using Licoreria.Api.Data.VentasDATA;
using Licoreria.Api.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Licoreria.Api.Controllers.VentasControllers
{
    [Authorize(Roles = "MOSTRADOR,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class VentasControllers : Controller
    {
        private readonly LicoreriaVentasDbContext _dbVentas;
        private readonly LicoreriaAlmacenDbContext _dbAlmacen;
        public VentasControllers(LicoreriaVentasDbContext dbVentas, LicoreriaAlmacenDbContext dbAlmacen)
        {
            _dbVentas = dbVentas;
            _dbAlmacen = dbAlmacen;
        }

        [HttpGet("BuscarProductoVenta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarProductoVenta([FromQuery] string codigo, [FromQuery] int sucursalId)
        {
            try
            {
                codigo = (codigo ?? "").Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                    return BadRequest(new { message = "Debes mandar parámetro codigo." });

                var producto = await _dbAlmacen.Productos
                    .AsNoTracking()
                    .Where(p => p.Status == true &&
                                (p.CodigoBarras == codigo || p.SkucodigoInterno == codigo))
                    .Select(p => new ProductoBusqueda_TicketDto
                    {
                        ProductoId = p.ProductoId,
                        Nombre = p.Nombre,
                        CodigoBarras = p.CodigoBarras,
                        SKUCodigoInterno = p.SkucodigoInterno,
                        Descripcion = p.Descripcion,
                        PrecioVenta = p.PrecioVenta,
                        Existencia = _dbAlmacen.ALM_InventarioProductos
                            .Where(i => i.ProductoId == p.ProductoId && i.SucursalId == sucursalId)
                            .Select(i => (int?)i.StockActual)
                            .FirstOrDefault() ?? 0,
                        SucursalId = sucursalId
                    })
                    .FirstOrDefaultAsync();

                if (producto == null)
                    return NotFound(new { message = "No se encontró producto con ese código." });

                return Ok(producto);
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
    }
}
