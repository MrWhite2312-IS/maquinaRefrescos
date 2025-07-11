using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using maquinaRefrescosBackend.Aplicacion;
using maquinaRefrescosBackend.Models;

namespace maquinaRefrescosBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendingMachineController : ControllerBase
    {

        private readonly IVendingMachineService _vendingMachineService;

        public VendingMachineController(IVendingMachineService vendingMachineService)
        {
            _vendingMachineService = vendingMachineService;
        }

        [HttpGet]
        public ActionResult<List<Bebida>> BebidasDisponibles()
        {
            var bebidas = _vendingMachineService.BebidasDisponibles();
            return Ok(bebidas);
        }

        [HttpPost]
        [Route("DevolverMonto")]
        public ActionResult<double> DevolverMonto([FromBody] List<Bebida> bebidas)
        {
            if (bebidas == null || !bebidas.Any())
            {
                return BadRequest("La lista de bebidas no puede estar vacía.");
            }
            var monto = _vendingMachineService.DevolverMonto(bebidas);
            return Ok(monto);
        }
        [HttpPut]
        public ActionResult<List<Moneda>> ComprarBebidas([FromBody] CompraReq request)
        {
            if (request.bebidas == null || !request.bebidas.Any())
            {
                return BadRequest("La lista de bebidas no puede estar vacía.");
            }
            if (request.monedas == null || !request.monedas.Any())
            {
                return BadRequest("La lista de monedas no puede estar vacía.");
            }
            try
            {
                var monedasVuelto = _vendingMachineService.ComprarBebidas(request.bebidas, request.monedas);
                return Ok(monedasVuelto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
