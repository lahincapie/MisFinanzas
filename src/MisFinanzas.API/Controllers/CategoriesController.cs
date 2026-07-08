using Microsoft.AspNetCore.Mvc;
using MisFinanzas.Application.Categories.Dtos;
using MisFinanzas.Application.Categories.Interfaces;

namespace MisFinanzas.API.Controllers
{
    /// <summary>
    /// Endpoints REST para gestionar las categorías.
    /// </summary>
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        /// <summary>Crea una nueva categoría.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var newId = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Create), new { id = newId }, new { id = newId });
        }
    }
}