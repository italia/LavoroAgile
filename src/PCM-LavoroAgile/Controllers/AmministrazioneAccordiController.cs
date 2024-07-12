using AutoMapper;
using Domain;
using Domain.Model;
using Domain.Model.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models;
using ReflectionIT.Mvc.Paging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    public class AmministrazioneAccordiController : Controller
    {
        private readonly ILogger<AccordiController> _logger;
        private readonly IRepository<Accordo, Guid> _repository;
        private readonly IAccordoService _accordoService;
        private readonly IMapper _mapper;

        public AmministrazioneAccordiController(ILogger<AccordiController> logger, IRepository<Accordo, Guid> repository, IAccordoService accordoService, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _accordoService = accordoService ?? throw new ArgumentNullException(nameof(accordoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [ResponseCache(NoStore = true, VaryByHeader = "None", Duration = 0)]
        public async Task<ActionResult> Index(string filter, CancellationToken cancellationToken)
        {
            AccordoViewModel accordoViewModel = new AccordoViewModel();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                AccordoSearch searchViewModel = new AccordoSearch();
                searchViewModel.Page = 1;
                searchViewModel.Codice = filter;
                var searchResults = await _accordoService.FindAsync(User.GetUserId(), RoleAndKeysClaimEnum.KEY_CLAIM_SEGRETERIA_TECNICA.ToDescriptionString(), _mapper.Map<AccordoSearch>(searchViewModel), cancellationToken);
                if (searchResults.Entities.Count == 1)
                {
                    Accordo accordo = await _repository.GetAsync(searchResults.Entities[0].Id, cancellationToken);
                    accordoViewModel = _mapper.Map<AccordoViewModel>(accordo);
                }
                else
                {
                    TempData.SendNotification(NotificationType.Error, "Nessun accordo trovato!");
                }                
            }
            
            return View(accordoViewModel);            
        }

        public async Task<ActionResult> ConfermaOperazione(Guid id, OperazioniAmmAccordo operazione, string codice, CancellationToken cancellationToken)
        {
            if (operazione == 0)
            {
                    TempData.SendNotification(NotificationType.Error, "Selezionare una operazione!");
                    return RedirectToAction(nameof(Index), new { filter = codice });
            }

            try
            {
                await _accordoService.ExecuteAmmOperation(id, operazione, cancellationToken);
            }
            catch (LavoroAgileException lae)
            {
                TempData.SendNotification(NotificationType.Error, lae.Message);
                return RedirectToAction(nameof(Index), new { filter = codice });
            }
            catch (Exception ex)
            {
                _logger.LogError("Operazioni Amminsitrazione Accordo", ex);
                TempData.SendNotification(NotificationType.Error, "Problemi durante l'operazione richiesta.");
                return RedirectToAction(nameof(Index), new { filter = codice });
            }

            TempData.SendNotification(NotificationType.Success, "Operazione effettuata con successo!");
            return RedirectToAction(nameof(Index), new { filter = codice });
        }
    }
}
