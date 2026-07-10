namespace MisFinanzas.Application.Auth.Dtos
{
    /// <summary>Datos para iniciar sesión.</summary>
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}