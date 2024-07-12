using System;

namespace PCM_LavoroAgile.Models
{
    /// <summary>
    /// View model dello stato di trasmissione dell'accordo.
    /// </summary>
    public class TransmissionStatusViewModel
    {
        /// <summary>
        /// Indica se le giornate sono state inviate con successo.
        /// </summary>
        public bool WorkingDaysSentSuccessfully { get; set; }

        /// <summary>
        /// Data dell'ultimo tentativo di invio effettuato.
        /// </summary>
        public DateTime? LastWorkingDaysSentDate { get; set; }

        /// <summary>
        /// Eventuale errore ricevuto dall'ultimo invio.
        /// </summary>
        public string WorkingDaysSendError { get; set; }

        /// <summary>
        /// Indica se le attività sono state inviate con successo.
        /// </summary>
        public bool WorkingActivitiesSentSuccessfully { get; set; }

        /// <summary>
        /// Data dell'ultimo tentativo di invio attività.
        /// </summary>
        public DateTime? LastWorkingActivitiesSentDate { get; set; }

        /// <summary>
        /// Eventuale errore ricevuto dall'ultimo invio delle attività.
        /// </summary>
        public string WorkingActivitiesSendError { get; set; }

        /// <summary>
        /// Indica se le nuova comunicazione al Ministero del Lavoro è stata inviata con successo.
        /// </summary>
        public bool NuovaComunicazioneMinisteroLavoroSentSuccessfully { get; set; }

        /// <summary>
        /// Data dell'ultimo tentativo di invio nuova comunicazione al Ministero del Lavoro.
        /// </summary>
        public DateTime? NuovaComunicazioneMinisteroLavoroLastSentDate { get; set; }

        /// <summary>
        /// Eventuale errore ricevuto dall'ultimo invio della nuova comunicazione al Ministero del Lavoro.
        /// </summary>
        public string NuovaComunicazioneMinisteroLavoroSendError { get; set; }

        /// <summary>
        /// Indica se le comunicazione di recesso al Ministero del Lavoro è stata inviata con successo.
        /// </summary>
        public bool RecessoComunicazioneMinisteroLavoroSentSuccessfully { get; set; }

        /// <summary>
        /// Data dell'ultimo tentativo di invio di recesso comunicazione al Ministero del Lavoro.
        /// </summary>
        public DateTime? RecessoComunicazioneMinisteroLavoroLastSentDate { get; set; }

        /// <summary>
        /// Eventuale errore ricevuto dall'ultimo invio di recesso comunicazione al Ministero del Lavoro.
        /// </summary>
        public string RecessoComunicazioneMinisteroLavoroSendError { get; set; }

        /// <summary>
        /// Identificativo dell'accordo di riferimento.
        /// </summary>
        public Guid AccordoId { get; set; }

    }
}
