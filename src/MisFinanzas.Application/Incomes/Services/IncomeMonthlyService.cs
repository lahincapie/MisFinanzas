using MisFinanzas.Application.Common;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Domain.Incomes;

namespace MisFinanzas.Application.Incomes.Services
{
    /// <summary>
    /// Genera automáticamente los registros mensuales (pendientes) de los ingresos
    /// que aplican a un mes, sin duplicar los que ya existen.
    /// </summary>
    public class IncomeMonthlyService : IIncomeMonthlyService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly IIncomeMonthlyRepository _monthlyRepository;

        public IncomeMonthlyService(
            IIncomeRepository incomeRepository,
            IIncomeMonthlyRepository monthlyRepository)
        {
            _incomeRepository = incomeRepository;
            _monthlyRepository = monthlyRepository;
        }

        public async Task<int> GenerateForMonthAsync(string month)
        {
            var incomes = await _incomeRepository.GetAllActiveAsync();

            var existingIds = (await _monthlyRepository.GetExistingIncomeIdsForMonthAsync(month))
                .ToHashSet();

            var created = 0;

            foreach (var income in incomes)
            {
                bool applies = ScheduleCalculator.AppliesToMonth(
                    income.Periodicity, income.StartMonth, income.EndMonth, month);

                if (!applies) continue;
                if (existingIds.Contains(income.Id)) continue;

                var monthly = new IncomeMonthly
                {
                    IncomeId = income.Id,
                    Month = month,
                    Status = IncomeStatus.Pending,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _monthlyRepository.AddAsync(monthly);
                created++;
            }

            if (created > 0)
                await _monthlyRepository.SaveChangesAsync();

            return created;
        }
    }
}