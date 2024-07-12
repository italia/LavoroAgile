using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Definisce il contratto da implementare per un logger di attività di invio
    /// giornate e attività lavorative.
    /// </summary>
    public interface ISendWDAndAStatusLogger
    {
        /// <summary>
        /// Aggiorna lo stato di invio delle giornate lavorative.
        /// </summary>
        /// <param name="accordoId">Identificativo dell'accordo di riferimento.</param>
        /// <param name="sentSuccessfully">Indica se l'invio è evvenuto con successo.</param>
        /// <param name="sendError">Eventuale descrizione dell'errore in invio.</param>
        Task SetWorkingDaysStatus(Guid accordoId, bool sentSuccessfully, string sendError, CancellationToken cancellationToken);

        /// <summary>
        /// Aggiorna lo stato di invio delle attività.
        /// </summary>
        /// <param name="accordoId">Identificativo dell'accordo di riferimento.</param>
        /// <param name="sentSuccessfully">Indica se l'invio è evvenuto con successo.</param>
        /// <param name="sendError">Eventuale descrizione dell'errore in invio.</param>
        Task SetActivitiesStatus(Guid accordoId, bool sentSuccessfully, string sendError, CancellationToken cancellationToken);
    
    }
}
