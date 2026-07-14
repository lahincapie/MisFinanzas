using MisFinanzas.Application.Dashboard.Dtos;

namespace MisFinanzas.Application.Dashboard.Interfaces
{
    /// <summary>Contrato del servicio de dashboard.</summary>
    public interface IDashboardService
    {
        /// <summary>Devuelve la foto financiera del mes ("YYYY-MM") para un usuario.</summary>
        Task<DashboardDto> GetMonthlyAsync(string month, string userId);
    }
}