namespace MisFinanzas.Domain.Incomes
{
    /// <summary>
    /// Estado de un ingreso en un mes concreto.
    /// (El estado "Atrasado" no se guarda: se calcula a partir de la fecha.)
    /// </summary>
    public enum IncomeStatus
    {
        Pending = 1,    // Pendiente
        Received = 2    // Recibido
    }
}