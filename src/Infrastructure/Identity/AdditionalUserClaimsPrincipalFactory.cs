using Domain.Model;
using Domain.Model.Identity;
using Domain.Model.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Extensions
{
    public class AdditionalUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, IdentityRole<Guid>>
    {
        private readonly IStrutturaService _strutturaService;
        private readonly IRepository<Accordo, Guid> _respositoryAccordo;
        private readonly ISegreteriaTecnicaRepository<SegreteriaTecnica, Guid> _repositorySegreteriaTecnica;

        public AdditionalUserClaimsPrincipalFactory(UserManager<AppUser> userManager, 
            RoleManager<IdentityRole<Guid>> roleManager, 
            IOptions<IdentityOptions> options, 
            IStrutturaService strutturaService, 
            IRepository<Accordo, Guid> repositoryAccordo, 
            ISegreteriaTecnicaRepository<SegreteriaTecnica, Guid> repositorySegreteriaTecnica) : base(userManager, roleManager, options)
        {
            _strutturaService = strutturaService;
            _repositorySegreteriaTecnica = repositorySegreteriaTecnica;
            _respositoryAccordo = repositoryAccordo;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (await UserManager.IsInRoleAsync(user, RoleAndKeysClaimEnum.ADMINISTRATOR.ToDescriptionString()))
            {
                //In questo caso al momento utilizziamo solo l'ADMINISTRATOR
                identity.AddClaim(new Claim(RoleAndKeysClaimEnum.ADMINISTRATOR.ToDescriptionString(),
                    RoleAndKeysClaimEnum.ADMINISTRATOR.ToDescriptionString()));
            }
            else
            {
                //Questo claims è quello di default per tutti gli utenti
                identity.AddClaim(new Claim(RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE.ToDescriptionString(), 
                    RoleAndKeysClaimEnum.UTENTE.ToDescriptionString()));

                // Aggiunta del claims relativo all'interdizione dell'utente a creare nuovi accordi.
                identity.AddClaim(new Claim(RoleAndKeysClaimEnum.INTERDICTED_USER.ToDescriptionString(), user.CannotCreateAccordo.ToString()));

                //In base alle configurazioni fatte nella definizione della struttura
                //vengono aggiunti i claims per identificare eventuali ulteriori ruoli

                // Verifica se l'utente è un capo struttura o se esiste un accordo che lo vede
                // indicato come tale
                if ((await _strutturaService.FindStrutturaAsync(s => 
                    s.CapoStruttura.Id.Equals(user.Id))).Entities.FirstOrDefault() != null ||
                    (await _respositoryAccordo.FindAsync(user.Id, role: RoleAndKeysClaimEnum.KEY_CLAIM_CAPO_STRUTTURA)).Entities.FirstOrDefault() != null  )
                {
                    identity.AddClaim(new Claim(RoleAndKeysClaimEnum.KEY_CLAIM_CAPO_STRUTTURA.ToDescriptionString(),
                    RoleAndKeysClaimEnum.CAPO_STRUTTURA.ToDescriptionString()));
                }

                // Verifica se l'utente è un capi intermedio o se esiste un accordo che lo vede
                // indicato come tale
                if ((await _strutturaService.FindStrutturaAsync(s =>
                    s.CapoIntermedio.Id.Equals(user.Id))).Entities.FirstOrDefault() != null ||
                    (await _respositoryAccordo.FindAsync(user.Id, role: RoleAndKeysClaimEnum.KEY_CLAIM_CAPO_INTERMEDIO)).Entities.FirstOrDefault() != null)
                {
                    identity.AddClaim(new Claim(RoleAndKeysClaimEnum.KEY_CLAIM_CAPO_INTERMEDIO.ToDescriptionString(),
                    RoleAndKeysClaimEnum.CAPO_INTERMEDIO.ToDescriptionString()));
                }

                // Verifica se l'utente è un responsabile di accordo o se esiste un accordo che lo vede
                // indicato come tale
                if ((await _strutturaService.FindStrutturaAsync(s => 
                    s.ResponsabileAccordo.Id.Equals(user.Id))).Entities.FirstOrDefault() != null ||
                    (await _respositoryAccordo.FindAsync(user.Id, role: RoleAndKeysClaimEnum.KEY_CLAIM_RESPONSABILE_ACCORDO)).Entities.FirstOrDefault() != null)
                {
                    identity.AddClaim(new Claim(RoleAndKeysClaimEnum.KEY_CLAIM_RESPONSABILE_ACCORDO.ToDescriptionString(),
                    RoleAndKeysClaimEnum.RESPONSABILE_ACCORDO.ToDescriptionString()));
                }

                // Verifica se l'utente è un referente interno o se esiste un accordo che lo vede
                // indicato come tale
                if ((await _strutturaService.FindStrutturaAsync(s =>
                    s.ReferenteInterno.Id.Equals(user.Id))).Entities.FirstOrDefault() != null)
                {
                    identity.AddClaim(new Claim(RoleAndKeysClaimEnum.KEY_CLAIM_REFERENTE_INTERNO.ToDescriptionString(),
                    RoleAndKeysClaimEnum.REFERENTE_INTERNO.ToDescriptionString()));
                }

                // Verifica se l'utente fa parte della segreteria tecnica
                if ((await _repositorySegreteriaTecnica.FindAsync(s => s.UserProfileId.Equals(user.Id))).Entities.FirstOrDefault() != null)
                {
                    identity.AddClaim(new Claim(RoleAndKeysClaimEnum.KEY_CLAIM_SEGRETERIA_TECNICA.ToDescriptionString(),
                    RoleAndKeysClaimEnum.SEGRETERIA_TECNICA.ToDescriptionString()));
                }
            }

            // Aggiunta del given name
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName ?? user.UserName));

            return identity;
        }
    }    
}