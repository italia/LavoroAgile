using AutoMapper;
using Domain.Model;
using Domain.Model.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models;
using PCM_LavoroAgile.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.ExtensionMethods;
using Domain;
using ObjectsComparer;
using Domain.Model.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Net.Http.Headers;
using Domain.Settings;
using Infrastructure.Utils;

namespace PCM_LavoroAgile.Controllers
{
    /// <summary>
    /// Controller per la gestione degli accordi.
    /// </summary>
    [Authorize]
    public class AccordiController : Controller
    {
        private readonly ILogger<AccordiController> _logger;
        private readonly IRepository<Accordo, Guid> _repository;
        private readonly IAccordoService _accordoService;
        private readonly IMapper _mapper;
        private readonly IDocumentsService _documentsService;
        private readonly IStrutturaService _strutturaService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccordiController(ILogger<AccordiController> logger, 
            IRepository<Accordo, Guid> repository, 
            IAccordoService accordoService, 
            IMapper mapper, 
            IDocumentsService documentsService,
            IStrutturaService strutturaService,
            IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _accordoService = accordoService ?? throw new ArgumentNullException(nameof(accordoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _documentsService = documentsService ?? throw new ArgumentNullException(nameof(documentsService));
            _strutturaService = strutturaService ?? throw new ArgumentNullException(nameof(strutturaService));
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Restituisce la pagina con i rultati di ricerca.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IActionResult Index(CancellationToken cancellationToken)
        {
            SearchViewModel searchViewModel = TempData.Get<SearchViewModel>("searchViewModel");
            if (searchViewModel != null)
            {
                ViewData["searchViewModel"] = JsonConvert.SerializeObject(searchViewModel);
                return View(searchViewModel);
            }
            else
            {
                return View(new SearchViewModel());
            }
        }

        /// <summary>
        /// Restituisce una pagina di risultati di ricerca.
        /// </summary>
        /// <param name="searchViewModel">Modello con i parametri di ricerca.</param>
        /// <returns>PartialView con la pagina richiesta.</returns>
        /// <remarks>Questa action viene invocata via Javascript dalla view Index</remarks>
        [Authorize(Policy = "AccordiSearch")]
        public async Task<ActionResult> GetSearchResultPage(SearchViewModel searchViewModel, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Se il ruolo in parametro non è presente fra quelli dell'utente, solleva eccezione.
                    if (!User.HasClaim(c => c.Type.Equals(searchViewModel.Role, StringComparison.OrdinalIgnoreCase)))
                    {
                        return Forbid();
                    }

                    TempData.Put<SearchViewModel>("searchViewModel", searchViewModel);

                    var searchResults = await _accordoService.FindAsync(User.GetUserId(), searchViewModel.Role, _mapper.Map<AccordoSearch>(searchViewModel), cancellationToken);
                    return PartialView("_SearchResults", new SearchResultCollectionViewModel(
                        searchResults.TotalElements,
                        searchResults.Entities?.Select(a => _mapper.Map<Accordo, AccordoHeaderViewModel>(a, options => options.AfterMap((a, wm) =>
                        {
                            //Se non è una revisione accordo allora si verifica se è un rinnovo
                            if (!a.RevisioneAccordo && a.ValutazioneEsitiAccordoPrecedente != null)
                            {
                                // E' un rinnovo se è valorizzato l'identificativo del padre.
                                wm.IsRinnovo = !Guid.Empty.Equals(a.ParentId);

                                // E' rinnovabile se è un rinnovo, se la data di oggi è antecedente di almeno due settimane 
                                // a quella di scadenza del contratto e se la valutazione c'è ed è positiva
                                wm.Rinnovabile = wm.IsRinnovo && a.ValutazioneEsitiAccordoPrecedente.Equals("Positiva", StringComparison.OrdinalIgnoreCase) && DateTime.UtcNow.Date > a.DataFineAccordoPrecedente.Value.AddDays(-14).Date;
                            }
                        }))).ToList(), role: searchViewModel.Role));
                }

                if (string.IsNullOrWhiteSpace(searchViewModel?.Role))
                {
                    _logger.LogError("Ricerca accordi: ruolo non specificato");
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ricerca accordi", ex);
                throw new Exception("Errore durante la ricerca");
            }

        }

        /// <summary>
        /// Mostra la pagina di creazione di un accordo
        /// </summary>
        /// <returns></returns>
        /// <remarks>Accesso consentito solo all'utente</remarks>
        [Authorize(policy: "PCM_InternalOperativeRole_Utente")]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var accordoViewModel = _mapper.Map<Accordo, AccordoViewModel>(await _accordoService.InitCreateAccordoAsync(User.GetUserId(), cancellationToken));

            // Automapper popola le date con 01/01/0001, quindi resetta data inizio in modo da evitarne la visualizzazione nella pagina.
            accordoViewModel.DataInizioUtc = null;
            accordoViewModel.DataFineUtc = null;

            await this.GetStruttureForSelect(cancellationToken);

            // Mappa l'accordo al suo view model e lo invia al chiamante.
            return View(accordoViewModel);

        }

        /// <summary>
        /// Crea un accordo.
        /// </summary>
        /// <param name="role">Ruolo utente</param>
        /// <param name="accordoViewModel">View model dell'accordo da creare</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>Accesso consentito solo all'utente.</remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(policy: "PCM_InternalOperativeRole_Utente")]
        public async Task<IActionResult> Create(string role, AccordoViewModel accordoViewModel, CancellationToken cancellationToken)
        {
            try
            {
                await this.GetStruttureForSelect(cancellationToken);

                if (ModelState.IsValid)
                {
                    // L'id utente viene comunque ripreso dalla sessione utente
                    Accordo accordo = _mapper.Map<AccordoViewModel, Accordo>(accordoViewModel, opts => opts.BeforeMap((vm, m) => { vm.Dipendente.Id = User.GetUserId(); vm.CreationDate = DateTime.UtcNow; vm.EditTime = DateTime.UtcNow; }));

                    await _accordoService.CreateAccordoAsync(accordo, cancellationToken);

                    TempData.SendNotification(NotificationType.Success, "Accordo salvato correttamente!");

                    return RedirectToAction(nameof(Index), new SearchViewModel { Role = role });
                }
                else
                {
                    TempData.SendNotification(NotificationType.Error, "Campi obbligatori da compilare");
                    return View(nameof(Create), accordoViewModel);
                }
            }
            catch (LavoroAgileException lae)
            {
                TempData.SendNotification(NotificationType.Error, lae.Message);
                return View(nameof(Create), accordoViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Crea accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore nel salvataggio dell'accordo.");
                return View(nameof(Create), accordoViewModel);
            }
        }

        /// <summary>
        /// Mostra il dettaglio di un accordo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo</param>
        /// <returns></returns>
        public async Task<ActionResult<AccordoViewModel>> Details(Guid id, string role, CancellationToken cancellationToken)
        {
            AccordoViewModel accordoViewModel = _mapper.Map<AccordoViewModel>(await _accordoService.GetAccordoAsync(id, cancellationToken));
            await this.GetStruttureForSelect(cancellationToken);

            return View(accordoViewModel);
        }

        /// <summary>
        /// Aggiorna il dettaglio di un accordo, nel caso il parametro trasmetti sia treu, effettua anche la tramissione dell'accordo
        /// </summary>
        /// <param name="id">Identificativo dell'accordo da aggiornare.</param>
        /// <param name="role">Ruolo con effettuare il salvataggio delle modifiche</param>
        /// <param name="accordoViewModel">View model dell'accordo da aggiornare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, string role, AccordoViewModel accordoViewModel, bool trasmetti=false, bool deroga=false, CancellationToken cancellationToken=default)
        {
            try
            {
                await this.GetStruttureForSelect(cancellationToken);

                if (ModelState.IsValid)
                {
                    Accordo accordo = _mapper.Map<AccordoViewModel, Accordo>(accordoViewModel, opts => opts.BeforeMap((vm, m) => { vm.EditTime = DateTime.UtcNow; }));

                    await _accordoService.UpdateAccordoAsync(accordo, cancellationToken);

                    TempData.SendNotification(NotificationType.Success, "Accordo salvato correttamente!");

                    //Verifico se effettuare una trasmissione che deve avviare il WF
                    if (trasmetti && !deroga)
                        return RedirectToAction("Bozza", "Approvals", new { id = id, role = role });

                    //Verifico se non effettuare una trasmissione che avvia il WF ed impostare lo stato di valutazione della Segr. Tecnica
                    if (trasmetti && deroga)
                    {
                        accordo.Stato = StatoAccordo.InAttesaValutazioneSegreteriaTecnica;
                        await _accordoService.UpdateAccordoAsync(accordo, cancellationToken);
                        TempData.SendNotification(NotificationType.Success, "Accordo in valutazione da parte della Segr. Tecnica!");
                        return RedirectToAction("Index", "Accordi", new { role = role });
                    }

                    return RedirectToAction(nameof(Details), new { Id = id, Role = role });
                }
                else
                {
                    TempData.SendNotification(NotificationType.Error, "Campi obbligatori da compilare");
                    return View(nameof(Details), accordoViewModel);
                }                
            }
            catch (LavoroAgileException lae)
            {
                TempData.SendNotification(NotificationType.Error, lae.Message);
                return View(nameof(Details), accordoViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Aggiorna accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore nel salvataggio dell'accordo.");
                return View(nameof(Details), accordoViewModel);
            }
        }

        /// <summary>
        /// Metodo che restituisce un FileStreamResult costituito da un pdf con le caratteristiche dell'accordo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GeneratePdf(Guid id, CancellationToken cancellationToken)
        {
            return await _documentsService.GeneratePdfAsync(id, cancellationToken);
        }

        /// <summary>
        /// Valorizza una variabile nel ViewData contenente la lista delle strutture, utile a valorizzare la select delle stesse nella vista
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> GetStruttureForSelect(CancellationToken cancellationToken)
        {
            var strutture = await _strutturaService.FindStrutturaAsync(cancellationToken: cancellationToken);
            ViewData["strutture"] = strutture?.Entities.Select(s => _mapper.Map<StrutturaViewModel>(s));

            return true;
        }

        [HttpGet]
        public async Task<JsonResult> GetStrutturaSelected(CancellationToken cancellationToken, string idStruttura)
        {
            var struttura = await _strutturaService.GetStrutturaAsync(Guid.Parse(idStruttura), cancellationToken);
            return Json(struttura);
        }

        [HttpPost]
        public ActionResult AddAttivita(AttivitaAccordoViewModel attivitaViewModel)
        {
            return PartialView("_AttivitaAccordoPartial", attivitaViewModel);
        }

        public async Task<ActionResult> Delete(Guid id, string role, CancellationToken cancellationToken)
        {
            try
            {
                return View(_mapper.Map<AccordoHeaderViewModel>(await _repository.GetAsync(id, cancellationToken)));
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePost(Guid id, string role, CancellationToken cancellationToken)
        {
            try
            {
                await _accordoService.DeleteAccordoAsync(id, cancellationToken);

                TempData.SendNotification(NotificationType.Success, "Accordo eliminato correttamente!");
                return RedirectToAction(nameof(Index), new SearchViewModel { Role = role });
            }
            catch (LavoroAgileException laex)
            {
                TempData.SendNotification(NotificationType.Error, laex.Message);
                return RedirectToAction(nameof(Details), new { Id = id, Role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError("Eliminazione accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore generale");
                return RedirectToAction(nameof(Details), new { Id = id, Role = role });
            }
        }


        /// <summary>
        /// Rinnova un accordo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo da rinnovare.</param>
        /// <param name="role">Ruolo con cui sta operando l'utente</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// Questo endpoint può essere acceduto solo da utenti nel ruolo utente.
        /// </remarks>
        [HttpGet]
        [Authorize(policy: "PCM_InternalOperativeRole_Utente")]
        public async Task<ActionResult> Rinnova(Guid id, string role, bool revisioneAccordo, CancellationToken cancellationToken)
        {
            try
            {
                var newId = await _accordoService.RinnovaAccordoAsync(id, revisioneAccordo, cancellationToken);

                TempData.SendNotification(NotificationType.Success, revisioneAccordo ? "Accordo pronto per essere revisionato!" : "Accordo pronto per essere rinnovato!");
                return RedirectToAction(nameof(Details), new { Id = newId, Role = role });
            }
            catch (LavoroAgileException laex)
            {
                TempData.SendNotification(NotificationType.Error, laex.Message);
                return RedirectToAction(nameof(Details), new { Id = id, Role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError("Rinnovo accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore generale");
                return RedirectToAction(nameof(Details), new { Id = id, Role = role });
            }
        }

        /// <summary>
        /// Restituisce la differenza fra due accordi.
        /// </summary>
        /// <param name="accordo1">Accordo 1</param>
        /// <param name="accordo2">Accordo 2</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lista delle differenze fra gli accordi.</returns>
        public async Task<ObjectResult> GetAccordoDiff(Guid accordo1, Guid accordo2, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _accordoService.GetDifferenzeAccordiAsync(accordo1, accordo2, cancellationToken));
            }
            catch (LavoroAgileException laex)
            {
                return BadRequest(laex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(GetAccordoDiff), ex);
                return BadRequest("Errore generico");
            }
        }

        /// <summary>
        /// Metodo che restituisce un FileStreamResult costituito da un pdf con la valutazione dell'accordo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GeneratePdfValutazione(Guid id, CancellationToken cancellationToken)
        {
            return await _documentsService.GeneratePdfValutazioneAsync(id, cancellationToken);
        }
    }
}
