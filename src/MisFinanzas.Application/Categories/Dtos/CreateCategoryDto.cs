namespace MisFinanzas.Application.Categories.Dtos
{
    /// <summary>
    /// Datos que el usuario envía para crear una categoría.
    /// Solo incluye los campos que se le permite definir.
    /// </summary>
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public int Order { get; set; }
    }
}