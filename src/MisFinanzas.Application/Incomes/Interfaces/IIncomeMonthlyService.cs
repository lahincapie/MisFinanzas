namespace MisFinanzas.Application.Incomes.Interfaces
{
    /// <summary>Contrato del servicio de registros mensuales de ingreso.</summary>
    public interface IIncomeMonthlyService
    {
        /// <summary>
        /// Genera los pendientes del mes ("YYYY-MM") para los ingresos que aplican
        /// y aún no lo tienen. Devuelve cuántos creó.
        /// </summary>
        Task<int> GenerateForMonthAsync(string month, string userId);
    }
}