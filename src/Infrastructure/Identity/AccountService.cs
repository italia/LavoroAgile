using Domain.Model.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    /// <summary>
    /// Implementa il servizio di cambio password.
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// Gestore delle utenze.
        /// </summary>
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Crea un nuovo <see cref="AccountService"/>.
        /// </summary>
        /// <param name="userManager">Gestore delle utenze.</param>
        public AccountService(UserManager<AppUser> userManager)
        {
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        }

        public async Task<IdentityResult> ChangePassword(string userName, string currentPassword, string newPassword, string newPasswordConfirm)
        {
            // Elenco degli errori di validazione rilevati
            var errors = new List<IdentityError>();

            if (string.IsNullOrWhiteSpace(userName))
            {
                errors.Add(new IdentityError { Description = "Username non valorizzata" });
            }
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                errors.Add(new IdentityError { Description = "Vecchia password non valorizzata" });

            }
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                errors.Add(new IdentityError { Description = "Nuova password non valorizzata" });


            }
            if (string.IsNullOrWhiteSpace(newPasswordConfirm))
            {
                errors.Add(new IdentityError { Description = "Conferma della nuova password non valorizzata" });

            }

            if (!string.Equals(newPasswordConfirm, newPassword, StringComparison.Ordinal))
            {
                errors.Add(new IdentityError { Description = "Password non coincidenti" });
            }

            // Se ci sono errori, fallisce.
            if (errors.Any())
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            // Recupera dettaglio utente a partire dalla username
            var appUser = await this._userManager.FindByNameAsync(userName);
            if (appUser != null)
            {
                return await this._userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);

            }

            return IdentityResult.Failed(new IdentityError { Description = "Utente non trovato" });

        }
    }
}
