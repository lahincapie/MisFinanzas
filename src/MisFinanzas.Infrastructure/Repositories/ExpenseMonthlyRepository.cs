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

        public async Task<List<ExpenseMonthly>> GetMonthWithDetailsAsync(string month, string userId)
        {
            return await _context.ExpenseMonthlies
                .Where(m => m.IsActive
                    && m.Month == month
                    && m.Expense!.IsActive
                    && m.Expense.UserId == userId)      // ← solo los del usuario
                .Include(m => m.Expense)                // trae el gasto (nombre, DueDay, fijo/variable...)
                    .ThenInclude(e => e!.Category)      // y de ese gasto, su categoría
                .Include(m => m.Payments.Where(p => p.IsActive))   // y su pago ACTIVO (si lo tiene)
                .OrderBy(m => m.Expense!.Name)
                .ToListAsync();
        }

        public async Task<decimal> GetAveragePaymentAsync(int expenseId, int lastN = 3)
        {
            var lastPayments = await _context.ExpensePayments
                .Where(p => p.IsActive && p.ExpenseMonthly!.ExpenseId == expenseId)
                .OrderByDescending(p => p.PaymentDate)   // los más recientes primero
                .Take(lastN)                              // solo los últimos N
                .Select(p => p.Amount)
                .ToListAsync();

            if (lastPayments.Count == 0) return 0;        // sin histórico → 0

            return lastPayments.Average();
        }

    }
}