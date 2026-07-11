using MisFinanzas.Domain.Common;
using MisFinanzas.Domain.Categories;

namespace MisFinanzas.Domain.Expenses
{
    /// <summary>
    /// Definición (plantilla) de un gasto recurrente que el usuario debe pagar,
    /// por ejemplo "Internet hogar". Pertenece a una Categoría.
    /// Hereda de BaseEntity (Id, soft-delete, auditoría).
    /// </summary>
    public class Expense : BaseEntity
    {
        /// <summary>Id del usuario dueño de este gasto.</summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>Nombre del gasto. Obligatorio y único por usuario.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Llave foránea: a qué categoría pertenece este gasto.</summary>
        public int CategoryId { get; set; }

        /// <summary>Propiedad de navegación hacia la categoría dueña del gasto.</summary>
        public Category? Category { get; set; }

        /// <summary>Cada cuánto se repite el gasto (mensual, bimestral, etc.).</summary>
        public Periodicity Periodicity { get; set; }

        /// <summary>
        /// Si es true, el monto es variable: se digita al pagar y no usa valor esperado.
        /// Si es false (fijo), usa ExpectedAmount.
        /// </summary>
        public bool IsVariable { get; set; }

        /// <summary>Valor esperado del gasto si es fijo. Nulo si es variable.</summary>
        public decimal? ExpectedAmount { get; set; }

        /// <summary>Día del mes (1-31) en que se corta la facturación.</summary>
        public int CutoffDay { get; set; }

        /// <summary>Día del mes (1-31) límite para pagar a tiempo.</summary>
        public int DueDay { get; set; }

        /// <summary>Día del mes (1-31) en que podrían suspender el servicio por no pago.</summary>
        public int SuspensionDay { get; set; }

        /// <summary>Mes de inicio de vigencia, formato "YYYY-MM". Opcional.</summary>
        public string? StartMonth { get; set; }

        /// <summary>Mes de fin de vigencia, formato "YYYY-MM". Opcional.</summary>
        public string? EndMonth { get; set; }

        /// <summary>Número de referencia o de cuenta del servicio. Opcional.</summary>
        public string? Reference { get; set; }

        /// <summary>Número de contrato. Opcional.</summary>
        public string? Contract { get; set; }
    }
}