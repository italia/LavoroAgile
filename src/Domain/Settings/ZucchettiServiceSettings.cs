namespace Domain.Settings
{
    /// <summary>
    /// Configurazione del servizio Zucchetti
    /// </summary>
    public class ZucchettiServiceSettings
    {
        /// <summary>
        /// Url del servizio di anagrafica.
        /// </summary>
        public string AnagUrl { get; set; }

        /// <summary>
        /// Url del servizio di invio giornate lavorative WS.
        /// </summary>
        public string SendJustifyUrl { get; set; }

        /// <summary>
        /// Url del servizio per il censimento delle attività.
        /// </summary>
        public string CreateActivitiesUrl { get; set; }

        /// <summary>
        /// Url del servizio per la creazione della relazione attività-macrocategoria-accordo.
        /// </summary>
        public string CreateRelUrl { get; set; }

        /// <summary>
        /// Url del servizio di cancellazione giornate.
        /// </summary>
        public string DeleteWorkingDaysUrl { get; set; }

        /// <summary>
        /// Url del servizio di recupero delle giornate lavorative.
        /// </summary>
        public string GetSWDaysUrl { get; set; }

        /// <summary>
        /// Username dell'utente di servizio da utilizzare per l'autenticazione al servizio di lettura.
        /// </summary>
        public string RUsername { get; set; }

        /// <summary>
        /// Password dell'utente di servizio da utilizzare per l'autenticazione al servizio di lettura.
        /// </summary>
        public string RPassword { get; set; }

        /// <summary>
        /// Username dell'utente di servizio da utilizzare per l'autenticazione al servizio di scrittura.
        /// </summary>
        public string WUsername { get; set; }

        /// <summary>
        /// Password dell'utente di servizio da utilizzare per l'autenticazione al servizio di scrittura.
        /// </summary>
        public string WPassword { get; set; }

        /// <summary>
        /// Identificativo dell'azienda.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Codice da utilizzare per indicare attività di SW.
        /// </summary>
        public string SmartWorkingCode { get; set; } = "SWK";

        /// <summary>
        /// Descrizione dell'attività di lavoro agile.
        /// </summary>
        public string SmartWorkingReason { get; set; } = "Lavoro agile";

        /// <summary>
        /// Descrizione dell'accordo di lavoro agile (utilizzato per il censimento della 
        /// activity Zucchetti).
        /// </summary>
        public string SmartWorkingActivityDescription { get; set; } = "Accordo lavoro agile";

        /// <summary>
        /// Descrizione della commessa specifica di accordo.
        /// </summary>
        public string SmartWorkingRelatedWorklistDescription { get; set; } = "Attività di accordo";

        /// <summary>
        /// Codici delle attività non programmabili.
        /// </summary>
        public string CodeActNotProgrammable { get; set; }

        /// <summary>
        /// Codice delle attività di formazione.
        /// </summary>
        public string CodeActTraining { get; set; }

        /// <summary>
        /// Elenco dei codici giustificativi da ricercare, racchiusi fra apice singolo
        /// e separati da virgola.
        /// </summary>
        public string WorklistToSearchCodes { get; set; } = "'SWK'";

        /// <summary>
        /// Verifica la validità della configurazione.
        /// </summary>
        public bool Valid => !(string.IsNullOrWhiteSpace(AnagUrl) ||
            string.IsNullOrWhiteSpace(SendJustifyUrl) ||
            string.IsNullOrWhiteSpace(CreateActivitiesUrl) ||
            string.IsNullOrWhiteSpace(DeleteWorkingDaysUrl) ||
            string.IsNullOrWhiteSpace(CreateRelUrl) ||
            string.IsNullOrWhiteSpace(GetSWDaysUrl) ||
            string.IsNullOrWhiteSpace(RUsername) ||
            string.IsNullOrWhiteSpace(RPassword) ||
            string.IsNullOrWhiteSpace(WUsername) ||
            string.IsNullOrWhiteSpace(WPassword) ||
            string.IsNullOrWhiteSpace(Company) ||
            string.IsNullOrWhiteSpace(SmartWorkingCode) ||
            string.IsNullOrWhiteSpace(SmartWorkingReason) ||
            string.IsNullOrWhiteSpace(SmartWorkingActivityDescription) ||
            string.IsNullOrWhiteSpace(SmartWorkingRelatedWorklistDescription)
            ); 
    }
}
