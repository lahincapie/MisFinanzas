using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MisFinanzas.Application.Reports.Interfaces;

namespace MisFinanzas.API.Controllers
{
    /// <summary>Reportes de gasto real por categoría y por gasto, en un rango de meses.</summary>
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportsController : ApiControllerBase
    {
        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        [HttpGet("by-category")]
        public async Task<IActionResult> ByCategory([FromQuery] string from, [FromQuery] string to)
        {
            var data = await _service.GetSpendByCategoryAsync(from, to, CurrentUserId);
            return Ok(data);
        }

        [HttpGet("by-expense")]
        public async Task<IActionResult> ByExpense([FromQuery] string from, [FromQuery] string to)
        {
            var data = await _service.GetSpendByExpenseAsync(from, to, CurrentUserId);
            return Ok(data);
        }
    }
}