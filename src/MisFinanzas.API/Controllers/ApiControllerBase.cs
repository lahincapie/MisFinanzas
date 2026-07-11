using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MisFinanzas.API.Controllers
{
    /// <summary>
    /// Controller base con utilidades comunes. Expone el UserId del usuario
    /// autenticado, extraído del token JWT.
    /// </summary>
    public abstract class ApiControllerBase : ControllerBase
    {
        /// <summary>Id del usuario autenticado, leído del token.</summary>
        protected string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario.");
    }
}