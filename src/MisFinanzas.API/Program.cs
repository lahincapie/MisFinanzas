using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MisFinanzas.API.Middleware;
using MisFinanzas.Application;
using MisFinanzas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---------- Servicios (inyecci�n de dependencias) ----------

// Lectura de secretos (User Secrets en dev, variables de entorno en produccion).
// Si falta alguno, la app NO arranca y avisa claramente que configurar.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Falta la cadena de conexion 'ConnectionStrings:DefaultConnection'. " +
        "Configurala con: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"...\"");

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException(
        "Falta la clave JWT 'Jwt:Key'. " +
        "Configurala con: dotnet user-secrets set \"Jwt:Key\" \"...\"");

// Capas del proyecto
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();

// Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Autenticaci�n con JWT
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
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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
app.UseDefaultFiles();   // si piden "/", busca index.html por defecto
app.UseStaticFiles();    // entrega los archivos de wwwroot (js, css, html)

// 3. Primero autenticaci�n (�qui�n eres?), luego autorizaci�n (�puedes?)
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();