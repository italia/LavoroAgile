using Domain.Model.ExternalCommunications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{

    /// <summary>
    /// Definisce il contratto per un servizio di rimozione fasce orarie ed attività ad un sistema esterno.
    /// </summary>
    public interface IDeleteWorkingDaysAndRestrictActivities
    {
        /// <summary>
        /// Elimina un elenco di giornate lavorative.
        /// </summary>
        /// <param name="workingDays">Informazioni sulle giornate lavorative.</param>
        /// <returns>Elenco delle date cancellate</returns>
        Task<IEnumerable<DateTime>> DeleteWorkingDays(WorkingDaysTransmission workingDays);

        /// <summary>
        /// Calcola le giornate da eliminare per uno specifico accordo e ne invoca la cancellazione
        /// </summary>
        /// <param name="accordoId">Identificativo dell'accordo da lavorare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lista delle date cancellate</returns>
        Task<IEnumerable<DateTime>> CalculateAndDeleteWorkingDaysAsync(Guid accordoId, CancellationToken cancellationToken);

        /// <summary>
        /// Restringe la validità di una serie di attività.
        /// </summary>
        /// <param name="workingActivityTransmission">Informazioni sulle attività.</param>
        /// <returns></returns>
        Task RestrictActivities(WorkingActivityTransmission workingActivityTransmission);

        /// <summary>
        /// Indica che l'integrazione è attiva.
        /// </summary>
        public bool Enabled { get; }

    }
}
