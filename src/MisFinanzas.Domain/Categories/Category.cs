using MisFinanzas.Domain.Common;

namespace MisFinanzas.Domain.Categories
{
    /// <summary>
    /// Categoría con la que el usuario clasifica sus gastos (ej. "Servicios", "Mercado").
    /// Hereda de BaseEntity, así que también tiene Id, soft-delete y auditoría.
    /// </summary>
    public class Category : BaseEntity
    {
        /// <summary>Nombre de la categoría. Obligatorio y único por usuario.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Descripción opcional.</summary>
        public string? Description { get; set; }

        /// <summary>Color opcional para mostrarla en la interfaz (ej. un código hexadecimal).</summary>
        public string? Color { get; set; }

        /// <summary>Ícono opcional para identificarla visualmente.</summary>
        public string? Icon { get; set; }

        /// <summary>Orden opcional para controlar cómo se listan las categorías.</summary>
        public int Order { get; set; }
    }
}
