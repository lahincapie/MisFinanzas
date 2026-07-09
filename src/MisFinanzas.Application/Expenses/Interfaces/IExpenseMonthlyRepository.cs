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
    }
}