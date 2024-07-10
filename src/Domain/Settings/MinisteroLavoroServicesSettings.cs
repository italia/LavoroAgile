using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Settings
{
    /// <summary>
    /// Configurazione servizi Ministero Lavoro
    /// </summary>
    public class MinisteroLavoroServicesSettings
    {
        public string Authorization  { get; set;}

        public string GetToken { get; set; }

        public string ScopeGetToken { get; set; }

        public string CreaComunincazione { get; set; }

        public string ModificaComunicazione { get; set; }

        public string DettaglioComunicazione { get; set; }

        public string RicercaComunicazione { get; set; }

        public string AnnullaComunicazione { get; set; }

        public string RecediComunicazione { get; set; }

        public string CodiceFiscaleDatoreLavoro { get; set; }

        public string DenominazioneDatoreLavoro { get; set; }

        public string PosizioneINAIL { get; set; }

        public string TariffaINAIL { get; set; }

        /// <summary>
        /// Verifica la validità della configurazione.
        /// </summary>
        public bool Valid => !(string.IsNullOrWhiteSpace(Authorization) ||
            string.IsNullOrWhiteSpace(GetToken) ||
            string.IsNullOrWhiteSpace(ScopeGetToken) ||
            string.IsNullOrWhiteSpace(CreaComunincazione) ||
            string.IsNullOrWhiteSpace(ModificaComunicazione) ||
            string.IsNullOrWhiteSpace(DettaglioComunicazione) ||
            string.IsNullOrWhiteSpace(RicercaComunicazione) ||
            string.IsNullOrWhiteSpace(AnnullaComunicazione) ||
            string.IsNullOrWhiteSpace(RecediComunicazione) ||
            string.IsNullOrWhiteSpace(CodiceFiscaleDatoreLavoro) ||
            string.IsNullOrWhiteSpace(DenominazioneDatoreLavoro) ||
            string.IsNullOrWhiteSpace(PosizioneINAIL) ||
            string.IsNullOrWhiteSpace(TariffaINAIL)
            );
    }
}
