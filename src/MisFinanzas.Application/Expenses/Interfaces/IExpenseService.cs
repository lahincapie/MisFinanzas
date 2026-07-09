using MisFinanzas.Application.Expenses.Dtos;

namespace MisFinanzas.Application.Expenses.Interfaces
{
    /// <summary>Contrato del servicio de gastos: los casos de uso disponibles.</summary>
    public interface IExpenseService
    {
        /// <summary>Crea un gasto y devuelve el Id generado.</summary>
        Task<int> CreateAsync(CreateExpenseDto dto);

        /// <summary>Devuelve todos los gastos activos.</summary>
        Task<List<ExpenseDto>> GetAllAsync();

    }
}