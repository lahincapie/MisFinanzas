namespace MisFinanzas.Application.Auth.Dtos
{
    /// <summary>Resultado de una autenticación exitosa.</summary>
    public class AuthResultDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;   // el JWT (se llena al hacer login)
    }
}