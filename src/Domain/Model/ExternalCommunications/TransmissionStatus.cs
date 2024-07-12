using System;

namespace Domain.Model.ExternalCommunications
{
    /// <summary>
    /// Rappresenta lo stato di invio delle informazioni verso sistemi esterni.
    /// </summary>
    public class TransmissionStatus
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

        /// <summary>
        /// Riferimento all'accordo di riferimento.
        /// </summary>
        public virtual Accordo Accordo { get; set; }

        internal TransmissionStatus()
        {

        }

        /// <summary>
        /// Inizializza un nuovo <see cref="TransmissionStatus"/>.
        /// </summary>
        /// <param name="accordoId">Identificativo dell'accordo di riferimento</param>
        public TransmissionStatus(Guid accordoId)
        {
            this.AccordoId = accordoId;

        }

        /// <summary>
        /// Inizializza un nuovo <see cref="TransmissionStatus"/>.
        /// </summary>
        /// <param name="accordoId">Identificativo dell'accordo di riferimento.</param>
        
        /// <param name="workingDaysSentSuccessfully">Esito invio giornate.</param>
        /// <param name="lastWorkingDaysSentDate">Data dell'ultimo invio delle giornate lavorative.</param>
        /// <param name="workingDaysSendError">Errore invio giornate.</param>
        
        /// <param name="workingActivitiesSentSuccessfully">Esito invio attività.</param>
        /// <param name="workingActivitiesSendError">Errore invio attività.</param>
        /// <param name="lastWorkingActivitiesSentDate">Data dell'ultimo invio delle attività.</param>
        
        /// <param name="nuovaComunicazioneMinisteroLavoroSentSuccessfully">Esito invio nuova comunicazione Minsitero Lavoro.</param>
        /// <param name="nuovaComunicazioneMinisteroLavoroSendError">Errore invio invio nuova comunicazione Ministero Lavoro.</param>
        /// <param name="nuovaComunicazioneMinisteroLavoroLastSentDate">Data dell'ultimo invio nuova comunicazione Ministero Lavoro.</param>
        
        /// <param name="recessoComunicazioneMinisteroLavoroSentSuccessfully">Esito invio recesso comunicazione Minsitero Lavoro.</param>
        /// <param name="recessoComunicazioneMinisteroLavoroSendError">Errore invio invio recesso comunicazione Ministero Lavoro.</param>
        /// <param name="recessoComunicazioneMinisteroLavoroLastSentDate">Data dell'ultimo invio recesso comunicazione Ministero Lavoro.</param>
        public TransmissionStatus(
            Guid accordoId, 
            bool workingDaysSentSuccessfully = false, 
            string workingDaysSendError = "",
            DateTime? lastWorkingDaysSentDate = null,
            
            bool workingActivitiesSentSuccessfully = false,
            string workingActivitiesSendError = "",
            DateTime? lastWorkingActivitiesSentDate = null,
            
            bool nuovaComunicazioneMinisteroLavoroSentSuccessfully = false,
            string nuovaComunicazioneMinisteroLavoroSendError = "",
            DateTime? nuovaComunicazioneMinisteroLavoroLastSentDate = null,
            
            bool recessoComunicazioneMinisteroLavoroSentSuccessfully = false,
            string recessoComunicazioneMinisteroLavoroSendError = "",
            DateTime? recessoComunicazioneMinisteroLavoroLastSentDate = null)
            : this(accordoId)
        {
            WorkingDaysSentSuccessfully = workingDaysSentSuccessfully;
            WorkingDaysSendError = workingDaysSendError;
            LastWorkingDaysSentDate = lastWorkingDaysSentDate;
            
            WorkingActivitiesSentSuccessfully = workingActivitiesSentSuccessfully;
            WorkingActivitiesSendError = workingActivitiesSendError;
            LastWorkingActivitiesSentDate = lastWorkingActivitiesSentDate;
            
            NuovaComunicazioneMinisteroLavoroSentSuccessfully = nuovaComunicazioneMinisteroLavoroSentSuccessfully;
            NuovaComunicazioneMinisteroLavoroSendError = nuovaComunicazioneMinisteroLavoroSendError;
            NuovaComunicazioneMinisteroLavoroLastSentDate = nuovaComunicazioneMinisteroLavoroLastSentDate;

            RecessoComunicazioneMinisteroLavoroSentSuccessfully = recessoComunicazioneMinisteroLavoroSentSuccessfully;
            RecessoComunicazioneMinisteroLavoroSendError = recessoComunicazioneMinisteroLavoroSendError;
            RecessoComunicazioneMinisteroLavoroLastSentDate = recessoComunicazioneMinisteroLavoroLastSentDate;
        }

    }
}
