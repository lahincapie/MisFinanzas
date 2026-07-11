using MisFinanzas.Domain.Common;

namespace MisFinanzas.Domain.Incomes
{
    /// <summary>
    /// Definición (plantilla) de un ingreso recurrente, por ejemplo "Sueldo".
    /// A diferencia de un gasto, no pertenece a una categoría.
    /// Hereda de BaseEntity.
    /// </summary>
    public class Income : BaseEntity
    {
        /// <summary>Id del usuario dueño de este ingreso.</summary>
        public string UserId { get; set; } = string.Empty;


        /// <summary>Nombre del ingreso. Obligatorio y único por usuario.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Cada cuánto se repite el ingreso (mensual, bimestral, etc.).</summary>
        public Periodicity Periodicity { get; set; }

        /// <summary>
        /// Si es true, el monto es variable y se digita al recibir.
        /// Si es false (fijo), usa ExpectedAmount.
        /// </summary>
        public bool IsVariable { get; set; }

        /// <summary>Valor esperado del ingreso si es fijo. Nulo si es variable.</summary>
        public decimal? ExpectedAmount { get; set; }

        /// <summary>Día del mes (1-31) en que se espera recibir el ingreso.</summary>
        public int ExpectedReceiptDay { get; set; }

        /// <summary>Mes de inicio de vigencia, formato "YYYY-MM". Opcional.</summary>
        public string? StartMonth { get; set; }

        /// <summary>Mes de fin de vigencia, formato "YYYY-MM". Opcional.</summary>
        public string? EndMonth { get; set; }
    }
}