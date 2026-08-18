using MisFinanzas.Application.Reports.Dtos;

namespace MisFinanzas.Application.Reports.Interfaces
{
    /// <summary>Consultas de agregación para los reportes de gasto real.</summary>
    public interface IReportRepository
    {
        Task<List<CategorySpendDto>> GetSpendByCategoryAsync(string from, string to, string userId);
        Task<List<ExpenseSpendDto>> GetSpendByExpenseAsync(string from, string to, string userId);
    }
}