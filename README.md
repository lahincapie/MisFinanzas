# MisFinanzas

API REST para la gestión de finanzas personales. Automatiza el registro de gastos e ingresos recurrentes: se definen una sola vez como plantillas y el sistema genera los pendientes de cada mes, calcula proyecciones y ofrece una vista consolidada del estado financiero.

Nace de un problema real: reemplazar una hoja de cálculo donde llevaba manualmente el control de mis cuentas mensuales.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
![C#](https://img.shields.io/badge/C%23-13.0-239120)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927)
![License](https://img.shields.io/badge/license-MIT-green)

---

## Tabla de contenidos

- [El problema](#el-problema)
- [Funcionalidades](#funcionalidades)
- [Arquitectura](#arquitectura)
- [Modelo de datos](#modelo-de-datos)
- [Decisiones de diseño](#decisiones-de-diseño)
- [Stack tecnológico](#stack-tecnológico)
- [Puesta en marcha](#puesta-en-marcha)
- [Endpoints](#endpoints)
- [Pruebas](#pruebas)
- [Estado del proyecto](#estado-del-proyecto)

---

## El problema

Llevar las finanzas personales en una hoja de cálculo funciona, pero tiene fricciones reales:

- Cada mes hay que **copiar y pegar** las mismas filas de gastos recurrentes.
- Es fácil **olvidar** un pago hasta que llega el aviso de suspensión.
- No hay forma rápida de saber **cuánto queda por pagar** este mes, ni de **proyectar** cómo cerrará.
- Los gastos con periodicidad distinta (trimestral, anual) se pierden entre los mensuales.

MisFinanzas resuelve esto: se define un gasto una vez ("Internet, mensual, $120.000, vence el 15") y el sistema genera automáticamente su registro de cada mes, calcula qué está vencido y proyecta el cierre.

---

## Funcionalidades

### Gestión de categorías
CRUD completo con soft-delete. Cada usuario define las suyas.

### Gastos e ingresos recurrentes
Se modelan como **plantillas** con periodicidad (mensual, bimestral, trimestral, semestral, anual) y vigencia opcional. El sistema distingue entre:

- **Fijos**: monto conocido de antemano (ej. una suscripción).
- **Variables**: monto que cambia cada mes (ej. la factura de energía).

### Generación automática de pendientes
Un endpoint genera los registros mensuales de los gastos e ingresos que aplican a un mes, según su periodicidad y vigencia. La operación es **idempotente**: ejecutarla varias veces no duplica registros.

### Ciclo de pagos y recepciones
Registrar un pago cambia el estado del mes a *Pagado* y crea el movimiento real, en una **operación atómica**. La reversión devuelve el estado a *Pendiente* y **conserva el movimiento anterior como inactivo**, manteniendo el historial completo para auditoría.

### Dashboard mensual
Vista consolidada con seis métricas —ingresos, gastos y balance, tanto reales como proyectados— y el detalle de cada movimiento del mes con su estado.

Las reglas de proyección son deliberadamente asimétricas, por prudencia financiera:

| Caso | Proyección | Criterio |
|---|---|---|
| Gasto fijo pendiente | Su valor esperado | Se conoce el monto |
| Gasto variable pendiente | Promedio de los últimos 3 pagos | Se estima con el histórico |
| Gasto variable sin histórico | 0 | No hay base para estimar |
| Ingreso fijo pendiente | Su valor esperado | Se conoce el monto |
| **Ingreso variable pendiente** | **0** | **Conservador: no contar con dinero incierto** |
| Ya pagado / recibido | 0 | Cuenta como real, no como proyección |

Los estados **vencido** y **atrasado** no se almacenan: se calculan al consultar, comparando la fecha actual con el día de vencimiento. Un registro pendiente cambia de "al día" a "vencido" sin que nada cambie en la base de datos, por lo que persistirlo obligaría a actualizarlo cada día.

### Autenticación y multiusuario
ASP.NET Core Identity con JWT. Las contraseñas se almacenan como hash. Los tokens se firman con HMAC-SHA256 y expiran.

**Aislamiento de datos**: cada registro pertenece a un usuario y las consultas filtran por su identificador, extraído del token. El acceso a datos ajenos devuelve `404`, no `403`: para un usuario, los datos de otro simplemente no existen.

### Manejo de errores
Middleware global que traduce excepciones a códigos HTTP semánticos, con un formato de respuesta consistente:

```json
{
  "traceId": "0HNMTQIBKIG77:00000001",
  "errorCode": "VALIDATION_ERROR",
  "message": "Uno o más campos no son válidos.",
  "details": ["El nombre es obligatorio."]
}
```

| Excepción | HTTP | Significado |
|---|---|---|
| `ValidationException` | 400 | Datos inválidos |
| `UnauthorizedAccessException` | 401 | Credenciales incorrectas |
| `KeyNotFoundException` | 404 | No existe |
| `InvalidOperationException` | 409 | Conflicto o regla de negocio |

---

## Arquitectura

Arquitectura por capas con **inversión de dependencias**. Las dependencias apuntan siempre hacia adentro:

```
        ┌──────────────────────────────────────┐
        │              API                     │  Controllers, middleware
        └──────────────┬───────────────────────┘
                       │
        ┌──────────────▼───────────────────────┐
        │          Application                 │  Casos de uso, DTOs,
        │  (define los contratos: interfaces)  │  validadores, contratos
        └──────────────┬───────────────────────┘
                       │
        ┌──────────────▼───────────────────────┐
        │            Domain                    │  Entidades, reglas
        │       (no depende de nadie)          │
        └──────────────▲───────────────────────┘
                       │
        ┌──────────────┴───────────────────────┐
        │        Infrastructure                │  EF Core, repositorios,
        │  (implementa los contratos)          │  generación de JWT
        └──────────────────────────────────────┘
```

**Por qué importa**: la capa `Application` define *qué* necesita mediante interfaces; `Infrastructure` define *cómo* se hace, con Entity Framework. La lógica de negocio no conoce SQL Server, lo que la hace independiente de la base de datos y testeable con dobles de prueba.

```
src/
├── MisFinanzas.Domain/          Entidades y enums. Sin dependencias.
├── MisFinanzas.Application/     Servicios, DTOs, interfaces, validadores.
│   ├── Common/                  Lógica pura compartida (calculadoras).
│   ├── Auth/  Categories/  Expenses/  Incomes/  Dashboard/
├── MisFinanzas.Infrastructure/  DbContext, repositorios, migraciones, JWT.
└── MisFinanzas.API/             Controllers, middleware, configuración.
```

### Flujo de una petición

```
Cliente
  │  HTTP
  ▼
Middleware de errores ──── envuelve todo en try/catch
  │
  ▼
Controller ─────────────── recibe y delega; sin lógica de negocio
  │
  ▼
Service ───────────────── valida, aplica reglas, traduce DTO ⇄ entidad
  │
  ▼
Repository ────────────── traduce a SQL con EF Core
  │
  ▼
SQL Server
```

---

## Modelo de datos

Gastos e ingresos se modelan como una **cadena de tres niveles**, que separa la definición recurrente del hecho concreto de cada mes:

```
Expense              ExpenseMonthly           ExpensePayment
(la plantilla)  ──▶  (el mes concreto)   ──▶  (el pago real)

"Internet,           "julio 2026,             "$120.000,
 mensual,             Pendiente/Pagado"        15-jul, PSE"
 $120.000"
```

Un mismo gasto puede estar pagado en enero y pendiente en febrero: por eso el estado vive en el registro mensual, no en la plantilla. Y el pago real se separa porque su monto puede diferir del esperado.

Los ingresos siguen la misma estructura (`Income → IncomeMonthly → IncomeReceipt`), con dos diferencias: no tienen categoría ni medio de pago.

**Convenciones aplicadas a todas las entidades:**
- **Soft-delete** (`IsActive`): los registros no se eliminan, se marcan como inactivos. En una aplicación financiera, perder datos es inaceptable.
- **Auditoría** (`CreatedAt`, `UpdatedAt`) en UTC.
- **Control de concurrencia** (`RowVersion`).

---

## Decisiones de diseño

Algunas decisiones que vale la pena explicar:

**El estado "vencido" no se persiste.** Depende de la fecha actual, no de un cambio en los datos. Un gasto pendiente con vencimiento el día 20 no está vencido el 19 y sí lo está el 21, sin que nadie toque la base de datos. Almacenarlo obligaría a un proceso diario de actualización; calcularlo al consultar es la única opción correcta.

**El enum de periodicidad es correlativo, pero el salto en meses no.** `Semiannual = 4` en el enum, pero su salto real es de 6 meses. Un método de traducción resuelve la conversión. Usar el valor del enum directamente como número de meses habría producido un error silencioso en las periodicidades semestral y anual.

**La lógica de cálculo está aislada en clases puras.** `ScheduleCalculator` (¿aplica este gasto a este mes?) y `ProjectionCalculator` (¿cuánto se proyecta?) no dependen de la base de datos: reciben datos y devuelven un resultado. Esto las hace comprobables de forma unitaria, sin infraestructura.

**Las funciones de fecha reciben "hoy" como parámetro.** En lugar de invocar `DateTime.Now` internamente, `IsOverdue(status, month, dueDay, today)` recibe la fecha. Esto permite comprobar el comportamiento en cualquier fecha simulada.

**El acceso a datos ajenos devuelve 404, no 403.** Filtrar por propietario en la consulta hace que un recurso de otro usuario simplemente no se encuentre. Además de ser más simple, no revela que ese recurso existe.

**La generación de pendientes es idempotente.** Antes de crear, consulta en una sola query qué registros ya existen y omite los duplicados. Ejecutarla dos veces sobre el mismo mes no produce efectos adicionales.

---

## Stack tecnológico

| Componente | Tecnología |
|---|---|
| Lenguaje | C# 13 / .NET 9 |
| Framework web | ASP.NET Core (Web API) |
| ORM | Entity Framework Core 9 (Code First) |
| Base de datos | SQL Server |
| Autenticación | ASP.NET Core Identity + JWT (HMAC-SHA256) |
| Validación | FluentValidation |
| Documentación | Swagger / OpenAPI |
| Pruebas manuales | Postman |

---

## Puesta en marcha

### Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express es suficiente)
- Visual Studio 2022 o VS Code

### Instalación

```bash
git clone https://github.com/lahincapie/MisFinanzas.git
cd MisFinanzas
```

**1. Configura la cadena de conexión** en `src/MisFinanzas.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR\\SQLEXPRESS;Database=MisFinanzasDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "una-clave-secreta-de-al-menos-32-caracteres",
    "Issuer": "MisFinanzasAPI",
    "Audience": "MisFinanzasClient",
    "ExpiryMinutes": 120
  }
}
```

> **Nota de seguridad**: en un entorno de producción, la clave del JWT debe gestionarse mediante variables de entorno o un gestor de secretos, nunca en el archivo de configuración versionado.

**2. Aplica las migraciones** (crea la base de datos y siembra el catálogo de medios de pago):

```bash
cd src/MisFinanzas.Infrastructure
dotnet ef database update --startup-project ../MisFinanzas.API
```

**3. Ejecuta la API:**

```bash
cd ../MisFinanzas.API
dotnet run
```

La documentación interactiva queda disponible en `https://localhost:7002/swagger`.

---

## Endpoints

### Autenticación (público)

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/auth/register` | Crea una cuenta |
| `POST` | `/api/auth/login` | Devuelve un token JWT |

### Categorías

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/categories` | Crea una categoría |
| `GET` | `/api/categories` | Lista las categorías del usuario |
| `PUT` | `/api/categories/{id}` | Edita una categoría |
| `DELETE` | `/api/categories/{id}` | Desactiva una categoría |

### Gastos

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/expenses` | Crea un gasto (plantilla) |
| `GET` | `/api/expenses` | Lista los gastos del usuario |
| `PUT` | `/api/expenses/{id}` | Edita un gasto |
| `DELETE` | `/api/expenses/{id}` | Desactiva un gasto |
| `POST` | `/api/expenses/generate-monthly?month=YYYY-MM` | Genera los pendientes del mes |
| `POST` | `/api/expenses/{id}/months/{month}/pay` | Registra el pago |
| `POST` | `/api/expenses/{id}/months/{month}/revert` | Revierte el pago |

### Ingresos

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/incomes` | Crea un ingreso (plantilla) |
| `GET` | `/api/incomes` | Lista los ingresos del usuario |
| `PUT` | `/api/incomes/{id}` | Edita un ingreso |
| `DELETE` | `/api/incomes/{id}` | Desactiva un ingreso |
| `POST` | `/api/incomes/generate-monthly?month=YYYY-MM` | Genera los pendientes del mes |
| `POST` | `/api/incomes/{id}/months/{month}/receive` | Registra la recepción |
| `POST` | `/api/incomes/{id}/months/{month}/revert` | Revierte la recepción |

### Dashboard

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/dashboard?month=YYYY-MM` | Métricas y detalle del mes |

Todos los endpoints excepto los de autenticación requieren la cabecera `Authorization: Bearer {token}`.

### Ejemplo de respuesta del dashboard

```json
{
  "month": "2026-07",
  "realIncome": 3500000.00,
  "realExpense": 450000.00,
  "realBalance": 3050000.00,
  "projectedIncome": 0,
  "projectedExpense": 1200000.00,
  "projectedBalance": 1850000.00,
  "expenses": [
    {
      "expenseId": 3,
      "name": "Seguro del auto",
      "categoryName": "Servicios",
      "status": "Pendiente",
      "paidAmount": null,
      "projectedAmount": 1200000.00,
      "dueDay": 10,
      "isOverdue": true
    }
  ],
  "incomes": [ "..." ]
}
```

---

## Pruebas

El repositorio incluye colecciones de Postman con más de 130 peticiones:

- **Por módulo**: para probar endpoints de forma individual.
- **Por usuario**: cada carpeta contiene el flujo completo de un usuario, con datos de ejemplo. Ejecutable con el *Collection Runner* para cargar un escenario de prueba completo.

Los scripts gestionan automáticamente los tokens y encadenan los identificadores generados entre peticiones.

Las pruebas automatizadas (unitarias e integración) están contempladas en la siguiente fase.

---

## Estado del proyecto

| Módulo | Estado |
|---|---|
| Arquitectura por capas | ✅ Completo |
| Categorías | ✅ Completo |
| Gastos (3 fases) | ✅ Completo |
| Ingresos (3 fases) | ✅ Completo |
| Autenticación (Identity + JWT) | ✅ Completo |
| Multiusuario | ✅ Completo |
| Dashboard | ✅ Completo |
| Pruebas automatizadas | 🔜 Siguiente |
| Integración continua | 🔜 Planificado |
| Frontend (Angular) | 🔜 Planificado |

---

## Autora

**Alejandra Hincapié** — [@lahincapie](https://github.com/lahincapie)

Proyecto académico de ingeniería de sistemas, desarrollado como práctica de arquitectura de software y buenas prácticas de backend.

## Licencia

MIT
