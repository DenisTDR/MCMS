MCMS


#### add submodule with ssh access
```
eval $(ssh-agent)
ssh-add key_path
git submodule add ssh://git@git.ligaac.ro:5022/upt/mcms.git MCMS
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
      * check `UPT.RMP.Base/Views/Shared/Formly/FormlyFormsScripts.cshtml` for more information
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
        * if `SENDGRID_KEY` is not set then emails are logged in stdout
        * SendGrid
          * `SENDGRID_KEY`
          * `SENDGRID_DEFAULT_SENDER`
