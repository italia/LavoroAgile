using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.MinisteroLavoro.Response
{
    public class ResponseRestEsitoDettaglio
    {
        public Esito Esito { get; set; }
        public Comunicazione Comunicazione { get; set; }
    }
    public class Comunicazione
    {
        public string codiceComunicazione { get; set; }
        public object codiceComunicazionePrecedente { get; set; }
        public string CFAzienda { get; set; }
        public string denominazioneAzienda { get; set; }
        public string CFLavoratore { get; set; }
        public string cognomeLavoratore { get; set; }
        public string nomeLavoratore { get; set; }
        public DateTime dataNascitaLavoratore { get; set; }
        public string codComuneStatoEsteroNascitaLavoratore { get; set; }
        public DateTime dataInizioRapporto { get; set; }
        public string posizioneINAIL { get; set; }
        public string tariffaINAIL { get; set; }
        public DateTime dataSottoscrizioneAccordo { get; set; }
        public string tipologiaDurataPeriodo { get; set; }
        public object CFSoggettoAbilitato { get; set; }
        public string codiceIdentificativoPeriodoSmartWorking { get; set; }
        public DateTime dataInvio { get; set; }
        public string modalitàInvio { get; set; }
        public DateTime dataUltimaModifica { get; set; }
        public DateTime dataFineAccordo { get; set; }
        public DateTime dataInizioPeriodo { get; set; }
        public string codTipologiaRapportoLavoro { get; set; }
        public string codTipologiaComunicazione { get; set; }
        public object codTipoSoggettoAbilitato { get; set; }
    }

}
