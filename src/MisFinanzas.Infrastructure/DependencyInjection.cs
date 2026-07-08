using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MisFinanzas.Application.Categories.Interfaces;
using MisFinanzas.Infrastructure.Persistence;
using MisFinanzas.Infrastructure.Repositories;

namespace MisFinanzas.Infrastructure
{
    /// <summary>
    /// Registra los servicios de la capa Infrastructure (DbContext y repositorios)
    /// en el contenedor de inyección de dependencias.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, string connectionString)
        {
            // El DbContext (antes estaba en Program.cs; lo movemos aquí)
            services.AddDbContext<MisFinanzasDbContext>(options =>
                options.UseSqlServer(connectionString));

            // El repositorio: cuando pidan ICategoryRepository, entregar CategoryRepository
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}