using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using brevo_csharp.Api;
using brevo_csharp.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Configuration = brevo_csharp.Client.Configuration;

namespace MCMS.Emailing.Clients.Brevo;

public class MBrevoClient(ILoggerFactory loggerFactory, IOptions<MBrevoClientOptions> clientOptions)
    : IMEmailClient
{
    private readonly ILogger _logger = loggerFactory.CreateLogger("MailClient");
    private readonly MBrevoClientOptions _options = clientOptions.Value;


    public async Task<bool> SendEmail(MimeMessage message)
    {
        Configuration.Default.AddApiKey("api-key", _options.ApiKey);

        var apiInstance = new TransactionalEmailsApi();
        var sendSmtpEmail = ConvertMimeMessageToSendSmtpEmail(message);


        _logger.LogInformation("Sending mail via Brevo:\nTo: {To}\nSubject: {Subject}",
            string.Join(", ", sendSmtpEmail.To.Select(t => t.Email)), sendSmtpEmail.Subject);
        try
        {
            var result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            _logger.LogTrace("Email result: {Result}", JsonConvert.SerializeObject(result));
            _logger.LogInformation("Email sent");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending email");
            return false;
        }

        return true;
    }

    private SendSmtpEmail ConvertMimeMessageToSendSmtpEmail(MimeMessage message) =>
        new()
        {
            Subject = message.Subject,
            Sender = new SendSmtpEmailSender
            {
                Name = message.From.Mailboxes.FirstOrDefault()?.Name ?? _options.DefaultSenderName,
                Email = message.From.Mailboxes.FirstOrDefault()?.Address ?? _options.DefaultSenderAddress,
            },
            To = message.To.Mailboxes
                .Select(m => new SendSmtpEmailTo(m.Address, string.IsNullOrEmpty(m.Name) ? m.Address : m.Name))
                .ToList(),
            Cc = message.Cc.Count != 0
                ? message.Cc.Mailboxes
                    .Select(m => new SendSmtpEmailCc(m.Address, m.Name))
                    .ToList()
                : null,
            Bcc = message.Bcc.Count != 0
                ? message.Bcc.Mailboxes
                    .Select(m => new SendSmtpEmailBcc(m.Address, m.Name))
                    .ToList()
                : null,
            ReplyTo = message.ReplyTo.Mailboxes.FirstOrDefault() is { } replyTo
                ? new SendSmtpEmailReplyTo(replyTo.Address, replyTo.Name)
                : null,
            HtmlContent = message.HtmlBody ?? null,
            TextContent = message.TextBody ?? null,
            Attachment = message.Attachments.Any()
                ? message.Attachments.Select(att =>
                {
                    using var ms = new MemoryStream();
                    if (att is MimePart part)
                        part.Content.DecodeTo(ms);

                    return new SendSmtpEmailAttachment
                    {
                        Name = att.ContentDisposition?.FileName ?? "file",
                        Content = ms.ToArray()
                    };
                }).ToList()
                : null
        };
}