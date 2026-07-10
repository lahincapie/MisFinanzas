using Microsoft.AspNetCore.Identity;

namespace MisFinanzas.Domain.Users
{
    /// <summary>
    /// Usuario de la aplicación. Hereda de IdentityUser, que ya trae
    /// Email, UserName, PasswordHash y demás campos de identidad.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>Nombre para mostrar del usuario (campo opcional).</summary>
        public string? DisplayName { get; set; }
    }
}