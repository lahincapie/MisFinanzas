using Microsoft.AspNetCore.Mvc;
using MisFinanzas.Application.Expenses.Dtos;
using MisFinanzas.Application.Expenses.Interfaces;

namespace MisFinanzas.API.Controllers
{
    /// <summary>Endpoints REST para gestionar los gastos.</summary>
    [ApiController]
    [Route("api/expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _service;

        public ExpensesController(IExpenseService service)
        {
            _service = service;
        }

        /// <summary>Crea un nuevo gasto.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
        {
            var newId = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Create), new { id = newId }, new { id = newId });
        }

        /// <summary>Devuelve todos los gastos activos.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var expenses = await _service.GetAllAsync();
            return Ok(expenses);
        }

        /// <summary>Edita un gasto existente.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto dto)
        {
            dto.Id = id;
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        /// <summary>Desactiva (soft-delete) un gasto.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }
    }
}