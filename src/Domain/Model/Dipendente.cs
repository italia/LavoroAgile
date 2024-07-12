using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    /// <summary>
    /// Modella il dipendente.
    /// </summary>
    public class Dipendente : ICloneable
    {
        /// <summary>
        /// Nome del dipendente, valorizzato solo in fase di recupero dell'anagrafica
        /// </summary>
        [NotMapped]
        public string Nome { get; set; }

        /// <summary>
        /// Cognome del dipendente, valorizzato solo in fase di recupero dell'anagrafica
        /// </summary>
        [NotMapped]
        public string Cognome { get; set; }

        public Guid Id { get; set; }

        public string NomeCognome { get; set; }

        public DateTime? DataDiNascita { get; set; }

        public string Sesso { get; set; }

        public string LuogoDiNascita { get; set; }

        public string CodiceFiscale { get; set; }

        public string Email { get; set; }

        public string PosizioneGiuridica { get; set; }

        public string CategoriaFasciaRetributiva { get; set; }

        /// <summary>
        /// Struttura di appartenenza del dipendente, valorizzato solo in fase di recupero dell'anagrafica
        /// </summary>
        [NotMapped]
        public Struttura Struttura { get; set; }

        /// <summary>
        /// Clona le informazioni sul dipendente.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();

        }
    }
}