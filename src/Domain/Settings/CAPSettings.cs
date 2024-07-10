namespace Domain.Settings
{
    /// <summary>
    /// Configurazione della coda.
    /// </summary>
    public class CAPSettings
    {
        /// <summary>
        /// Intervallo fra due tentativi di invio. Default: un'ora
        /// </summary>
        public int FailedRetryInterval { get; set; } = 3600;

        /// <summary>
        /// Numero massimo di tentativi. Default: 192 per arrivare, di default a poco meno di 8 giorni.
        /// </summary>
        public int FailedRetryCount { get; set; } = 192;

        /// <summary>
        /// Nome della sezione di configurazione da cui recuperare le impostazioni del CAP.
        /// </summary>
        public const string Position = nameof(CAPSettings);

    }
}
