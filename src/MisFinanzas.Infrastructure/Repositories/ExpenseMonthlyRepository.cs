using Microsoft.EntityFrameworkCore;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Domain.Expenses;
using MisFinanzas.Infrastructure.Persistence;

namespace MisFinanzas.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación con EF Core del contrato IExpenseMonthlyRepository.
    /// </summary>
    public class ExpenseMonthlyRepository : IExpenseMonthlyRepository
    {
        private readonly MisFinanzasDbContext _context;

        public ExpenseMonthlyRepository(MisFinanzasDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetExistingExpenseIdsForMonthAsync(string month)
        {
            return await _context.ExpenseMonthlies
                .Where(m => m.IsActive && m.Month == month)
                .Select(m => m.ExpenseId)   // trae SOLO la columna ExpenseId
                .ToListAsync();
        }

        public async Task AddAsync(ExpenseMonthly monthly)
        {
            await _context.ExpenseMonthlies.AddAsync(monthly);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ExpenseMonthly?> GetByExpenseAndMonthAsync(int expenseId, string month)
        {
            return await _context.ExpenseMonthlies
                .FirstOrDefaultAsync(m => m.IsActive
                    && m.ExpenseId == expenseId
                    && m.Month == month);
        }

        public async Task AddPaymentAsync(ExpensePayment payment)
        {
            await _context.ExpensePayments.AddAsync(payment);
        }

        public async Task<ExpensePayment?> GetActivePaymentAsync(int expenseMonthlyId)
        {
            return await _context.ExpensePayments
                .FirstOrDefaultAsync(p => p.IsActive && p.ExpenseMonthlyId == expenseMonthlyId);
        }
    }
}