namespace Infrastructure.Utils
{
    /// <summary>
    /// Nomi dei canali CAP.
    /// </summary>
    public static class CAPChannels
    {
        /// <summary>
        /// Identificativo del canale di invio giornate.
        /// </summary>
        public const string SendDays = "SendDays";

        /// <summary>
        /// Identificativo del canale di invio attività
        /// </summary>
        public const string SendActivities = "SendActivities";

        /// <summary>
        /// Identificativo del canale per la chiusura dell'accordo precedente non più in corso
        /// </summary>
        public const string ChiusuraAccordoPrecedente = "ChiusuraAccordoPrecedente";

        /// <summary>
        /// Identificativo del canale per la cancellazione delle giornate.
        /// </summary>
        public const string DeleteDays = "DeleteDays";

        /// <summary>
        /// Identificativo del canale per la cancellazione delle giornate da amministrazione (remediation).
        /// </summary>
        public const string DeleteDaysRemediation = "DeleteDaysRemediation";

        /// <summary>
        /// Identificativo del canale per l'invio email.
        /// </summary>
        public const string SendEmail = "SendEmail";

        /// <summary>
        /// Identificativo del canale per l'invio di una nuova comunicazione al Ministero del Lavoro
        /// </summary>
        public const string NuovaComunicazioneMinisteroLavoro = "NuovaComunicazioneMinisteroLavoro";

        /// <summary>
        /// Identificativo del canale per l'invio di una comunicazione di recesso al Ministero del Lavoro
        /// </summary>
        public const string RecessoComunicazioneMinisteroLavoro = "RecessoComunicazioneMinisteroLavoro";

    }

}
