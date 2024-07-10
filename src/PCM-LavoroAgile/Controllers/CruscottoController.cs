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



namespace PCM_LavoroAgile.Controllers
{
    /// <summary>
    /// Controller per la gestione degli accordi.
    /// </summary>
    [Authorize]
    public class CruscottoController : Controller
    {
        private readonly ILogger<CruscottoController> _logger;
        //private readonly IRepository<Accordo, Guid> _repository;
        private readonly IStrutturaService _strutturaService;
        private readonly IMapper _mapper;
        private readonly IAccordoService _accordoService;

        public CruscottoController(ILogger<CruscottoController> logger, 
            //IRepository<Accordo, Guid> repository,
            IStrutturaService strutturaService,
            IMapper mapper,
            IAccordoService accordoService)
        {
            //_repository = repository;
            _strutturaService = strutturaService;
            _logger = logger;
            _mapper = mapper;
            _accordoService = accordoService;
        }

        /// <summary>
        /// Restituisce la pagina con i rultati di ricerca.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string role, CancellationToken cancellationToken)
        {
            CruscottoViewModel myModel = new CruscottoViewModel();
            myModel.MenuStrutture = await SearchStruttureFromUser(role,cancellationToken);
            return View(myModel);
        }

        public async Task<List<Struttura>> SearchStruttureFromUser(string role, CancellationToken cancellationToken)
        {
            try
            {
                List<Struttura> searchResult = new List<Struttura>();

                string id = (User.Identity.Name.Split('@')[0] + "@").ToLower().Trim();

                if (role == RoleAndKeysClaimEnum.KEY_CLAIM_CAPO_STRUTTURA.ToDescriptionString())
                {
                    searchResult = (await _strutturaService.FindStrutturaAsync(s => s.CapoStruttura.Email.ToLower().Trim().StartsWith(id), cancellationToken: cancellationToken)).Entities.OrderBy(x => x.StrutturaCompleta).ToList();
                }
                if (role == RoleAndKeysClaimEnum.KEY_CLAIM_RESPONSABILE_ACCORDO.ToDescriptionString())
                {
                    searchResult = (await _strutturaService.FindStrutturaAsync(s => s.ResponsabileAccordo.Email.ToLower().Trim().StartsWith(id), cancellationToken: cancellationToken)).Entities.OrderBy(x => x.StrutturaCompleta).ToList();
                }
                if (role == RoleAndKeysClaimEnum.KEY_CLAIM_CAPO_INTERMEDIO.ToDescriptionString())
                {
                    searchResult = (await _strutturaService.FindStrutturaAsync(s => s.CapoIntermedio.Email.ToLower().Trim().StartsWith(id), cancellationToken: cancellationToken)).Entities.OrderBy(x => x.StrutturaCompleta).ToList();
                }
                if (role == RoleAndKeysClaimEnum.KEY_CLAIM_REFERENTE_INTERNO.ToDescriptionString())
                {
                    searchResult = (await _strutturaService.FindStrutturaAsync(s => s.ReferenteInterno.Email.ToLower().Trim().StartsWith(id), cancellationToken: cancellationToken)).Entities.OrderBy(x => x.StrutturaCompleta).ToList();
                }
                if (role == RoleAndKeysClaimEnum.KEY_CLAIM_SEGRETERIA_TECNICA.ToDescriptionString())
                {
                    searchResult = (await _strutturaService.FindStrutturaAsync(cancellationToken: cancellationToken)).Entities.OrderBy(x => x.StrutturaCompleta).ToList();
                }

                return searchResult;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetSearchStatPage(string role, Guid id, CancellationToken cancellationToken)
        {

            Struttura _struttura = (await _strutturaService.FindStrutturaAsync(x => x.Id == id, cancellationToken: cancellationToken)).Entities.FirstOrDefault();

            try
            {
                // Se il ruolo in parametro non è presente fra quelli dell'utente, solleva eccezione.
                if (!User.HasClaim(c => c.Type.Equals(role, StringComparison.OrdinalIgnoreCase)))
                {
                    return Forbid();
                }

                //bugfixSimone ricerca se metto come ruolo referenteinterno non mi trova nulla.
                //string ruolo = role.ToEnum<RoleAndKeysClaimEnum>();
                //if(ruolo== "PCM_InternalOperativeRole_ReferenteInterno") ruolo="PCM_InternalOperativeRole_CapoStruttura";

                //bugfixSimone2 Devo cercare tutti gli accordi delle strutture e relative sottostrutture.
                List<AccordoViewModel> searchResults = (await _accordoService.GetAccordiForRole(User.GetUserId(), role, cancellationToken)).Entities.Select(a => _mapper.Map<AccordoViewModel>(a)).AsQueryable().OrderBy(a => a.Id).ToList<AccordoViewModel>();

                searchResults.ForEach(x => x.StrutturaUfficioServizio += "/");
                string temp = _struttura.StrutturaCompleta;
                temp += "/";
                searchResults = searchResults.FindAll(x => x.StrutturaUfficioServizio.Contains(temp));

                return PartialView("_SearchStatResults", new SearchStatViewModel(searchResults,role));
            }
            catch (Exception ex)
            {
                _logger.LogError("Ricerca Statistiche", ex);
                throw new Exception("Errore durante la ricerca");
            }

        }
    }

}
