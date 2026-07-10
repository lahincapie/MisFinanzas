namespace MisFinanzas.Application.Auth.Dtos
{
    /// <summary>Datos para registrar un nuevo usuario.</summary>
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
    }
}