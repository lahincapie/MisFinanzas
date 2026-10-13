using MisFinanzas.Application.Expenses.Dtos;
using MisFinanzas.Application.Incomes.Dtos;

namespace MisFinanzas.Application.OneOff.Interfaces
{
    /// <summary>Crea movimientos únicos (no recurrentes) ya reales en el mes actual.</summary>
    public interface IOneOffService
    {
        Task CreateOneOffExpenseAsync(CreateOneOffExpenseDto dto, string userId);
        Task CreateOneOffIncomeAsync(CreateOneOffIncomeDto dto, string userId);
    }
}