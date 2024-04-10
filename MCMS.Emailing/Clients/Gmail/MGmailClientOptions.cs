namespace MCMS.Emailing.Clients.Gmail
{
    public class MGmailClientOptions
    {
        public string GmailCredentialsJsonPath { get; set; }
        public string GmailTokenJsonPath { get; set; }
        public string DefaultSenderAddress { get; set; }
        public string DefaultSenderName { get; set; }
    }
}