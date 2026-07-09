using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MisFinanzas.Application.Categories.Interfaces;
using MisFinanzas.Application.Categories.Services;
using MisFinanzas.Application.Expenses.Interfaces;
using MisFinanzas.Application.Expenses.Services;
using MisFinanzas.Application.Incomes.Interfaces;
using MisFinanzas.Application.Incomes.Services;
using System.Reflection;

namespace MisFinanzas.Application
{
    /// <summary>
    /// Registra los servicios de la capa Application (casos de uso y validadores)
    /// en el contenedor de inyección de dependencias.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Servicios de aplicación (casos de uso)
            services.AddScoped<ICategoryService, CategoryService>();

            // Registra automáticamente TODOS los validadores de este proyecto
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IExpenseMonthlyService, ExpenseMonthlyService>();
            services.AddScoped<IExpensePaymentService, ExpensePaymentService>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IIncomeMonthlyService, IncomeMonthlyService>();
            services.AddScoped<IIncomeReceiptService, IncomeReceiptService>();



            return services;
        }
    }
}