using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace ContentDeliverySystem.SVC.Helpers
{
    public class BatchMail : IDisposable
    {
        private bool enableSSL = false;
        private int smtpPort = 587;

        public string SmtpMailUser { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpAdress { get; set; }
        public bool EnableSSL { get { return enableSSL; } set { enableSSL = value; } }
        public int SmtpPort { get { return smtpPort; } set { smtpPort = value; } }

        public string To { get; set; }

        public string From { get; set; }
        public string ReplyTo { get; set; }
        public string ReplyToName { get; set; }
        public List<string> CopyTo
        {
            get
            {
                return _Copyto;
            }
        }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool isHtml { get; set; }

        private List<string> _Copyto = new List<string>();
        private SmtpClient SmtpServer = new SmtpClient();
        private NetworkCredential SmtpCredentials = new NetworkCredential();
        private MailMessage EmailMessage = new MailMessage();

        public void Send()
        {
            ConfigureCredentials();
            ConfigureSmtp();
            BuildMessage();


            SmtpServer.Send(EmailMessage);
        }

        private void ConfigureCredentials()
        {
            SmtpCredentials.UserName = SmtpMailUser;
            SmtpCredentials.Password = SmtpPassword;
        }

        private void ConfigureSmtp()
        {
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = SmtpCredentials;
            SmtpServer.Host = SmtpAdress;
            SmtpServer.Port = SmtpPort;
            SmtpServer.EnableSsl = EnableSSL;
        }

        private void BuildMessage()
        {
            EmailMessage = new MailMessage(SmtpMailUser, To, Subject, Message);
            if (!string.IsNullOrEmpty(ReplyTo) && !string.IsNullOrEmpty(ReplyToName))
                EmailMessage.ReplyToList.Add(new MailAddress(ReplyTo, ReplyToName));



            CopyTo.ForEach(cc => EmailMessage.CC.Add(new MailAddress(cc)));
            EmailMessage.IsBodyHtml = isHtml;
        }

        public void SendAsync()
        {
            ConfigureCredentials();
            ConfigureSmtp();
            BuildMessage();

            SmtpServer.SendAsync(EmailMessage, null);
        }

        public void Dispose()
        {
            SmtpCredentials = null;
            SmtpServer = null;
            EmailMessage.Dispose();
        }
    }
}