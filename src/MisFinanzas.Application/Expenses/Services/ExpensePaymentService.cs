using FluentValidation;
using MisFinanzas.Application.Expenses.Dtos;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Application.PaymentMethods.Interfaces;
using MisFinanzas.Domain.Expenses;

namespace MisFinanzas.Application.Expenses.Services
{
    /// <summary>
    /// Registra el pago de un gasto: crea el ExpensePayment y cambia el estado
    /// del registro mensual a Pagado, en una sola operación.
    /// </summary>
    public class ExpensePaymentService : IExpensePaymentService
    {
        private readonly IExpenseMonthlyRepository _monthlyRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IValidator<RegisterExpensePaymentDto> _validator;
        private readonly IExpenseRepository _expenseRepository;

        public ExpensePaymentService(
            IExpenseMonthlyRepository monthlyRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IValidator<RegisterExpensePaymentDto> validator,
            IExpenseRepository expenseRepository)
        {
            _monthlyRepository = monthlyRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _validator = validator;
            _expenseRepository = expenseRepository;
        }

        public async Task RegisterPaymentAsync(int expenseId, string month, RegisterExpensePaymentDto dto, string userId)
        {
            // 1. Validación de forma
            await _validator.ValidateAndThrowAsync(dto);

            // 2. El gasto debe existir y pertenecer al usuario autenticado
            var expense = await _expenseRepository.GetByIdAsync(expenseId, userId);
            if (expense is null)
                throw new KeyNotFoundException("El gasto no existe o no te pertenece.");

            // 3. El registro mensual debe existir
            var monthly = await _monthlyRepository.GetByExpenseAndMonthAsync(expenseId, month);
            if (monthly is null)
                throw new KeyNotFoundException("No existe un registro pendiente para ese gasto y mes.");

            // 4. Debe estar en estado Pendiente
            if (monthly.Status != ExpenseStatus.Pending)
                throw new InvalidOperationException("El gasto de ese mes ya está pagado.");

            // 5. El medio de pago debe existir
            if (!await _paymentMethodRepository.ExistsAsync(dto.PaymentMethodId))
                throw new KeyNotFoundException("El medio de pago seleccionado no existe.");

            // 6. Crear el pago
            var payment = new ExpensePayment
            {
                ExpenseMonthlyId = monthly.Id,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethodId = dto.PaymentMethodId,
                Notes = dto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _monthlyRepository.AddPaymentAsync(payment);

            // 7. Cambiar el estado del registro mensual a Pagado
            monthly.Status = ExpenseStatus.Paid;
            monthly.UpdatedAt = DateTime.UtcNow;

            // 8. Guardar todo junto (el pago nuevo + el cambio de estado)
            await _monthlyRepository.SaveChangesAsync();
        }

        public async Task RevertPaymentAsync(int expenseId, string month, string userId)
        {
            // 1. El gasto debe existir y pertenecer al usuario autenticado
            var expense = await _expenseRepository.GetByIdAsync(expenseId, userId);
            if (expense is null)
                throw new KeyNotFoundException("El gasto no existe o no te pertenece.");

            // 2. El registro mensual debe existir
            var monthly = await _monthlyRepository.GetByExpenseAndMonthAsync(expenseId, month);
            if (monthly is null)
                throw new KeyNotFoundException("No existe un registro para ese gasto y mes.");

            
            // 3. Debe estar Pagado para poder revertir
            if (monthly.Status != ExpenseStatus.Paid)
                throw new InvalidOperationException("El gasto de ese mes no está pagado.");

            // 4. Marcar el pago activo como inactivo (se conserva para historial)
            var payment = await _monthlyRepository.GetActivePaymentAsync(monthly.Id);
            if (payment is not null)
            {
                payment.IsActive = false;
                payment.UpdatedAt = DateTime.UtcNow;
            }

            // 5. Devolver el registro mensual a Pendiente
            monthly.Status = ExpenseStatus.Pending;
            monthly.UpdatedAt = DateTime.UtcNow;

            // 6. Guardar todo junto
            await _monthlyRepository.SaveChangesAsync();
        }

    }
}