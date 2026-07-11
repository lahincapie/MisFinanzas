using MisFinanzas.Application.Expenses.Dtos;

namespace MisFinanzas.Application.Expenses.Interfaces
{
    /// <summary>Contrato del servicio de pagos de gasto.</summary>
    public interface IExpensePaymentService
    {
        /// <summary>Registra el pago de un gasto en un mes y lo marca como Pagado.</summary>
        Task RegisterPaymentAsync(int expenseId, string month, RegisterExpensePaymentDto dto, string userId);

        /// <summary>Revierte el pago de un gasto en un mes: lo devuelve a Pendiente
        /// y conserva el pago anterior como inactivo.</summary>
        Task RevertPaymentAsync(int expenseId, string month, string userId);
    }
}