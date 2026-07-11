using MisFinanzas.Application.Categories.Dtos;

namespace MisFinanzas.Application.Categories.Interfaces
{
    /// <summary>
    /// Contrato del servicio de categorías: los casos de uso disponibles.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>Crea una categoría y devuelve el Id generado.</summary>
        Task<int> CreateAsync(CreateCategoryDto dto, string userId);
        
        /// <summary>Devuelve todas las categorías activas.</summary>
        Task<List<CategoryDto>> GetAllAsync(string userId);

        /// <summary>Edita una categoría existente.</summary>
        Task UpdateAsync(UpdateCategoryDto dto, string userId);

        /// <summary>Desactiva (soft-delete) una categoría.</summary>
        Task DeactivateAsync(int id, string userId);
    }
}