using System;
using System.Collections.Generic;

namespace Domain.Model.ExternalCommunications
{
    /// <summary>
    /// Rappresenta dati da trasmettere relativamente ad una trasmissione di attività.
    /// </summary>
    public class WorkingActivityTransmission
    {
        /// <summary>
        /// Identificativo dell'accordo.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Codice dell'accordo.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Informazioni sulle attività.
        /// </summary>
        public List<ActivityDetails> Activities { get; set; }

        /// <summary>
        /// Identificativo dell'utente.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Data inizio validità attività.
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Data fine validità attività.
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Numero di giorni lavorativi.
        /// </summary>
        public int WorkingDaysCount { get; set; }

        /// <summary>
        /// Dettaglio dell'attività.
        /// </summary>
        public class ActivityDetails
        {
            /// <summary>
            /// Codice dell'attività
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// Descrizione dell'attività
            /// </summary>
            public string Description { get; set; }

        }

        /// <summary>
        /// Eventuale accordo precedente da gestire.
        /// </summary>
        public WorkingActivityTransmission OldAccordo { get; set; }

    }

}
