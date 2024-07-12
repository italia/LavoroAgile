using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Domain.Model.Identity
{
    /// <summary>
    /// Definisce il contratto per l'implementazione della gestione della sign in su LDAP.
    /// </summary>
    public interface ILdapSignInService
    {
        /// <summary>
        /// Esegue la login con username e password.
        /// </summary>
        /// <param name="userName">Username dell'utente.</param>
        /// <param name="password">Password dell'utente.</param>
        /// <param name="isPersisted">true se la sessione deve rimanere attiva alla chiusura del browser.</param>
        /// <returns>Esito del processo di autenticazone.</returns>
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersisted);
    }
}
