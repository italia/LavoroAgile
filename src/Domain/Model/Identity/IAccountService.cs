using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Domain.Model.Identity
{
    /// <summary>
    /// Definisce il contratto del servizio di gestione account.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Modifica la password di un utente.
        /// </summary>
        /// <param name="userName">Username.</param>
        /// <param name="currentPassword">Vacchia password.</param>
        /// <param name="newPassword">Nuova password.</param>
        /// <param name="newPasswordConfirm">Conferma della nuova password.</param>
        /// <returns></returns>
        Task<IdentityResult> ChangePassword(string userName, string currentPassword, string newPassword, string newPasswordConfirm);

    }
}
