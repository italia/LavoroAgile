using AutoMapper;
using Domain.Model;
using Domain.Model.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccordoService _accordoService;
        private readonly IMapper _mapper;
        private readonly IRepository<Accordo, Guid> _repositoryAccordo;

        public HomeController(ILogger<HomeController> logger, IAccordoService accordoService, IMapper mapper, IRepository<Accordo, Guid> repositoryAccordo)
        {
            _logger = logger;
            _accordoService = accordoService;
            _mapper = mapper;
            _repositoryAccordo = repositoryAccordo;
        }

        //In questo caso la ricerca dell'eventuale accordo in corso viene fatta solo per il ruolo utente perchè questo è un ruolo
        //che sicuramente hanno tutti e che viene impostato di default al caricamento iniziale della pagina
        public async Task<ActionResult<AccordoHeaderViewModel>> Index(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Verifico se esiste un accordo in corso");

            // Verifica se l'utente ha un accordo Attivo (lo fa considerando come ruolo "UTENTE")
            var accordoAttivo = (await _repositoryAccordo.FindAsync(User.GetUserId(),
                                                                    a => DateTime.UtcNow.Date <= a.DataFineUtc.Date &&
                                                                    a.Stato != StatoAccordo.RifiutataRA &&
                                                                    a.Stato != StatoAccordo.RifiutataCI &&
                                                                    a.Stato != StatoAccordo.RifiutataCS &&
                                                                    a.Stato != StatoAccordo.AccordoConclusoCambioStrutturaDipendente, RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE)).Entities.FirstOrDefault();

            _logger.LogInformation($"Accordo individuato: { accordoAttivo?.Id.ToString() ?? string.Empty }");

            return View(_mapper.Map<AccordoHeaderViewModel>(accordoAttivo));
        }

        public async Task<ActionResult> GetToDoForRole(CancellationToken cancellationToken, string role)
        {
            //Accordi su cui l'utente con il ruolo selezionato ha delle attività da svolgere
            var toDo = await _accordoService.GetAccordiToDoAsync(User.GetUserId(), role, cancellationToken);
            
            return Json(toDo);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> GetToDoValutazioni(CancellationToken cancellationToken, string role)
        {
            //Accordi su cui il responsabile accordo deve fare delle valutazioni
            var toDo  =  await _accordoService.GetToDoValutazioniAsync(User.GetUserId(), role, cancellationToken);

            return Json(toDo);
        }

        public ActionResult Faq()
        {
            return View("Faq");
        }
    }
}
