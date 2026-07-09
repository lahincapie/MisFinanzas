using MisFinanzas.Domain.Incomes;

namespace MisFinanzas.Application.Incomes.Interfaces
{
    /// <summary>Contrato de acceso a datos para los registros mensuales de ingreso.</summary>
    public interface IIncomeMonthlyRepository
    {
        /// <summary>Devuelve los Id de los ingresos que YA tienen registro activo para ese mes.</summary>
        Task<List<int>> GetExistingIncomeIdsForMonthAsync(string month);

        /// <summary>Agrega un nuevo registro mensual.</summary>
        Task AddAsync(IncomeMonthly monthly);

        /// <summary>Guarda los cambios pendientes.</summary>
        Task SaveChangesAsync();
        Task<IncomeMonthly?> GetByIncomeAndMonthAsync(int incomeId, string month);
        Task AddReceiptAsync(IncomeReceipt receipt);
        Task<IncomeReceipt?> GetActiveReceiptAsync(int incomeMonthlyId);
    }
}