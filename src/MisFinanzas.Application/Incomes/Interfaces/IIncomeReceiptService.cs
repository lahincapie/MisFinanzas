using MisFinanzas.Application.Incomes.Dtos;

namespace MisFinanzas.Application.Incomes.Interfaces
{
    /// <summary>Contrato del servicio de recepciones de ingreso.</summary>
    public interface IIncomeReceiptService
    {
        /// <summary>Registra la recepción de un ingreso en un mes y lo marca como Recibido.</summary>
        Task RegisterReceiptAsync(int incomeId, string month, RegisterIncomeReceiptDto dto);

        /// <summary>Revierte la recepción: la devuelve a Pendiente y conserva la anterior como inactiva.</summary>
        Task RevertReceiptAsync(int incomeId, string month);
    }
}