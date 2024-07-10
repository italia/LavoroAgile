using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.MinisteroLavoro.Request
{
    public class RequestRestRecediComunicazione
    {
        public List<RecediComunicazione> RecediComunicazione { get; set; }
    }

    public class RecediComunicazione
    {
        public int id { get; set; }
        public string CodiceComunicazione { get; set; }
        public string DataFinePeriodo { get; set; }
        public string CodTipologiaComunicazione { get; set; }
    }
}
