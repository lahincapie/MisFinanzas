namespace MisFinanzas.Application.Dashboard.Dtos
{
    /// <summary>Un gasto del mes, tal como se muestra en el dashboard.</summary>
    public class DashboardExpenseItemDto
    {
        public int ExpenseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>Estado del mes: "Pendiente" o "Pagado".</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Monto real pagado (null si sigue pendiente).</summary>
        public decimal? PaidAmount { get; set; }

        /// <summary>Monto estimado si está pendiente (0 si ya se pagó).</summary>
        public decimal ProjectedAmount { get; set; }

        /// <summary>Día de pago oportuno (1-31).</summary>
        public int DueDay { get; set; }

        /// <summary>CALCULADO: true si está pendiente y ya pasó el día de pago oportuno.</summary>
        public bool IsOverdue { get; set; }
    }
}