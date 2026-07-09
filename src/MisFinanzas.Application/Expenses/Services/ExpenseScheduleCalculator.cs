using MisFinanzas.Domain.Common;

namespace MisFinanzas.Application.Expenses.Services
{
    /// <summary>
    /// Decide si un gasto aplica a un mes dado, según su periodicidad y vigencia.
    /// Lógica pura (sin base de datos), para poder probarla de forma aislada.
    /// </summary>
    public static class ExpenseScheduleCalculator
    {
        /// <summary>
        /// Traduce la periodicidad a su salto real en meses.
        /// (El enum es correlativo 1..5, así que NO se puede usar su valor como salto.)
        /// </summary>
        public static int ToMonthStep(Periodicity periodicity) => periodicity switch
        {
            Periodicity.Monthly => 1,
            Periodicity.Bimonthly => 2,
            Periodicity.Quarterly => 3,
            Periodicity.Semiannual => 6,
            Periodicity.Annual => 12,
            _ => throw new ArgumentOutOfRangeException(nameof(periodicity))
        };

        /// <summary>Indica si un gasto aplica al mes objetivo.</summary>
        public static bool AppliesToMonth(
            Periodicity periodicity, string? startMonth, string? endMonth, string targetMonth)
        {
            var target = ToIndex(targetMonth);

            // 1. Vigencia: el mes objetivo debe estar dentro de [StartMonth, EndMonth]
            if (!string.IsNullOrWhiteSpace(startMonth) && target < ToIndex(startMonth))
                return false;
            if (!string.IsNullOrWhiteSpace(endMonth) && target > ToIndex(endMonth))
                return false;

            // 2. Periodicidad: la distancia en meses desde el ancla debe ser múltiplo del salto.
            var anchor = !string.IsNullOrWhiteSpace(startMonth) ? ToIndex(startMonth) : target;
            var step = ToMonthStep(periodicity);
            var distance = target - anchor;

            return distance >= 0 && distance % step == 0;
        }

        /// <summary>Convierte "YYYY-MM" en un índice de mes absoluto (año*12 + mes).</summary>
        private static int ToIndex(string month)
        {
            var parts = month.Split('-');
            var year = int.Parse(parts[0]);
            var mon = int.Parse(parts[1]);
            return year * 12 + (mon - 1);
        }
    }
}