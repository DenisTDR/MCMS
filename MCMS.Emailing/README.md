## MCMS.Emailing

#### Environment variables 
  * Gmail SMTP - [tutorial](https://developers.google.com/gmail/api/quickstart/dotnet)
    * `GMAIL_CREDENTIALS_JSON_PATH` - `credentials.json` path
    * `GMAIL_TOKEN_JSON_PATH` - path to store the user's access and refresh tokens
  * SendGrid
    * `SENDGRID_KEY`
    * `SENDGRID_DEFAULT_SENDER`
    * `SENDGRID_DEFAULT_SENDER_NAME` - optional
  * if neither `SENDGRID_KEY` or `GMAIL_CREDENTIALS_JSON_PATH` is not set the emails are logged in stdout.