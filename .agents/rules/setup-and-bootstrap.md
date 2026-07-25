# MCMS — Application Setup & Bootstrap

## Bootstrap Pattern

MCMS uses a **Specifications + Builder** pattern to compose the application. Each feature module is a `MSpecifications` subclass. The `MAppBuilder` chains them together, then `MApp` orchestrates service registration and middleware.

### Minimal `Program.cs`

```csharp
Env.LoadEnvFiles(); // loads .env, .local.env, .docker.env

var builder = WebApplication.CreateBuilder(args);
var mAppBuilder = new MAppBuilder(builder.Environment)
    .AddSpecifications<MCommonSpecifications>()       // translations
    .AddSpecifications<MyAppSpecifications>()          // your app module
    .AddSpecifications<MEmailingSpecifications>()      // optional: emailing
    .AddSpecifications<MLoggingSpecifications>()       // optional: audit logs
    .WithPostgres<AppDbContext>()                      // PostgreSQL + EF Core
    .WithSwagger(new SwaggerConfigOptions
    {
        Title = "My App",
        Version = "v1",
        UiType = DocsUiType.Both                       // Swagger UI + ReDoc
    });

var mApp = mAppBuilder.Build();
mApp.ConfigureServices(builder.Services);

var app = builder.Build();
mApp.Configure(app, app.Services);
app.Run();
```

`MAppBuilder.Build()` automatically inserts the required base specifications (`MBaseSpecifications`, `MAuthSpecifications`, etc.) if not already added.

## Writing a Specifications Class

Create one `MSpecifications` subclass per feature module (or per app):

```csharp
public class MyAppSpecifications : MSpecifications
{
    public override void ConfigureServices(IServiceCollection services)
    {
        // Configure the navigation menu
        services.Configure<MenuConfig>(menu =>
        {
            menu.Add(new MenuSection("My Section", "fas fa-cog")
            {
                Items = { new MenuLink("Items", typeof(MyItemsUiController), nameof(MyItemsUiController.Index)) }
            });
        });

        // Configure site metadata
        services.AddOptions<SiteConfig>().Configure(c =>
        {
            c.SiteName = "My App";
            c.SiteCopyright = $"Copyright &copy; {DateTime.Now.Year}";
        });

        // Register seed sources
        services.AddOptions<SeedSources>().Configure(ss =>
            ss.Add((typeof(MyAppSpecifications).Assembly, "my-seed.json")));

        // Register entity seeders
        services.AddOptions<EntitySeeders>().Configure(s => s.Add<MyEntitySeeder>());
    }

    public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
    {
        // Middleware or startup logic here
    }
}
```

## Custom DbContext

Extend `BaseDbContext` to add domain-specific behavior:

```csharp
public class AppDbContext : BaseDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options, IOptions<EntitiesConfig> config)
        : base(options, config) { }
}
```

Register it with `.WithPostgres<AppDbContext>()`. MCMS automatically registers `BaseDbContext` as an alias for the concrete type so both can be injected.

## Service Auto-Registration

Decorate any class with `[Service]` to have it auto-registered by `BaseSpecifications`:

```csharp
[Service]                                    // Scoped by default
public class MyService { }

[Service(ServiceLifetime.Singleton)]         // Explicit lifetime
public class MySingletonService { }

[Service<IMyService>]                        // Register against interface
public class MyService : IMyService { }
```

Auto-registration scans all assemblies from all registered specifications.

## Ordering of Specifications

The order in which specifications are added matters for middleware pipeline ordering. Base specs are always inserted first by `MAppBuilder.Build()`. Add your app specs after `MCommonSpecifications` and before emailing/logging.
