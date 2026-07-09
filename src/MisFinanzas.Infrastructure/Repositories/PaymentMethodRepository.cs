using Microsoft.EntityFrameworkCore;
using MisFinanzas.Application.PaymentMethods.Interfaces;
using MisFinanzas.Infrastructure.Persistence;

namespace MisFinanzas.Infrastructure.Repositories
{
    /// <summary>Implementación con EF Core del contrato IPaymentMethodRepository.</summary>
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly MisFinanzasDbContext _context;

        public PaymentMethodRepository(MisFinanzasDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.PaymentMethods
                .AnyAsync(p => p.IsActive && p.Id == id);
        }
    }
}