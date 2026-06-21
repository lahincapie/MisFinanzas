namespace MisFinanzas.Domain.Common
{
    /// <summary>
    /// Frecuencia con la que se repite un gasto o un ingreso.
    /// La usan tanto los gastos como los ingresos.
    /// </summary>
    public enum Periodicity
    {
        Monthly = 1,     // Mensual
        Bimonthly = 2,   // Bimestral (cada 2 meses)
        Quarterly = 3,   // Trimestral (cada 3 meses)
        Semiannual = 4,  // Semestral (cada 6 meses)
        Annual = 5       // Anual (cada 12 meses)
    }
}