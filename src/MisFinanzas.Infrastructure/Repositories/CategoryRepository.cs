using Microsoft.EntityFrameworkCore;
using MisFinanzas.Application.Categories.Interfaces;
using MisFinanzas.Domain.Categories;
using MisFinanzas.Infrastructure.Persistence;
using MisFinanzas.Domain.Expenses;

namespace MisFinanzas.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación con EF Core del contrato ICategoryRepository.
    /// Aquí se ejecutan las consultas reales contra la base de datos.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MisFinanzasDbContext _context;

        public CategoryRepository(MisFinanzasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllActiveAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return await _context.Categories
                .AnyAsync(c => c.IsActive
                    && c.Name.ToLower() == name.ToLower()
                    && (excludeId == null || c.Id != excludeId.Value));
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasActiveExpensesAsync(int categoryId)
        {
            return await _context.Expenses
                .AnyAsync(e => e.IsActive && e.CategoryId == categoryId);
        }
    }
}