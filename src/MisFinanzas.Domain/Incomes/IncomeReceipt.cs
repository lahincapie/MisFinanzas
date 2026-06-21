using MisFinanzas.Domain.Common;

namespace MisFinanzas.Domain.Incomes
{
    /// <summary>
    /// Recepción real de un ingreso en un mes: monto, fecha y observaciones.
    /// Puede haber varias por mes, pero solo una activa; las reversiones dejan
    /// inactiva la anterior (soft-delete).
    /// </summary>
    public class IncomeReceipt : BaseEntity
    {
        /// <summary>Llave foránea: a qué registro mensual pertenece esta recepción.</summary>
        public int IncomeMonthlyId { get; set; }

        /// <summary>Propiedad de navegación hacia el registro mensual.</summary>
        public IncomeMonthly? IncomeMonthly { get; set; }

        /// <summary>Monto real recibido.</summary>
        public decimal Amount { get; set; }

        /// <summary>Fecha en que se recibió el ingreso.</summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>Observaciones opcionales sobre la recepción.</summary>
        public string? Notes { get; set; }
    }
}