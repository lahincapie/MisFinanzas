namespace MisFinanzas.Application.Reports.Dtos
{
    /// <summary>Total recibido (real) de un ingreso dentro de un rango de meses.</summary>
    public class IncomeReceivedDto
    {
        public int IncomeId { get; set; }
        public string IncomeName { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}