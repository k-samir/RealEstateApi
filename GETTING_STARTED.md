# Getting Started with Real Estate API

## What's Been Implemented

### ✅ Complete Features

1. **Hexagonal Architecture Setup**
   - Domain Layer: Property & Unit entities
   - Application Layer: Repository interfaces (ports) + DTOs
   - Infrastructure Layer: EF Core repositories (adapters)
   - API Layer: REST controllers with JWT auth

2. **Properties API** (`/api/properties`)
   - ✅ GET `/api/properties` - List all properties (public, with filters)
   - ✅ GET `/api/properties/{id}` - Get property details (public)
   - ✅ GET `/api/properties/my-properties` - Get agent's properties (authenticated)
   - ✅ POST `/api/properties` - Create property (agents only)
   - ✅ PUT `/api/properties/{id}` - Update property (owner or admin)
   - ✅ DELETE `/api/properties/{id}` - Delete property (owner or admin)

3. **Authentication & Authorization**
   - JWT Bearer authentication
   - Integration with Better Auth tokens from Next.js
   - Role-based authorization (user, admin, superadmin)
   - Owner-based authorization (agents own their properties)

4. **Infrastructure**
   - PostgreSQL database support
   - EF Core 8.0.11 with Npgsql
   - Repository pattern implementation
   - JSON column support for complex types

5. **API Documentation**
   - Swagger UI with JWT support
   - Accessible at root URL in development
   - Full API documentation

## Quick Start

### 1. Prerequisites

- .NET 8 SDK
- PostgreSQL database (same as Next.js)
- Better Auth running on Next.js (for JWT tokens)

### 2. Configuration

The `appsettings.Development.json` is already configured with:
- **Shared Supabase database** (same as Next.js Better Auth)
- **JWT settings** matching Better Auth configuration

Connection details in `src/RealEstateApi.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ankiewsutcjudisofnwt.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=lgARbakPzx9NOGAf;SSL Mode=Require"
  },
  "JwtSettings": {
    "SecretKey": "cfb2c821d47c3a8d3853b658d7566a173aeb2f6f157c75dcb6785e85551955c5",
    "Issuer": "http://localhost:3000",
    "Audience": "http://localhost:3000"
  }
}
```

**Note**: The database is shared with Next.js Better Auth. Property/unit tables are managed by EF Core, auth tables by Drizzle.

### 3. Database Setup

The database schema has already been created with the `InitialPropertySchema` migration which includes:
- **property** table with FK constraint to Better Auth `user` table
- **unit** table with FK constraint to `property` table
- Indexes on `agent_id`, `location`, and `status` columns

If you need to regenerate migrations:

```bash
# Remove existing migration (if needed)
dotnet ef migrations remove --project src/RealEstateApi.Infrastructure --startup-project src/RealEstateApi.Api

# Generate new migration
dotnet ef migrations add MigrationName \
  --project src/RealEstateApi.Infrastructure \
  --startup-project src/RealEstateApi.Api

# Apply migration
dotnet ef database update \
  --project src/RealEstateApi.Infrastructure \
  --startup-project src/RealEstateApi.Api
```

### 4. Run the API

```bash
cd src/RealEstateApi.Api
dotnet run
```

The API will start at:
- HTTP: `http://localhost:5001`
- Swagger UI: `http://localhost:5001/swagger` (in development)

## Testing with Swagger

1. **Open Swagger UI**: Navigate to `https://localhost:5001`

2. **Get JWT Token from Next.js**:
   - Login to Next.js frontend (`http://localhost:3000`)
   - Open browser DevTools → Console
   - Run: `localStorage.getItem('better-auth.session_token')`
   - Or use Better Auth client to get JWT:
     ```typescript
     const token = await authClient.getJWT();
     console.log(token);
     ```

3. **Authorize in Swagger**:
   - Click "Authorize" button
   - Enter: `Bearer YOUR_JWT_TOKEN`
   - Click "Authorize"

4. **Test Endpoints**:
   - Try `GET /api/properties` (no auth required)
   - Try `GET /api/properties/my-properties` (requires auth)
   - Try `POST /api/properties` (requires auth)

## API Endpoints Reference

### Public Endpoints (No Authentication)

```
GET /api/properties
GET /api/properties/{id}
GET /health
```

### Protected Endpoints (Requires Authentication)

```
GET  /api/properties/my-properties  # Agent's properties
POST /api/properties                # Create property
PUT  /api/properties/{id}           # Update (owner or admin)
DELETE /api/properties/{id}         # Delete (owner or admin)
```

### Query Parameters for Filtering

`GET /api/properties?location=Nador&type=Villa&status=published&page=1&pageSize=20`

Parameters:
- `location` - Filter by city/location
- `type` - Property type (Villa, Apartment, etc.)
- `status` - published, draft, sold, reserved
- `search` - Search in name, description, location
- `page` - Page number (default: 1)
- `pageSize` - Results per page (default: 20)

## Example Requests

### Create Property (POST /api/properties)

```json
{
  "name": "Luxury Villa in Nador",
  "location": "Nador",
  "latitude": 35.1686,
  "longitude": -2.9335,
  "type": "Villa",
  "completionDate": "2025",
  "description": "Beautiful modern villa with sea view",
  "longDescription": "Detailed description...",
  "features": ["Piscine", "Jardin", "Garage"],
  "amenities": [
    { "icon": "Waves", "label": "Piscine" },
    { "icon": "Trees", "label": "Jardin" }
  ],
  "specifications": [
    { "label": "Surface", "value": "300m²" },
    { "label": "Chambres", "value": "4" }
  ],
  "mainImage": "/images/villa-main.jpg",
  "images": ["/images/villa-1.jpg", "/images/villa-2.jpg"],
  "bedroomsRange": "4-5",
  "bathroomsRange": "3-4",
  "areaRange": "280-350m²",
  "priceRange": "À partir de 3,500,000 MAD"
}
```

### Update Property (PUT /api/properties/{id})

```json
{
  "status": "published",
  "description": "Updated description"
}
```

Only provided fields will be updated.

## Authorization Rules

### Property Ownership

- **Agents** can only edit/delete their own properties
- **Admins** (`admin`, `superadmin`) can edit/delete any property
- Ownership is checked via `agentId` field (matches JWT `sub` claim)

### JWT Claims Expected

The JWT from Better Auth should contain:

```json
{
  "sub": "user-id-123",           // User ID
  "email": "agent@example.com",
  "role": "user",                 // or "admin", "superadmin"
  "exp": 1234567890
}
```

## Next Steps

### Immediate Tasks

1. ⏳ **Create UnitsController** - Manage units within properties
2. ⏳ **Add image upload** - S3 or local storage for property images
3. ⏳ **Add validation** - FluentValidation for DTOs
4. ⏳ **Add unit tests** - Test business logic

### Integration with Next.js

1. Update Next.js to call .NET API instead of direct DB
2. Example:
   ```typescript
   // BEFORE (Next.js server action)
   const properties = await db.select().from(property);

   // AFTER (.NET API call)
   const token = await authClient.getJWT();
   const response = await fetch('http://localhost:5000/api/properties/my-properties', {
     headers: { 'Authorization': `Bearer ${token}` }
   });
   const properties = await response.json();
   ```

### Future Enhancements

- Image upload/management
- Property search with Elasticsearch
- Caching with Redis
- Rate limiting
- API versioning
- WebSockets for real-time updates

## Troubleshooting

### Database Connection Issues

- Ensure PostgreSQL is running
- Check connection string in `appsettings.Development.json`
- Verify database exists: `createdb realestate`

### JWT Authentication Issues

- Verify JWT secret matches Better Auth configuration
- Check token hasn't expired
- Ensure `Issuer` and `Audience` match between APIs

### Build Errors

- Ensure all NuGet packages are restored: `dotnet restore`
- Check .NET SDK version: `dotnet --version` (should be 8.0.x)

## Project Structure

```
src/
├── RealEstateApi.Domain/           # Pure business entities
│   └── Entities/
│       ├── Property.cs
│       └── Unit.cs
│
├── RealEstateApi.Application/      # Use cases & interfaces
│   ├── Interfaces/
│   │   ├── IPropertyRepository.cs
│   │   └── IUnitRepository.cs
│   └── DTOs/
│       └── PropertyDto.cs
│
├── RealEstateApi.Infrastructure/   # EF Core implementation
│   └── Persistence/
│       ├── ApplicationDbContext.cs
│       ├── Configurations/
│       └── Repositories/
│
└── RealEstateApi.Api/              # REST API
    ├── Controllers/
    │   └── PropertiesController.cs
    └── Program.cs
```

## Support

For issues or questions:
- Check the [README.md](README.md) for architecture overview
- Review [ARCHITECTURE.md](ARCHITECTURE.md) for implementation details
- Open an issue on GitHub
