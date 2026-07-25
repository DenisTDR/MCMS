# MCMS — Authentication & Authorization

## Overview

MCMS supports two auth modes:

1. **Cookie-based auth** (default) — ASP.NET Core Identity with Bootstrap 4 UI. Used for browser-based admin access.
2. **JWT auth** — Stateless token auth for API clients. Provided by `MCMS.Auth` via `MJwtAuthSpecifications`.

Both modes use the same `User` and `Role` entities from `MCMS.Base.Auth`.

## Cookie Auth (Default)

`MAuthSpecifications` (included automatically by `MAppBuilder`) configures ASP.NET Core Identity:

- Custom `User` entity with `FullName` support
- `Role` entity with role-based authorization
- `MUserClaimsPrincipalFactory` for custom claims injection
- Bootstrap 4 Identity UI (login, register, password reset)

### Environment Variables

| Variable | Effect |
|---|---|
| `REQUIRE_CONFIRMED_ACCOUNT` | Require email confirmation before login |
| `FIRST_USER_ADMIN` | Grant `Admin` role to the first registered user |
| `DISABLE_REGISTRATION` | Disable the public registration page |

### Cookie Persistence (Production)

In production, configure `PERSISTED_KEYS_DIRECTORY` so data-protection keys survive restarts:

```bash
PERSISTED_KEYS_DIRECTORY=/app/keys
```

Or configure programmatically in your `MSpecifications`:

```csharp
services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    });
```

## JWT Auth (MCMS.Auth)

Add `MJwtAuthSpecifications` to enable JWT:

```csharp
mAppBuilder.AddSpecifications<MJwtAuthSpecifications>();
```

Requires `EXTERNAL_URL` and `PERSISTED_KEYS_DIRECTORY` environment variables.

### JWT Flow

1. `POST /auth/login` with `LoginRequestFormModel` → returns `SessionDto`
2. `SessionDto` contains `AccessToken` (60 min) and `RefreshToken` (24 h)
3. Include `Authorization: Bearer <token>` on subsequent requests
4. `POST /auth/refresh` with refresh token → returns new `SessionDto`
5. `POST /auth/revoke` to invalidate a refresh token

### Removing the Default Auth Controller

If you want to provide your own login endpoint:

```csharp
mAppBuilder.AddSpecifications(new MJwtAuthSpecifications
{
    RemoveDefaultAuthController = true
});
```

Then inject `ISessionService` and call `CreateSession(user)` to generate tokens.

### Custom Session Service

Extend `SessionService` to add custom claims or session logic:

```csharp
[Service<ISessionService>]
public class MySessionService : SessionService
{
    public MySessionService(/* same ctor params */) : base(...) { }

    public override async Task<SessionDto> CreateSession(User user, string ipAddress = null)
    {
        var session = await base.CreateSession(user, ipAddress);
        // Add custom data to session
        return session;
    }
}
```

## Role-Based Authorization

Use standard ASP.NET Core `[Authorize]` attributes:

```csharp
[Authorize]                          // any authenticated user
[Authorize(Roles = "Admin")]         // Admin role only
[Authorize(Roles = "Admin,Moderator")] // either role
```

Built-in roles seeded by MCMS: `Admin`.

### Menu Role Restrictions

```csharp
new MenuLink("Admin Panel", typeof(AdminController), nameof(AdminController.Index))
    .RequiresRoles("Admin")
```

## Claims

Standard claim keys are defined in `MCMS.Auth.Jwt.Claims`:

| Constant | Claim type | Value |
|---|---|---|
| `Claims.Id` | user id | User's string ID |
| `Claims.Username` | username | User's username |
| `Claims.Role` | role | One entry per role |

Access in controllers via `User.FindFirst(Claims.Id)?.Value`.
