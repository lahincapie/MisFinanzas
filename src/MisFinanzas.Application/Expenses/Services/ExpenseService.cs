using FluentValidation;
using MisFinanzas.Application.Categories.Interfaces;
using MisFinanzas.Application.Expenses.Dtos;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Domain.Expenses;

namespace MisFinanzas.Application.Expenses.Services
{
    /// <summary>
    /// Servicio de aplicación de gastos: orquesta validación, reglas de negocio
    /// (incluida la existencia de la categoría) y persistencia.
    /// </summary>
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExpenseMonthlyRepository _monthlyRepository;
        private readonly IValidator<CreateExpenseDto> _createValidator;
        private readonly IValidator<UpdateExpenseDto> _updateValidator;

        public ExpenseService(
            IExpenseRepository repository,
            ICategoryRepository categoryRepository,
            IExpenseMonthlyRepository monthlyRepository,
            IValidator<CreateExpenseDto> createValidator,
            IValidator<UpdateExpenseDto> updateValidator)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _monthlyRepository = monthlyRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<int> CreateAsync(CreateExpenseDto dto, string userId)
        {
            // 1. Validación de forma
            await _createValidator.ValidateAndThrowAsync(dto);

            // 2. La categoría debe existir y estar activa
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId);
            if (category is null)
                throw new KeyNotFoundException("La categoría seleccionada no existe o no está activa.");

            // 3. Nombre de gasto único
            if (await _repository.ExistsByNameAsync(dto.Name, userId))
                throw new InvalidOperationException("Ya existe un gasto con ese nombre.");

            // 4. Traducir DTO -> entidad del Domain
            var expense = new Expense
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Periodicity = dto.Periodicity,
                IsVariable = dto.IsVariable,
                ExpectedAmount = dto.IsVariable ? null : dto.ExpectedAmount,
                CutoffDay = dto.CutoffDay,
                DueDay = dto.DueDay,
                SuspensionDay = dto.SuspensionDay,
                StartMonth = dto.StartMonth,
                EndMonth = dto.EndMonth,
                AnchorMonth = dto.AnchorMonth,
                Reference = dto.Reference,
                Contract = dto.Contract,
                UserId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 5. Guardar
            await _repository.AddAsync(expense);
            await _repository.SaveChangesAsync();

            return expense.Id;
        }

        public async Task<List<ExpenseDto>> GetAllAsync(string userId)
        {
            var expenses = await _repository.GetAllActiveAsync(userId);

            return expenses.Where(e => !e.IsOneOff).Select(e => new ExpenseDto
            {
                Id = e.Id,
                Name = e.Name,
                CategoryId = e.CategoryId,
                CategoryName = e.Category?.Name ?? string.Empty,
                Periodicity = e.Periodicity,
                IsVariable = e.IsVariable,
                ExpectedAmount = e.ExpectedAmount,
                CutoffDay = e.CutoffDay,
                DueDay = e.DueDay,
                SuspensionDay = e.SuspensionDay,
                StartMonth = e.StartMonth,
                EndMonth = e.EndMonth,
                AnchorMonth = e.AnchorMonth,
                Reference = e.Reference,
                Contract = e.Contract
            }).ToList();
        }

        
        public async Task UpdateAsync(UpdateExpenseDto dto, string userId)
        {
            // 1. Validación de forma
            await _updateValidator.ValidateAndThrowAsync(dto);

            // 2. El gasto debe existir
            var expense = await _repository.GetByIdAsync(dto.Id, userId);
            if (expense is null)
                throw new KeyNotFoundException("El gasto no existe.");

            // 3. La categoría (que puede haber cambiado) debe existir y estar activa
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId);
            if (category is null)
                throw new KeyNotFoundException("La categoría seleccionada no existe o no está activa.");

            // 4. Nombre único, excluyendo el propio gasto
            if (await _repository.ExistsByNameAsync(dto.Name, userId, dto.Id))
                throw new InvalidOperationException("Ya existe otro gasto con ese nombre.");

            // 5. Modificar los campos
            expense.Name = dto.Name;
            expense.CategoryId = dto.CategoryId;
            expense.Periodicity = dto.Periodicity;
            expense.IsVariable = dto.IsVariable;
            expense.ExpectedAmount = dto.IsVariable ? null : dto.ExpectedAmount;
            expense.CutoffDay = dto.CutoffDay;
            expense.DueDay = dto.DueDay;
            expense.SuspensionDay = dto.SuspensionDay;
            expense.StartMonth = dto.StartMonth;
            expense.EndMonth = dto.EndMonth;
            expense.AnchorMonth = dto.AnchorMonth;
            expense.Reference = dto.Reference;
            expense.Contract = dto.Contract;
            expense.UpdatedAt = DateTime.UtcNow;

            // 6. Guardar (EF rastrea 'expense' y detecta los cambios)
            await _repository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id, string userId)
        {
            // 1. El gasto debe existir
            var expense = await _repository.GetByIdAsync(id, userId);
            if (expense is null)
                throw new KeyNotFoundException("El gasto no existe.");

            // 2. Soft-delete: marcar inactivo (no se borra)
            expense.IsActive = false;
            expense.UpdatedAt = DateTime.UtcNow;

            // 3. Guardar (EF rastrea el cambio)
            await _repository.SaveChangesAsync();

            // Nota (Fase 2): aquí también se desactivarán los pendientes futuros del gasto (RF-11).
        }

        public async Task TerminateAsync(int id, string userId)
        {
            var expense = await _repository.GetByIdAsync(id, userId);
            if (expense is null)
                throw new KeyNotFoundException("El gasto no existe.");

            // Deuda saldada: la vigencia termina en el mes actual (hora de Bogotá).
            // EndMonth es inclusive, así que el mes actual aún cuenta y deja de generarse desde el próximo.
            var endMonth = DateTime.UtcNow.AddHours(-5).ToString("yyyy-MM");
            expense.EndMonth = endMonth;
            expense.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            // Limpia ocurrencias futuras aún no reales (pendientes/omitidas) ya generadas,
            // para que no queden colgando en meses posteriores. Las pagadas se conservan.
            await _monthlyRepository.DeactivateFutureNonRealAsync(id, endMonth);
        }

        public async Task ReactivateAsync(int id, string userId)
        {
            var expense = await _repository.GetByIdAsync(id, userId);
            if (expense is null)
                throw new KeyNotFoundException("El gasto no existe.");

            // Vuelve a estar vigente sin fin: se genera cada mes como antes.
            expense.EndMonth = null;
            expense.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
        }
    }
}