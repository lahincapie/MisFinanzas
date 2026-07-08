namespace MisFinanzas.Application.Categories.Dtos
{
    /// <summary>
    /// Datos que el usuario envía para editar una categoría existente.
    /// </summary>
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
    }
}