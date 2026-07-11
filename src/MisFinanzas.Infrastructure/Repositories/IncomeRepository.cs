using Microsoft.EntityFrameworkCore;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Domain.Incomes;
using MisFinanzas.Infrastructure.Persistence;

namespace MisFinanzas.Infrastructure.Repositories
{
    /// <summary>Implementación con EF Core del contrato IIncomeRepository.</summary>
    public class IncomeRepository : IIncomeRepository
    {
        private readonly MisFinanzasDbContext _context;

        public IncomeRepository(MisFinanzasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Income>> GetAllActiveAsync(string userId)
        {
            return await _context.Incomes
                .Where(i => i.IsActive && i.UserId == userId)   // ← filtro por dueño
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<Income?> GetByIdAsync(int id, string userId)
        {
            return await _context.Incomes
                .FirstOrDefaultAsync(i => i.Id == id && i.IsActive && i.UserId == userId);
        }

        public async Task<bool> ExistsByNameAsync(string name, string userId, int? excludeId = null)
        {
            return await _context.Incomes
                .AnyAsync(i => i.IsActive
                    && i.UserId == userId
                    && i.Name.ToLower() == name.ToLower()
                    && (excludeId == null || i.Id != excludeId.Value));
        }

        public async Task AddAsync(Income income)
        {
            await _context.Incomes.AddAsync(income);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}