using MisFinanzas.Application.Common;   // ScheduleCalculator
using MisFinanzas.Domain.Common;        // Periodicity
using Xunit;

namespace MisFinanzas.Application.Tests
{
    /// <summary>
    /// Pruebas de la calculadora de periodicidad y vigencia (lógica pura).
    /// Verifica ToMonthStep (traducción del enum) y AppliesToMonth (¿le toca a este mes?).
    /// </summary>
    public class ScheduleCalculatorTests
    {
        // ───────────────────────────────────────────────
        //  ToMonthStep: enum correlativo → salto real
        // ───────────────────────────────────────────────

        [Theory]
        [InlineData(Periodicity.Monthly, 1)]
        [InlineData(Periodicity.Bimonthly, 2)]
        [InlineData(Periodicity.Quarterly, 3)]
        [InlineData(Periodicity.Semiannual, 6)]   // el enum vale 4, pero el salto es 6
        [InlineData(Periodicity.Annual, 12)]      // el enum vale 5, pero el salto es 12
        public void ToMonthStep_TraduceElEnumASuSaltoReal(Periodicity periodicity, int esperado)
        {
            var step = ScheduleCalculator.ToMonthStep(periodicity);
            Assert.Equal(esperado, step);
        }

        [Fact]
        public void ToMonthStep_ConPeriodicidadInvalida_LanzaExcepcion()
        {
            // Un valor que no existe en el enum fuerza el caso _ => throw
            var invalida = (Periodicity)99;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => ScheduleCalculator.ToMonthStep(invalida));
        }

        // ───────────────────────────────────────────────
        //  AppliesToMonth: la periodicidad (ancla = StartMonth)
        // ───────────────────────────────────────────────

        [Theory]
        // Mensual: le toca todos los meses
        [InlineData(Periodicity.Monthly, "2026-01", "2026-02", true)]
        [InlineData(Periodicity.Monthly, "2026-01", "2026-03", true)]
        // Bimestral desde enero: ene sí (dist 0), feb no (1), mar sí (2)
        [InlineData(Periodicity.Bimonthly, "2026-01", "2026-01", true)]
        [InlineData(Periodicity.Bimonthly, "2026-01", "2026-02", false)]
        [InlineData(Periodicity.Bimonthly, "2026-01", "2026-03", true)]
        // Trimestral desde enero: abr sí (3), mar no (2)
        [InlineData(Periodicity.Quarterly, "2026-01", "2026-04", true)]
        [InlineData(Periodicity.Quarterly, "2026-01", "2026-03", false)]
        // Semestral desde enero: jul sí (6), jun no (5)
        [InlineData(Periodicity.Semiannual, "2026-01", "2026-07", true)]
        [InlineData(Periodicity.Semiannual, "2026-01", "2026-06", false)]
        // Anual desde enero: ene 2027 sí (12), dic 2026 no (11)
        [InlineData(Periodicity.Annual, "2026-01", "2027-01", true)]
        [InlineData(Periodicity.Annual, "2026-01", "2026-12", false)]
        public void AppliesToMonth_SegunPeriodicidad(
            Periodicity periodicity, string startMonth, string targetMonth, bool esperado)
        {
            // Sin EndMonth: vigencia abierta hacia el futuro
            var aplica = ScheduleCalculator.AppliesToMonth(
                periodicity, startMonth, endMonth: null, targetMonth);
            Assert.Equal(esperado, aplica);
        }

        // ───────────────────────────────────────────────
        //  AppliesToMonth: la vigencia [StartMonth, EndMonth]
        // ───────────────────────────────────────────────

        [Theory]
        // Antes del StartMonth → nunca aplica, aunque la periodicidad calzara
        [InlineData("2026-06", null, "2026-03", false)]
        // Después del EndMonth → ya no aplica
        [InlineData("2026-01", "2026-04", "2026-07", false)]
        // Dentro del rango → aplica
        [InlineData("2026-01", "2026-12", "2026-05", true)]
        // En el borde EndMonth → todavía aplica (es inclusivo)
        [InlineData("2026-01", "2026-05", "2026-05", true)]
        public void AppliesToMonth_RespetaLaVigencia(
            string startMonth, string? endMonth, string targetMonth, bool esperado)
        {
            // Mensual, para aislar el efecto de la vigencia del de la periodicidad
            var aplica = ScheduleCalculator.AppliesToMonth(
                Periodicity.Monthly, startMonth, endMonth, targetMonth);
            Assert.Equal(esperado, aplica);
        }

        // ───────────────────────────────────────────────
        //  Caso fino: sin StartMonth, el ancla es el propio mes
        // ───────────────────────────────────────────────

        [Theory]
        [InlineData(Periodicity.Monthly, "2026-07")]
        [InlineData(Periodicity.Annual, "2026-07")]      // ¡incluso anual aplica!
        [InlineData(Periodicity.Semiannual, "2026-03")]
        public void AppliesToMonth_SinStartMonth_AplicaSiempre(
            Periodicity periodicity, string targetMonth)
        {
            // Sin StartMonth el ancla es el propio mes → distancia 0 → aplica.
            // (En el sistema real la generación siempre pasa un StartMonth; este es
            //  el comportamiento por defecto cuando no hay vigencia que fije el ritmo.)
            var aplica = ScheduleCalculator.AppliesToMonth(
                periodicity, startMonth: null, endMonth: null, targetMonth);
            Assert.True(aplica);
        }

        [Theory]
        [InlineData(Periodicity.Bimonthly, "2025-11", "2025-12", false)] // distancia 1 → no
        [InlineData(Periodicity.Bimonthly, "2025-11", "2026-01", true)]  // distancia 2, cruzó el año → sí
        [InlineData(Periodicity.Annual, "2024-03", "2026-03", true)]  // distancia 24, dos años exactos → sí
        public void AppliesToMonth_CruzaElCambioDeAnio(
    Periodicity periodicity, string startMonth, string targetMonth, bool esperado)
        {
            var aplica = ScheduleCalculator.AppliesToMonth(
                periodicity, startMonth, endMonth: null, targetMonth);
            Assert.Equal(esperado, aplica);
        }
    }
}