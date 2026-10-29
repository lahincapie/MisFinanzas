using Microsoft.AspNetCore.Identity;
using MisFinanzas.Application.Auth.Dtos;
using MisFinanzas.Application.Auth.Interfaces;
using MisFinanzas.Application.Categories;
using MisFinanzas.Application.Categories.Interfaces;
using MisFinanzas.Domain.Users;

namespace MisFinanzas.Application.Auth.Services
{
    /// <summary>
    /// Servicio de autenticación: usa el UserManager de Identity para
    /// crear y validar usuarios de forma segura.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly ICategoryRepository _categoryRepository;


        public AuthService(UserManager<ApplicationUser> userManager,
            IJwtTokenGenerator tokenGenerator,
            ICategoryRepository categoryRepository)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _categoryRepository = categoryRepository;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            // 1. ¿Ya existe un usuario con ese email?
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing is not null)
                throw new InvalidOperationException("Ya existe una cuenta con ese correo.");

            // 2. Crear el usuario
            var user = new ApplicationUser
            {
                UserName = dto.Email,   // se usa el email como nombre de usuario
                Email = dto.Email,
                DisplayName = dto.DisplayName
            };

            // 3. Identity crea el usuario y ENCRIPTA la contraseña
            var result = await _userManager.CreateAsync(user, dto.Password);

            // 4. Si Identity rechazó algo (ej. contraseña débil), reportarlo
            if (!result.Succeeded)
            {
                var errores = string.Join(" ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"No se pudo registrar: {errores}");
            }

            // 5. Sembrar las categorías por defecto del usuario nuevo (RF-USR-01)
            foreach (var category in DefaultCategories.BuildFor(user.Id))
                await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            // 6. Devolver los datos (el Token se llenará cuando implementemos login)
            return new AuthResultDto
            {
                UserId = user.Id,
                Email = user.Email!,
                Token = string.Empty
            };
        }
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            // 1. Buscar al usuario por email
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                throw new UnauthorizedAccessException("Correo o contraseña incorrectos.");

            // 2. Verificar la contraseña (Identity la compara contra el hash guardado)
            var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordOk)
                throw new UnauthorizedAccessException("Correo o contraseña incorrectos.");

            // 3. Credenciales válidas: generar el token
            var token = _tokenGenerator.GenerateToken(user);

            // 4. Devolver los datos + el token
            return new AuthResultDto
            {
                UserId = user.Id,
                Email = user.Email!,
                Token = token
            };
        }
    }
}