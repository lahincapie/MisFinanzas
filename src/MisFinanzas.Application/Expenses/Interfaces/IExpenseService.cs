using MisFinanzas.Application.Expenses.Dtos;

namespace MisFinanzas.Application.Expenses.Interfaces
{
    /// <summary>Contrato del servicio de gastos: los casos de uso disponibles.</summary>
    public interface IExpenseService
    {
        /// <summary>Crea un gasto y devuelve el Id generado.</summary>
        Task<int> CreateAsync(CreateExpenseDto dto, string userId);

        /// <summary>Devuelve todos los gastos activos.</summary>
        Task<List<ExpenseDto>> GetAllAsync(string userId);

        /// <summary>Edita un gasto existente.</summary>
        Task UpdateAsync(UpdateExpenseDto dto, string userId);

        /// <summary>Desactiva (soft-delete) un gasto.</summary>
        Task DeactivateAsync(int id, string userId);

    }
}