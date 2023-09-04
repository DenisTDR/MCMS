namespace MCMS.Emailing.Clients.Smtp
{
    public class MSmtpClientOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DefaultSender { get; set; }
        public string DefaultSenderName { get; set; }
    }
}