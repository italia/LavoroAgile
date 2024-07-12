using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IMinisteroLavoroStatusLogger
    {
        /// <summary>
        /// Aggiorna lo stato di invio della nuova comunicazione al Ministero del Lavoro
        /// </summary>
        /// <param name="accordoId">Identificativo dell'accordo di riferimento.</param>
        /// <param name="sentSuccessfully">Indica se l'invio è evvenuto con successo.</param>
        /// <param name="sendError">Eventuale descrizione dell'errore in invio.</param>
        Task SetNuovaComunicazioneStatus(Guid accordoId, bool sentSuccessfully, string sendError, CancellationToken cancellationToken);

        /// <summary>
        /// Aggiorna lo stato di invio del recesso comunicazione al Ministero del Lavoro
        /// </summary>
        /// <param name="accordoId">Identificativo dell'accordo di riferimento.</param>
        /// <param name="sentSuccessfully">Indica se l'invio è evvenuto con successo.</param>
        /// <param name="sendError">Eventuale descrizione dell'errore in invio.</param>
        Task SetRecessoComunicazioneStatus(Guid accordoId, bool sentSuccessfully, string sendError, CancellationToken cancellationToken);
    }
}
