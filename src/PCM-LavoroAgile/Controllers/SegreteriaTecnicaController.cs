using AutoMapper;
using Domain;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SegreteriaTecnicaController : Controller
    {
        private readonly ILogger<SegreteriaTecnicaController> _logger;
        private readonly ISegreteriaTecnicaRepository<SegreteriaTecnica, Guid> _repositorySegreteriaTecnica;
        private readonly ISegreteriaTecnicaService _segreteriaTecnicaService;
        private readonly IMapper _mapper;

        public SegreteriaTecnicaController(ILogger<SegreteriaTecnicaController> logger, ISegreteriaTecnicaRepository<SegreteriaTecnica, Guid> repositorySegreteriaTecnica, ISegreteriaTecnicaService segreteriaTecnicaService, IMapper mapper)
        {
            _repositorySegreteriaTecnica = repositorySegreteriaTecnica;
            _segreteriaTecnicaService = segreteriaTecnicaService;
            _logger = logger;
            _mapper = mapper;
        }

        [ResponseCache(NoStore = true, VaryByHeader = "None", Duration = 0)]
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            SegreteriaTecnicaViewModel segreteriaTecnicaViewModel= new SegreteriaTecnicaViewModel();

            ViewData.Add("UtentiSegreteriaTecnica", (await _repositorySegreteriaTecnica.FindAsync(cancellationToken: cancellationToken)).Entities.Select(s => _mapper.Map<SegreteriaTecnicaViewModel>(s)).ToList());

            return View(segreteriaTecnicaViewModel);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SegreteriaTecnicaViewModel segreteriaTecnicaViewModel, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SegreteriaTecnica segreteriaTecnica = _mapper.Map<SegreteriaTecnicaViewModel, SegreteriaTecnica>(segreteriaTecnicaViewModel, opts => opts.BeforeMap((vm, m) => { vm.Author = User.Identity.Name; vm.CreationDate = vm.CreationDate = DateTime.UtcNow; vm.EditTime = DateTime.UtcNow; }));

                    await _segreteriaTecnicaService.CreateSegreteriaTecnicaAsync(segreteriaTecnica, cancellationToken);

                    TempData.SendNotification(NotificationType.Success, "Uetente salvato correttamente!");
                    return RedirectToAction(nameof(Index));
                }
                catch (LavoroAgileException lae)
                {
                    TempData.SendNotification(NotificationType.Error, lae.Message);
                    ViewData.Add("UtentiSegreteriaTecnica", (await _repositorySegreteriaTecnica.FindAsync(cancellationToken: cancellationToken)).Entities.Select(s => _mapper.Map<SegreteriaTecnicaViewModel>(s)).ToList());
                    return View("Index", segreteriaTecnicaViewModel);
                }
                catch (Exception)
                {
                    TempData.SendNotification(NotificationType.Error, "Si è verificato un errore non identificato durante il salvataggio");
                    ViewData.Add("UtentiSegreteriaTecnica",(await _repositorySegreteriaTecnica.FindAsync(cancellationToken: cancellationToken)).Entities.Select(s => _mapper.Map<SegreteriaTecnicaViewModel>(s)).ToList());
                    return View("Index", segreteriaTecnicaViewModel);
                }
            }
            else
            {
                TempData.SendNotification(NotificationType.Error, "Campi obbligatori da compilare");
                ViewData.Add("UtentiSegreteriaTecnica", (await _repositorySegreteriaTecnica.FindAsync(cancellationToken: cancellationToken)).Entities.Select(s => _mapper.Map<SegreteriaTecnicaViewModel>(s)).ToList());
                return View("Index", segreteriaTecnicaViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _repositorySegreteriaTecnica.DeleteAsync(id, cancellationToken);

                TempData.SendNotification(NotificationType.Success, "Utente eliminato correttamente!");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}
