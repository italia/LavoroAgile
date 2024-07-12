using System;
using System.Collections.Generic;

namespace Domain.Model.ExternalCommunications
{
    /// <summary>
    /// Rappresenta dati da trasmettere relativamente ad una trasmissione di giornate lavorative.
    /// </summary>
    public class WorkingDaysTransmission
    {
        /// <summary>
        /// Identificativo dell'accordo.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificativo dell'utente.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Lista delle giornate lavorative.
        /// </summary>
        public List<string> WorkingDays { get; set; }

        /// <summary>
        /// Inizializza una nuova <see cref="WorkingDaysTransmission"/>.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente.</param>
        public WorkingDaysTransmission(string uid)
        {
            this.UserId = uid ?? throw new ArgumentNullException(nameof(uid));
        }

        public WorkingDaysTransmission()
        {

        }

        /// <summary>
        /// Eventuale vecchio accordo da gestire.
        /// </summary>
        public WorkingDaysTransmission OldAccordo { get; set; }

    }

}
