using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MisFinanzas.Application.PaymentMethods.Dtos;
using MisFinanzas.Application.PaymentMethods.Interfaces;

namespace MisFinanzas.API.Controllers
{
    /// <summary>Catálogo de medios de pago (solo lectura).</summary>
    [ApiController]
    [Route("api/payment-methods")]
    [Authorize]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentMethodRepository _repository;

        public PaymentMethodsController(IPaymentMethodRepository repository)
        {
            _repository = repository;
        }

        /// <summary>Lista los medios de pago activos.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var methods = await _repository.GetAllActiveAsync();
            var dtos = methods.Select(m => new PaymentMethodDto { Id = m.Id, Name = m.Name }).ToList();
            return Ok(dtos);
        }
    }
}