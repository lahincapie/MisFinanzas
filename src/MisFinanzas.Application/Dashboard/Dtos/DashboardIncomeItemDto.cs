namespace MisFinanzas.Application.Dashboard.Dtos
{
    /// <summary>Un ingreso del mes, tal como se muestra en el dashboard.</summary>
    public class DashboardIncomeItemDto
    {
        public int IncomeId { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>Estado del mes: "Pendiente" o "Recibido".</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Monto real recibido (null si sigue pendiente).</summary>
        public decimal? ReceivedAmount { get; set; }

        /// <summary>Monto estimado si está pendiente (0 si ya se recibió, o si es variable).</summary>
        public decimal ProjectedAmount { get; set; }

        /// <summary>Día esperado de recepción (1-31).</summary>
        public int ExpectedReceiptDay { get; set; }

        /// <summary>CALCULADO: true si está pendiente y ya pasó el día esperado.</summary>
        public bool IsLate { get; set; }
    }
}