using MisFinanzas.Domain.Common;

namespace MisFinanzas.Domain.Expenses
{
    /// <summary>
    /// Registro de un gasto en un mes concreto. Por cada mes que aplica,
    /// el sistema genera uno en estado Pendiente; al pagarlo pasa a Pagado.
    /// </summary>
    public class ExpenseMonthly : BaseEntity
    {
        /// <summary>Llave foránea: a qué gasto (plantilla) pertenece este registro.</summary>
        public int ExpenseId { get; set; }

        /// <summary>Propiedad de navegación hacia el gasto dueño.</summary>
        public Expense? Expense { get; set; }

        /// <summary>Mes que representa este registro, formato "YYYY-MM".</summary>
        public string Month { get; set; } = string.Empty;

        /// <summary>Estado del registro en el mes: Pendiente o Pagado.</summary>
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

        /// <summary>Pagos de este registro mensual (solo uno activo a la vez; los revertidos quedan inactivos).</summary>
        public ICollection<ExpensePayment> Payments { get; set; } = new List<ExpensePayment>();

    }
}