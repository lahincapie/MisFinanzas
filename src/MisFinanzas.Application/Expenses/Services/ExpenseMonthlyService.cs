using MisFinanzas.Application.Common;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Domain.Expenses;

namespace MisFinanzas.Application.Expenses.Services
{
    /// <summary>
    /// Genera automáticamente los registros mensuales (pendientes) de los gastos
    /// que aplican a un mes, sin duplicar los que ya existen.
    /// </summary>
    public class ExpenseMonthlyService : IExpenseMonthlyService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExpenseMonthlyRepository _monthlyRepository;

        public ExpenseMonthlyService(
            IExpenseRepository expenseRepository,
            IExpenseMonthlyRepository monthlyRepository)
        {
            _expenseRepository = expenseRepository;
            _monthlyRepository = monthlyRepository;
        }

        public async Task<int> GenerateForMonthAsync(string month, string userId)
        {
            // 1. Gastos activos
            var expenses = await _expenseRepository.GetAllActiveAsync(userId);

            // 2. Ids que YA tienen registro ese mes (para no duplicar).
            //    Usamos un HashSet para búsquedas rápidas.
            var existingIds = (await _monthlyRepository.GetExistingExpenseIdsForMonthAsync(month))
                .ToHashSet();

            var created = 0;

            // 3. Por cada gasto que aplique y no exista aún, crear su pendiente
            foreach (var expense in expenses)
            {
                bool applies = ScheduleCalculator.AppliesToMonth(
                    expense.Periodicity, expense.AnchorMonth, expense.StartMonth, expense.EndMonth, month);

                if (!applies) continue;
                if (existingIds.Contains(expense.Id)) continue;

                var monthly = new ExpenseMonthly
                {
                    ExpenseId = expense.Id,
                    Month = month,
                    Status = ExpenseStatus.Pending,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _monthlyRepository.AddAsync(monthly);
                created++;
            }

            // 4. Guardar todos los nuevos de una sola vez
            if (created > 0)
                await _monthlyRepository.SaveChangesAsync();

            return created;
        }

        /// <summary>Omite un gasto en un mes: lo pasa de Pendiente a Omitido.</summary>
        public async Task SkipMonthAsync(int expenseId, string month, string userId)
        {
            // Seguridad: el gasto debe existir y ser del usuario
            var expense = await _expenseRepository.GetByIdAsync(expenseId, userId);
            if (expense is null)
                throw new KeyNotFoundException("El gasto no existe.");

            var monthly = await _monthlyRepository.GetByExpenseAndMonthAsync(expenseId, month);
            if (monthly is null)
                throw new KeyNotFoundException("Ese gasto no tiene registro en el mes indicado.");
            if (monthly.Status != ExpenseStatus.Pending)
                throw new InvalidOperationException("Solo se puede omitir un gasto que está pendiente.");

            monthly.Status = ExpenseStatus.Skipped;
            await _monthlyRepository.SaveChangesAsync();
        }

        /// <summary>Reactiva un gasto omitido en un mes: lo devuelve a Pendiente.</summary>
        public async Task UnskipMonthAsync(int expenseId, string month, string userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId, userId);
            if (expense is null)
                throw new KeyNotFoundException("El gasto no existe.");

            var monthly = await _monthlyRepository.GetByExpenseAndMonthAsync(expenseId, month);
            if (monthly is null)
                throw new KeyNotFoundException("Ese gasto no tiene registro en el mes indicado.");
            if (monthly.Status != ExpenseStatus.Skipped)
                throw new InvalidOperationException("Solo se puede reactivar un gasto que está omitido.");

            monthly.Status = ExpenseStatus.Pending;
            await _monthlyRepository.SaveChangesAsync();
        }

    }
}