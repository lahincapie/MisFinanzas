using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MisFinanzas.Application.Auth.Interfaces;
using MisFinanzas.Application.Categories.Interfaces;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Application.PaymentMethods.Interfaces;
using MisFinanzas.Domain.Users;
using MisFinanzas.Infrastructure.Auth;
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
            // El DbContext
            services.AddDbContext<MisFinanzasDbContext>(options =>
                options.UseSqlServer(connectionString));

            // El repositorio: cuando pidan ICategoryRepository, entregar CategoryRepository
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IExpenseMonthlyRepository, ExpenseMonthlyRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IIncomeRepository, IncomeRepository>();
            services.AddScoped<IIncomeMonthlyRepository, IncomeMonthlyRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                // Reglas de contraseña
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Email único por usuario
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<MisFinanzasDbContext>();

            return services;
        }
    }
}