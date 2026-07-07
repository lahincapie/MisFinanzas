namespace MisFinanzas.Domain.Common
{
    /// <summary>
    /// Clase base de la que heredan todas las entidades del dominio.
    /// Centraliza los campos comunes (identificador, soft-delete y auditoría)
    /// para no repetirlos en cada entidad (principio DRY).
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>Identificador único del registro (llave primaria).</summary>
        public int Id { get; set; }

        /// <summary>
        /// Indica si el registro está activo. Soporta el "soft-delete":
        /// al desactivar, se marca en false en lugar de borrarlo físicamente.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Fecha y hora en que se creó el registro.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Fecha y hora de la última modificación. Nulo si nunca se ha editado.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Sello de concurrencia optimista. SQL Server lo actualiza en cada
        /// cambio de la fila; EF lo usa para detectar ediciones en conflicto.
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
