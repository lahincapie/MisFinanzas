using MisFinanzas.Domain.Common;

namespace MisFinanzas.Domain.PaymentMethods
{
    /// <summary>
    /// Medio de pago para registrar el pago de un gasto. Es un catálogo de lista
    /// fija (Efectivo, PSE, Transferencia, etc.) que se cargará al crear la base de datos.
    /// </summary>
    public class PaymentMethod : BaseEntity
    {
        /// <summary>Nombre del medio de pago. Ej.: "Efectivo", "PSE".</summary>
        public string Name { get; set; } = string.Empty;
    }
}