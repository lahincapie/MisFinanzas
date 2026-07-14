using Microsoft.EntityFrameworkCore;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Domain.Incomes;
using MisFinanzas.Infrastructure.Persistence;

namespace MisFinanzas.Infrastructure.Repositories
{
    /// <summary>Implementación con EF Core del contrato IIncomeMonthlyRepository.</summary>
    public class IncomeMonthlyRepository : IIncomeMonthlyRepository
    {
        private readonly MisFinanzasDbContext _context;

        public IncomeMonthlyRepository(MisFinanzasDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetExistingIncomeIdsForMonthAsync(string month)
        {
            return await _context.IncomeMonthlies
                .Where(m => m.IsActive && m.Month == month)
                .Select(m => m.IncomeId)
                .ToListAsync();
        }

        public async Task AddAsync(IncomeMonthly monthly)
        {
            await _context.IncomeMonthlies.AddAsync(monthly);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IncomeMonthly?> GetByIncomeAndMonthAsync(int incomeId, string month)
        {
            return await _context.IncomeMonthlies
                .FirstOrDefaultAsync(m => m.IsActive
                    && m.IncomeId == incomeId
                    && m.Month == month);
        }

        public async Task AddReceiptAsync(IncomeReceipt receipt)
        {
            await _context.IncomeReceipts.AddAsync(receipt);
        }

        public async Task<IncomeReceipt?> GetActiveReceiptAsync(int incomeMonthlyId)
        {
            return await _context.IncomeReceipts
                .FirstOrDefaultAsync(r => r.IsActive && r.IncomeMonthlyId == incomeMonthlyId);
        }

        public async Task<List<IncomeMonthly>> GetMonthWithDetailsAsync(string month, string userId)
        {
            return await _context.IncomeMonthlies
                .Where(m => m.IsActive
                    && m.Month == month
                    && m.Income!.IsActive
                    && m.Income.UserId == userId)       // ← solo los del usuario
                .Include(m => m.Income)                  // trae el ingreso
                .Include(m => m.Receipts.Where(r => r.IsActive))   // y su recepción ACTIVA
                .OrderBy(m => m.Income!.Name)
                .ToListAsync();
        }

    }
}