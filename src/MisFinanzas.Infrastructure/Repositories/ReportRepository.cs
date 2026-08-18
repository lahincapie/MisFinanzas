using Microsoft.EntityFrameworkCore;
using MisFinanzas.Application.Reports.Dtos;
using MisFinanzas.Application.Reports.Interfaces;
using MisFinanzas.Infrastructure.Persistence;

namespace MisFinanzas.Infrastructure.Repositories
{
    /// <summary>
    /// Consultas de agregación (EF Core) para los reportes de gasto real.
    /// Suma los pagos ACTIVOS cuyo mes cae en [from, to], del usuario.
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        private readonly MisFinanzasDbContext _context;

        public ReportRepository(MisFinanzasDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategorySpendDto>> GetSpendByCategoryAsync(string from, string to, string userId)
        {
            return await _context.ExpensePayments
                .Where(p => p.IsActive
                    && p.ExpenseMonthly!.IsActive
                    && string.Compare(p.ExpenseMonthly.Month, from) >= 0
                    && string.Compare(p.ExpenseMonthly.Month, to) <= 0
                    && p.ExpenseMonthly.Expense!.IsActive
                    && p.ExpenseMonthly.Expense.UserId == userId)
                .GroupBy(p => p.ExpenseMonthly!.Expense!.Category!.Name)
                .Select(g => new CategorySpendDto
                {
                    CategoryName = g.Key!,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(r => r.Total)
                .ToListAsync();
        }

        public async Task<List<ExpenseSpendDto>> GetSpendByExpenseAsync(string from, string to, string userId)
        {
            return await _context.ExpensePayments
                .Where(p => p.IsActive
                    && p.ExpenseMonthly!.IsActive
                    && string.Compare(p.ExpenseMonthly.Month, from) >= 0
                    && string.Compare(p.ExpenseMonthly.Month, to) <= 0
                    && p.ExpenseMonthly.Expense!.IsActive
                    && p.ExpenseMonthly.Expense.UserId == userId)
                .GroupBy(p => new
                {
                    p.ExpenseMonthly!.Expense!.Id,
                    p.ExpenseMonthly.Expense.Name,
                    CategoryName = p.ExpenseMonthly.Expense.Category!.Name
                })
                .Select(g => new ExpenseSpendDto
                {
                    ExpenseId = g.Key.Id,
                    ExpenseName = g.Key.Name,
                    CategoryName = g.Key.CategoryName,
                    Total = g.Sum(x => x.Amount)
                })
                .OrderByDescending(r => r.Total)
                .ToListAsync();
        }
    }
}