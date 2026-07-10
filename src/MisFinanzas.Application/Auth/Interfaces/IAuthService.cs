using MisFinanzas.Application.Auth.Dtos;

namespace MisFinanzas.Application.Auth.Interfaces
{
    /// <summary>Contrato del servicio de autenticación.</summary>
    public interface IAuthService
    {
        /// <summary>Registra un nuevo usuario y devuelve sus datos.</summary>
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);

        /// <summary>Verifica las credenciales y devuelve los datos del usuario con su token.</summary>
        Task<AuthResultDto> LoginAsync(LoginDto dto);
    }
}