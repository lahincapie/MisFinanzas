using FluentValidation;
using MisFinanzas.Application.Categories.Dtos;
using MisFinanzas.Application.Categories.Interfaces;
using MisFinanzas.Domain.Categories;

namespace MisFinanzas.Application.Categories.Services
{
    /// <summary>
    /// Servicio de aplicación de categorías: orquesta validación,
    /// reglas de negocio y persistencia para cada caso de uso.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IValidator<CreateCategoryDto> _createValidator;
        private readonly IValidator<UpdateCategoryDto> _updateValidator;

        public CategoryService(
            ICategoryRepository repository,
            IValidator<CreateCategoryDto> createValidator,
            IValidator<UpdateCategoryDto> updateValidator)
        {
            _repository = repository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<int> CreateAsync(CreateCategoryDto dto, string userId)
        {
            // 1. Validación de forma (FluentValidation)
            await _createValidator.ValidateAndThrowAsync(dto);

            // 2. Regla de negocio: el nombre debe ser único
            if (await _repository.ExistsByNameAsync(dto.Name, userId))
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

            // 3. Traducir DTO -> entidad del Domain
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                Icon = dto.Icon,
                Order = dto.Order,
                UserId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 4. Guardar
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            return category.Id;
        }

        public async Task<List<CategoryDto>> GetAllAsync(string userId)
        {
            var categories = await _repository.GetAllActiveAsync(userId);

            // Traducir cada entidad -> DTO de salida
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Color = c.Color,
                Icon = c.Icon,
                Order = c.Order
            }).ToList();
        }

        public async Task UpdateAsync(UpdateCategoryDto dto, string userId)
        {
            // 1. Validación de forma
            await _updateValidator.ValidateAndThrowAsync(dto);

            // 2. Debe existir
            var category = await _repository.GetByIdAsync(dto.Id, userId);
            if (category is null)
                throw new KeyNotFoundException("La categoría no existe.");

            // 3. Nombre único, excluyendo la propia categoría
            if (await _repository.ExistsByNameAsync(dto.Name, userId, dto.Id))
                throw new InvalidOperationException("Ya existe otra categoría con ese nombre.");

            // 4. Modificar los campos permitidos
            category.Name = dto.Name;
            category.Description = dto.Description;
            category.Color = dto.Color;
            category.Icon = dto.Icon;
            category.Order = dto.Order;
            category.UpdatedAt = DateTime.UtcNow;

            // 5. Guardar: EF ya rastrea 'category' y detecta los cambios
            await _repository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id, string userId)
        {
            // 1. Debe existir
            var category = await _repository.GetByIdAsync(id, userId);
            if (category is null)
                throw new KeyNotFoundException("La categoría no existe.");

            // 2. Regla de negocio: no desactivar si tiene gastos activos
            if (await _repository.HasActiveExpensesAsync(id))
                throw new InvalidOperationException("No se puede desactivar: la categoría tiene gastos asociados.");

            // 3. Soft-delete: marcar inactiva (no se borra)
            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            // 4. Guardar (EF rastrea el cambio)
            await _repository.SaveChangesAsync();
        }
    }
}