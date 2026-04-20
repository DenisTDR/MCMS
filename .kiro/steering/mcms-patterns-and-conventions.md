---
inclusion: manual
---

# MCMS — Patterns & Conventions

## Naming Conventions

| Type | Convention | Example |
|---|---|---|
| Entity | `<Name>Entity` or plain `<Name>` | `MyItemEntity`, `MyItem` |
| ViewModel | `<Name>ViewModel` | `MyItemViewModel` |
| Form model | `<Name>FormModel` | `MyItemFormModel` |
| Repository | `<Name>Repository` | `MyItemRepository` |
| Service | `<Name>Service` | `MyItemService` |
| Specifications | `<Feature>Specifications` | `MyFeatureSpecifications` |
| API controller | `<Name>ApiController` | `MyItemsApiController` |
| UI controller | `<Name>UiController` | `MyItemsUiController` |
| Mapping config | `<Name>Map` | `MyItemMap` |

## Feature Module Structure

Group related files by feature, not by layer:

```
MyFeature/
├── MyFeatureEntity.cs          # EF Core entity
├── MyFeatureViewModel.cs       # ViewModel + IMappingConfig
├── MyFeatureFormModel.cs       # Create/edit form model
├── MyFeatureApiController.cs   # JSON API (extends AdminApiController)
├── MyFeatureUiController.cs    # MVC views (extends AdminUiController)
├── MyFeatureService.cs         # Business logic ([Service] attribute)
├── MyFeatureRepository.cs      # Custom repo (optional)
└── MyFeatureTypeConfig.cs      # EF Core type configuration (optional)
```

## CRUD Controller Pair Pattern

Every resource typically has a paired API + UI controller:

**API controller** handles data operations:
- `DtQuery` — DataTables server-side query (GET)
- `Create` — Create new entity (POST)
- `Edit` / `Patch` — Update entity (POST / PATCH)
- `Delete` — Delete entity (DELETE)
- `Get` — Get single entity (GET)

**UI controller** handles view rendering:
- `Index` — List view with TableConfig
- `Create` — Create form view (GET)
- `Edit(id)` — Edit form view (GET)
- `Details(id)` — Detail view (GET)

## AutoMapper Conventions

Always define mappings in a nested `IMappingConfig` class within the ViewModel file:

```csharp
public class MyEntityViewModel : IViewModel
{
    // ... properties
}

public class MyEntityMap : IMappingConfig
{
    public void CreateMaps(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<MyEntity, MyEntityViewModel>();
        cfg.CreateMap<MyFormModel, MyEntity>();
    }
}
```

For entity-to-entity mappings, use `EntityModelMappingConfig<TSource, TDest>`:

```csharp
public class MyEntityDetailsMap : EntityModelMappingConfig<MyEntityViewModel, MyEntityDetails> { }
```

## Error Handling

Throw `KnownException` for expected business errors — `CustomExceptionFilter` converts them to HTTP responses:

```csharp
throw new KnownException("Item not found", 404);
throw new KnownException("Validation failed", 400);
```

For 404 on entity lookup, use the built-in helper:

```csharp
var entity = await Repo<MyEntity>().GetOneOrThrow(id, "Item not found");
```

## Response Wrappers

API controllers should return consistent response shapes:

```csharp
// Single model response
return OkModel(viewModel, entity.Id);
// Equivalent to: Ok(new ModelResponse<T>(viewModel, entity.Id))

// Form submit response (with snack notification)
return Ok(new FormSubmitResponse<MyFormModel>(model, entity.Id)
{
    Snack = "Saved successfully",
    SnackType = "success",
    SnackDuration = 3000
});
```

## Environment Variable Access

Use `Env` helper (from `MCMS.Base.Helpers`) to read environment variables:

```csharp
var dbUrl = Env.GetOrThrow("DB_URL");          // throws if missing
var optVal = Env.Get("OPTIONAL_VAR");           // returns null if missing
var flag = Env.GetBool("FEATURE_FLAG");         // parses bool, false if missing
```

`Env.LoadEnvFiles()` at startup loads `.env`, `.local.env`, and `.docker.env` files.

## Dependency Injection in Controllers

Use the `Service<T>()` and `Repo<T>()` helpers instead of constructor injection in controllers:

```csharp
// Preferred in controllers
var myService = Service<MyService>();
var repo = Repo<MyEntity>();

// Constructor injection is fine in services/repositories
public class MyService
{
    public MyService(IRepository<MyEntity> repo, IMapper mapper) { ... }
}
```

## JSON Serialization

MCMS uses **Newtonsoft.Json** (not System.Text.Json) with these defaults:
- Enum values serialized as camelCase strings
- Null values omitted from output
- Object references preserved (`PreserveReferencesHandling.Objects`)

Use `[JsonConverter(typeof(ToStringJsonConverter))]` to serialize complex types as their `ToString()` value.

## CORS

Configure allowed origins via environment variable:

```bash
ALLOWED_CORS_HOSTS=https://app.example.com;https://admin.example.com
# or allow all:
ALLOWED_CORS_HOSTS=*
```
