using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public abstract class PeopleCommon : ICloneable
    {
        /// <summary>
        /// Identificativo univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Full name.
        /// </summary>
        public string NomeCognome { get; set; } = string.Empty;

        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        protected PeopleCommon()
        {
            
        }

        public PeopleCommon(string name, string lastName, string email)
            : this()
        {
            NomeCognome = $"{name} {lastName}";
            Email = email;

        }

        public override bool Equals(object obj)
        {
            var d = obj as PeopleCommon;
            return d != null && string.Equals(Email, d.Email, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(NomeCognome, d.NomeCognome, StringComparison.OrdinalIgnoreCase);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString() => NomeCognome;
    }
}
