using AutoMapper;
using Domain.Model;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StrutturaController : Controller
    {
        private readonly ILogger<StrutturaController> _logger;
        private readonly IStrutturaService _strutturaService;
        private readonly IMapper _mapper;

        public StrutturaController(ILogger<StrutturaController> logger,
                IStrutturaService strutturaService,
                IMapper mapper)
        {
            _strutturaService = strutturaService;
            _logger = logger;
            _mapper = mapper;
        }

        [ResponseCache(NoStore = true, VaryByHeader = "None", Duration = 0)]
        public async Task<ActionResult> Index(string filter, CancellationToken cancellationToken, int pageIndex = 1)
        {
            IOrderedQueryable<StrutturaViewModel> listaStrutture = (await _strutturaService.FindStrutturaAsync(cancellationToken: cancellationToken)).Entities.Select(s => _mapper.Map<StrutturaViewModel>(s)).AsQueryable().OrderBy(s => s.StrutturaCompleta);

            if (!string.IsNullOrWhiteSpace(filter))
                listaStrutture = listaStrutture.Where(s => s.StrutturaCompleta.Contains(filter, StringComparison.InvariantCultureIgnoreCase)).OrderBy(s => s.StrutturaCompleta);
            
            var model = PagingList.Create<StrutturaViewModel>(listaStrutture, 10, pageIndex);
            model.RouteValue = new RouteValueDictionary { { "filter", filter} };

            return View(model);            
        }

        public async Task<IActionResult> AutocompleteLivelloStruttura(string term, string livello, CancellationToken cancellationToken)
        {
            try
            {
                List<string> searchResult = new List<string>();

                switch (livello)
                {
                    case "StrutturaLiv1":
                        searchResult = (await _strutturaService.FindStrutturaAsync(s => s.StrutturaLiv1 != null && s.StrutturaLiv1.ToUpper().Contains(term.ToUpper()), cancellationToken: cancellationToken))
                            .Entities.Select(s => s.StrutturaLiv1).Distinct().ToList();
                        break;

                    case "StrutturaLiv2":
                        searchResult = (await _strutturaService.FindStrutturaAsync(s => s.StrutturaLiv2 != null && s.StrutturaLiv2.ToUpper().Contains(term.ToUpper()), cancellationToken: cancellationToken))
                            .Entities.Select(s => s.StrutturaLiv2).Distinct().ToList();
                        break;

                    case "StrutturaLiv3":
                            searchResult = (await _strutturaService.FindStrutturaAsync(s => s.StrutturaLiv3 != null && s.StrutturaLiv3.ToUpper().Contains(term.ToUpper()), cancellationToken: cancellationToken))
                            .Entities.Select(s => s.StrutturaLiv3).Distinct().ToList();
                        break;
                }                

                return Json(searchResult);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                StrutturaViewModel strutturaViewModel = _mapper.Map<Struttura, StrutturaViewModel>(await _strutturaService.InitCreateStrutturaAsync());
                return View(strutturaViewModel);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StrutturaViewModel strutturaViewModel, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Struttura struttura = _mapper.Map<StrutturaViewModel, Struttura>(strutturaViewModel, opts => opts.BeforeMap((vm, m) => { vm.Author = User.Identity.Name; vm.CreationDate = DateTime.UtcNow; vm.EditTime = DateTime.UtcNow; vm.DirigenteResponsabile = vm.ResponsabileAccordo; }));

                    await _strutturaService.InsertStrutturaAsync(struttura, cancellationToken);

                    TempData.SendNotification(NotificationType.Success, "Struttura salvata correttamente!");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData.SendNotification(NotificationType.Error, "Si è verificato un errore non identificato durante il salvataggio");
                    return View(strutturaViewModel);
                }
            }
            else
            {
                TempData.SendNotification(NotificationType.Error, "Campi obbligatori da compilare");
                return View("Create", strutturaViewModel);
            }    
        }

        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                StrutturaViewModel strutturaViewModel = (_mapper.Map<Struttura, StrutturaViewModel>(await _strutturaService.GetStrutturaAsync(id, cancellationToken)));
                return View(strutturaViewModel);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, [Bind] StrutturaViewModel strutturaViewModel, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Struttura struttura = _mapper.Map<StrutturaViewModel, Struttura>(strutturaViewModel, opts => opts.BeforeMap((vm, m) => { vm.Author = User.Identity.Name; vm.CreationDate = DateTime.UtcNow; vm.EditTime = DateTime.UtcNow; }));

                    await _strutturaService.UpdateStrutturaAsync(struttura, cancellationToken);

                    TempData.SendNotification(NotificationType.Success, "Struttura salvata correttamente!");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData.SendNotification(NotificationType.Error, "Si è verificato un errore non identificato durante il salvataggio delle modifiche");
                    return View("Details", strutturaViewModel);
                }
            }
            else
            {
                TempData.SendNotification(NotificationType.Error, "Campi obbligatori da compilare");
                return View("Details", strutturaViewModel);
            }
        }

        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                return View(_mapper.Map<StrutturaViewModel>(await _strutturaService.GetStrutturaAsync(id, cancellationToken)));
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePost(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _strutturaService.DeleteStrutturaAsync(id, cancellationToken);

                TempData.SendNotification(NotificationType.Success, "Struttura eliminata correttamente!");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}
