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

        public async Task<int> GenerateForMonthAsync(string month)
        {
            // 1. Gastos activos
            var expenses = await _expenseRepository.GetAllActiveAsync();

            // 2. Ids que YA tienen registro ese mes (para no duplicar).
            //    Usamos un HashSet para búsquedas rápidas.
            var existingIds = (await _monthlyRepository.GetExistingExpenseIdsForMonthAsync(month))
                .ToHashSet();

            var created = 0;

            // 3. Por cada gasto que aplique y no exista aún, crear su pendiente
            foreach (var expense in expenses)
            {
                bool applies = ExpenseScheduleCalculator.AppliesToMonth(
                    expense.Periodicity, expense.StartMonth, expense.EndMonth, month);

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
    }
}