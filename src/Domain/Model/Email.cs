using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Email
    {
        public List<string> destinatari { get; set; } = new List<string>();

        public List<string> destinatariCC { get; set; } = new List<string>();

        public string oggetto { get; set; }
        
        public string testoEmail { get; set; }
    }
}
