using Microsoft.AspNetCore.Identity;
using System;

namespace Domain.Model.Identity
{
    /// <summary>
    /// Rappresenta il profilo utente.
    /// </summary>
    public class AppUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Nome e cognome
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Proprietà utilizzata per indicare che un utente non può creare accordi, perché bloccato dai dirigenti
        /// </summary>
        public bool CannotCreateAccordo { get; set; }

        public AppUser()
        {
            
        }

        public AppUser(string fullName, string email)
        {
            UserName = email;
            Email = email;
            FullName = fullName;
            
        }

        public Dipendente ToDipendente()
        {
            return new Dipendente
            {
                Id = this.Id,
                Email = this.Email,
                NomeCognome = this.FullName
            };
        }

    }
}
