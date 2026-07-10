using Microsoft.AspNetCore.Mvc;
using MisFinanzas.Application.Auth.Dtos;
using MisFinanzas.Application.Auth.Interfaces;

namespace MisFinanzas.API.Controllers
{
    /// <summary>Endpoints de autenticación: registro e inicio de sesión.</summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Registra un nuevo usuario.</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

        /// <summary>Inicia sesión y devuelve un token JWT.</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
    }
}