using MisFinanzas.Domain.Common;

namespace MisFinanzas.Domain.Incomes
{
    /// <summary>
    /// Registro de un ingreso en un mes concreto. Cada mes que aplica nace
    /// Pendiente; al recibirlo pasa a Recibido.
    /// </summary>
    public class IncomeMonthly : BaseEntity
    {
        /// <summary>Llave foránea: a qué ingreso (plantilla) pertenece este registro.</summary>
        public int IncomeId { get; set; }

        /// <summary>Propiedad de navegación hacia el ingreso dueño.</summary>
        public Income? Income { get; set; }

        /// <summary>Mes que representa este registro, formato "YYYY-MM".</summary>
        public string Month { get; set; } = string.Empty;

        /// <summary>Estado del registro en el mes: Pendiente o Recibido.</summary>
        public IncomeStatus Status { get; set; } = IncomeStatus.Pending;

        /// <summary>Recepciones de este registro mensual (solo una activa a la vez).</summary>
        public ICollection<IncomeReceipt> Receipts { get; set; } = new List<IncomeReceipt>();
    }
}