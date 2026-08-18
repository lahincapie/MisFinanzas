using MisFinanzas.Application.Reports.Dtos;

namespace MisFinanzas.Application.Reports.Interfaces
{
    /// <summary>Servicio de reportes: gasto real por categoría y por gasto en un rango de meses.</summary>
    public interface IReportService
    {
        Task<List<CategorySpendDto>> GetSpendByCategoryAsync(string from, string to, string userId);
        Task<List<ExpenseSpendDto>> GetSpendByExpenseAsync(string from, string to, string userId);
    }
}