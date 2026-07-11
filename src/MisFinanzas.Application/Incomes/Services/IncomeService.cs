using FluentValidation;
using MisFinanzas.Application.Incomes.Dtos;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Domain.Incomes;

namespace MisFinanzas.Application.Incomes.Services
{
    /// <summary>
    /// Servicio de aplicación de ingresos: orquesta validación, reglas y persistencia.
    /// </summary>
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _repository;
        private readonly IValidator<CreateIncomeDto> _createValidator;
        private readonly IValidator<UpdateIncomeDto> _updateValidator;

        public IncomeService(
            IIncomeRepository repository,
            IValidator<CreateIncomeDto> createValidator,
            IValidator<UpdateIncomeDto> updateValidator)
        {
            _repository = repository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<int> CreateAsync(CreateIncomeDto dto, string userId)
        {
            // 1. Validación de forma
            await _createValidator.ValidateAndThrowAsync(dto);

            // 2. Nombre único
            if (await _repository.ExistsByNameAsync(dto.Name, userId))
                throw new InvalidOperationException("Ya existe un ingreso con ese nombre.");

            // 3. DTO -> entidad
            var income = new Income
            {
                Name = dto.Name,
                Periodicity = dto.Periodicity,
                IsVariable = dto.IsVariable,
                ExpectedAmount = dto.ExpectedAmount,   // el ingreso siempre lo lleva (fijo o variable)
                ExpectedReceiptDay = dto.ExpectedReceiptDay,
                StartMonth = dto.StartMonth,
                EndMonth = dto.EndMonth,
                UserId = userId,              // ← el dueño
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 4. Guardar
            await _repository.AddAsync(income);
            await _repository.SaveChangesAsync();

            return income.Id;
        }

        public async Task<List<IncomeDto>> GetAllAsync(string userId)
        {
            var incomes = await _repository.GetAllActiveAsync(userId);

            return incomes.Select(i => new IncomeDto
            {
                Id = i.Id,
                Name = i.Name,
                Periodicity = i.Periodicity,
                IsVariable = i.IsVariable,
                ExpectedAmount = i.ExpectedAmount,
                ExpectedReceiptDay = i.ExpectedReceiptDay,
                StartMonth = i.StartMonth,
                EndMonth = i.EndMonth
            }).ToList();
        }

        public async Task UpdateAsync(UpdateIncomeDto dto, string userId)
        {
            // 1. Validación de forma
            await _updateValidator.ValidateAndThrowAsync(dto);

            // 2. Debe existir
            var income = await _repository.GetByIdAsync(dto.Id, userId);
            if (income is null)
                throw new KeyNotFoundException("El ingreso no existe.");

            // 3. Nombre único, excluyendo el propio
            if (await _repository.ExistsByNameAsync(dto.Name, userId, dto.Id))
                throw new InvalidOperationException("Ya existe otro ingreso con ese nombre.");

            // 4. Modificar
            income.Name = dto.Name;
            income.Periodicity = dto.Periodicity;
            income.IsVariable = dto.IsVariable;
            income.ExpectedAmount = dto.ExpectedAmount;
            income.ExpectedReceiptDay = dto.ExpectedReceiptDay;
            income.StartMonth = dto.StartMonth;
            income.EndMonth = dto.EndMonth;
            income.UpdatedAt = DateTime.UtcNow;

            // 5. Guardar (EF rastrea el cambio)
            await _repository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id, string userId)
        {
            var income = await _repository.GetByIdAsync(id, userId);
            if (income is null)
                throw new KeyNotFoundException("El ingreso no existe.");

            income.IsActive = false;
            income.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();

            // Nota (Fase 2): aquí también se desactivarán los pendientes futuros del ingreso (RF-18).
        }
    }
}