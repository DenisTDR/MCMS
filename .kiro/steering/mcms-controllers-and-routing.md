---
inclusion: manual
---

# MCMS — Controllers & Routing

## Controller Hierarchy

```
BaseController (MCMS.Base)
├── AdminApiController   → JSON API, requires auth, admin-api Swagger group
│   └── Your API controllers
└── UiController         → MVC views
    └── AdminUiController → Requires auth, admin route prefix
        └── Your UI controllers
```

## Admin API Controller

Extend `AdminApiController` for authenticated JSON endpoints:

```csharp
[Authorize(Roles = "Admin")]
public class MyItemsApiController : AdminApiController
{
    private IRepository<MyEntity> Repo => Repo<MyEntity>();

    [HttpGet]
    public async Task<ActionResult<DtResult<MyEntityViewModel>>> DtQuery()
    {
        var dtService = Service<DtQueryService<MyEntityViewModel>>();
        // ... configure and return
    }

    [HttpPost]
    [ModelValidation]
    [MAuditLog]
    public async Task<ActionResult<ModelResponse<MyFormModel>>> Create(
        [Required][FromBody] MyFormModel model)
    {
        var entity = Mapper.Map<MyEntity>(model);
        await Repo<MyEntity>().Add(entity);
        return OkModel(model, entity.Id);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, [FromBody] JsonPatchDocument<MyFormModel> patch)
    {
        // ...
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await Repo<MyEntity>().Delete(id);
        return Ok();
    }
}
```

### Useful Helpers in BaseController

| Helper | Description |
|---|---|
| `Repo<T>()` | Resolves `IRepository<T>` from DI |
| `Service<T>()` | Resolves any service `T` from DI |
| `Mapper` | AutoMapper instance |
| `OkModel<T>(model, id)` | Returns `ModelResponse<T>` wrapped OK result |

## Admin UI Controller

Extend `AdminUiController` for MVC view-based pages:

```csharp
[Authorize(Roles = "Admin")]
public class MyItemsUiController : AdminUiController
{
    public override async Task<IActionResult> Index()
    {
        var tcService = new TableConfigServiceOfT<MyEntityViewModel>(Url)
        {
            TableItemsApiUrl = Url.ActionLink(nameof(MyItemsApiController.DtQuery), "MyItemsApi"),
            ServerSide = true,
        };
        return View(await tcService.GetTableConfig());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var entity = await Repo<MyEntity>().GetOneOrThrow(id);
        return View(Mapper.Map<MyFormModel>(entity));
    }
}
```

## Route Attributes

| Attribute | Applied to | Route pattern |
|---|---|---|
| `[AdminApiRoute]` | API controllers | `[ROUTE_PREFIX]/[ADMIN_ROUTE_PREFIX]/api/[controller]/[action]` |
| `[AdminRoute]` | UI controllers | `[ROUTE_PREFIX]/[ADMIN_ROUTE_PREFIX]/[controller]/[action]` |
| `[ApiRoute]` | Public API | `[ROUTE_PREFIX]/api/[controller]/[action]` |

These are configured via `ROUTE_PREFIX` and `ADMIN_ROUTE_PREFIX` environment variables.

## Filters & Conventions

- `[ModelValidation]` — Returns 400 with validation errors if `ModelState` is invalid
- `[MAuditLog]` — Records the action in the audit log (requires `MLoggingSpecifications`)
- `[OptionalAuthorization]` — Allows anonymous access but populates user if authenticated
- `CustomExceptionFilter` — Converts `KnownException` to appropriate HTTP responses
- `LayoutFilter` — Injects layout data (menu, site config) into every view

## Swagger / API Explorer

API controllers decorated with `[ApiExplorerSettings(GroupName = "admin-api")]` appear in the admin Swagger doc. Use `GroupName = "api"` for public endpoints. The Swagger UI is available at `[ROUTE_PREFIX]/api/docs`.
