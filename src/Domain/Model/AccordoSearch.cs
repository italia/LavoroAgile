using System;
using System.Collections.Generic;

namespace Domain.Model
{
    /// <summary>
    /// Parametri di ricerca dell'accordo.
    /// </summary>
    public class AccordoSearch
    {
        /// <summary>
        /// Codice dell'accordo da ricercare.
        /// </summary>
        public string Codice { get; set; }

        /// <summary>
        /// Estremo inferiore dell'intervallo di date di validità
        /// </summary>
        public DateTime? DataDa { get; set; }

        /// <summary>
        /// Estremo superiore dell'intervallo di date di validità
        /// </summary>
        public DateTime? DataA { get; set; }

        /// <summary>
        /// Lista degli stati in cui deve trovarsi l'accordo 
        /// </summary>
        public List<StatoAccordo> Stati { get; set; } = new List<StatoAccordo>();

        public bool VistoSegreteriaTecnica { get; set; }

        public string Proponente { get; set; }

        public string Dipartimento { get; set; }

        /// <summary>
        /// Cerca Tra gli accordi da Valutare.
        /// </summary>
        public bool daValutare { get; set; }
        /// <summary>
        /// Ruolo con cui effettuare le ricerche.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Numero di pagina da restituire.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Dimensione della pagina di ricerca.
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
