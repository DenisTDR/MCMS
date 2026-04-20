---
inclusion: manual
---

# MCMS — Data Layer

## Entity Contracts

All entities must implement `IEntity` (or extend `Entity`):

```csharp
// Minimal entity
public class MyEntity : Entity
{
    public string Name { get; set; }
}
```

`Entity` provides: `Id` (string, DB-generated), `Created` (DateTime), `Updated` (DateTime, auto-managed).

### Optional Entity Interfaces

| Interface | Effect |
|---|---|
| `ICanBeDeleted` | Adds `Deleted` flag; repository automatically filters out deleted records |
| `ISluggable` | Adds `Slug` property; `GetOne(id)` matches on both `Id` and `Slug` |
| `IOrderable` | Adds `Order` property; repository automatically orders by it |

## Repository Pattern

`IRepository<T>` / `Repository<T>` is the generic data access layer. Inject via `IRepository<T>` or use the `Repo<T>()` helper in controllers.

```csharp
// In a controller or service
var repo = Repo<MyEntity>();

var entity = await repo.GetOne(id);
var all    = await repo.GetAll();
var found  = await repo.GetOne(e => e.Name == "foo");
var exists = await repo.Any(e => e.Name == "foo");

var created = await repo.Add(new MyEntity { Name = "bar" });
await repo.Patch(id, jsonPatchDocument);
await repo.Delete(id);
```

### Customizing the Queryable

```csharp
repo.ChainQueryable(q => q.Include(e => e.RelatedEntity).Where(e => e.Active));
```

## Entity Type Configuration

Register EF Core type configurations via `EntitiesConfig`. MCMS discovers them automatically through `BaseDbContext.OnModelCreating`:

```csharp
// In your Specifications.ConfigureServices:
services.AddOptions<EntitiesConfig>().Configure(config =>
{
    config.Add<MyEntity, MyEntityTypeConfig>();
});

// Type configuration class
public class MyEntityTypeConfig : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
    }
}
```

## ViewModel & AutoMapper

ViewModels must implement `IViewModel`. Map entities to ViewModels using `IMappingConfig`:

```csharp
public class MyEntityViewModel : IViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class MyEntityMap : IMappingConfig
{
    public void CreateMaps(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<MyEntity, MyEntityViewModel>();
        cfg.CreateMap<MyEntityViewModel, MyEntity>();
    }
}
```

MCMS auto-discovers all `IMappingConfig` implementations across all specification assemblies.

## DataTables (DtQuery) — Server-Side Tables

`DtQueryService<TVm>` handles server-side filtering, sorting, and pagination for DataTables.

Columns are configured via `[TableColumn]` attributes on the ViewModel (see `mcms-display-ui.md`). The service maps DataTables requests to EF Core queries automatically.

## Seeding

Seed data is loaded from embedded JSON files:

```csharp
// Register seed source in Specifications
services.AddOptions<SeedSources>().Configure(ss =>
    ss.Add((typeof(MySpecifications).Assembly, "my-seed.json")));

// Register entity seeders
services.AddOptions<EntitySeeders>().Configure(s => s.Add<MySeeder>());
```

Set `SEED_ON_START=true` and optionally `SEED_FILE_PATH` to trigger seeding on startup.

## Migrations

Set `MIGRATE_ON_START=true` to auto-apply EF Core migrations on startup. Generate migrations with:

```bash
dotnet ef migrations add MigrationName --project YourProject
```
