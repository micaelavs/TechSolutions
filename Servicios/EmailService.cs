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
        private readonly string _smtpServer = "smtp.hostinger.com.ar";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "info@micaelasanchez.com.ar";
        private readonly string _smtpPass = "Catalagata05_";
        //pflt ecyg iiez oqsy
        /*private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587; //25 587 //465
        private readonly string _smtpUser = "miicaelavs@gmail.com"; // Tu correo de Gmail
        private readonly string _smtpPass = "szjj akcj gfgy kiou";  // La contraseña de aplicación generada*/
        
        public void SendEmail(string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true, // Define si el cuerpo del mensaje es HTML o texto plano
                };

                mailMessage.To.Add(to);

                var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                    EnableSsl = true, // TLS (STARTTLS) para cifrar la conexión
                    //DeliveryMethod = SmtpDeliveryMethod.Network,
                    //UseDefaultCredentials = false, // Se desactiva para que no utilice las credenciales predeterminadas
                };

                smtpClient.Send(mailMessage);
                Console.WriteLine("Correo enviado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
        }
    }
}
