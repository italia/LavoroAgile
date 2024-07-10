using Domain.Model.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    /// <summary>
    /// Implementa il servizio per la gestione del login.
    /// </summary>
    public class LdapSignInService : ILdapSignInService
    {
        /// <summary>
        /// Gestore della signin.
        /// </summary>
        private readonly SignInManager<AppUser> _signInManager;

        /// <summary>
        /// Gestore del login su AD.
        /// </summary>
        private readonly ILdapService _ldapService;


        public LdapSignInService(SignInManager<AppUser> signInManager, ILdapService ldapService)
        {
            _signInManager = signInManager;
            _ldapService = ldapService;
        }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersisted)
        {
            // Verifica se le credenziali sono valide su AD.
            if (!_ldapService.Authenticate(userName, password))
            {
                return SignInResult.Failed;
            }

            // Carica i dettagli utente ed avvia la sessione
            var user = await _signInManager.UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            await _signInManager.SignInAsync(user, isPersisted);

            return SignInResult.Success;
        }
    }
}
