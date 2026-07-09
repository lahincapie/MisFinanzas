namespace MisFinanzas.Application.Expenses.Dtos
{
    /// <summary>Datos para registrar el pago de un gasto en un mes.</summary>
    public class RegisterExpensePaymentDto
    {
        /// <summary>Monto real pagado.</summary>
        public decimal Amount { get; set; }

        /// <summary>Fecha en que se realizó el pago.</summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>Medio de pago usado (Id del catálogo PaymentMethod).</summary>
        public int PaymentMethodId { get; set; }

        /// <summary>Observaciones opcionales.</summary>
        public string? Notes { get; set; }
    }
}