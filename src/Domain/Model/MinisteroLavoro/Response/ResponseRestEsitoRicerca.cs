using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.MinisteroLavoro.Response
{
    public class ResponseRestEsitoRicerca
    {
        public Esito Esito { get; set; }
        public List<Comunicazioni> Comunicazioni { get; set; }
    }

    public class Comunicazioni
    {
        public string codiceComunicazione { get; set; }
        public string CFLavoratore { get; set; }
        public string nomeLavoratore { get; set; }
        public string cognomeLavoratore { get; set; }
        public DateTime dataNascitaLavoratore { get; set; }
        public string codComuneStatoEsteroNascitaLavoratore { get; set; }
        public DateTime dataInizioPeriodo { get; set; }
        public DateTime dataFineAccordo { get; set; }
        public DateTime dataInizioRapporto { get; set; }
        public DateTime dataInvio { get; set; }
        public string CFAzienda { get; set; }
        public string denominazioneAzienda { get; set; }
    }
}
