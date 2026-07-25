# MCMS — Micro CMS Framework Overview

MCMS is a modular ASP.NET Core framework (C#) used as a git submodule. It provides a full-stack admin CMS scaffold: data layer, authentication, display configuration, dynamic forms via Swagger/Formly, emailing, file management, and logging. Consumer projects add it as a submodule and extend it through the **Specifications** pattern.

## Sub-Projects

| Project | Purpose |
|---|---|
| `MCMS.Base` | Core abstractions: entities, repositories, controllers, builder, attributes, helpers |
| `MCMS` | Main framework: DbContext, Repository impl, DtQuery, display configs, admin dashboard, Swagger/Formly |
| `MCMS.Auth` | JWT authentication, session management, refresh tokens, identity integration |
| `MCMS.Common` | Translations, multi-language support |
| `MCMS.Files` | File upload management with purpose-based organization |
| `MCMS.Logging` | Audit logging and application logging |
| `MCMS.Emailing` | Multi-provider email (SMTP, SendGrid, Brevo, Gmail, Stdout) |

## Adding the Submodule

```bash
git submodule add https://github.com/DenisTDR/MCMS.git MCMS
git submodule update --init --recursive
```

## Required Environment Variables

| Variable | Description |
|---|---|
| `DB_URL` | PostgreSQL connection string, e.g. `Host=localhost;Database=db;Username=u;Password=p` |
| `EXTERNAL_URL` | Public base URL of the app (used for JWT issuer/audience) |
| `CONTENT_PATH` | Path to persistent storage for user-uploaded files |
| `PERSISTED_KEYS_DIRECTORY` | *(Production only)* Path to persist data-protection/JWT signing keys |

## Optional Environment Variables

| Variable | Description |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` / `Development` |
| `ASPNETCORE_URLS` | Bind URLs for the web server |
| `ALLOWED_CORS_HOSTS` | `;`-separated hosts or `*` |
| `MIGRATE_ON_START` | `true` to auto-run EF migrations on startup |
| `SEED_ON_START` | `true` to auto-seed data on startup |
| `RAZOR_RUNTIME_COMPILATION` | `true` when running with `dotnet watch run` |
| `REQUIRE_CONFIRMED_ACCOUNT` | `true` to require email confirmation |
| `FIRST_USER_ADMIN` | `true` to grant Admin role to first registered user |
| `DISABLE_REGISTRATION` | `true` to disable public registration |
| `ROUTE_PREFIX` | URL prefix for all routes (default `/`) |
| `ADMIN_ROUTE_PREFIX` | Additional prefix for admin routes (default `/`) |
| `DEFAULT_LANGUAGE` | Default UI language (default `en`) |
| `FORMLY_DEBUG` | `true` to proxy Formly dev server |
| `FORMLY_SERVE_URL` | Dev server URL when `FORMLY_DEBUG=true` |

## Route Conventions

- Admin UI controllers: `[ROUTE_PREFIX]/[ADMIN_ROUTE_PREFIX]/[Controller]/[Action]`
- Admin API controllers: `[ROUTE_PREFIX]/[ADMIN_ROUTE_PREFIX]/api/[Controller]/[Action]`
- Public API controllers: `[ROUTE_PREFIX]/api/[Controller]/[Action]`
- Swagger docs: `[ROUTE_PREFIX]/api/docs`

## Deep-Dive Reference Docs

This is a summary. For detailed guidance on a specific area, **open the relevant file below** — read it directly, the same as you would any other source file:

| Topic | File |
|---|---|
| App bootstrap, `MSpecifications`, `MAppBuilder` | [.agents/rules/setup-and-bootstrap.md](.agents/rules/setup-and-bootstrap.md) |
| Entities, `Repository<T>`, EF config, seeding | [.agents/rules/data-layer.md](.agents/rules/data-layer.md) |
| Controllers, routing, filters, Swagger groups | [.agents/rules/controllers-and-routing.md](.agents/rules/controllers-and-routing.md) |
| TableConfig, DetailsConfig, Menu, `MRichLink` | [.agents/rules/display-ui.md](.agents/rules/display-ui.md) |
| Cookie auth, JWT, roles, claims | [.agents/rules/auth.md](.agents/rules/auth.md) |
| Translations, Emailing, Files, Logging modules | [.agents/rules/optional-modules.md](.agents/rules/optional-modules.md) |
| Naming conventions, CRUD patterns, error handling | [.agents/rules/patterns-and-conventions.md](.agents/rules/patterns-and-conventions.md) |
