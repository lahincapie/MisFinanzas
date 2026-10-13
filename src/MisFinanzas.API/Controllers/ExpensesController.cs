using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MisFinanzas.Application.Expenses.Dtos;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Application.OneOff.Interfaces;

namespace MisFinanzas.API.Controllers
{
    /// <summary>Endpoints REST para gestionar los gastos.</summary>
    [ApiController]
    [Route("api/expenses")]
    [Authorize]   // ← ahora este controller exige token válido
    public class ExpensesController : ApiControllerBase
    {
        private readonly IExpenseService _service;
        private readonly IExpenseMonthlyService _monthlyService;
        private readonly IExpensePaymentService _paymentService;
        private readonly IOneOffService _oneOffService;

        public ExpensesController(IExpenseService service, 
            IExpenseMonthlyService monthlyService,
            IExpensePaymentService paymentService,
            IOneOffService oneOffService)
        {
            _service = service;
            _monthlyService = monthlyService;
            _paymentService = paymentService;
            _oneOffService = oneOffService;
        }

        /// <summary>Crea un nuevo gasto.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
        {
            var newId = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(Create), new { id = newId }, new { id = newId });
        }

        /// <summary>Devuelve todos los gastos activos.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var expenses = await _service.GetAllAsync(CurrentUserId);
            return Ok(expenses);
        }

        /// <summary>Edita un gasto existente.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto dto)
        {
            dto.Id = id;
            await _service.UpdateAsync(dto, CurrentUserId);
            return NoContent();
        }

        /// <summary>Desactiva (soft-delete) un gasto.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _service.DeactivateAsync(id, CurrentUserId);
            return NoContent();
        }

        /// <summary>
        /// Genera los registros mensuales pendientes del mes indicado ("YYYY-MM")
        /// para los gastos que aplican y aún no lo tienen.
        /// </summary>
        [HttpPost("generate-monthly")]
        public async Task<IActionResult> GenerateMonthly([FromQuery] string month)
        {
            var created = await _monthlyService.GenerateForMonthAsync(month, CurrentUserId);
            return Ok(new { month, created });
        }

        /// <summary>Registra el pago de un gasto en un mes (lo marca como Pagado).</summary>
        [HttpPost("{id}/months/{month}/pay")]
        public async Task<IActionResult> Pay(
            int id, string month, [FromBody] RegisterExpensePaymentDto dto)
        {
            await _paymentService.RegisterPaymentAsync(id, month, dto, CurrentUserId);
            return NoContent();
        }

        /// <summary>Registra un gasto único (real) en el mes actual.</summary>
        [HttpPost("one-off")]
        public async Task<IActionResult> CreateOneOff([FromBody] CreateOneOffExpenseDto dto)
        {
            await _oneOffService.CreateOneOffExpenseAsync(dto, CurrentUserId);
            return NoContent();
        }

        /// <summary>Revierte el pago de un gasto en un mes (lo devuelve a Pendiente).</summary>
        [HttpPost("{id}/months/{month}/revert")]
        public async Task<IActionResult> Revert(int id, string month)
        {
            await _paymentService.RevertPaymentAsync(id, month, CurrentUserId);
            return NoContent();
        }

        /// <summary>Omite un gasto en un mes (lo marca como Omitido).</summary>
        [HttpPost("{id}/months/{month}/skip")]
        public async Task<IActionResult> Skip(int id, string month)
        {
            await _monthlyService.SkipMonthAsync(id, month, CurrentUserId);
            return NoContent();
        }

        /// <summary>Reactiva un gasto omitido en un mes (lo devuelve a Pendiente).</summary>
        [HttpPost("{id}/months/{month}/unskip")]
        public async Task<IActionResult> Unskip(int id, string month)
        {
            await _monthlyService.UnskipMonthAsync(id, month, CurrentUserId);
            return NoContent();
        }
    }
}