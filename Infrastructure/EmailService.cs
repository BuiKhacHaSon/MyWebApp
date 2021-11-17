using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyWebApp.Infrastructure
{
    public class EmailSettings
    {
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string CcEmail { get; set; }
        public string BccEmail { get; set; }

        public EmailSettings() { }
    }

    public class SMTPSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool EnableSsl { get; set; }

        public SMTPSettings() { }

        public SMTPSettings(string server, int port)
        {
            Server = server;
            Port = port;
        }
    }
    public interface IEmailService
    {
        void SendMail(string fromName, string fromEmail, string toEmail, string cc, string bcc, string subject, string body, bool isBodyHtml);
        void ReportError(Exception ex);
        void ReportToDev(string subject, string body);
        public void Notification(string name, string email, string title, string message);
    }
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        private readonly SMTPSettings smtpSettings;
        public EmailService(IOptions<EmailSettings> emailSettings, IOptions<SMTPSettings> smtpSettings)
        {
            this.emailSettings = emailSettings.Value;
            this.smtpSettings = smtpSettings.Value;
        }

        public void Notification(string name, string email, string title, string message)
        {
            string subject = "[Buikhachason.com] Thông báo";
            string body = "****************<br/>";
            body += "From: " + name + " email: " + email + " <br/>";
            body += "<h3> " + title + "</h3>";
            body += "<br/><p> " + message + "</>";
            this.SendMail(
                this.emailSettings.FromName,
                this.emailSettings.FromEmail,
                this.emailSettings.ToEmail,
                this.emailSettings.CcEmail,
                this.emailSettings.BccEmail,
                subject,
                body,
                true);
        }

        public void ReportError(Exception ex)
        {
            string subject = "[buikhachason] Error occur";
            string body = "";
            body += "Error message: " + ex.Message + "<br/>";
            body += "<p>Error details:</p>";
            body += ex.ToString();
            this.SendMail(
                this.emailSettings.FromName,
                this.emailSettings.FromEmail,
                this.emailSettings.ToEmail,
                this.emailSettings.CcEmail,
                this.emailSettings.BccEmail,
                subject,
                body,
                true);
        }

        public void ReportToDev(string subject, string body)
        {
            this.SendMail(
                this.emailSettings.FromName,
                this.emailSettings.FromEmail,
                this.emailSettings.ToEmail,
                this.emailSettings.CcEmail,
                this.emailSettings.BccEmail,
                subject,
                body,
                true);
        }

        public void SendMail(string fromName, string fromEmail, string toEmail, string cc, string bcc, string subject, string body, bool isBodyHtml)
        {
            fromName = fromName ?? "";
            fromEmail = fromEmail ?? "";
            toEmail = toEmail ?? "";
            bcc = bcc ?? "";
            subject = subject ?? "";
            body = body ?? "";

            MailMessage mail = new MailMessage();
            if (!string.IsNullOrWhiteSpace(fromName))
                mail.From = new MailAddress(fromEmail, fromName);
            else
                mail.From = new MailAddress(fromEmail);

            List<string> tos = (toEmail ?? "").Split(';').Where(t => !string.IsNullOrWhiteSpace(t.ToString())).ToList();
            foreach (string mto in tos)
            {
                mail.To.Add(new MailAddress(mto));
            }
            List<string> ccs = (cc ?? "").Split(';').Where(o => !string.IsNullOrWhiteSpace(o.ToString())).ToList();
            foreach (string mcc in ccs)
            {
                mail.CC.Add(new MailAddress(mcc));
            }
            List<string> bccs = (bcc ?? "").Split(';').Where(o => !string.IsNullOrWhiteSpace(o.ToString())).ToList();
            foreach (string mbcc in bccs)
            {
                mail.Bcc.Add(new MailAddress(mbcc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isBodyHtml;

            using (var client = new SmtpClient(this.smtpSettings.Server))
            {
                client.Port = this.smtpSettings.Port;
                client.Credentials = new System.Net.NetworkCredential(this.smtpSettings.UserName, this.smtpSettings.Password);
                client.EnableSsl = this.smtpSettings.EnableSsl;
                client.Send(mail);
            }
        }
    }

}
