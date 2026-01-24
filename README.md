# FitFanShop Backend API

**Clean Architecture REST API** built with **.NET 8** for e-commerce and event ticketing platform.

---

## ?? Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [API Documentation](#api-documentation)
- [Authentication](#authentication)
- [Testing](#testing)
- [Project Structure](#project-structure)
- [Key Features](#key-features)

---

## ?? Overview

FitFanShop is a comprehensive backend API supporting:
- **E-commerce**: Product catalog, shopping cart, orders, payments
- **Event Ticketing**: Events, ticket types, ticket sales
- **User Management**: Registration, authentication, profiles, memberships
- **Reviews & Ratings**: Product reviews with statistics
- **Admin Dashboard**: Activity logs, discount management

---

## ??? Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
???????????????????????????????????????????????????????????
?                    FitFanShop.API                       ?
?          (Controllers, Middleware, Configuration)       ?
???????????????????????????????????????????????????????????
                        ?
???????????????????????????????????????????????????????????
?               FitFanShop.Application                    ?
?     (CQRS Commands/Queries, Business Logic, DTOs)       ?
???????????????????????????????????????????????????????????
                        ?
        ?????????????????????????????????
        ?                               ?
????????????????????          ??????????????????????
?   FitFanShop     ?          ?   FitFanShop       ?
?   Domain         ?          ?   Infrastructure   ?
?   (Entities)     ?          ?   (EF Core, DB)    ?
????????????????????          ??????????????????????
        ?
        ?
????????????????????
?   FitFanShop     ?
?   Shared         ?
?   (Common DTOs)  ?
????????????????????
```

### Layer Responsibilities

| Layer | Responsibility |
|-------|---------------|
| **API** | HTTP endpoints, middleware, CORS, Swagger, dependency injection setup |
| **Application** | CQRS implementation (MediatR), business logic, validation (FluentValidation), DTOs |
| **Domain** | Entities, value objects, domain logic |
| **Infrastructure** | EF Core, database context, migrations, interceptors, seeders |
| **Shared** | Shared DTOs, options, constants |
| **Tests** | Integration tests with WebApplicationFactory |

---

## ??? Tech Stack

- **.NET 8** - Target framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with SQL Server
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **JWT Authentication** - Secure authentication with refresh tokens
- **Serilog** - Structured logging
- **xUnit** - Integration testing framework
- **Swagger/OpenAPI** - API documentation

---

## ? Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, or full version)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Rider](https://www.jetbrains.com/rider/)
- [Git](https://git-scm.com/)

---

## ?? Getting Started

### 1. Clone Repository

```bash
git clone https://dev.azure.com/rs1-2025-26-FIT-FanShop/FITFanShop/_git/FITFanShop
cd FitFanShop/FitFanShop.Backend
```

### 2. Configure Connection String

Edit `FitFanShop.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FitFanShopDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build Solution

```bash
dotnet build
```

### 5. Run Application

```bash
cd FitFanShop.API
dotnet run
```

**API will be available at:**
- HTTPS: `https://localhost:7260`
- HTTP: `http://localhost:5099`
- Swagger UI: `https://localhost:7260/swagger`

---

## ?? Database Setup

### Apply Migrations

```bash
cd FitFanShop.Infrastructure
dotnet ef database update --startup-project ../FitFanShop.API
```

### Seed Database

Database is automatically seeded on first run with:

**Static Data:**
- Roles: `User`, `Admin`
- Order Statuses: `Pending`, `Confirmed`, `Delivered`, `Cancelled`

**Dynamic Data (via `DynamicDataSeeder`):**
- Admin user: `admin@fitfanshop.com` / `Admin123!`
- Sample categories, products, events, tickets

### Create New Migration

```bash
cd FitFanShop.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../FitFanShop.API
```

---

## ?? API Documentation

Access interactive Swagger documentation at: **`https://localhost:7260/swagger`**

### ?? Authentication Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Register new user | ? |
| POST | `/api/auth/login` | Login and get JWT token | ? |
| POST | `/api/auth/refresh` | Refresh access token | ? |
| POST | `/api/auth/logout` | Logout and invalidate refresh token | ? |

**Register/Login Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
  "expiresAt": "2025-01-24T12:00:00Z"
}
```

---

### ?? Products Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/products` | Get all products (paginated, filterable) | ? |
| GET | `/api/products/{id}` | Get product by ID | ? |
| POST | `/api/products` | Create new product | ? Admin |
| PUT | `/api/products/{id}` | Update product | ? Admin |
| DELETE | `/api/products/{id}` | Soft delete product | ? Admin |

**Query Parameters for GET /api/products:**
- `page` (int): Page number
- `pageSize` (int): Items per page
- `search` (string): Search by name/description
- `categoryId` (int): Filter by category
- `minPrice` (decimal): Minimum price
- `maxPrice` (decimal): Maximum price
- `sortBy` (string): Sort field (Name, Price, CreatedAt)
- `sortOrder` (string): asc/desc

---

### ?? Categories Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/categories` | Get all categories | ? |
| GET | `/api/categories/{id}` | Get category by ID | ? |
| POST | `/api/categories` | Create category | ? Admin |
| PUT | `/api/categories/{id}` | Update category | ? Admin |
| DELETE | `/api/categories/{id}` | Soft delete category | ? Admin |

---

### ??? Cart Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/cart` | Get user's cart | ? User |
| POST | `/api/cart/items` | Add item to cart | ? User |
| PUT | `/api/cart/items/{id}` | Update cart item quantity | ? User |
| DELETE | `/api/cart/items/{id}` | Remove item from cart | ? User |
| DELETE | `/api/cart/clear` | Clear entire cart | ? User |

---

### ?? Orders Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/orders` | Get user's orders | ? User |
| GET | `/api/orders/all` | Get all orders (admin) | ? Admin |
| GET | `/api/orders/{id}` | Get order by ID | ? User |
| POST | `/api/orders` | Create order from cart | ? User |
| PUT | `/api/orders/{id}/status` | Update order status | ? Admin |

**Order Statuses:**
- `Pending` - Order created, awaiting payment
- `Confirmed` - Payment confirmed (auto-set after payment)
- `Delivered` - Order delivered (admin sets manually)
- `Cancelled` - Order cancelled

---

### ?? Payments Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/payments/mock-checkout` | Mock payment & create order | ? User |
| GET | `/api/payments/order/{orderId}` | Get payment for order | ? User |

**Mock Checkout Workflow:**
1. Validates cart items (checks product availability)
2. Creates order from cart items
3. Creates mock payment record
4. **Auto-confirms order** (sets status to `Confirmed`)
5. Clears cart

---

### ? Reviews Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/reviews/product/{productId}` | Get product reviews (paginated) | ? |
| GET | `/api/reviews/product/{productId}/stats` | Get review statistics | ? |
| GET | `/api/reviews/my` | Get user's reviews | ? User |
| GET | `/api/reviews/{id}` | Get review by ID | ? |
| POST | `/api/reviews` | Create review | ? User |
| PUT | `/api/reviews/{id}` | Update review | ? User (own reviews) |
| DELETE | `/api/reviews/{id}` | Delete review | ? User (own reviews) |

**Review Stats Response:**
```json
{
  "productId": 1,
  "totalReviews": 42,
  "averageRating": 4.5,
  "ratingDistribution": {
    "5": 20,
    "4": 15,
    "3": 5,
    "2": 1,
    "1": 1
  }
}
```

---

### ??? Events Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/events` | Get all events | ? |
| GET | `/api/events/{id}` | Get event by ID | ? |
| POST | `/api/events` | Create event | ? Admin |
| PUT | `/api/events/{id}` | Update event | ? Admin |
| DELETE | `/api/events/{id}` | Soft delete event | ? Admin |

---

### ?? Wishlist Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/wishlist` | Get user's wishlist | ? User |
| POST | `/api/wishlist/items` | Add product to wishlist | ? User |
| DELETE | `/api/wishlist/items/{id}` | Remove from wishlist | ? User |

---

### ?? Discounts Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/discounts` | Get all discounts | ? Admin |
| GET | `/api/discounts/{id}` | Get discount by ID | ? Admin |
| POST | `/api/discounts` | Create discount | ? Admin |
| PUT | `/api/discounts/{id}` | Update discount | ? Admin |
| DELETE | `/api/discounts/{id}` | Delete discount | ? Admin |

---

### ?? Users Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/users` | Get all users | ? Admin |
| GET | `/api/users/{id}` | Get user by ID | ? Admin |
| GET | `/api/users/profile` | Get current user profile | ? User |
| PUT | `/api/users/profile` | Update profile | ? User |
| DELETE | `/api/users/{id}` | Delete user | ? Admin |

---

### ?? Activity Logs Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/activitylogs` | Get activity logs (admin only) | ? Admin |

---

## ?? Authentication

### JWT Configuration

JWT tokens are used for authentication with the following settings:

- **Access Token Lifetime**: 60 minutes
- **Refresh Token Lifetime**: 7 days
- **Algorithm**: HMAC-SHA256

### Using Authentication

1. **Register or Login** to get tokens:
```bash
POST /api/auth/login
{
  "email": "admin@fitfanshop.com",
  "password": "Admin123!"
}
```

2. **Include token in requests**:
```
Authorization: Bearer <access_token>
```

3. **Refresh expired tokens**:
```bash
POST /api/auth/refresh
{
  "refreshToken": "<refresh_token>"
}
```

### Default Credentials

**Admin Account:**
```
Email: admin@fitfanshop.com
Password: Admin123!
```

### Roles

- **User**: Standard customer (can shop, review, manage cart)
- **Admin**: Full access (manage products, categories, orders, users)

---

## ?? Testing

### Run All Tests

```bash
cd FitFanShop.Tests
dotnet test
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~ReviewCrudTests"
```

### Test Coverage

Current test suite includes:
- ? **Product CRUD Tests** (22 tests)
- ? **Review CRUD Tests** (21 tests)
- ? **Cart Tests**
- ? **Order Tests**
- ? **Authentication Tests**
- **Total: 80+ integration tests**

Tests use `WebApplicationFactory` for realistic end-to-end testing with in-memory database.

---

## ?? Project Structure

```
FitFanShop.Backend/
?
??? FitFanShop.API/                 # Entry point, controllers, middleware
?   ??? Controllers/                # API endpoints
?   ??? Middleware/                 # Exception handling, localization
?   ??? Program.cs                  # Application configuration
?   ??? appsettings.json            # Configuration files
?
??? FitFanShop.Application/         # Business logic layer
?   ??? Abstractions/               # Interfaces (IAppDbContext, IAppCurrentUser)
?   ??? Modules/                    # CQRS organized by feature
?   ?   ??? Auth/                   # Authentication (Register, Login, Refresh)
?   ?   ??? Catalog/                # Products, Categories
?   ?   ??? Sales/                  # Orders, Cart
?   ?   ??? Payments/               # Mock checkout
?   ?   ??? Reviews/                # Product reviews
?   ?   ??? Events/                 # Event management
?   ??? Common/                     # Shared utilities (PageResult, Exceptions)
?   ??? Behaviors/                  # MediatR pipeline behaviors (Validation)
?
??? FitFanShop.Domain/              # Domain entities
?   ??? Entities/
?       ??? Identity/               # User, Role, RefreshToken
?       ??? Catalog/                # Product, Category, ProductVariant
?       ??? Sales/                  # Order, OrderItem, Cart, CartItem
?       ??? Reviews/                # Review
?       ??? Events/                 # Event, TicketType, Ticket
?       ??? Common/                 # BaseEntity (soft delete, audit)
?
??? FitFanShop.Infrastructure/      # Data access & external services
?   ??? Database/
?   ?   ??? DatabaseContext.cs      # EF Core DbContext
?   ?   ??? Configurations/         # Entity configurations
?   ?   ??? Interceptors/           # EF interceptors (cascade delete, stock reduction)
?   ?   ??? Migrations/             # EF Core migrations (28 migrations)
?   ?   ??? Seeders/                # Database seeders
?   ??? Services/                   # Service implementations
?   ??? DependencyInjection.cs      # Infrastructure registration
?
??? FitFanShop.Shared/              # Shared DTOs and options
?   ??? Options/                    # Configuration options (JWT, CORS, Localization)
?
??? FitFanShop.Tests/               # Integration tests
    ??? Products/                   # Product CRUD tests
    ??? Reviews/                    # Review CRUD tests
    ??? Helpers/                    # Test utilities (authenticated clients)
    ??? BaseIntegrationTest.cs      # Test base class
```

---

## ? Key Features

### ?? CQRS Pattern with MediatR

Commands and queries are separated for better maintainability:

```csharp
// Command
public record CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}

// Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    // Implementation
}
```

### ? FluentValidation

Automatic request validation:

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

### ??? Soft Delete Pattern

All entities inherit from `BaseEntity` with soft delete:

```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}
```

Global query filter automatically excludes soft-deleted entities.

### ?? EF Core Interceptors

Custom interceptors for cross-cutting concerns:

- **ProductVariantCascadeDeleteInterceptor**: Auto soft-deletes variants when product is deleted
- **StockReductionInterceptor**: Automatically reduces stock when orders are created
- **ProductCategoryValidationInterceptor**: Validates category relationships

### ?? Pagination

Consistent pagination across all list endpoints:

```json
{
  "items": [...],
  "currentPage": 1,
  "totalPages": 5,
  "pageSize": 10,
  "totalItems": 50,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### ?? Localization

Multilingual support with resource files:
- English (en-US)
- Bosnian (bs-BA)

---

## ?? Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=FitFanShopDb;..."
  },
  "JwtSettings": {
    "Secret": "your-secret-key-min-32-characters",
    "Issuer": "FitFanShop",
    "Audience": "FitFanShopUsers",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  },
  "Localization": {
    "DefaultCulture": "en-US",
    "SupportedCultures": ["en-US", "bs-BA"]
  }
}
```

---

## ?? Development Workflow

### Adding a New Feature

1. **Define Entity** in `FitFanShop.Domain/Entities/`
2. **Add EF Configuration** in `FitFanShop.Infrastructure/Database/Configurations/`
3. **Create Migration**: `dotnet ef migrations add FeatureName`
4. **Implement CQRS** in `FitFanShop.Application/Modules/FeatureName/`
   - Commands (Create, Update, Delete)
   - Queries (GetAll, GetById)
   - DTOs
   - Validators
5. **Add Controller** in `FitFanShop.API/Controllers/`
6. **Write Tests** in `FitFanShop.Tests/FeatureName/`

---

## ?? Deployment

### Build for Production

```bash
dotnet publish -c Release -o ./publish
```

### Environment Variables

Set these in production:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection=<production-connection-string>`
- `JwtSettings__Secret=<strong-secret-key>`

---

## ?? Support

For issues or questions, contact the development team:
- **Backend Lead**: Tuli
- **Repository**: [Azure DevOps](https://dev.azure.com/rs1-2025-26-FIT-FanShop/FITFanShop/_git/FITFanShop)

---

## ?? License

Internal project for FIT FanShop team.

---

**Built with ?? using Clean Architecture and .NET 8**
