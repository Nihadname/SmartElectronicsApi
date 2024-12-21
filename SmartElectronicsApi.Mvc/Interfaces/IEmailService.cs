namespace SmartElectronicsApi.Mvc.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(string from, string to, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser);
        public Task SendEmailAsyncToManyPeople(string from, List<string> recipients, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser);
    }
}
