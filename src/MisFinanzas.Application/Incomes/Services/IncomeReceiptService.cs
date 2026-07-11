using FluentValidation;
using MisFinanzas.Application.Incomes.Dtos;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Domain.Incomes;

namespace MisFinanzas.Application.Incomes.Services
{
    /// <summary>
    /// Registra y revierte recepciones de ingreso, cambiando el estado del
    /// registro mensual. Verifica que el ingreso pertenezca al usuario.
    /// </summary>
    public class IncomeReceiptService : IIncomeReceiptService
    {
        private readonly IIncomeMonthlyRepository _monthlyRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly IValidator<RegisterIncomeReceiptDto> _validator;

        public IncomeReceiptService(
            IIncomeMonthlyRepository monthlyRepository,
            IIncomeRepository incomeRepository,
            IValidator<RegisterIncomeReceiptDto> validator)
        {
            _monthlyRepository = monthlyRepository;
            _incomeRepository = incomeRepository;
            _validator = validator;
        }

        public async Task RegisterReceiptAsync(
            int incomeId, string month, RegisterIncomeReceiptDto dto, string userId)
        {
            // 1. Validación de forma
            await _validator.ValidateAndThrowAsync(dto);

            // 2. El ingreso debe existir Y ser del usuario
            var income = await _incomeRepository.GetByIdAsync(incomeId, userId);
            if (income is null)
                throw new KeyNotFoundException("El ingreso no existe.");

            // 3. El registro mensual debe existir
            var monthly = await _monthlyRepository.GetByIncomeAndMonthAsync(incomeId, month);
            if (monthly is null)
                throw new KeyNotFoundException("No existe un registro pendiente para ese ingreso y mes.");

            // 4. Debe estar Pendiente
            if (monthly.Status != IncomeStatus.Pending)
                throw new InvalidOperationException("El ingreso de ese mes ya está recibido.");

            // 5. Crear la recepción
            var receipt = new IncomeReceipt
            {
                IncomeMonthlyId = monthly.Id,
                Amount = dto.Amount,
                ReceiptDate = dto.ReceiptDate,
                Notes = dto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _monthlyRepository.AddReceiptAsync(receipt);

            // 6. Cambiar estado a Recibido
            monthly.Status = IncomeStatus.Received;
            monthly.UpdatedAt = DateTime.UtcNow;

            // 7. Guardar todo junto
            await _monthlyRepository.SaveChangesAsync();
        }

        public async Task RevertReceiptAsync(int incomeId, string month, string userId)
        {
            // 1. El ingreso debe existir Y ser del usuario
            var income = await _incomeRepository.GetByIdAsync(incomeId, userId);
            if (income is null)
                throw new KeyNotFoundException("El ingreso no existe.");

            // 2. El registro mensual debe existir
            var monthly = await _monthlyRepository.GetByIncomeAndMonthAsync(incomeId, month);
            if (monthly is null)
                throw new KeyNotFoundException("No existe un registro para ese ingreso y mes.");

            // 3. Debe estar Recibido
            if (monthly.Status != IncomeStatus.Received)
                throw new InvalidOperationException("El ingreso de ese mes no está recibido.");

            // 4. Marcar la recepción activa como inactiva (se conserva)
            var receipt = await _monthlyRepository.GetActiveReceiptAsync(monthly.Id);
            if (receipt is not null)
            {
                receipt.IsActive = false;
                receipt.UpdatedAt = DateTime.UtcNow;
            }

            // 5. Volver a Pendiente
            monthly.Status = IncomeStatus.Pending;
            monthly.UpdatedAt = DateTime.UtcNow;

            // 6. Guardar todo junto
            await _monthlyRepository.SaveChangesAsync();
        }
    }
}