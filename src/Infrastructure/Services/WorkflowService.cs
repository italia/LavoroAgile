using Domain.Model;
using Elsa.Models;
using Elsa.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Elsa.Persistence.Specifications.WorkflowInstances;
using Elsa.Persistence.Specifications;
using Elsa.Persistence;
using Elsa.Activities.Signaling.Services;
using Domain;
using System.Linq.Expressions;

namespace Infrastructure.Services
{
    /// <summary>
    /// Implementa il servizio di gestione del workflow.
    /// </summary>
    public class WorkflowService(IWorkflowRegistry workflowRegistry, IWorkflowDefinitionDispatcher workflowDispatcher, IWorkflowInstanceStore workflowInstanceStore, ISignaler signaler) : IWorkflowService
    {
        public async Task StartWorkflowAsync(WorkflowNames workflowName, Guid correlationId, bool singleton, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(workflowName.ToString()))
            {
                throw new ArgumentNullException(nameof(workflowName));
            }
            if (correlationId.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(correlationId));
            }
            
            // Recupera tutti i bluprint di workflow che abbiano come tag il nome passato per
            // parametro e siano stati pubblicati. Se non ne trova uno, solleva un'eccezione.
            var workflowBlueprints = await workflowRegistry.FindManyByTagAsync(workflowName.ToString(), VersionOptions.Published,  cancellationToken: cancellationToken);

            if (!workflowBlueprints.Any())
            {
                throw new LavoroAgileException($"Workflow non trovato {workflowName }");
            }

            // Ad oggi ci si aspetta che il flusso individuato sia uno solo. Se ne trova di più, solleva
            // un'eccezione
            if (workflowBlueprints.Count() > 1)
            {
                throw new LavoroAgileException($"Trovato più di un workflow con lo stesso nome { workflowName }");
            }

            // Se è richiesto che debba esistere un solo flusso in esecuzione per lo specifico correlationId
            // lo lancia solo se non esiste già un flusso in esecuzione.
            if ((singleton && !(await AlreadyExistsOne(workflowBlueprints.First().Id, correlationId.ToString()))) || !singleton)
            {
                await workflowDispatcher.DispatchAsync(new ExecuteWorkflowDefinitionRequest(workflowBlueprints.First().Id, CorrelationId: correlationId.ToString(), Input: new WorkflowInput(correlationId)), cancellationToken);

            }

        }

        public async Task SendSignalToWorkflowAsync(Guid correlationId, LavoroAgileSignals lavoroAgileSignal, string note = null, CancellationToken cancellationToken = default)
        {
            if (correlationId.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(nameof(correlationId));
            }

            await signaler.DispatchSignalAsync(signal: lavoroAgileSignal.ToString(), input: note, correlationId: correlationId.ToString(), cancellationToken: cancellationToken);
            
        }

        /// <summary>
        /// Verifica se esiste già un flusso in esecuzione a partire dall'identificativo della sua
        /// definizione e dal correlationId.
        /// </summary>
        /// <param name="workflowDefinitionId">Identificativo del blueprint da ricercare.</param>
        /// <param name="correlationId">Id di correlazione del flusso.</param>
        /// <returns>true se esiste già un flusso in esecuzione; false altrimenti.</returns>
        private async Task<bool> AlreadyExistsOne(string workflowDefinitionId, string correlationId)
        {
            // Setup query specification.
            var specification = new WorkflowDefinitionIdSpecification(workflowDefinitionId).And(new WorkflowIsAlreadyExecutingSpecification()).WithCorrelationId(correlationId);

            // Count matching results.
            return await workflowInstanceStore.CountAsync(specification) > 0;
        }

        /// <summary>
        /// Elimina l'instanza di WF per lo specifico accordo
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public async Task DeleteInstance(string correlationId, CancellationToken cancellationToken)
        {
            var specification = Specification<WorkflowInstance>.Identity;
            specification = specification.WithCorrelationId(correlationId);

            await workflowInstanceStore.DeleteManyAsync(specification, cancellationToken);
        }
    }   

}
