# Hexagonal Architecture Refactoring

## Overview

This document explains the hexagonal architecture implementation in the Real Estate API, detailing the separation of concerns across layers and the proper flow of dependencies.

## Architecture Layers

### 1. Domain Layer (Core)
**Location:** `src/RealEstateApi.Domain/`

**Responsibility:** Pure business logic and entities

**Key Files:**
- `Entities/Property.cs` - Rich domain model with business rules
- `Entities/Unit.cs` - Rich domain model with business rules
- `Enums/PropertyStatus.cs` - Type-safe status values
- `Enums/UnitStatus.cs` - Type-safe status values
- `Exceptions/DomainException.cs` - Domain rule violations

**Business Rules Enforced:**
- Properties must have required fields (agent, name, location, type, description)
- Properties can only be published if they have name, description, main image, and at least one unit
- Sold properties cannot be edited or reverted to draft
- Only published/reserved properties can be sold
- Units cannot be removed from sold properties
- Sold units cannot be updated or have their price changed

**Key Methods:**
```csharp
// Property
Property.Create() - Factory method ensuring valid creation
property.Publish() - Enforces publication rules
property.MarkAsSold() - Validates state transitions
property.IsOwnedBy(userId) - Ownership check
property.UpdateDetails() - Validates required fields

// Unit
Unit.Create() - Factory method with validation
unit.Reserve() - State transition with rules
unit.MarkAsSold() - Validates sale conditions
```

### 2. Application Layer (Use Cases / Orchestration)
**Location:** `src/RealEstateApi.Application/`

**Responsibility:** Business workflows and orchestration

**Key Files:**
- `Interfaces/IPropertyService.cs` - Service contract
- `Services/PropertyService.cs` - Use case implementation
- `Exceptions/NotFoundException.cs` - Resource not found
- `Exceptions/ForbiddenException.cs` - Authorization failure

**Use Cases Implemented:**
- `GetPropertyByIdAsync` - Retrieve single property
- `GetAllPropertiesAsync` - List properties with filtering
- `GetAgentPropertiesAsync` - Get agent's properties
- `CreatePropertyAsync` - Create new property (uses domain factory)
- `UpdatePropertyAsync` - Update property (uses domain methods)
- `DeletePropertyAsync` - Remove property (checks authorization)
- `PublishPropertyAsync` - Publish property (uses domain validation)

**Flow Example (Create Property):**
```csharp
1. Service receives CreatePropertyDto and agentId
2. Calls Property.Create() - domain validates required fields
3. Calls property.UpdateDetails() - domain validates data
4. Calls property.UpdateImages() - domain method
5. Saves via repository
6. Maps to PropertyResponseDto
7. Returns DTO to controller
```

### 3. Infrastructure Layer (Adapters)
**Location:** `src/RealEstateApi.Infrastructure/`

**Responsibility:** Technical implementations (database, external services)

**Key Files:**
- `Persistence/ApplicationDbContext.cs` - EF Core context
- `Persistence/Configurations/PropertyConfiguration.cs` - EF mapping with field-based access
- `Persistence/Configurations/UnitConfiguration.cs` - EF mapping with field-based access
- `Persistence/Repositories/PropertyRepository.cs` - Implements IPropertyRepository
- `Persistence/Repositories/UnitRepository.cs` - Implements IUnitRepository

**EF Core Configuration for Private Setters:**
```csharp
builder.UsePropertyAccessMode(PropertyAccessMode.Field);
```

This allows EF Core to work with properties that have private setters by accessing their backing fields directly.

**Enum Storage:**
```csharp
builder.Property(p => p.Status)
    .HasConversion<string>(); // Store enum as string in database
```

### 4. API Layer (HTTP Adapter)
**Location:** `src/RealEstateApi.Api/`

**Responsibility:** HTTP concerns only (routing, serialization, status codes)

**Key Files:**
- `Controllers/PropertiesController.cs` - Thin HTTP adapter
- `Program.cs` - Dependency injection configuration

**Controller Responsibilities:**
- Parse HTTP requests
- Extract user claims from JWT
- Call application service
- Map exceptions to HTTP status codes
- Return HTTP responses

**Proper Exception Handling:**
```csharp
catch (NotFoundException ex) → 404 Not Found
catch (ForbiddenException ex) → 403 Forbidden
catch (DomainException ex) → 400 Bad Request
catch (Exception ex) → 500 Internal Server Error
```

## Dependency Flow (Hexagonal Pattern)

```
┌────────────────────────────────────────────────────────┐
│                  API Layer (Adapter)                   │
│            PropertiesController                        │
│         ↓ depends on ↓                                 │
│      IPropertyService (interface)                      │
└────────────────────────────────────────────────────────┘
                       ↓
┌────────────────────────────────────────────────────────┐
│            Application Layer (Use Cases)               │
│              PropertyService                           │
│    ↓ depends on ↓           ↓ depends on ↓            │
│  IPropertyRepository     Domain Entities               │
└────────────────────────────────────────────────────────┘
         ↑                          ↓
         │                          │
┌────────┴──────────┐    ┌─────────▼──────────────────┐
│  Infrastructure   │    │   Domain Layer (Core)      │
│  PropertyRepository│   │   Property, Unit           │
│  (Adapter)        │    │   Business Rules           │
└───────────────────┘    │   No Dependencies          │
                         └────────────────────────────┘
```

**Key Principle:** Dependencies point inward toward the Domain. The Domain has no dependencies on outer layers.

## Benefits of This Architecture

### 1. Testability
- Domain logic can be tested without database or HTTP
- Application services can be tested with mock repositories
- Each layer can be tested independently

### 2. Maintainability
- Clear separation of concerns
- Business rules centralized in Domain
- Easy to locate and modify specific functionality

### 3. Flexibility
- Can swap databases without touching Domain or Application
- Can add new API types (GraphQL, gRPC) without changing business logic
- Can change authentication without modifying use cases

### 4. Business Rule Enforcement
- Impossible to create invalid entities (private setters + factory methods)
- State transitions controlled by domain methods
- Validation happens in one place (Domain), not scattered across layers

## Examples of Proper Architecture

### ✅ Good: Using Domain Methods
```csharp
// Application Service
var property = Property.Create(agentId, name, location, type, description);
property.Publish(); // ✅ Domain enforces rules
await _repository.UpdateAsync(property);
```

### ❌ Bad: Bypassing Domain (Before Refactoring)
```csharp
// Controller directly setting properties
var property = new Property();
property.Status = "published"; // ❌ No validation
property.AgentId = agentId;     // ❌ Could be empty
await _repository.UpdateAsync(property);
```

### ✅ Good: Service Orchestration
```csharp
// Application Service orchestrates workflow
public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto dto, string agentId)
{
    var property = Property.Create(...);  // Domain creates
    property.UpdateDetails(...);          // Domain validates
    await _repository.CreateAsync(property); // Infrastructure saves
    return MapToDto(property);            // Service maps
}
```

### ❌ Bad: Business Logic in Controller (Before)
```csharp
// Controller doing too much
var property = new Property { /* field assignments */ };
if (!string.IsNullOrEmpty(dto.Name)) property.Name = dto.Name; // ❌ Logic in controller
await _repository.CreateAsync(property);
```

## State Transitions (Managed by Domain)

```
Draft → Published → Reserved → Sold
  ↑         ↑
  └─────────┘ (can revert if not sold)
```

**Enforced by Domain Methods:**
- `property.Publish()` - Only from Draft
- `property.Reserve()` - Only from Published
- `property.MarkAsSold()` - Only from Published/Reserved
- `property.RevertToDraft()` - Cannot revert if Sold

## Migration Notes

### Database Schema
The status columns now store enum values as strings:
- `"Draft"` instead of `"draft"`
- `"Published"` instead of `"published"`
- `"Sold"` instead of `"sold"`

EF Core handles the conversion automatically via `HasConversion<string>()`.

### API Contract
Status field in DTOs remains a string for backward compatibility, but now uses enum values internally.

## Next Steps for Further Improvement

1. **Add Unit Service** - Create `IUnitService` and `UnitService` for unit management
2. **Add Validation Layer** - Use FluentValidation for DTO validation before service calls
3. **Add Domain Events** - Emit events when properties are published, sold, etc.
4. **Add CQRS Pattern** - Separate read and write models for better performance
5. **Add Unit Tests** - Test domain rules, application services, and controllers independently

## Summary

This refactoring transforms an anemic domain model with scattered business logic into a proper hexagonal architecture with:

- **Rich Domain Models** - Entities protect their invariants
- **Clear Separation** - Each layer has a single responsibility
- **Proper Dependencies** - Dependencies point inward toward Domain
- **Business Rule Enforcement** - Impossible to create invalid states
- **Thin Controllers** - Controllers only handle HTTP concerns
- **Orchestrating Services** - Application services coordinate workflows
