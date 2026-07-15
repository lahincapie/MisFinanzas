using MisFinanzas.Application.Common;
using MisFinanzas.Domain.Expenses;
using MisFinanzas.Domain.Incomes;
using Xunit;

namespace MisFinanzas.Application.Tests
{
    /// <summary>
    /// Pruebas de los estados calculados: vencido (gastos) y atrasado (ingresos).
    /// Al recibir 'today' como parámetro, podemos simular cualquier fecha.
    /// </summary>
    public class OverdueTests
    {
        [Fact]
        public void IsOverdue_PendienteYPasoElDia_EsVencido()
        {
            // Arrange: gasto pendiente, vence el día 20, y "hoy" es el 25 del mismo mes
            var status = ExpenseStatus.Pending;
            var dueDay = 20;
            var today = new DateTime(2026, 7, 25);

            // Act
            var resultado = ProjectionCalculator.IsOverdue(status, "2026-07", dueDay, today);

            // Assert: pasó el día 20 y sigue pendiente → vencido
            Assert.True(resultado);
        }

        [Fact]
        public void IsOverdue_PendientePeroNoHaLlegadoElDia_NoEsVencido()
        {
            // Arrange: vence el 20, pero "hoy" es el 15 (aún no llega)
            var today = new DateTime(2026, 7, 15);

            // Act
            var resultado = ProjectionCalculator.IsOverdue(
                ExpenseStatus.Pending, "2026-07", 20, today);

            // Assert: no ha pasado el día → no vencido
            Assert.False(resultado);
        }

        [Fact]
        public void IsOverdue_YaPagado_NuncaEsVencido()
        {
            // Arrange: aunque "hoy" (25) pasó el día de pago (20), ya está PAGADO
            var today = new DateTime(2026, 7, 25);

            // Act
            var resultado = ProjectionCalculator.IsOverdue(
                ExpenseStatus.Paid, "2026-07", 20, today);

            // Assert: un gasto pagado no puede estar vencido
            Assert.False(resultado);
        }

        [Fact]
        public void IsLate_IngresoPendienteYPasoElDia_EsAtrasado()
        {
            // Arrange: ingreso esperado el día 5, "hoy" es el 10, sigue pendiente
            var today = new DateTime(2026, 7, 10);

            // Act
            var resultado = ProjectionCalculator.IsLate(
                IncomeStatus.Pending, "2026-07", 5, today);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void IsLate_YaRecibido_NuncaEsAtrasado()
        {
            // Arrange: aunque pasó el día esperado, ya se RECIBIÓ
            var today = new DateTime(2026, 7, 10);

            // Act
            var resultado = ProjectionCalculator.IsLate(
                IncomeStatus.Received, "2026-07", 5, today);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void IsOverdue_DiaVence31EnFebrero_SeAjustaAlUltimoDia()
        {
            // Arrange: vence el "31", pero febrero 2026 tiene 28 días.
            // "Hoy" es 28 de febrero → es el último día, aún NO ha pasado.
            var today = new DateTime(2026, 2, 28);

            // Act
            var resultado = ProjectionCalculator.IsOverdue(
                ExpenseStatus.Pending, "2026-02", 31, today);

            // Assert: el día efectivo es el 28; hoy ES el 28, no lo superó → no vencido
            Assert.False(resultado);
        }

        [Fact]
        public void IsOverdue_DiaVence31EnFebrero_YaPasoElUltimoDia_EsVencido()
        {
            // Arrange: vence el "31" (ajustado al 28 en febrero), y "hoy" es 1 de marzo
            var today = new DateTime(2026, 3, 1);

            // Act
            var resultado = ProjectionCalculator.IsOverdue(
                ExpenseStatus.Pending, "2026-02", 31, today);

            // Assert: ya pasamos el último día de febrero → vencido
            Assert.True(resultado);
        }

        [Fact]
        public void IsOverdue_DiaVence31EnMesDe31_FuncionaNormal()
        {
            // Arrange: vence el 31, y el mes SÍ tiene 31 días (julio). "Hoy" es 30.
            var today = new DateTime(2026, 7, 30);

            // Act
            var resultado = ProjectionCalculator.IsOverdue(
                ExpenseStatus.Pending, "2026-07", 31, today);

            // Assert: aún no llega el 31 → no vencido
            Assert.False(resultado);
        }
    }
}