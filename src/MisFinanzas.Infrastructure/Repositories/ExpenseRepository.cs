using Microsoft.EntityFrameworkCore;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Domain.Expenses;
using MisFinanzas.Infrastructure.Persistence;

namespace MisFinanzas.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación con EF Core del contrato IExpenseRepository.
    /// </summary>
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly MisFinanzasDbContext _context;

        public ExpenseRepository(MisFinanzasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> GetAllActiveAsync()
        {
            return await _context.Expenses
                .Where(e => e.IsActive)
                .Include(e => e.Category)   // trae también la categoría
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Expense?> GetByIdAsync(int id)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return await _context.Expenses
                .AnyAsync(e => e.IsActive
                    && e.Name.ToLower() == name.ToLower()
                    && (excludeId == null || e.Id != excludeId.Value));
        }

        public async Task AddAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}