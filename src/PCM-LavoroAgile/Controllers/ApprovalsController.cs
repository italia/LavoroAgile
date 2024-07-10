using AutoMapper;
using Domain;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    /// <summary>
    /// Questo controller offre endpoint per l'approvazione, ed il recesso di un accordo.
    /// </summary>
    public class ApprovalsController : Controller
    {
        /// <summary>
        /// Riferimento al servizio di gestione dei workflow.
        /// </summary>
        private readonly IWorkflowService _workflowService;

        /// <summary>
        /// Riferimento al servizio di gestione degli accordi.
        /// </summary>
        private readonly IAccordoService _accordoService;

        /// <summary>
        /// Logger attività controller.
        /// </summary>
        private readonly ILogger<ApprovalsController> _logger;


        private readonly IMapper _mapper;

        /// <summary>
        /// Istanzia un nuovo <see cref="ApprovalsController"/>.
        /// </summary>
        /// <param name="workflowService"></param>
        /// <param name="accordoService"></param>
        /// <param name="logger"></param>
        public ApprovalsController(IWorkflowService workflowService, IAccordoService accordoService, ILogger<ApprovalsController> logger, IMapper mapper)
        {
            _workflowService = workflowService;
            _accordoService = accordoService;
            _logger = logger;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Avvia il workflow per l'approvazione di un accordo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo su cui avviare il workflow di approvazione.</param>
        /// <param name="role">Ruolo con cui sta operando l'utente.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Pagina di dettaglio dell'accordo.</returns>
        /// <remarks>Questo endoint può essere invocato solo da un utente.</remarks>
        [Authorize(policy: "PCM_InternalOperativeRole_Utente")]
        public async Task<ActionResult> Bozza(Guid id, string role, CancellationToken cancellationToken)
        {
            try
            {
                // Si sfrutta come correlation id l'id dell'accordo da lavorare in modo da semplificare 
                // la diagnostica e le operazioni sul flusso.
                await _workflowService.StartWorkflowAsync(WorkflowNames.ApprovazioneLavoroAgileRidotto, id, true, cancellationToken);
                TempData.SendNotification(NotificationType.Success, "Flusso di approvazione avviato con successo!");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (LavoroAgileException lae)
            {
                TempData.SendNotification(NotificationType.Error, lae.Message);
                return RedirectToAction("Details", "Accordi", new { id = id, role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError("Avvia workflow accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore durante l'avvio del flusso di approvazione dell'accordo.");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
        }

        /// <summary>
        /// Approvazione/Rifiuto/Richiesta integrazione accordo da parte dei dirigenti.
        /// </summary>
        /// <param name="ids">Elenco degli identificativi degli accordi da approvare/rifiutare/integrare (conterrà elementi nel caso di azione svolta dalla dialog delle azioni massive).</param>
        /// <param name="id">Identificativo dell'accordo su cui effettuare l'operazione.</param>
        /// <param name="role">Ruolo con cui sta operando l'utente.</param>
        /// <param name="action">Azione da eseguire</param>
        /// <param name="note">Note associate all'azione</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Pagina di dettaglio dell'accordo.</returns>
        /// <remarks>Questo endoint può essere invocato solo ai responsabili accordo, capi intermedi e capi struttura.</remarks>
        [Authorize(policy: "AccordiApprovaRifiutaIntegra")]
        public async Task<ActionResult> ApprovaRifiutaIntegra(ICollection<Guid> ids, Guid id, string role, string note, LavoroAgileSignals action, CancellationToken cancellationToken)
        {
            try
            {
                // Se è stato passato un singolo id, viene aggiunto alla lista di id.
                if (!Guid.Empty.Equals(id))
                {
                    ids.Add(id);
                }

                foreach (var id1 in ids)
                {
                    // Si sfrutta come correlation id l'id dell'accordo da lavorare in modo da semplificare 
                    // la diagnostica e le operazioni sul flusso.
                    await _workflowService.SendSignalToWorkflowAsync(id1, action, note, cancellationToken);
                }
                TempData.SendNotification(NotificationType.Success, "Elaborato correttamente!");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invio segnale {action} ai workflow con id di correlazione {string.Join(",", ids)}", ex);
                TempData.SendNotification(NotificationType.Error, "Errore durante l'invio della richiesta di approvazione/rifiuto/integrazione.");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }

        }


        /// <summary>
        /// Sottoscrizione accordo da parte del responsabile accordo o del proponente.
        /// </summary>
        /// <param name="ids">Elenco degli identificativi degli accordi da sottoscrivere (conterrà elementi nel caso di azione svolta dalla dialog delle azioni massive).</param>
        /// <param name="id">Identificativo dell'accordo da sottoscrivere (sarà valorizzato nel caso di azione svolta nella pagina di dettaglio dell'accordo)</param>
        /// <param name="action">Segnala da inviare al flusso di approvazione</param>
        /// <param name="role">Ruolo con cui sta operando l'utente.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Pagina di dettaglio dell'accordo.</returns>
        /// <remarks>Questo endoint può essere invocato solo a responsabile accordo e utente.</remarks>
        [Authorize(policy: "AccordiRecediSottoscrivi")]
        public async Task<ActionResult> Sottoscrivi(ICollection<Guid> ids, Guid id, string role, LavoroAgileSignals action, CancellationToken cancellationToken)
        {
            try
            {
                // Se è stato passato un singolo id, viene aggiunto alla lista di id.
                if (!Guid.Empty.Equals(id))
                {
                    ids.Add(id);
                }

                // Si sfrutta come correlation id l'id dell'accordo da lavorare in modo da semplificare 
                // la diagnostica e le operazioni sul flusso.
                foreach (var id1 in ids)
                {
                    await _workflowService.SendSignalToWorkflowAsync(id1, action, cancellationToken: cancellationToken);

                }
                TempData.SendNotification(NotificationType.Success, "Elaborato correttamente!");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invio segnale {action} ai workflow con id di correlazione {string.Join(",", ids)}", ex);
                TempData.SendNotification(NotificationType.Error, "Errore durante l'invio della richiesta di sottoscrizione.");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
        }

        /// <summary>
        /// Invia la modifica sull'accordo al responsabile dell'accordo
        /// </summary>
        /// <param name="id">Identificativo dell'accordo.</param>
        /// <param name="role">Ruolo con cui sta operando l'utente.</param>
        /// <param name="action">Segnale da inviare al flusso.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(policy: "PCM_InternalOperativeRole_Utente")]
        public async Task<ActionResult> InviaModifica(Guid id, string role, LavoroAgileSignals action, CancellationToken cancellationToken)
        {
            try
            {
                // Si sfrutta come correlation id l'id dell'accordo da lavorare in modo da semplificare 
                // la diagnostica e le operazioni sul flusso.
                await _workflowService.StartWorkflowAsync(WorkflowNames.ApprovazioneLavoroAgileRidotto, id, true, cancellationToken);
                TempData.SendNotification(NotificationType.Success, "Elaborato correttamente!");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invio segnale {action} al workflow con id di correlazione {id}", ex);
                TempData.SendNotification(NotificationType.Error, "Errore durante l'invio dell'integrazione.");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
        }

        /// <summary>
        /// Recesso dell'accordo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo da recedere</param>
        /// <param name="role">Ruolo con cui sta operando l'utente.</param>
        /// <param name="dataRecesso">Data di recesso</param>
        /// <param name="note">Note</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Pagina di dettaglio dell'accordo.</returns>
        /// <remarks>Questo endoint può essere invocato solo a responsabile accordo e utente.</remarks>
        [Authorize(policy: "AccordiRecediSottoscrivi")]
        public async Task<ActionResult> Recedi(Guid id, string role, DateTime dataRecesso, string note, CancellationToken cancellationToken)
        {
            try
            {
                await _accordoService.RecediAsync(User.GetUserId(), id, dataRecesso, note, cancellationToken);
                TempData.SendNotification(NotificationType.Success, "Recesso salvato!");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (LavoroAgileException laex)
            {
                TempData.SendNotification(NotificationType.Error, laex.Message);
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Errore nel recesso di un accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore durante il recesso dell'accordo.");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
        }

        /// <summary>
        /// Recesso dell'accordo per giustificato motivo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo da recedere</param>
        /// <param name="role">Ruolo con cui sta operando l'utente.</param>
        /// <param name="dataRecesso">Data di recesso</param>
        /// <param name="note">Note</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Pagina di dettaglio dell'accordo.</returns>
        /// <remarks>Questo endoint può essere invocato solo a responsabile accordo e utente.</remarks>
        [Authorize(policy: "AccordiRecediSottoscrivi")]
        public async Task<ActionResult> RecediGiustificatoMotivo(Guid id, string role, DateTime dataRecessoGiustificatoMotivo, string note, string giustificatoMotivo, CancellationToken cancellationToken)
        {
            try
            {
                if (giustificatoMotivo.ToUpper().Equals("Personalizzato")) 
                {
                    Accordo accordo = await _accordoService.GetAccordoAsync(id, cancellationToken);
                    giustificatoMotivo = accordo.GiustificatoMotivoDiRecesso;
                }

                await _accordoService.RecediGiustificatoMotivoAsync(User.GetUserId(), id, dataRecessoGiustificatoMotivo, note, giustificatoMotivo, cancellationToken);
                TempData.SendNotification(NotificationType.Success, "Recesso salvato!");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (LavoroAgileException laex)
            {
                TempData.SendNotification(NotificationType.Error, laex.Message);
                return RedirectToAction("Details", "Accordi", new { id = id, role = role });                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Errore nel recesso di un accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore durante il recesso dell'accordo.");
                return RedirectToAction("Details", "Accordi", new { id = id, role = role });
            }
        }

        /// <summary>
        /// Approva la deroga dell'accordo, funzionalità riservata alla Segr. Tecnica
        /// </summary>
        /// <param name="id">Identificativo dell'accordo.</param>
        /// <param name="role">Ruolo con cui sta operando l'utente.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(policy: "PCM_InternalOperativeRole_Utente")]
        public async Task<ActionResult> AprovaDeroga(Guid id, string role, CancellationToken cancellationToken)
        {
            try
            {
                await _workflowService.StartWorkflowAsync(WorkflowNames.ApprovazioneLavoroAgileRidotto, id, true, cancellationToken);
                
                TempData.SendNotification(NotificationType.Success, "Deroga approvata correttamente!");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore nell'apporvazione della deroga dell'accordo con id:" + id, ex.Message);
                TempData.SendNotification(NotificationType.Error, "Errore nell'apporvazione della deroga.");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
        }

        /// <summary>
        /// Rifiuta la deroga dell'accordo, funzionalità riservata alla Segr. Tecnica
        /// </summary>
        /// <param name="id">Identificativo dell'accordo.</param>
        /// <param name="role">Ruolo con cui sta operando l'utente.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(policy: "PCM_InternalOperativeRole_Utente")]
        public async Task<ActionResult> RifiutaDeroga(Guid id, string role, CancellationToken cancellationToken)
        {
            try
            {
                await _accordoService.RifiutaDeroga(User.GetUserId(), id, cancellationToken);

                TempData.SendNotification(NotificationType.Success, "Deroga rifiutata correttamente!");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore nel rifiuto della deroga dell'accordo con id:" + id, ex.Message);
                TempData.SendNotification(NotificationType.Error, "Errore nel rifiuto della deroga.");
                return RedirectToAction("Index", "Accordi", new { role = role });
            }
        }

    }
}
