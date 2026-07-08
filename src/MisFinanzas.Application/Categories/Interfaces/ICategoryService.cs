using MisFinanzas.Application.Categories.Dtos;

namespace MisFinanzas.Application.Categories.Interfaces
{
    /// <summary>
    /// Contrato del servicio de categorías: los casos de uso disponibles.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>Crea una categoría y devuelve el Id generado.</summary>
        Task<int> CreateAsync(CreateCategoryDto dto);
    }
}