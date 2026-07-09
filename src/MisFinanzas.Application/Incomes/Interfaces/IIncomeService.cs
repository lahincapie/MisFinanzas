using MisFinanzas.Application.Incomes.Dtos;

namespace MisFinanzas.Application.Incomes.Interfaces
{
    /// <summary>Contrato del servicio de ingresos: los casos de uso disponibles.</summary>
    public interface IIncomeService
    {
        /// <summary>Crea un ingreso y devuelve el Id generado.</summary>
        Task<int> CreateAsync(CreateIncomeDto dto);

        /// <summary>Devuelve todos los ingresos activos.</summary>
        Task<List<IncomeDto>> GetAllAsync();

        /// <summary>Edita un ingreso existente.</summary>
        Task UpdateAsync(UpdateIncomeDto dto);

        /// <summary>Desactiva (soft-delete) un ingreso.</summary>
        Task DeactivateAsync(int id);
    }
}