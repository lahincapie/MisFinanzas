using MisFinanzas.Application.Categories.Interfaces;
using MisFinanzas.Application.Expenses.Dtos;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Application.Incomes.Dtos;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Application.OneOff.Interfaces;
using MisFinanzas.Domain.Categories;
using MisFinanzas.Domain.Common;
using MisFinanzas.Domain.Expenses;
using MisFinanzas.Domain.Incomes;

namespace MisFinanzas.Application.OneOff.Services
{
    /// <summary>
    /// Crea gastos/ingresos ÚNICOS (no recurrentes): quedan ya reales
    /// (Pagado/Recibido) en el mes actual y marcados con IsOneOff.
    /// </summary>
    public class OneOffService : IOneOffService
    {
        private const string UnicosCategoryName = "Únicos";

        private readonly ICategoryRepository _categoryRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExpenseMonthlyRepository _expenseMonthlyRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IIncomeMonthlyRepository _incomeMonthlyRepository;

        public OneOffService(
            ICategoryRepository categoryRepository,
            IExpenseRepository expenseRepository,
            IExpenseMonthlyRepository expenseMonthlyRepository,
            IIncomeRepository incomeRepository,
            IIncomeMonthlyRepository incomeMonthlyRepository)
        {
            _categoryRepository = categoryRepository;
            _expenseRepository = expenseRepository;
            _expenseMonthlyRepository = expenseMonthlyRepository;
            _incomeRepository = incomeRepository;
            _incomeMonthlyRepository = incomeMonthlyRepository;
        }

        public async Task CreateOneOffExpenseAsync(CreateOneOffExpenseDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre es obligatorio.");
            if (dto.Amount <= 0)
                throw new ArgumentException("El monto debe ser mayor que cero.");
            if (dto.PaymentMethodId <= 0)
                throw new ArgumentException("Debe seleccionar un medio de pago.");

            var today = DateTime.UtcNow.AddHours(-5);   // hora de Bogotá (UTC-5)
            var month = today.ToString("yyyy-MM");

            // 1. Categoría "Únicos" (crear si no existe)
            var categoryId = await EnsureUnicosCategoryAsync(userId);

            // 2. El gasto (marcado como único; periodicidad/días son irrelevantes)
            var expense = new Expense
            {
                UserId = userId,
                Name = dto.Name.Trim(),
                CategoryId = categoryId,
                Periodicity = Periodicity.Monthly,
                IsVariable = false,
                ExpectedAmount = dto.Amount,
                CutoffDay = today.Day,
                DueDay = today.Day,
                SuspensionDay = today.Day,
                IsOneOff = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _expenseRepository.AddAsync(expense);
            await _expenseRepository.SaveChangesAsync();

            // 3. Registro mensual, ya en estado Pagado
            var monthly = new ExpenseMonthly
            {
                ExpenseId = expense.Id,
                Month = month,
                Status = ExpenseStatus.Paid,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _expenseMonthlyRepository.AddAsync(monthly);
            await _expenseMonthlyRepository.SaveChangesAsync();

            // 4. El pago real
            var payment = new ExpensePayment
            {
                ExpenseMonthlyId = monthly.Id,
                Amount = dto.Amount,
                PaymentDate = today.Date,
                PaymentMethodId = dto.PaymentMethodId,
                Notes = dto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _expenseMonthlyRepository.AddPaymentAsync(payment);
            await _expenseMonthlyRepository.SaveChangesAsync();
        }

        public async Task CreateOneOffIncomeAsync(CreateOneOffIncomeDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre es obligatorio.");
            if (dto.Amount <= 0)
                throw new ArgumentException("El monto debe ser mayor que cero.");

            var today = DateTime.UtcNow.AddHours(-5);
            var month = today.ToString("yyyy-MM");

            // 1. El ingreso (marcado como único)
            var income = new Income
            {
                UserId = userId,
                Name = dto.Name.Trim(),
                Periodicity = Periodicity.Monthly,
                IsVariable = false,
                ExpectedAmount = dto.Amount,
                ExpectedReceiptDay = today.Day,
                IsOneOff = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _incomeRepository.AddAsync(income);
            await _incomeRepository.SaveChangesAsync();

            // 2. Registro mensual, ya en estado Recibido
            var monthly = new IncomeMonthly
            {
                IncomeId = income.Id,
                Month = month,
                Status = IncomeStatus.Received,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _incomeMonthlyRepository.AddAsync(monthly);
            await _incomeMonthlyRepository.SaveChangesAsync();

            // 3. La recepción real
            var receipt = new IncomeReceipt
            {
                IncomeMonthlyId = monthly.Id,
                Amount = dto.Amount,
                ReceiptDate = today.Date,
                Notes = dto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _incomeMonthlyRepository.AddReceiptAsync(receipt);
            await _incomeMonthlyRepository.SaveChangesAsync();
        }

        /// <summary>Devuelve el Id de la categoría "Únicos" del usuario; la crea si no existe.</summary>
        private async Task<int> EnsureUnicosCategoryAsync(string userId)
        {
            var categories = await _categoryRepository.GetAllActiveAsync(userId);
            var unicos = categories.FirstOrDefault(c =>
                string.Equals(c.Name, UnicosCategoryName, StringComparison.OrdinalIgnoreCase));
            if (unicos is not null)
                return unicos.Id;

            var newCategory = new Category
            {
                UserId = userId,
                Name = UnicosCategoryName,
                Color = "#8a8a9a",
                Icon = "bolt",
                Order = 999,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _categoryRepository.AddAsync(newCategory);
            await _categoryRepository.SaveChangesAsync();
            return newCategory.Id;
        }
    }
}