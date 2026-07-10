using MisFinanzas.Domain.Users;

namespace MisFinanzas.Application.Auth.Interfaces
{
    /// <summary>Contrato para generar tokens JWT.</summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>Genera un token JWT firmado para el usuario dado.</summary>
        string GenerateToken(ApplicationUser user);
    }
}