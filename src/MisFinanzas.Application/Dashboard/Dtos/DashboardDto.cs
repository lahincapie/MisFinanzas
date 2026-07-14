namespace MisFinanzas.Application.Dashboard.Dtos
{
    /// <summary>Foto financiera de un mes: métricas y listados.</summary>
    public class DashboardDto
    {
        /// <summary>El mes consultado (YYYY-MM).</summary>
        public string Month { get; set; } = string.Empty;

        // ---- Métricas reales (lo que YA ocurrió) ----
        public decimal RealIncome { get; set; }
        public decimal RealExpense { get; set; }
        public decimal RealBalance { get; set; }

        // ---- Métricas proyectadas (lo que FALTA por ocurrir) ----
        public decimal ProjectedIncome { get; set; }
        public decimal ProjectedExpense { get; set; }
        public decimal ProjectedBalance { get; set; }

        // ---- Listados del mes ----
        public List<DashboardExpenseItemDto> Expenses { get; set; } = new();
        public List<DashboardIncomeItemDto> Incomes { get; set; } = new();
    }
}