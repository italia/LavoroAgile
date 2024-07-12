using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Servizio per l'avvio di workflow.
    /// </summary>
    public interface IWorkflowService
    {
        /// <summary>
        /// Avvia un workflow.
        /// </summary>
        /// <param name="workflowName">Nome del flusso da avviare.</param>
        /// <param name="correlationId">Id di correlazione.</param>
        /// <param name="singleton">Indica se deve esistere un unica istanza del workflow attiva per il <paramref name="correlationId"/>.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartWorkflowAsync(WorkflowNames workflowName, Guid correlationId, bool singleton = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Invia un segnale a workflow correlati dal <paramref name="correlationId"/>.
        /// </summary>
        /// <param name="correlationId">Id di correlazione dei flussi a cui inviare il segnale</param>
        /// <param name="lavoroAgileSignal">Nome del segnale da inviare</param>
        /// <param name="note">Note da inviare insieme al segnale.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendSignalToWorkflowAsync(Guid correlationId, LavoroAgileSignals lavoroAgileSignal, string note = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina l'instanza di WF per lo specifico accordo
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        Task DeleteInstance(string correlationId, CancellationToken cancellationToken);       
    }

}
