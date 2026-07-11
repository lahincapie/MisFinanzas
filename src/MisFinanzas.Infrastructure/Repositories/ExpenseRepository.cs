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

        public async Task<List<Expense>> GetAllActiveAsync(string userId)
        {
            return await _context.Expenses
                .Where(e => e.IsActive && e.UserId == userId)   // ← filtro por dueño
                .Include(e => e.Category)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Expense?> GetByIdAsync(int id, string userId)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive && e.UserId == userId);
        }

        public async Task<bool> ExistsByNameAsync(string name, string userId, int? excludeId = null)
        {
            return await _context.Expenses
                .AnyAsync(e => e.IsActive
                    && e.UserId == userId
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