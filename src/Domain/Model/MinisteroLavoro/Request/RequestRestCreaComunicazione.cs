using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.MinisteroLavoro.Request
{
    public class RequestRestCreaComunicazione
    {
        public List<Comunicazione> Comunicazione { get; set; }
    }

    public class Comunicazione
    {
        public int id { get; set; }
        public SezioneDatoreLavoro SezioneDatoreLavoro { get; set; }
        public SezioneLavoratore SezioneLavoratore { get; set; }
        public SezioneRapportoLavoro SezioneRapportoLavoro { get; set; }
        public SezioneAccordoSmartWorking SezioneAccordoSmartWorking { get; set; }
        public string codTipologiaComunicazione { get; set; }
    }

    public class SezioneAccordoSmartWorking
    {
        public string dataSottoscrizioneAccordo { get; set; }
        public string dataInizioPeriodo { get; set; }
        public string dataFinePeriodo { get; set; }
        public string tipologiaDurataPeriodo { get; set; }
    }

    public class SezioneDatoreLavoro
    {
        public string codiceFiscaleDatoreLavoro { get; set; }
        public string denominazioneDatoreLavoro { get; set; }
    }

    public class SezioneLavoratore
    {
        public string codiceFiscaleLavoratore { get; set; }
        public string nomeLavoratore { get; set; }
        public string cognomeLavoratore { get; set; }
        public string dataNascitaLavoratore { get; set; }
        public string codComuneNascitaLavoratore { get; set; }
    }

    public class SezioneRapportoLavoro
    {
        public string dataInizioRapportoLavoro { get; set; }
        public string codTipologiaRapportoLavoro { get; set; }
        public string posizioneINAIL { get; set; }
        public string tariffaINAIL { get; set; }
    }


}
