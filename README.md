# Milo API

API REST desarrollada en .NET 6.0 para la gestión de un sistema de restaurante, incluyendo funcionalidades de productos, categorías, pedidos, usuarios y autenticación con JWT.

## Tabla de Contenidos

- [Características](#características)
- [Stack Tecnológico](#stack-tecnológico)
- [Arquitectura](#arquitectura)
- [Requisitos](#requisitos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [Documentación de la API](#documentación-de-la-api)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Testing](#testing)
- [Autenticación y Autorización](#autenticación-y-autorización)
- [Health Checks](#health-checks)
- [Logging](#logging)

## Características

- Gestión completa de productos con imágenes en Base64
- Sistema de categorías para organizar productos
- Gestión de pedidos con estados y transiciones validadas
- Sistema de autenticación JWT con roles
- Paginación en todas las listas
- Búsqueda y filtros avanzados
- Ordenamiento dinámico de resultados
- Health checks para monitoreo
- Logging estructurado con Serilog
- Validación de modelos automática
- Sistema de estados de pedidos con máquina de estados

## Stack Tecnológico

### Backend
- .NET 6.0
- ASP.NET Core Web API
- Entity Framework Core 6.0.36
- PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL 6.0.29)
- MediatR (CQRS Pattern)
- JWT Authentication (Microsoft.AspNetCore.Authentication.JwtBearer)
- Swagger/OpenAPI (Swashbuckle.AspNetCore)
- Serilog (Logging estructurado)
- BCrypt.Net-Next (Hashing de contraseñas)

### Testing
- xUnit
- Moq
- FluentAssertions
- Entity Framework Core InMemory

### Frontend
- Vue.js 3 con TypeScript
- Vue Router
- Pinia (State Management)
- Axios (HTTP Client)
- Vite

## Arquitectura

El proyecto sigue Clean Architecture y CQRS, dividido en las siguientes capas:

### Milo.Domain
Contiene las entidades del dominio y la lógica de negocio pura. No tiene dependencias externas.

Entidades principales:
- Producto
- Categoria
- Pedido
- PedidoDetalle
- Usuario
- Rol
- Mesa
- Venta
- VentaDetalle
- Reservacion
- RefreshToken
- EstadoPedido (Enum)

### Milo.Application
Contiene la lógica de aplicación, casos de uso (Commands/Queries) y DTOs. Implementa el patrón CQRS con MediatR.

Estructura:
- Parameter: Queries y Handlers para productos, categorías y pedidos
- Usuarios: Queries y Handlers para autenticación y gestión de usuarios
- Services: Servicios de aplicación (EstadoPedidoService, HealthCheckService, RefreshTokenService)
- Models: DTOs compartidos y configuración

### Milo.Infrastructure
Contiene la implementación de la persistencia (Entity Framework Core) y servicios externos.

Componentes:
- Persistence: ApplicationDbContext y ApplicationDbContextFactory
- Migrations: Migraciones de Entity Framework Core para PostgreSQL

### MiloAPI
Capa de presentación que contiene los controllers, middleware, configuración y el punto de entrada.

Componentes:
- Controllers: ParameterController, UsuariosController
- Middleware: JwtMiddleware, ValidationMiddleware
- Configuration: Configuración de Swagger y servicios

## Requisitos

- .NET 6.0 SDK o superior
- PostgreSQL 12 o superior
- Visual Studio 2022 / Visual Studio Code / JetBrains Rider
- Node.js 18+ y npm (para el frontend)

## Instalación

1. Clonar el repositorio:
```
git clone <url-del-repositorio>
cd MiloAPI
```

2. Restaurar las dependencias de NuGet:
```
dotnet restore
```

3. Configurar la base de datos:
- Crear una base de datos en PostgreSQL:
```
CREATE DATABASE MiloDB;
CREATE USER milo_admin WITH PASSWORD 'Milo@1706';
GRANT ALL PRIVILEGES ON DATABASE MiloDB TO milo_admin;
```

4. Configurar las variables de entorno o actualizar appsettings.json (ver sección de Configuración)

5. Aplicar las migraciones:
```
dotnet ef database update --project Milo.Infrastructure --startup-project MiloAPI
```

## Configuración

### appsettings.json

El archivo `appsettings.json` contiene las siguientes configuraciones:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MiloDB;Username=milo_admin;Password=Milo@1706"
  },
  "Jwt": {
    "Secret": "tu-secret-key-aqui",
    "LifespanMinutes": 60,
    "Issuer": "milo.api",
    "Audience": "milo.frontend"
  }
}
```

IMPORTANTE: En producción, mover la configuración sensible (ConnectionString, JWT Secret) a variables de entorno o un sistema de gestión de secretos.

### Variables de Entorno (Recomendado para Producción)

- CONNECTION_STRING: Cadena de conexión de PostgreSQL
- JWT_SECRET: Secret key para JWT
- JWT_ISSUER: Issuer del token JWT
- JWT_AUDIENCE: Audience del token JWT

## Ejecución

### Desarrollo

1. Ejecutar el proyecto:
```
dotnet run --project MiloAPI
```

2. La API estará disponible en:
- HTTP: http://localhost:7039
- Swagger UI: http://localhost:7039/swagger

### Producción

1. Compilar el proyecto:
```
dotnet publish -c Release -o ./publish
```

2. Ejecutar desde la carpeta publish:
```
dotnet MiloAPI.dll
```

## Documentación de la API

### Base URL
```
http://localhost:7039/api
```

### Autenticación

La API utiliza JWT Bearer Authentication. Para acceder a endpoints protegidos, incluye el token en el header:

```
Authorization: Bearer <token>
```

El token se obtiene mediante el endpoint de login (ver Endpoints de Usuarios).

### Respuestas de la API

#### Respuesta Exitosa
```json
{
  "code": 200,
  "message": "Procedimiento ejecutado correctamente.",
  "type": "Tipo de operación",
  "stackTrace": null
}
```

#### Respuesta con Lista
```json
{
  "apiResponse": {
    "code": 200,
    "message": "Procedimiento ejecutado correctamente.",
    "type": "Tipo de operación"
  },
  "list": [...]
}
```

#### Respuesta con Objeto
```json
{
  "apiResponse": {
    "code": 200,
    "message": "Procedimiento ejecutado correctamente.",
    "type": "Tipo de operación"
  },
  "data": {...}
}
```

#### Respuesta Paginada
```json
{
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 50,
    "totalPages": 5,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

#### Respuesta de Error
```json
{
  "code": 400,
  "message": "Error, porfavor verificar los datos ingresados",
  "type": "Tipo de operación",
  "stackTrace": "Detalle del error"
}
```

### Endpoints de Usuarios

#### POST /api/Usuarios/login-user
Inicia sesión y obtiene un token JWT.

Request Body:
```json
{
  "email": "usuario@example.com",
  "password": "contraseña123"
}
```

Response:
```json
{
  "apiResponse": {
    "code": 200,
    "message": "Procedimiento ejecutado correctamente.",
    "type": "Login Usuario"
  },
  "data": {
    "id": 1,
    "nombre": "Juan Pérez",
    "email": "usuario@example.com",
    "direccion": "Calle 123",
    "telefono": "3001234567",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "rol": {
      "id": 1,
      "nombre": "Administrador"
    }
  }
}
```

#### POST /api/Usuarios/crear-usuario
Crea un nuevo usuario. No requiere autenticación.

Request Body:
```json
{
  "nombre": "Juan Pérez",
  "email": "usuario@example.com",
  "contraseña": "contraseña123",
  "rol": 1,
  "direccion": "Calle 123",
  "telefono": "3001234567"
}
```

Response:
```json
{
  "code": 200,
  "message": "Procedimiento ejecutado correctamente.",
  "type": "Creación de usuarios."
}
```

#### GET /api/Usuarios/get-roles
Obtiene la lista de roles disponibles. No requiere autenticación.

Response:
```json
{
  "apiResponse": {
    "code": 200,
    "message": "Procedimiento ejecutado correctamente.",
    "type": "Obtener Roles."
  },
  "list": [
    {
      "id": 1,
      "nombre": "Administrador"
    },
    {
      "id": 2,
      "nombre": "Mesero"
    }
  ]
}
```

#### POST /api/Usuarios/admin-roles
Gestiona roles (crear, editar, eliminar). Requiere rol: Administrador.

Query Parameters:
- opcion: "crear", "editar" o "eliminar"

Request Body:
```json
{
  "id": 1,
  "nombre": "Administrador"
}
```

### Endpoints de Productos

#### GET /api/Parameter/get-productos
Obtiene una lista paginada de productos. No requiere autenticación.

Query Parameters:
- page (int, default: 1): Número de página
- pageSize (int, default: 10, max: 100): Tamaño de página
- searchTerm (string, optional): Término de búsqueda (busca en nombre y descripción)
- sortBy (string, optional): Campo por el cual ordenar (nombre, precio, stock, categoria)
- sortDescending (bool, default: false): Orden descendente

Example:
```
GET /api/Parameter/get-productos?page=1&pageSize=10&searchTerm=pizza&sortBy=precio&sortDescending=false
```

Response:
```json
{
  "data": [
    {
      "id": 1,
      "nombre": "Pizza Margarita",
      "descripcion": "Pizza con tomate y queso",
      "precio": 15000,
      "stock": 50,
      "categoriaId": 1,
      "imagen": "base64string...",
      "activo": true,
      "categoria": {
        "id": 1,
        "nombre": "Pizzas"
      }
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 50,
    "totalPages": 5,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

#### GET /api/Parameter/get-productos-filtros
Obtiene productos con filtros avanzados. No requiere autenticación.

Query Parameters (además de los de paginación):
- precioMinimo (decimal, optional): Precio mínimo
- precioMaximo (decimal, optional): Precio máximo
- stockMinimo (int, optional): Stock mínimo
- stockMaximo (int, optional): Stock máximo
- categoriaId (int, optional): Filtrar por categoría
- soloStockBajo (bool, optional): Solo productos con stock bajo
- limiteStockBajo (int, optional): Límite para considerar stock bajo
- soloDisponibles (bool, optional): Solo productos activos

Example:
```
GET /api/Parameter/get-productos-filtros?page=1&pageSize=10&precioMinimo=10000&precioMaximo=20000&categoriaId=1&soloDisponibles=true
```

#### POST /api/Parameter/admin-productos
Gestiona productos (crear, editar, eliminar). Requiere rol: Administrador.

Query Parameters:
- opcion: "crear", "editar" o "eliminar"

Request Body:
```json
{
  "id": 1,
  "nombre": "Pizza Margarita",
  "descripcion": "Pizza con tomate y queso",
  "precio": 15000,
  "stock": 50,
  "categoriaId": 1,
  "imagen": "data:image/jpeg;base64,/9j/4AAQSkZJRg...",
  "activo": true
}
```

Response:
```json
{
  "code": 200,
  "message": "Procedimiento ejecutado correctamente.",
  "type": "Administracion de Productos."
}
```

### Endpoints de Categorías

#### GET /api/Parameter/get-categorias
Obtiene una lista paginada de categorías. No requiere autenticación.

Query Parameters:
- page (int, default: 1): Número de página
- pageSize (int, default: 10, max: 100): Tamaño de página
- searchTerm (string, optional): Término de búsqueda
- sortBy (string, optional): Campo por el cual ordenar
- sortDescending (bool, default: false): Orden descendente

Response:
```json
{
  "data": [
    {
      "id": 1,
      "nombre": "Pizzas"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 10,
    "totalPages": 1,
    "hasNext": false,
    "hasPrevious": false
  }
}
```

#### POST /api/Parameter/admin-categorias
Gestiona categorías (crear, editar, eliminar). Requiere rol: Administrador.

Query Parameters:
- opcion: "crear", "editar" o "eliminar"

Request Body:
```json
{
  "id": 1,
  "nombre": "Pizzas"
}
```

### Endpoints de Pedidos

#### GET /api/Parameter/get-pedidos
Obtiene una lista paginada de pedidos con filtros avanzados. Requiere rol: Administrador o Mesero.

Query Parameters:
- page (int, default: 1): Número de página
- pageSize (int, default: 10, max: 100): Tamaño de página
- searchTerm (string, optional): Término de búsqueda
- sortBy (string, optional): Campo por el cual ordenar
- sortDescending (bool, default: false): Orden descendente
- estadoFiltro (int, optional): Filtrar por estado (1: Pendiente, 2: Preparando, 3: Listo, 4: Entregado, 5: Cancelado)
- fechaDesde (DateTime, optional): Fecha inicial del rango
- fechaHasta (DateTime, optional): Fecha final del rango
- usuarioId (int, optional): Filtrar por usuario
- totalMinimo (decimal, optional): Total mínimo
- totalMaximo (decimal, optional): Total máximo

Example:
```
GET /api/Parameter/get-pedidos?page=1&pageSize=10&estadoFiltro=1&fechaDesde=2024-01-01&fechaHasta=2024-12-31
```

Response:
```json
{
  "data": [
    {
      "id": 1,
      "usuarioId": 1,
      "usuarioNombre": "Juan Pérez",
      "usuarioEmail": "usuario@example.com",
      "fecha": "2024-01-15T10:30:00Z",
      "total": 25000,
      "estado": 1,
      "estadoNombre": "Pendiente",
      "observaciones": "Sin cebolla",
      "detalles": [
        {
          "id": 1,
          "pedidoId": 1,
          "productoId": 1,
          "cantidad": 2,
          "precio": 15000,
          "subtotal": 30000
        }
      ]
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 50,
    "totalPages": 5,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

#### POST /api/Parameter/admin-pedidos
Gestiona pedidos (crear, editar, eliminar). Requiere autenticación.

Query Parameters:
- opcion: "crear", "editar" o "eliminar"

Request Body:
```json
{
  "id": 1,
  "usuarioId": 1,
  "fecha": "2024-01-15T10:30:00Z",
  "total": 25000,
  "estado": 1,
  "observaciones": "Sin cebolla",
  "detalles": [
    {
      "productoId": 1,
      "cantidad": 2,
      "precio": 15000
    }
  ]
}
```

#### GET /api/Parameter/get-estados-pedido
Obtiene la lista de estados disponibles para pedidos. No requiere autenticación.

Response:
```json
{
  "code": 200,
  "message": "Procedimiento ejecutado correctamente.",
  "list": [
    {
      "id": 1,
      "nombre": "Pendiente",
      "descripcion": "Pedido recibido, esperando preparación"
    },
    {
      "id": 2,
      "nombre": "Preparando",
      "descripcion": "En cocina preparando el pedido"
    },
    {
      "id": 3,
      "nombre": "Listo",
      "descripcion": "Pedido terminado, listo para entregar"
    },
    {
      "id": 4,
      "nombre": "Entregado",
      "descripcion": "Pedido entregado al cliente"
    },
    {
      "id": 5,
      "nombre": "Cancelado",
      "descripcion": "Pedido cancelado"
    }
  ]
}
```

#### POST /api/Parameter/cambiar-estado-pedido
Cambia el estado de un pedido validando la transición. Requiere rol: Administrador o Mesero.

Request Body:
```json
{
  "pedidoId": 1,
  "estado": 2,
  "observaciones": "Iniciando preparación"
}
```

Estados válidos y transiciones permitidas:
- Pendiente (1) -> Preparando (2) o Cancelado (5)
- Preparando (2) -> Listo (3) o Cancelado (5)
- Listo (3) -> Entregado (4)
- Entregado (4) -> (Estado final, no hay transiciones)
- Cancelado (5) -> (Estado final, no hay transiciones)

Response:
```json
{
  "code": 200,
  "message": "Procedimiento ejecutado correctamente.",
  "type": "Cambio de Estado de Pedido"
}
```

### Endpoints de Health Checks

#### GET /health
Verifica el estado general del sistema.

Response:
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "checks": [
    {
      "name": "Database",
      "status": "Healthy",
      "description": "Database connection is working",
      "duration": "00:00:00.001"
    },
    {
      "name": "External Services",
      "status": "Healthy",
      "description": "No external services configured",
      "duration": "00:00:00"
    }
  ]
}
```

#### GET /ready
Verifica si la aplicación está lista para recibir tráfico.

#### GET /live
Verifica si la aplicación está viva.

## Estructura del Proyecto

```
MiloAPI/
├── Milo.Domain/               # Capa de dominio
│   └── Entities/             # Entidades del dominio
├── Milo.Application/           # Capa de aplicación
│   ├── Parameter/             # Casos de uso de productos, categorías, pedidos
│   ├── Usuarios/              # Casos de uso de usuarios y autenticación
│   ├── Services/              # Servicios de aplicación
│   └── Models/                # DTOs y modelos compartidos
├── Milo.Infrastructure/       # Capa de infraestructura
│   ├── Persistence/           # DbContext y configuración de EF
│   └── Migrations/            # Migraciones de base de datos
├── MiloAPI/                   # Capa de presentación
│   ├── Controllers/           # Controladores de la API
│   ├── Middleware/            # Middleware personalizado
│   ├── Configuration/         # Configuración de servicios
│   └── Program.cs             # Punto de entrada
└── MiloAPI.Tests/             # Proyecto de pruebas
    ├── Handlers/              # Tests de handlers
    ├── Services/               # Tests de servicios
    └── Controllers/            # Tests de controladores
```

## Testing

El proyecto incluye tests unitarios usando xUnit, Moq y FluentAssertions.

### Ejecutar Tests

```
dotnet test
```

### Cobertura de Tests

- Handlers: GetProductosQueryHandler, CambiarEstadoPedidoQueryHandler, CreateUserQueryHandler, LoginUserQueryHandler
- Services: RefreshTokenService
- Base de datos en memoria para tests de integración

### Ejemplo de Test

```csharp
[Fact]
public async Task Handle_ProductosExistentes_DeberiaRetornarListaPaginada()
{
    // Arrange
    var categoria = new Categoria { Id = 1, Nombre = "Bebidas" };
    _context.categorias.Add(categoria);
    
    var productos = new List<Producto>
    {
        new Producto { Id = 1, Nombre = "Coca Cola", Precio = 2.50m, Stock = 100, CategoriaId = 1 }
    };
    _context.productos.AddRange(productos);
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _handler.Handle(new GetProductosQuery(...), CancellationToken.None);
    
    // Assert
    result.Data.Should().HaveCount(1);
    result.Pagination.TotalItems.Should().Be(1);
}
```

## Autenticación y Autorización

### Roles Disponibles

- Administrador: Acceso completo a todos los endpoints
- Mesero: Acceso a gestión de pedidos y cambio de estados

### Flujo de Autenticación

1. El usuario realiza login mediante POST /api/Usuarios/login-user
2. El servidor valida las credenciales y genera un token JWT
3. El cliente incluye el token en el header Authorization: Bearer <token>
4. El middleware JwtMiddleware valida el token en cada request
5. Los endpoints protegidos verifican los roles mediante [Authorize(Roles = "...")]

### Seguridad

- Contraseñas hasheadas con BCrypt
- Tokens JWT con expiración configurable (default: 60 minutos)
- Validación de tokens en cada request
- Middleware personalizado para manejo de autenticación
- Roles almacenados en claims del token

## Health Checks

La API incluye endpoints de health checks para monitoreo:

- GET /health: Estado general del sistema
- GET /ready: Verifica si está listo para tráfico
- GET /live: Verifica si la aplicación está viva

Cada health check verifica:
- Conexión a la base de datos
- Servicios externos (si están configurados)

## Logging

El proyecto utiliza Serilog para logging estructurado:

- Logs en consola durante desarrollo
- Logs en archivos (carpeta logs/) en producción
- Niveles de log configurables por namespace
- Rotación diaria de archivos de log
- Retención de 7 días de logs

### Configuración de Logging

Los logs se configuran en appsettings.json:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Milo.Application": "Debug"
      }
    }
  }
}
```

## Migraciones de Base de Datos

### Crear una nueva migración

```
dotnet ef migrations add NombreMigracion --project Milo.Infrastructure --startup-project MiloAPI
```

### Aplicar migraciones

```
dotnet ef database update --project Milo.Infrastructure --startup-project MiloAPI
```

### Revertir migración

```
dotnet ef database update NombreMigracionAnterior --project Milo.Infrastructure --startup-project MiloAPI
```

## Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (git checkout -b feature/AmazingFeature)
3. Commit tus cambios (git commit -m 'Add some AmazingFeature')
4. Push a la rama (git push origin feature/AmazingFeature)
5. Abre un Pull Request

## Licencia

Este proyecto es de uso educativo y demostrativo.

## Contacto

Para consultas o sugerencias, contactar al desarrollador.

## Changelog

### Versión 1.0.0
- Implementación inicial de la API
- Sistema de autenticación JWT
- Gestión de productos, categorías y pedidos
- Paginación y filtros avanzados
- Health checks
- Logging estructurado
- Sistema de estados de pedidos con validación de transiciones

