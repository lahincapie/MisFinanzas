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

        public CategoryService(
            ICategoryRepository repository,
            IValidator<CreateCategoryDto> createValidator)
        {
            _repository = repository;
            _createValidator = createValidator;
        }

        public async Task<int> CreateAsync(CreateCategoryDto dto)
        {
            // 1. Validación de forma (FluentValidation)
            await _createValidator.ValidateAndThrowAsync(dto);

            // 2. Regla de negocio: el nombre debe ser único
            if (await _repository.ExistsByNameAsync(dto.Name))
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

            // 3. Traducir DTO -> entidad del Domain
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                Icon = dto.Icon,
                Order = dto.Order,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 4. Guardar
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            return category.Id;
        }
    }
}