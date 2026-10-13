using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MisFinanzas.Application.Incomes.Dtos;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Application.OneOff.Interfaces;

namespace MisFinanzas.API.Controllers
{
    /// <summary>Endpoints REST para gestionar los ingresos.</summary>
    [ApiController]
    [Route("api/incomes")]
    [Authorize]   // ← ahora este controller exige token válido
    public class IncomesController : ApiControllerBase
    {
        private readonly IIncomeService _service;
        private readonly IIncomeMonthlyService _monthlyService;
        private readonly IIncomeReceiptService _receiptService;
        private readonly IOneOffService _oneOffService;

        public IncomesController(IIncomeService service,
            IIncomeMonthlyService monthlyService,
            IIncomeReceiptService receiptService,
            IOneOffService oneOffService)
        {
            _service = service;
            _monthlyService = monthlyService;
            _receiptService = receiptService;
            _oneOffService = oneOffService;
        }

        /// <summary>Crea un nuevo ingreso.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIncomeDto dto)
        {
            var newId = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(Create), new { id = newId }, new { id = newId });
        }

        /// <summary>Devuelve todos los ingresos activos.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var incomes = await _service.GetAllAsync(CurrentUserId);
            return Ok(incomes);
        }

        /// <summary>Edita un ingreso existente.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIncomeDto dto)
        {
            dto.Id = id;
            await _service.UpdateAsync(dto, CurrentUserId);
            return NoContent();
        }

        /// <summary>Desactiva (soft-delete) un ingreso.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _service.DeactivateAsync(id, CurrentUserId);
            return NoContent();
        }

        /// <summary>
        /// Genera los registros mensuales pendientes del mes indicado ("YYYY-MM")
        /// para los ingresos que aplican y aún no lo tienen.
        /// </summary>
        [HttpPost("generate-monthly")]
        public async Task<IActionResult> GenerateMonthly([FromQuery] string month)
        {
            var created = await _monthlyService.GenerateForMonthAsync(month, CurrentUserId);
            return Ok(new { month, created });
        }

        /// <summary>Registra un ingreso único (real) en el mes actual.</summary>
        [HttpPost("one-off")]
        public async Task<IActionResult> CreateOneOff([FromBody] CreateOneOffIncomeDto dto)
        {
            await _oneOffService.CreateOneOffIncomeAsync(dto, CurrentUserId);
            return NoContent();
        }

        /// <summary>Registra la recepción de un ingreso en un mes (lo marca como Recibido).</summary>
        [HttpPost("{id}/months/{month}/receive")]
        public async Task<IActionResult> Receive(
            int id, string month, [FromBody] RegisterIncomeReceiptDto dto)
        {
            await _receiptService.RegisterReceiptAsync(id, month, dto, CurrentUserId);
            return NoContent();
        }

        /// <summary>Revierte la recepción de un ingreso en un mes (lo devuelve a Pendiente).</summary>
        [HttpPost("{id}/months/{month}/revert")]
        public async Task<IActionResult> Revert(int id, string month)
        {
            await _receiptService.RevertReceiptAsync(id, month, CurrentUserId);
            return NoContent();
        }
    }
}