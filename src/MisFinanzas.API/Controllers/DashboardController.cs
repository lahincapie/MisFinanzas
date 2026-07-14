using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MisFinanzas.Application.Dashboard.Interfaces;

namespace MisFinanzas.API.Controllers
{
    /// <summary>Endpoint del dashboard mensual (RF-23).</summary>
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve la foto financiera del mes: métricas reales vs proyectadas
        /// y los listados de gastos e ingresos.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMonthly([FromQuery] string month)
        {
            var dashboard = await _service.GetMonthlyAsync(month, CurrentUserId);
            return Ok(dashboard);
        }
    }
}