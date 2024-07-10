using System;
using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models
{
    public class DirigenteViewModel
    {
        
        /// <summary>
        /// Identificativo univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Full name.
        /// </summary>
        public string NomeCognome { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; }

        public override bool Equals(object obj)
        {
            var user = obj as DirigenteViewModel;
            return obj is DirigenteViewModel && 
                string.Equals(NomeCognome, user.NomeCognome, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Email, user.Email, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString() => NomeCognome;

    }
}
