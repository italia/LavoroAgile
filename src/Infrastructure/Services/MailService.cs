using Domain.Model;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    /// <summary>
    /// Implementa la classe di servizio per la gestione delle email
    /// </summary>
    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;

        private readonly MailSettings _mailSettings;

        public MailService(ILogger<MailService> logger,
                            IOptions<MailSettings> mailSettings)
        {
            _logger = logger;
            _mailSettings = mailSettings?.Value;
        }

        public async Task SendEmailAsync(Email mail)
        {
            try
            {
                SmtpClient client = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
                //client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(_mailSettings.UserName, _mailSettings.Password);

                MailMessage message = new MailMessage();
                message.From = new MailAddress(_mailSettings.DefaultSender);

                if (mail.destinatari != null && mail.destinatari.Count() > 0)
                {
                    foreach (string destinatario in mail.destinatari)
                    {
                        message.To.Add(new MailAddress(destinatario));
                    }

                    if (mail.destinatariCC != null && mail.destinatariCC.Count() > 0)
                    {
                        foreach (string destinatarioCC in mail.destinatariCC)
                        {
                            message.CC.Add(new MailAddress(destinatarioCC));
                        }
                    }
                    message.IsBodyHtml = true;
                    message.Body = mail.testoEmail;                    
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = mail.oggetto;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;

                    await client.SendMailAsync(message);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Errore durante l'invio della email: " + ex.Message);
            }
        }
    }
}
