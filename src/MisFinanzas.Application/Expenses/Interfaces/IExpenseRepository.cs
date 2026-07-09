using MisFinanzas.Domain.Expenses;

namespace MisFinanzas.Application.Expenses.Interfaces
{
    /// <summary>
    /// Contrato de acceso a datos para los gastos (plantilla). La implementación
    /// con EF Core vive en Infrastructure.
    /// </summary>
    public interface IExpenseRepository
    {
        /// <summary>Devuelve los gastos activos, incluyendo su categoría.</summary>
        Task<List<Expense>> GetAllActiveAsync();

        /// <summary>Busca un gasto activo por su Id (con su categoría). Null si no existe.</summary>
        Task<Expense?> GetByIdAsync(int id);

        /// <summary>Indica si ya existe un gasto activo con ese nombre, opcionalmente excluyendo un Id.</summary>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        /// <summary>Agrega un nuevo gasto.</summary>
        Task AddAsync(Expense expense);

        /// <summary>Guarda los cambios pendientes en la base de datos.</summary>
        Task SaveChangesAsync();
    }
}