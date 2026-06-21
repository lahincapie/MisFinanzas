using MisFinanzas.Domain.Common;
using MisFinanzas.Domain.PaymentMethods;

namespace MisFinanzas.Domain.Expenses
{
    /// <summary>
    /// Pago real de un gasto en un mes: monto, fecha, medio y observaciones.
    /// Puede haber varios por mes, pero solo uno activo; las reversiones dejan
    /// inactivo el anterior (soft-delete).
    /// </summary>
    public class ExpensePayment : BaseEntity
    {
        /// <summary>Llave foránea: a qué registro mensual pertenece este pago.</summary>
        public int ExpenseMonthlyId { get; set; }

        /// <summary>Propiedad de navegación hacia el registro mensual.</summary>
        public ExpenseMonthly? ExpenseMonthly { get; set; }

        /// <summary>Monto real pagado.</summary>
        public decimal Amount { get; set; }

        /// <summary>Fecha en que se realizó el pago.</summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>Llave foránea: con qué medio de pago se pagó.</summary>
        public int PaymentMethodId { get; set; }

        /// <summary>Propiedad de navegación hacia el medio de pago.</summary>
        public PaymentMethod? PaymentMethod { get; set; }

        /// <summary>Observaciones opcionales sobre el pago.</summary>
        public string? Notes { get; set; }
    }
}