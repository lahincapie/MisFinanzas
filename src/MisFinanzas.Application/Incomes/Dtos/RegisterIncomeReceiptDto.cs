namespace MisFinanzas.Application.Incomes.Dtos
{
    /// <summary>Datos para registrar la recepción de un ingreso en un mes.</summary>
    public class RegisterIncomeReceiptDto
    {
        /// <summary>Monto real recibido.</summary>
        public decimal Amount { get; set; }

        /// <summary>Fecha en que se recibió el ingreso.</summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>Observaciones opcionales.</summary>
        public string? Notes { get; set; }
    }
}