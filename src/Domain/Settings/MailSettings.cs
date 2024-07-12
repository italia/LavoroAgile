using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings
{
    public class MailSettings
    {
        public string DefaultSender { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string TestoEmailNotificaRecesso { get; set; }
    }
}
