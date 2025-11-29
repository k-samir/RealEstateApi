# Real Estate API - Hexagonal Architecture

.NET 8 Web API for real estate property management with Clean/Hexagonal Architecture.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     API Layer (Adapters)                    │
│  HTTP Controllers, Middleware, JWT Authentication          │
│                   Port: HTTP/REST API                       │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│              Application Layer (Use Cases/Ports)            │
│  Business Logic, Repository Interfaces, DTOs               │
│     Ports: IPropertyRepository, IUnitRepository            │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                   Domain Layer (Core)                       │
│    Entities: Property, Unit, Amenity, Specification        │
│          Pure business logic, no dependencies              │
└─────────────────────────────────────────────────────────────┘
                       ▲
┌──────────────────────┴──────────────────────────────────────┐
│            Infrastructure Layer (Adapters)                  │
│  EF Core, PostgreSQL, External Services                    │
│    Implements: IPropertyRepository, IUnitRepository         │
└─────────────────────────────────────────────────────────────┘
```

## Project Structure

```
src/
├── RealEstateApi.Domain/           # Core business entities (no dependencies)
│   └── Entities/
│       ├── Property.cs
│       └── Unit.cs
│
├── RealEstateApi.Application/      # Use cases and ports
│   ├── Interfaces/                 # Repository interfaces (ports)
│   │   ├── IPropertyRepository.cs
│   │   └── IUnitRepository.cs
│   └── Services/                   # Business logic services (TBD)
│
├── RealEstateApi.Infrastructure/   # Adapters (EF Core, DB)
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/        # EF Core entity configurations
│   │   └── Repositories/          # Repository implementations
│   └── Migrations/
│
└── RealEstateApi.Api/              # HTTP Adapter (REST API)
    ├── Controllers/
    ├── Middleware/
    └── Program.cs
```

## Hexagonal Architecture Principles

### 1. **Domain Layer** (Center - Pure Business Logic)
- No external dependencies
- Contains entities and business rules
- Independent of frameworks, databases, UI

### 2. **Application Layer** (Use Cases/Ports)
- Defines interfaces (ports) for external dependencies
- Contains business logic orchestration
- Depends only on Domain layer

### 3. **Infrastructure Layer** (Adapters)
- Implements ports defined in Application layer
- Database access (EF Core, PostgreSQL)
- External services
- Depends on Domain and Application

### 4. **API Layer** (HTTP Adapter)
- HTTP controllers and middleware
- JWT authentication
- Request/Response transformation
- Depends on Application and Infrastructure

## Integration with Next.js Frontend

### Authentication Flow

1. **Next.js Better Auth** generates JWT token
2. **Frontend** sends HTTP requests with JWT in `Authorization: Bearer <token>` header
3. **.NET API** validates JWT and extracts user claims (userId, email, role)
4. **API** authorizes based on user role and ownership

### JWT Claims Expected

```json
{
  "sub": "user-id-123",           // User ID from Better Auth
  "email": "agent@example.com",
  "role": "user | admin | superadmin",
  "exp": 1234567890
}
```

## Environment Variables

```bash
# PostgreSQL connection (same database as Next.js)
DATABASE_URL="Host=localhost;Database=realestate;Username=postgres;Password=yourpassword"

# JWT Configuration (from Better Auth)
JWT_ISSUER="http://localhost:3000"          # Next.js URL
JWT_AUDIENCE="http://localhost:5000"        # .NET API URL
JWT_SECRET_KEY="<fetch from Better Auth JWKS endpoint>"
```

## Development Commands

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API
cd src/RealEstateApi.Api
dotnet run

# Create migration
dotnet ef migrations add InitialCreate --project src/RealEstateApi.Infrastructure --startup-project src/RealEstateApi.Api

# Apply migrations
dotnet ef database update --project src/RealEstateApi.Infrastructure --startup-project src/RealEstateApi.Api

# Run tests
dotnet test
```

## API Endpoints (Planned)

### Properties
- `GET /api/properties` - List all properties (public + filtered)
- `GET /api/properties/{id}` - Get property details
- `POST /api/properties` - Create property (agent only)
- `PUT /api/properties/{id}` - Update property (owner or admin)
- `DELETE /api/properties/{id}` - Delete property (owner or admin)

### Units
- `GET /api/properties/{propertyId}/units` - List units for a property
- `POST /api/properties/{propertyId}/units` - Create unit (owner or admin)
- `PUT /api/units/{id}` - Update unit (owner or admin)
- `DELETE /api/units/{id}` - Delete unit (owner or admin)

## Next Steps

1. ✅ Setup project structure with Hexagonal Architecture
2. ✅ Create domain entities (Property, Unit)
3. ✅ Define repository interfaces (ports)
4. ⏳ Implement EF Core DbContext and repositories (adapters)
5. ⏳ Setup JWT authentication from Better Auth
6. ⏳ Create REST API controllers
7. ⏳ Add authorization policies (agent, admin)
8. ⏳ Update Next.js to call .NET API instead of direct DB

## Benefits of This Architecture

- **Testable**: Core logic has no external dependencies
- **Flexible**: Easy to swap databases or frameworks
- **Maintainable**: Clear separation of concerns
- **Scalable**: Can add new adapters (GraphQL, gRPC) easily
- **Independent**: Domain logic doesn't change when infrastructure changes
