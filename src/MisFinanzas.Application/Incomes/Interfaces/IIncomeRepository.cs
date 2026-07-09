using MisFinanzas.Domain.Incomes;

namespace MisFinanzas.Application.Incomes.Interfaces
{
    /// <summary>Contrato de acceso a datos para los ingresos (plantilla).</summary>
    public interface IIncomeRepository
    {
        /// <summary>Devuelve los ingresos activos.</summary>
        Task<List<Income>> GetAllActiveAsync();

        /// <summary>Busca un ingreso activo por su Id. Null si no existe.</summary>
        Task<Income?> GetByIdAsync(int id);

        /// <summary>Indica si ya existe un ingreso activo con ese nombre, opcionalmente excluyendo un Id.</summary>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        /// <summary>Agrega un nuevo ingreso.</summary>
        Task AddAsync(Income income);

        /// <summary>Guarda los cambios pendientes.</summary>
        Task SaveChangesAsync();
    }
}