using MisFinanzas.Application.Common;
using MisFinanzas.Application.Dashboard.Dtos;
using MisFinanzas.Application.Dashboard.Interfaces;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Domain.Expenses;
using MisFinanzas.Domain.Incomes;

namespace MisFinanzas.Application.Dashboard.Services
{
    /// <summary>
    /// Arma el dashboard mensual: trae los registros del mes, calcula
    /// proyecciones y totales (reales vs proyectados).
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IExpenseMonthlyRepository _expenseMonthlyRepository;
        private readonly IIncomeMonthlyRepository _incomeMonthlyRepository;

        public DashboardService(
            IExpenseMonthlyRepository expenseMonthlyRepository,
            IIncomeMonthlyRepository incomeMonthlyRepository)
        {
            _expenseMonthlyRepository = expenseMonthlyRepository;
            _incomeMonthlyRepository = incomeMonthlyRepository;
        }

        public async Task<DashboardDto> GetMonthlyAsync(string month, string userId)
        {
            var today = DateTime.UtcNow.AddHours(-5).Date;   // hora de Bogotá (UTC-5)

            var dashboard = new DashboardDto { Month = month };

            // ---------- GASTOS ----------
            var expenseMonthlies = await _expenseMonthlyRepository
                .GetMonthWithDetailsAsync(month, userId);

            foreach (var m in expenseMonthlies)
            {
                var expense = m.Expense!;
                var payment = m.Payments.FirstOrDefault();   // el activo (ya viene filtrado)

                // Si es variable y sigue pendiente, necesitamos su promedio de pagos
                decimal average = 0;
                if (expense.IsVariable && m.Status == ExpenseStatus.Pending)
                {
                    average = await _expenseMonthlyRepository
                        .GetAveragePaymentAsync(expense.Id, 3);
                }

                var projected = ProjectionCalculator.ProjectExpense(
                    m.Status, expense.IsVariable, expense.ExpectedAmount, average);

                var item = new DashboardExpenseItemDto
                {
                    ExpenseId = m.ExpenseId,       // ← la FK del registro mensual, siempre confiable
                    Name = expense.Name,
                    CategoryName = expense.Category?.Name ?? "(sin categoría)",
                    Status = m.Status == ExpenseStatus.Paid ? "Pagado" : "Pendiente",
                    PaidAmount = payment?.Amount,
                    ProjectedAmount = projected,
                    DueDay = expense.DueDay,
                    IsOverdue = ProjectionCalculator.IsOverdue(
                        m.Status, month, expense.DueDay, today)
                };

                dashboard.Expenses.Add(item);

                // Acumular totales
                dashboard.RealExpense += payment?.Amount ?? 0;
                dashboard.ProjectedExpense += projected;
            }

            // ---------- INGRESOS ----------
            var incomeMonthlies = await _incomeMonthlyRepository
                .GetMonthWithDetailsAsync(month, userId);

            foreach (var m in incomeMonthlies)
            {
                var income = m.Income!;
                var receipt = m.Receipts.FirstOrDefault();

                var projected = ProjectionCalculator.ProjectIncome(
                    m.Status, income.IsVariable, income.ExpectedAmount);

                var item = new DashboardIncomeItemDto
                {
                    IncomeId = m.IncomeId,
                    Name = income.Name,
                    Status = m.Status == IncomeStatus.Received ? "Recibido" : "Pendiente",
                    ReceivedAmount = receipt?.Amount,
                    ProjectedAmount = projected,
                    ExpectedReceiptDay = income.ExpectedReceiptDay,
                    IsLate = ProjectionCalculator.IsLate(
                        m.Status, month, income.ExpectedReceiptDay, today)
                };

                dashboard.Incomes.Add(item);

                dashboard.RealIncome += receipt?.Amount ?? 0;
                dashboard.ProjectedIncome += projected;
            }

            // ---------- BALANCES ----------
            dashboard.RealBalance = dashboard.RealIncome - dashboard.RealExpense;

            dashboard.ProjectedBalance =
                (dashboard.RealIncome + dashboard.ProjectedIncome)
                - (dashboard.RealExpense + dashboard.ProjectedExpense);

            return dashboard;
        }
    }
}