using System.Net.Mail;
using System.Net;
using SmartElectronicsApi.Mvc.Interfaces;

namespace SmartElectronicsApi.Mvc.Services
{
    public class EmailService:IEmailService
    {
        public void SendEmail(string from, string to, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser, string smtpPass)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);
            mailMessage.To.Add(new MailAddress(to));
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = smtpHost;
            smtpClient.Port = smtpPort;
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
            smtpClient.Send(mailMessage);
        }

        public async Task SendEmailAsyncToManyPeople(string from, List<string> recipients, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser, string smtpPass)
        {
            foreach (var to in recipients)
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(new MailAddress(to));
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = smtpHost;
                smtpClient.Port = smtpPort;
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
