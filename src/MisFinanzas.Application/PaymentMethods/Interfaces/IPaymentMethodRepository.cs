using MisFinanzas.Domain.PaymentMethods;

namespace MisFinanzas.Application.PaymentMethods.Interfaces
{
    /// <summary>Contrato de acceso a datos para el catálogo de medios de pago.</summary>
    public interface IPaymentMethodRepository
    {
        /// <summary>Indica si existe un medio de pago activo con ese Id.</summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>Lista los medios de pago activos (para llenar el dropdown del cliente).</summary>
        Task<List<PaymentMethod>> GetAllActiveAsync();
    }
}