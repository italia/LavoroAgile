using Esprima.Ast;
using System;
using System.Linq;
using System.Security.Claims;

namespace PCM_LavoroAgile.Extensions
{
    /// <summary>
    /// Offre estensioni per l'estrazione di claims dal principal.
    /// </summary>
    public static class IdentityExtension
    {
        /// <summary>
        /// Recupera la user id
        /// </summary>
        /// <param name="claimsPrincipal">Principal da cui estrarre il claim.</param>
        /// <returns>Identificativo dell'utente.</returns>
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal) => Guid.Parse(claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value);
        
        /// <summary>
        /// Recupera la mail dell'utente
        /// </summary>
        /// <param name="claimsPrincipal">Principal da cui estrarre la mail.</param>
        /// <returns>Email dell'utente.</returns>
        public static string? GetUserEmail(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.Identity.Name;

    }
}
