namespace MisFinanzas.Application.Reports.Dtos
{
    /// <summary>Total gastado (real) en una categoría dentro de un rango de meses.</summary>
    public class CategorySpendDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}