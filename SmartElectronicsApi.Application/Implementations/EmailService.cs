using SmartElectronicsApi.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string from, string to, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("nihadcoding@gmail.com\r\n");
            mailMessage.To.Add(new MailAddress(to));
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = smtpHost;
            smtpClient.Port = smtpPort;
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new NetworkCredential("nihadcoding@gmail.com", "kixx kxou qgdj wgmx");
            smtpClient.Send(mailMessage);
        }

        public async Task SendEmailAsyncToManyPeople(string from, List<string> recipients, string subject, string body, string smtpHost, int smtpPort, bool enableSsl, string smtpUser)
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
                smtpClient.Credentials = new NetworkCredential(smtpUser, "kixx kxou qgdj wgmx");

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
