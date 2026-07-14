using MisFinanzas.Domain.Expenses;
using MisFinanzas.Domain.Incomes;

namespace MisFinanzas.Application.Common
{
    /// <summary>
    /// Calcula la proyección de gastos e ingresos pendientes según las reglas del SRS.
    /// Lógica pura: sin base de datos, fácil de probar.
    /// </summary>
    public static class ProjectionCalculator
    {
        /// <summary>
        /// Proyección de un gasto en un mes.
        /// Pagado -> 0. Fijo pendiente -> valor esperado. Variable pendiente -> promedio de últimos 3 pagos.
        /// </summary>
        /// <param name="status">Estado del registro mensual.</param>
        /// <param name="isVariable">Si el gasto es variable.</param>
        /// <param name="expectedAmount">Valor esperado (solo lo tienen los fijos).</param>
        /// <param name="averageOfLastPayments">Promedio de los últimos 3 pagos (0 si no hay histórico).</param>
        public static decimal ProjectExpense(
            ExpenseStatus status,
            bool isVariable,
            decimal? expectedAmount,
            decimal averageOfLastPayments)
        {
            // Ya pagado: no se proyecta nada (cuenta como real)
            if (status == ExpenseStatus.Paid)
                return 0;

            // Pendiente y variable: se estima con el promedio (0 si no hay histórico)
            if (isVariable)
                return averageOfLastPayments;

            // Pendiente y fijo: su valor esperado (0 si por alguna razón no lo tiene)
            return expectedAmount ?? 0;
        }

        /// <summary>
        /// Proyección de un ingreso en un mes.
        /// Recibido -> 0. Variable pendiente -> 0 (regla del SRS). Fijo pendiente -> valor esperado.
        /// </summary>
        public static decimal ProjectIncome(
            IncomeStatus status,
            bool isVariable,
            decimal? expectedAmount)
        {
            // Ya recibido: no se proyecta (cuenta como real)
            if (status == IncomeStatus.Received)
                return 0;

            // Pendiente y VARIABLE: no proyecta, aunque tenga valor esperado (regla del SRS:
            // criterio conservador — no contar con dinero que puede no llegar)
            if (isVariable)
                return 0;

            // Pendiente y fijo: su valor esperado
            return expectedAmount ?? 0;
        }

        /// <summary>
        /// CALCULADO: un gasto está vencido si sigue pendiente y ya pasó su día de pago oportuno.
        /// </summary>
        public static bool IsOverdue(ExpenseStatus status, string month, int dueDay, DateTime today)
        {
            if (status != ExpenseStatus.Pending) return false;   // pagado no puede estar vencido
            return HasDayPassed(month, dueDay, today);
        }

        /// <summary>
        /// CALCULADO: un ingreso está atrasado si sigue pendiente y ya pasó su día esperado.
        /// </summary>
        public static bool IsLate(IncomeStatus status, string month, int expectedDay, DateTime today)
        {
            if (status != IncomeStatus.Pending) return false;
            return HasDayPassed(month, expectedDay, today);
        }

        /// <summary>
        /// ¿Ya pasó el día 'day' del mes 'YYYY-MM', comparado con 'today'?
        /// Ajusta al último día del mes si el mes es corto (regla del SRS).
        /// </summary>
        private static bool HasDayPassed(string month, int day, DateTime today)
        {
            var parts = month.Split('-');
            var year = int.Parse(parts[0]);
            var monthNumber = int.Parse(parts[1]);

            // Si el mes no tiene ese día (ej. 31 en febrero), se usa el último día del mes
            var daysInMonth = DateTime.DaysInMonth(year, monthNumber);
            var effectiveDay = Math.Min(day, daysInMonth);

            var limitDate = new DateTime(year, monthNumber, effectiveDay);

            return today.Date > limitDate;
        }
    }
}