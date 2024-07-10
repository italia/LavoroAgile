using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.MinisteroLavoro.Request
{
    public class RequestRestAnnullaComunicazione
    {
        public List<AnnullaComunicazione> AnnullaComunicazione { get; set; }
    }

    public class AnnullaComunicazione
    {
        public int id { get; set; }
        public string CodiceComunicazione { get; set; }
        public string CodTipologiaComunicazione { get; set; }
    }
}
