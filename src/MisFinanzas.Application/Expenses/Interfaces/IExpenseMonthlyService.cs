namespace MisFinanzas.Application.Expenses.Interfaces
{
    /// <summary>Contrato del servicio de registros mensuales de gasto.</summary>
    public interface IExpenseMonthlyService
    {
        /// <summary>
        /// Genera los pendientes del mes indicado ("YYYY-MM") para los gastos que aplican
        /// y aún no lo tienen. Devuelve cuántos registros creó.
        /// </summary>
        Task<int> GenerateForMonthAsync(string month, string userId);
    }
}