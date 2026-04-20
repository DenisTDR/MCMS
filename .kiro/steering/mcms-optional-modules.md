---
inclusion: manual
---

# MCMS — Optional Modules

## MCMS.Common — Translations

Provides multi-language translation support with a database-backed key/value store.

```csharp
mAppBuilder.AddSpecifications<MCommonSpecifications>();
```

### Usage

Inject `ITranslationsRepository` to look up translated strings:

```csharp
var label = await _translationsRepo.GetValueOrSlug("my.translation.key");
```

Seed translations via JSON:

```csharp
services.AddOptions<SeedSources>().Configure(ss =>
    ss.Add((typeof(MySpecifications).Assembly, "my-translations.json")));
```

---

## MCMS.Emailing — Email Sending

Multi-provider email support. See `MCMS/MCMS.Emailing/README.md` for full details.

```csharp
mAppBuilder.AddSpecifications<MEmailingSpecifications>();
```

### Supported Providers

| Provider | Environment variable |
|---|---|
| SMTP | `SMTP_HOST`, `SMTP_PORT`, `SMTP_USER`, `SMTP_PASS` |
| SendGrid | `SENDGRID_API_KEY` |
| Brevo (Sendinblue) | `BREVO_API_KEY` |
| Gmail | `GMAIL_USER`, `GMAIL_APP_PASSWORD` |
| Stdout | `EMAIL_PROVIDER=stdout` (dev/testing) |

### Sending an Email

```csharp
[Service<IEmailService>]
public class MyEmailService
{
    private readonly IEmailSender _emailSender;

    public async Task SendWelcome(string toEmail, string name)
    {
        await _emailSender.SendEmailAsync(toEmail, "Welcome!", "<h1>Hello " + name + "</h1>");
    }
}
```

---

## MCMS.Files — File Management

Purpose-based file upload and storage management.

```csharp
mAppBuilder.AddSpecifications<MFilesSpecifications>();
```

### Key Concepts

- Files are organized by **purpose** (e.g., `"avatar"`, `"document"`)
- Supports local filesystem and cloud storage backends
- `CONTENT_PATH` environment variable sets the root storage directory

### Uploading a File

```csharp
// In an API controller
[HttpPost]
public async Task<IActionResult> Upload([FromForm] IFormFile file)
{
    var fileService = Service<IFilesService>();
    var savedFile = await fileService.SaveFile(file, purpose: "document");
    return Ok(savedFile);
}
```

### Annotating Form Models for Formly

```csharp
public class MyFormModel
{
    [FormlyFile(Purpose = "avatar", Accept = "image/*")]
    public string AvatarFileId { get; set; }
}
```

---

## MCMS.Logging — Audit & Application Logs

Provides structured audit logging and application log storage.

```csharp
mAppBuilder.AddSpecifications<MLoggingSpecifications>();
```

### Audit Logging

Decorate API controller actions with `[MAuditLog]` to automatically record who did what:

```csharp
[HttpPost]
[MAuditLog]
public async Task<IActionResult> Create([FromBody] MyFormModel model)
{
    // Action is automatically logged with user, timestamp, and request details
}
```

Audit logs are viewable in the admin dashboard under the Logs section.

### Application Logs

Application logs are stored in the database and viewable in the admin dashboard. Configure log level via standard ASP.NET Core `Logging` configuration in `appsettings.json`.

---

## MCMS.Auth — JWT Authentication

See `mcms-auth.md` for full JWT authentication documentation.

```csharp
mAppBuilder.AddSpecifications<MJwtAuthSpecifications>();
```

---

## Admin Dashboard

The admin dashboard is provided automatically by `MBaseSpecifications`. It includes:

- User management (list, create, edit, assign roles)
- Framework info (loaded assemblies, NuGet packages)
- Audit log viewer (with `MLoggingSpecifications`)
- Translation management (with `MCommonSpecifications`)

Access at: `[ADMIN_ROUTE_PREFIX]/AdminDashboard`
