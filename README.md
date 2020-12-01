MCMS


#### add submodule with ssh access
```
git submodule add https://github.com/DenisTDR/MCMS.git MCMS
```

#### Environment variables 
  * Required
    * `DB_URL` - ex: `Host=localhost;Database=rmp_2020;Username=root;Password=parola01`
    * `EXTERNAL_URL` -  
    * `CONTENT_PATH` - path to a persistent storage dir
  * Optional
    * `ASPNETCORE_ENVIRONMENT` - Production/Development
    * `ASPNETCORE_URLS` - Web server binds to those Urls
    * `VIEWS_AND_WWW_PATHS` - Only with ``dotnet [watch] run`` - ';' separated list of relative directory paths of projects which includes Views or wwwroot folders.
    * `ALLOWED_CORS_HOSTS` - ';' separated list of hosts (http[s]://domain[:port]), or `*`(star) to allow any host.
    * `FORMLY_DEBUG` - `True` if you want to use formly form with an iframe with reverse proxy from /formly-proxy to `FORMLY_SERVE_URL` with query params.
      * `FORMLY_SERVE_URL` - domain+port for the ng serve app (ex: `http://localhost:4455`)
      * `FORMLY_SCRIPTS_PATH` - defaults to `~`, change it if the js files should be served from another domain/host
      * check `MCMS/Views/Shared/Formly/FormlyFormsScripts.cshtml` for more information
    * Seed & Migrations
        * `SEED_ON_START` - boolean
          * `SEED_FILE_PATH` - string
        * `MIGRATE_ON_START` - boolean
    *  Users
      * `REQUIRE_CONFIRMED_ACCOUNT` - boolean - if `true` new users needs to confirm their email addresses.
      * `FIRST_USER_ADMIN` - boolean - if `true` give `Admin` role to first registered user
    * `ADMIN_ROUTE_PREFIX` - defaults to `/`
    * `DEFAULT_LANGUAGE` - defaults tot `en`
    * `HIDE_PRIVACY_POLICY` - boolean - hide privacy policy link in footer
    * Emailing
      * [MCMS.Emailing/README.md](./MCMS.Emailing/README.md)