using MisFinanzas.Application.Common;
using MisFinanzas.Domain.Expenses;
using MisFinanzas.Domain.Incomes;
using Xunit;

namespace MisFinanzas.Application.Tests
{
    public class ProjectionCalculatorTests
    {
        [Fact]
        public void ProjectExpense_CuandoYaEstaPagado_ProyectaCero()
        {
            // Arrange
            var status = ExpenseStatus.Paid;
            var isVariable = false;
            decimal? expectedAmount = 100000;
            decimal average = 0;

            // Act
            var resultado = ProjectionCalculator.ProjectExpense(
                status, isVariable, expectedAmount, average);

            // Assert
            Assert.Equal(0, resultado);
        }

        [Theory]
        [InlineData(ExpenseStatus.Paid, false, 100000d, 0, 0)]
        [InlineData(ExpenseStatus.Paid, true, null, 5000, 0)]
        [InlineData(ExpenseStatus.Pending, false, 100000d, 0, 100000)]
        [InlineData(ExpenseStatus.Pending, true, null, 80000, 80000)]
        [InlineData(ExpenseStatus.Pending, true, null, 0, 0)]
        public void ProjectExpense_AplicaLaReglaSegunElCaso(
            ExpenseStatus status,
            bool isVariable,
            double? expectedAmount,
            double average,
            double esperado)
        {
            // Los double del InlineData se convierten a decimal (que es lo que usa el método real)
            decimal? expected = expectedAmount.HasValue ? (decimal)expectedAmount.Value : (decimal?)null;

            // Act
            var resultado = ProjectionCalculator.ProjectExpense(
                status, isVariable, expected, (decimal)average);

            // Assert
            Assert.Equal((decimal)esperado, resultado);
        }

        [Theory]
        // status,                  isVariable, expectedAmount, → esperado
        [InlineData(IncomeStatus.Received, false, 3000000d, 0)]        // recibido → 0 (aunque sea fijo)
        [InlineData(IncomeStatus.Received, true, 500000d, 0)]        // recibido → 0 (aunque sea variable)
        [InlineData(IncomeStatus.Pending, false, 3000000d, 3000000)]  // fijo pendiente → su valor esperado
        [InlineData(IncomeStatus.Pending, true, 500000d, 0)]        // ⭐ VARIABLE pendiente → 0 (¡aunque tenga valor esperado!)
        [InlineData(IncomeStatus.Pending, false, null, 0)]        // fijo sin valor esperado → 0 (defensa)
        public void ProjectIncome_AplicaLaReglaSegunElCaso(
            IncomeStatus status,
            bool isVariable,
            double? expectedAmount,
            double esperado)
        {
            // Puente double -> decimal (como en los tests de gasto)
            decimal? expected = expectedAmount.HasValue ? (decimal)expectedAmount.Value : (decimal?)null;

            // Act
            var resultado = ProjectionCalculator.ProjectIncome(status, isVariable, expected);

            // Assert
            Assert.Equal((decimal)esperado, resultado);
        }
    }
}