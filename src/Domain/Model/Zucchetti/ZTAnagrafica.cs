using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Zucchetti
{
    public class ZTAnagrafica
    {
        public List<ZTAccordo> activit_mstd { get; set; }
        public List<ZTMacrocategoria> jobordr_mstd { get; set; }
        public List<ZTAttivita> entity1_mstd { get; set; }
        public List<ZTAccordoUtente> attr_mstd { get; set; }

    }
}
