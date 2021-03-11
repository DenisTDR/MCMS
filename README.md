MCMS

#### Add this submodule to new project

```
git submodule add https://github.com/DenisTDR/MCMS.git MCMS
```

#### Update this submodule in existing project

```
git submodule update --init --recursive
```

#### Environment variables

* Required
    * `DB_URL` - ex: `Host=localhost;Database=db_name;Username=root;Password=passwd01`
    * `EXTERNAL_URL` -
    * `CONTENT_PATH` - path to a persistent storage dir (ex.: for user uploaded files)
* Required when `ASPNETCORE_ENVIRONMENT=Production`
    * `PERSISTED_KEYS_DIRECTORY` - path to a persistent storage dir to keep private key used to sign auth cookies or jwt
* Optional
    * `ASPNETCORE_ENVIRONMENT` - Production/Development
    * `ASPNETCORE_URLS` - Web server binds to those Urls
    * `ALLOWED_CORS_HOSTS` - ';' separated list of hosts (http[s]://domain[:port]), or `*`(star) to allow any host.
    * `LIB_STATIC_FILES_LOAD_TYPE` - set to `pre-publish` if you run the project with `dotnet [watch] run`
    * `RAZOR_RUNTIME_COMPILATION` - set to `True` only if project is running with `dotnet [watch] run`, calls 
      `AddRazorRuntimeCompilation()` on MvcBuilder and registers Razor View paths with PhysicalFileProvider
    * Formly forms
        * `FORMLY_DEBUG` - `True` if you want to use formly form with an iframe with reverse proxy from /formly-proxy
          to `FORMLY_SERVE_URL` with query params.
            * `FORMLY_SERVE_URL` - domain+port for the ng serve app (ex: `http://localhost:4455`)
        * `FORMLY_SCRIPTS_PATH` - defaults to `~`, change it if the js files should be served from another domain/host
        * check `MCMS/Views/Shared/Formly/FormlyFormsScripts.cshtml` for more information
    * Seed & Migrations
        * `SEED_ON_START` - boolean
            * `SEED_FILE_PATH` - string
        * `MIGRATE_ON_START` - boolean
    * Users
        * `REQUIRE_CONFIRMED_ACCOUNT` - boolean - if `true` new users needs to confirm their email addresses.
        * `FIRST_USER_ADMIN` - boolean - if `true` the first registered user will receive `Admin` role.
        * `SHOW_NON_CONFIRMED_ACCOUNT_ALERT`
        * `DISABLE_REGISTRATION` - disable admin user registration
    * Route prefixes
        * `ROUTE_PREFIX` - defaults to `/`
        * `ADMIN_ROUTE_PREFIX` - defaults to `/`
        * Results:
            * AdminUiController: `[ROUTE_PREFIX]/[ADMIN_ROUTE_PREFIX]/UiController`
            * AdminApiController: `[ROUTE_PREFIX]/[ADMIN_ROUTE_PREFIX]/api/UiController`
            * ApiController: `[ROUTE_PREFIX]/api/UiController`
            * Swagger docs `[ROUTE_PREFIX]/api/docs`
    * `DEFAULT_LANGUAGE` - defaults tot `en`
    * `HIDE_PRIVACY_POLICY` - boolean - hide privacy policy link in footer
    * Emailing
        * [MCMS.Emailing/README.md](./MCMS.Emailing/README.md)