using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TechSolutions.Servicios
{
 
    public class EmailService
    {
        private readonly string _smtpServer = "";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "";
        private readonly string _smtpPass = "";

        public void SendEmail(string to, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true,
            };

            smtpClient.Send(mailMessage);
        }
    }
}
