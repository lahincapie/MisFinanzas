using Microsoft.AspNetCore.Authorization;
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
    [Authorize]   // ← ahora este controller exige token válido
    public class CategoriesController : ApiControllerBase
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
            var newId = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(Create), new { id = newId }, new { id = newId });
        }

        /// <summary>Devuelve todas las categorías activas.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllAsync(CurrentUserId);
            return Ok(categories);
        }

        /// <summary>Edita una categoría existente.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            dto.Id = id;                       // el Id de la ruta es la fuente de verdad
            await _service.UpdateAsync(dto, CurrentUserId);
            return NoContent();
        }

        /// <summary>Desactiva (soft-delete) una categoría.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _service.DeactivateAsync(id, CurrentUserId);
            return NoContent();
        }
    }

}