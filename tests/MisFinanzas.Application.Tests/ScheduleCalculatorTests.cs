using MisFinanzas.Application.Common;   // ScheduleCalculator
using MisFinanzas.Domain.Common;        // Periodicity
using Xunit;

namespace MisFinanzas.Application.Tests
{
    /// <summary>
    /// Pruebas de la calculadora de periodicidad y vigencia (lógica pura).
    /// Orden de argumentos: (periodicity, anchorMonth, startMonth, endMonth, targetMonth).
    /// El ancla del ciclo es AnchorMonth; la vigencia es [StartMonth, EndMonth].
    /// </summary>
    public class ScheduleCalculatorTests
    {
        // ── ToMonthStep: enum correlativo → salto real ──
        [Theory]
        [InlineData(Periodicity.Monthly, 1)]
        [InlineData(Periodicity.Bimonthly, 2)]
        [InlineData(Periodicity.Quarterly, 3)]
        [InlineData(Periodicity.Semiannual, 6)]
        [InlineData(Periodicity.Annual, 12)]
        public void ToMonthStep_TraduceElEnumASuSaltoReal(Periodicity periodicity, int esperado)
        {
            var step = ScheduleCalculator.ToMonthStep(periodicity);
            Assert.Equal(esperado, step);
        }

        [Fact]
        public void ToMonthStep_ConPeriodicidadInvalida_LanzaExcepcion()
        {
            var invalida = (Periodicity)99;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => ScheduleCalculator.ToMonthStep(invalida));
        }

        // ── Periodicidad (ancla = AnchorMonth, vigencia abierta) ──
        [Theory]
        [InlineData(Periodicity.Monthly, "2026-01", "2026-02", true)]
        [InlineData(Periodicity.Monthly, "2026-01", "2026-03", true)]
        [InlineData(Periodicity.Bimonthly, "2026-01", "2026-01", true)]
        [InlineData(Periodicity.Bimonthly, "2026-01", "2026-02", false)]
        [InlineData(Periodicity.Bimonthly, "2026-01", "2026-03", true)]
        [InlineData(Periodicity.Quarterly, "2026-01", "2026-04", true)]
        [InlineData(Periodicity.Quarterly, "2026-01", "2026-03", false)]
        [InlineData(Periodicity.Semiannual, "2026-01", "2026-07", true)]
        [InlineData(Periodicity.Semiannual, "2026-01", "2026-06", false)]
        [InlineData(Periodicity.Annual, "2026-01", "2027-01", true)]
        [InlineData(Periodicity.Annual, "2026-01", "2026-12", false)]
        public void AppliesToMonth_SegunPeriodicidad(
            Periodicity periodicity, string anchorMonth, string targetMonth, bool esperado)
        {
            // (periodicity, anchorMonth, startMonth, endMonth, targetMonth)
            var aplica = ScheduleCalculator.AppliesToMonth(
                periodicity, anchorMonth, null, null, targetMonth);
            Assert.Equal(esperado, aplica);
        }

        // ── Vigencia [StartMonth, EndMonth] (mensual para aislar) ──
        [Theory]
        [InlineData("2026-06", null, "2026-03", false)]
        [InlineData("2026-01", "2026-04", "2026-07", false)]
        [InlineData("2026-01", "2026-12", "2026-05", true)]
        [InlineData("2026-01", "2026-05", "2026-05", true)]
        public void AppliesToMonth_RespetaLaVigencia(
            string startMonth, string? endMonth, string targetMonth, bool esperado)
        {
            // Mensual y sin ancla: aísla el efecto de la vigencia.
            var aplica = ScheduleCalculator.AppliesToMonth(
                Periodicity.Monthly, null, startMonth, endMonth, targetMonth);
            Assert.Equal(esperado, aplica);
        }

        // ── Sin ancla ──
        [Theory]
        [InlineData("2026-07")]
        [InlineData("2026-01")]
        public void AppliesToMonth_MensualSinAncla_Aplica(string targetMonth)
        {
            // Mensual no necesita ancla: aplica siempre (dentro de vigencia).
            var aplica = ScheduleCalculator.AppliesToMonth(
                Periodicity.Monthly, null, null, null, targetMonth);
            Assert.True(aplica);
        }

        [Theory]
        [InlineData(Periodicity.Bimonthly, "2026-07")]
        [InlineData(Periodicity.Quarterly, "2026-07")]
        [InlineData(Periodicity.Semiannual, "2026-03")]
        [InlineData(Periodicity.Annual, "2026-07")]
        public void AppliesToMonth_NoMensualSinAncla_NoAplica(
            Periodicity periodicity, string targetMonth)
        {
            // Sin ancla no se puede calcular el ciclo → no aplica (evita inflar proyecciones).
            var aplica = ScheduleCalculator.AppliesToMonth(
                periodicity, null, null, null, targetMonth);
            Assert.False(aplica);
        }

        // ── Cruce de año (ancla = AnchorMonth) ──
        [Theory]
        [InlineData(Periodicity.Bimonthly, "2025-11", "2025-12", false)]
        [InlineData(Periodicity.Bimonthly, "2025-11", "2026-01", true)]
        [InlineData(Periodicity.Annual, "2024-03", "2026-03", true)]
        public void AppliesToMonth_CruzaElCambioDeAnio(
            Periodicity periodicity, string anchorMonth, string targetMonth, bool esperado)
        {
            var aplica = ScheduleCalculator.AppliesToMonth(
                periodicity, anchorMonth, null, null, targetMonth);
            Assert.Equal(esperado, aplica);
        }
    }
}