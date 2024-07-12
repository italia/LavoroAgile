using Domain.Model.ExternalCommunications;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Definisce il contratto per un servizio di invio fasce orarie ed attività ad un sistema esterno.
    /// </summary>
    public interface ISendWorkingDaysAndActivities
    {
        /// <summary>
        /// Invia le giornate lavorative
        /// </summary>
        /// <param name="workingDays">Informazioni sulle giornate lavorative.</param>
        /// <returns></returns>
        Task SendWorkingDays(WorkingDaysTransmission workingDays);

        /// <summary>
        /// Elimina le giornate lavorative.
        /// </summary>
        /// <param name="workingDays">Informazioni sulle giornate lavorative.</param>
        /// <returns></returns>
        Task DeleteWorkingDays(WorkingDaysTransmission workingDays);

        /// <summary>
        /// Invia le informazioni sulle attività.
        /// </summary>
        /// <param name="workingActivityTransmission">Informazioni sulle attività da inviare.</param>
        /// <returns></returns>
        Task SendActivities(WorkingActivityTransmission workingActivityTransmission);

        /// <summary>
        /// Indica che l'integrazione è attiva.
        /// </summary>
        public bool Enabled { get; }

    }

}
