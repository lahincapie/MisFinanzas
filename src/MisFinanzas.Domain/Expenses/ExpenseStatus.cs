namespace MisFinanzas.Domain.Expenses
{
    /// <summary>
    /// Estado de un gasto en un mes concreto.
    /// (El estado "Vencido" no se guarda: se calcula a partir de la fecha actual.)
    /// </summary>
    public enum ExpenseStatus
    {
        Pending = 1,   // Pendiente
        Paid = 2,      // Pagado
        Skipped = 3    // Omitido (este mes no se paga; el próximo que le toque, reaparece)
    }
}