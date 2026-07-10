using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MisFinanzas.API.Middleware;
using MisFinanzas.Application;
using MisFinanzas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---------- Servicios (inyección de dependencias) ----------

// Capas del proyecto
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddApplication();

// Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Autenticación con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,               // rechaza tokens caducados
            ValidateIssuerSigningKey = true,       // verifica la firma
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var app = builder.Build();

// ---------- Pipeline de middlewares (el ORDEN importa) ----------

// 1. Manejo de errores: envuelve todo el pipeline
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Swagger (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. Primero autenticación (¿quién eres?), luego autorización (¿puedes?)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();