using MisFinanzas.Domain.Expenses;

namespace MisFinanzas.Application.Expenses.Interfaces
{
    /// <summary>
    /// Contrato de acceso a datos para los registros mensuales de gasto (ExpenseMonthly).
    /// </summary>
    public interface IExpenseMonthlyRepository
    {
        /// <summary>
        /// Devuelve los Id de los gastos que YA tienen registro mensual (activo) para ese mes.
        /// Sirve para no volver a generar los que ya existen.
        /// </summary>
        Task<List<int>> GetExistingExpenseIdsForMonthAsync(string month);

        /// <summary>Agrega un nuevo registro mensual.</summary>
        Task AddAsync(ExpenseMonthly monthly);

        /// <summary>Guarda los cambios pendientes en la base de datos.</summary>
        Task SaveChangesAsync();

        /// <summary>Busca el registro mensual activo de un gasto en un mes concreto. Null si no existe.</summary>
        Task<ExpenseMonthly?> GetByExpenseAndMonthAsync(int expenseId, string month);

        /// <summary>Agrega un pago de gasto.</summary>
        Task AddPaymentAsync(ExpensePayment payment);

        /// <summary>Devuelve el pago activo de un registro mensual, si existe. Null si no hay.</summary>
        Task<ExpensePayment?> GetActivePaymentAsync(int expenseMonthlyId);

        /// <summary>Trae los registros mensuales de gasto de un mes, con su gasto, categoría y pago activo.</summary>
        Task<List<ExpenseMonthly>> GetMonthWithDetailsAsync(string month, string userId);

        /// <summary>Promedio de los últimos N pagos activos de un gasto (0 si no hay histórico).</summary>
        Task<decimal> GetAveragePaymentAsync(int expenseId, int lastN = 3);

    }
}