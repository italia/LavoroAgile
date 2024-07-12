using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.MinisteroLavoro.Response
{
    public class Esito
    {
        public int idComunicazione { get; set; }
        public string codiceComunicazione { get; set; }
        public string codice { get; set; }
        public string messaggio { get; set; }
        public string linguaggio { get; set; }
    }
}
