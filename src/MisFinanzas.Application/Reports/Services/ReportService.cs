using MisFinanzas.Application.Reports.Dtos;
using MisFinanzas.Application.Reports.Interfaces;

namespace MisFinanzas.Application.Reports.Services
{
    /// <summary>
    /// Reportes de gasto real. Valida el rango y delega la agregación al repositorio.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository)
        {
            _repository = repository;
        }

        public Task<List<CategorySpendDto>> GetSpendByCategoryAsync(string from, string to, string userId)
        {
            NormalizeRange(ref from, ref to);
            return _repository.GetSpendByCategoryAsync(from, to, userId);
        }

        public Task<List<ExpenseSpendDto>> GetSpendByExpenseAsync(string from, string to, string userId)
        {
            NormalizeRange(ref from, ref to);
            return _repository.GetSpendByExpenseAsync(from, to, userId);
        }

        /// <summary>Valida formato y, si el rango viene invertido, lo intercambia.</summary>
        private static void NormalizeRange(ref string from, ref string to)
        {
            if (!IsValidMonth(from) || !IsValidMonth(to))
                throw new ArgumentException("El rango debe tener meses con formato YYYY-MM.");

            if (string.Compare(from, to) > 0)
                (from, to) = (to, from);
        }

        private static bool IsValidMonth(string month) =>
            System.Text.RegularExpressions.Regex.IsMatch(month ?? "", @"^\d{4}-(0[1-9]|1[0-2])$");
    }
}