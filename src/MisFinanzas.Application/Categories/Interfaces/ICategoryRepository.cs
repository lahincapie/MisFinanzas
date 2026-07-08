using MisFinanzas.Domain.Categories;

namespace MisFinanzas.Application.Categories.Interfaces
{
    /// <summary>
    /// Contrato de acceso a datos para las categorías. Define QUÉ operaciones
    /// necesita la aplicación; la implementación concreta (con EF Core) vive
    /// en la capa Infrastructure.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>Devuelve las categorías activas del usuario.</summary>
        Task<List<Category>> GetAllActiveAsync();

        /// <summary>Busca una categoría activa por su Id. Devuelve null si no existe.</summary>
        Task<Category?> GetByIdAsync(int id);

        /// <summary>Indica si ya existe una categoría activa con ese nombre (para validar duplicados).</summary>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>Agrega una nueva categoría.</summary>
        Task AddAsync(Category category);

        /// <summary>Marca cambios pendientes y los guarda en la base de datos.</summary>
        Task SaveChangesAsync();
    }
}