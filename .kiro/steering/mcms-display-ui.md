---
inclusion: manual
---

# MCMS — Display & UI Configuration

## TableConfig — Data Tables

`TableConfigService` / `TableConfigServiceOfT<TVm>` configures the DataTables-powered list views.

### Defining Table Columns via Attributes

Decorate ViewModel properties with `[TableColumn]`:

```csharp
public class MyEntityViewModel : IViewModel
{
    [TableColumn]
    public string Id { get; set; }

    [TableColumn(Title = "Full Name")]
    public string Name { get; set; }

    [TableColumn(Invisible = true)]          // hidden by default, user can toggle
    public string Email { get; set; }

    [TableColumn(Searchable = SearchType.Server, Orderable = OrderType.Server)]
    public string Status { get; set; }

    [TableColumn(Type = TableColumnType.Bool)]
    public bool Active { get; set; }
}
```

### Building TableConfig in a UI Controller

```csharp
public override async Task<IActionResult> Index()
{
    var tcService = new TableConfigServiceOfT<MyEntityViewModel>(Url)
    {
        TableItemsApiUrl = Url.ActionLink(nameof(MyApiController.DtQuery), "MyApi"),
        ServerSide = true,
        UseCreateNewItemLink = true,
        CreateNewItemLink = new MRichLink("New Item", typeof(MyUiController), nameof(Create))
            .AsButton("outline-primary")
            .WithIconClasses("fas fa-plus")
            .WithModal(),
    };

    var tableConfig = await tcService.GetTableConfig();

    // Per-row action buttons
    tableConfig.ItemActions = new List<MRichLink>
    {
        new MRichLink("", typeof(MyUiController), nameof(Edit))
            .AsButton("outline-primary").WithModal()
            .WithIconClasses("fas fa-pencil-alt")
            .WithValues(new { id = "ENTITY_ID" }),

        new MRichLink("", typeof(MyUiController), nameof(Delete))
            .AsButton("outline-danger").WithModal()
            .WithIconClasses("fas fa-trash")
            .WithValues(new { id = "ENTITY_ID" }),
    };

    return View(tableConfig);
}
```

### Dynamic Placeholders in Item Actions

Use `ENTITY_ID` and custom placeholders to inject row data into action URLs:

```csharp
tableConfig.ItemActionsPlaceholders = new Dictionary<string, string>
{
    { "IS_ACTIVE", "active" },   // key = placeholder name, value = ViewModel property
};

// Then in action link:
new MRichLink(...)
    .WithData("hidden", "IS_ACTIVE")   // hide button when active == true
    .WithValues(new { id = "ENTITY_ID" })
```

## DetailsConfig — Detail Views

`DetailsConfigService<TVm>` auto-discovers properties for detail views. Control rendering with `[DetailsField]`:

```csharp
public class MyEntityViewModel : IViewModel
{
    [DetailsField(Title = "Identifier")]
    public string Id { get; set; }

    [DetailsField(OrderIndex = 1)]
    public string Name { get; set; }

    [DetailsField(Hidden = true)]    // exclude from details view
    public string InternalCode { get; set; }
}
```

Wrap a model for the view:

```csharp
var detailsService = Service<IDetailsConfigServiceT<MyEntityViewModel>>();
return View(detailsService.Wrap(viewModel));
```

## Menu Configuration

Configure the navigation menu in your `MSpecifications.ConfigureServices`:

```csharp
services.Configure<MenuConfig>(menu =>
{
    menu.Add(new MenuSection("My Section", index: 10)
    {
        Items =
        {
            new MenuLink("Items", typeof(MyItemsUiController), nameof(MyItemsUiController.Index))
                .WithIconClasses("fas fa-list"),
            new MenuLink("Settings", typeof(SettingsUiController), nameof(SettingsUiController.Index))
                .WithIconClasses("fas fa-cog")
                .RequiresRoles("Admin"),
        }
    });
});
```

### Extending an Existing Menu Section

Use `PartialMenuSection` to add items to a section defined in another specification:

```csharp
menu.Add(new PartialMenuSection(isExtensionOf: "existing-section-id")
{
    Items = { new MenuLink("Extra Item", ...) }
});
```

## MRichLink — Flexible Link Builder

`MRichLink` is the universal link/button builder used throughout the display system:

```csharp
new MRichLink("Label", typeof(MyController), nameof(MyController.Action))
    .AsButton("outline-primary")          // render as Bootstrap button
    .WithIconClasses("fas fa-edit")       // prepend icon
    .WithModal()                          // open in modal dialog
    .WithTitle("Tooltip text")
    .WithValues(new { id = "ENTITY_ID" }) // route values
    .WithData("key", "VALUE_PLACEHOLDER") // HTML data attributes
    .WithTag("edit")                      // tag for grouping/filtering
```

## Formly Dynamic Forms

MCMS integrates with Angular Formly to render dynamic forms from Swagger/OpenAPI specs. The form parameters are generated server-side and passed to the Formly frontend.

- Annotate form model properties with `[FormlyFile]` for file upload fields
- Use `SwaggerFormly` controllers to serve form configuration
- In development, set `FORMLY_DEBUG=true` and `FORMLY_SERVE_URL=http://localhost:4455` to proxy the Angular dev server
