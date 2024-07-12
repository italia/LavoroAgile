using AutoMapper;
using Domain.Model;
using Domain.Model.Identity;
using Domain.Model.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

namespace PCM_LavoroAgile.Controllers
{
    /// <summary>
    /// Controller per la gestione delle valutazioni, ricalca accordiController.
    /// </summary>
    [Authorize]
    public class ValutazioniController : Controller
    {
        private readonly ILogger<ValutazioniController> _logger;
        private readonly IRepository<Accordo, Guid> _repository;
        private readonly IAccordoService _accordoService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        
        public ValutazioniController(UserManager<AppUser> userManager,
            ILogger<ValutazioniController> logger, 
            IRepository<Accordo, Guid> repository, 
            IAccordoService accordoService, 
            IMapper mapper)
        {
            _repository = repository;
            _accordoService = accordoService;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;            
        }

        /// <summary>
        /// Restituisce la pagina con i risuultati di ricerca degli accordi in attesa di valutazione.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IActionResult Index(CancellationToken cancellationToken)
        {
            return View(new SearchViewModel());
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

                    //bugFixSimone ricerca solo tra gli accordi che sono da valutare e forza lo stato in corso
                    searchViewModel.daValutare = true;
                    //searchViewModel.Stati.Add(StatoAccordo.InCorso);
                    var searchResults = await _accordoService.FindAsync(User.GetUserId(), searchViewModel.Role, _mapper.Map<AccordoSearch>(searchViewModel), cancellationToken);
                    return PartialView("_SearchResults", new SearchResultCollectionViewModel(searchResults.TotalElements, searchResults.Entities?.Select(a => _mapper.Map<AccordoHeaderViewModel>(a)).ToList(), role: searchViewModel.Role));
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
        /// Mostra il dettaglio di un accordo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo</param>
        /// <returns></returns>
        public async Task<ActionResult<AccordoViewModel>> Details(Guid id, string role, CancellationToken cancellationToken)
        {
            AccordoViewModel accordoViewModel = _mapper.Map<AccordoViewModel>(await _repository.GetAsync(id, cancellationToken));
            return View(accordoViewModel);
        }

        /// <summary>
        /// Aggiorna le valutazioni delle attivita' di un accordo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo da aggiornare.</param>
        /// <param name="role">Ruolo con effettuare il salvataggio delle modifiche</param>
        /// <param name="accordoViewModel">View model dell'accordo da aggiornare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, string role, AccordoViewModel accordoViewModel, CancellationToken cancellationToken)
        {
            try
            {
                //Verifico se sono stati inseriti tutti i target
                if (!TargetIsValid(accordoViewModel))
                {
                    TempData.SendNotification(NotificationType.Error, "I target raggiunti sono campi obbligatori.");
                    return View(nameof(Details), accordoViewModel);
                }

                //Aggiorno l'accordo con i dati scritti dal dipendente 
                Accordo accordo = await _repository.GetAsync(accordoViewModel.Id, cancellationToken);
                accordo.NotaDipendente = accordoViewModel.NotaDipendente;
                accordo.DataNotaDipendente = DateTime.UtcNow;
                //Aggiorno i target raggiunti delle attivita' dell'accordo
                accordo.ListaAttivita = accordo.ListaAttivita.Join(accordoViewModel.ListaAttivita,
                    la => la.Index,
                    mo => mo.Index,
                    (la, mo) => new AttivitaAccordo
                    {
                        Id = la.Id,
                        AccordoId = la.AccordoId,
                        Index = la.Index,
                        Attivita = la.Attivita,
                        Risultati = la.Risultati,
                        DenominazioneIndicatore = la.DenominazioneIndicatore,
                        TipologiaIndicatore = la.TipologiaIndicatore,
                        OperatoreLogicoIndicatoreTesto = la.OperatoreLogicoIndicatoreTesto,
                        TestoTarget = la.TestoTarget,
                        TestoTargetRaggiunto = mo.TestoTargetRaggiunto,
                        OperatoreLogicoIndicatoreNumeroAssoluto = la.OperatoreLogicoIndicatoreNumeroAssoluto,
                        NumeroAssolutoTarget = la.NumeroAssolutoTarget,
                        NumeroAssolutoRaggiunto = mo.NumeroAssolutoRaggiunto,
                        NumeroAssolutoDaTarget = la.NumeroAssolutoDaTarget,
                        NumeroAssolutoATarget = la.NumeroAssolutoATarget,
                        PercentualeIndicatoreDenominazioneNumeratore = la.PercentualeIndicatoreDenominazioneNumeratore,
                        PercentualeIndicatoreDenominazioneDenominatore = la.PercentualeIndicatoreDenominazioneDenominatore,
                        OperatoreLogicoIndicatorePercentuale = la.OperatoreLogicoIndicatorePercentuale,
                        PercentualeTarget = la.PercentualeTarget,
                        PercentualeDaTarget = la.PercentualeDaTarget,
                        PercentualeATarget = la.PercentualeATarget,
                        OperatoreLogicoIndicatoreData = la.OperatoreLogicoIndicatoreData,
                        DataTarget = la.DataTarget,
                        DataTargetRaggiunto = mo.DataTargetRaggiunto,
                        DataDaTarget = la.DataDaTarget,
                        DataATarget = la.DataATarget,
                        PercentualeDenominatoreTargetRaggiunto = mo.PercentualeDenominatoreTargetRaggiunto,
                        PercentualeNumeratoreTargetRaggiunto = mo.PercentualeNumeratoreTargetRaggiunto
                    }).ToList();
                await _repository.UpdateAsync(accordo, cancellationToken);

                TempData.SendNotification(NotificationType.Success, "Richiesta di Valutazione salvata correttamente!");
                return RedirectToAction("Details", "Accordi", new { role = role, id = id });                
            }
            catch (Exception ex)
            {
                _logger.LogError("Aggiorna accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore nel salvataggio dell'accordo.");
                return RedirectToAction("Details", "Accordi", new { role = role, id = id });
            }
        }

        public async Task<ActionResult<AccordoViewModel>> Valutation(Guid id, string role, CancellationToken cancellationToken)
        {
            AccordoViewModel accordoViewModel = _mapper.Map<AccordoViewModel>(await _repository.GetAsync(id, cancellationToken));
            return View(accordoViewModel);
        }
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Save(Guid id, string role, AccordoViewModel accordoViewModel, CancellationToken cancellationToken)
        {
            try
            {
                //Verifico che sia stata selezionata una opzione di valutazione
                if(!accordoViewModel.checkValutazionePositiva && !accordoViewModel.checkValutazioneNegativa)
                {
                    TempData.SendNotification(NotificationType.Error, "E' obbligatorio selezionare una valutazione.");
                    return RedirectToAction("Valutation", "Valutazioni", new { role = role, id = id });
                }
              
                //Aggiorno l'accordo con i dati scritti dal dipendente 
                Accordo accordo = await _repository.GetAsync(accordoViewModel.Id, cancellationToken);
                accordo.NotaDirigente = accordoViewModel.NotaDirigente;
                accordo.DataNotaDirigente = DateTime.UtcNow;
                
                if (accordoViewModel.checkValutazionePositiva)
                    accordo.isValutazionePositiva = true;
                if (accordoViewModel.checkValutazioneNegativa)
                    accordo.isValutazionePositiva = false;

                AppUser user = await _userManager.GetUserAsync(User);
                user.CannotCreateAccordo = !accordo.isValutazionePositiva;
                await _repository.UpdateAsync(accordo, cancellationToken);
                await _userManager.UpdateAsync(user);

                TempData.SendNotification(NotificationType.Success, "Valutazione salvata correttamente!");
                return RedirectToAction("index", "Valutazioni", new { role = role, id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError("Aggiorna accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Errore nel salvataggio dell'accordo.");
                return RedirectToAction("Details", "Accordi", new { role = role, id = id });
            }
        }


        [HttpPost]
        public ActionResult AddAttivita(AttivitaAccordoViewModel attivitaViewModel)
        {
            return PartialView("_AttivitaAccordoPartial", attivitaViewModel);
        }

        private bool TargetIsValid(AccordoViewModel acccordoViewModel)
        {
            bool result = true;

            foreach(AttivitaAccordoViewModel attivita in acccordoViewModel.ListaAttivita)
            {
                switch (attivita.TipologiaIndicatore.ToUpper())
                {
                    case "TESTO":
                        if (String.IsNullOrEmpty(attivita.TestoTargetRaggiunto))
                            result = false;
                        break;
                    case "NUMEROASSOLUTO":
                        if (String.IsNullOrEmpty(attivita.NumeroAssolutoRaggiunto))
                            result = false;
                        break;
                    case "PERCENTUALE":
                        if (String.IsNullOrEmpty(attivita.PercentualeNumeratoreTargetRaggiunto) || String.IsNullOrEmpty(attivita.PercentualeDenominatoreTargetRaggiunto))
                            result = false;
                        break;
                    case "DATA":
                        if (!attivita.DataTargetRaggiunto.HasValue)
                            result = false;
                        break;
                }
            }

            return result;
        }

    }

}
