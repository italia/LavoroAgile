using Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models.Search
{
    /// <summary>
    /// Rappresenta il view model di ricerca.
    /// </summary>
    public class SearchViewModel
    {
        /// <summary>
        /// Codice dell'accordo da ricercare.
        /// </summary>
        public string Codice { get; set; }

        /// <summary>
        /// Tipo di ricerca per data
        /// </summary>
        public string SingolaIntervallo { get; set; }

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
        /// Ruolo con cui effettuare le ricerche.
        /// </summary>
        [Required()]
        public string Role { get; set; }

        /// <summary>
        /// Cerca Tra gli accordi da Valutare.
        /// </summary>
        public bool daValutare { get; set; }

        /// <summary>
        /// Numero di pagina da restituire.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Dimensione della pagina di ricerca.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Inizializza un nuovo <see cref="SearchViewModel"/>.
        /// </summary>
        public SearchViewModel()
        {

        }

        /// <summary>
        /// Inizializza un nuovo <see cref="SearchViewModel"/>.
        /// </summary>
        /// <param name="searchTerms">Testo da ricercare nella banca dati.</param>
        /// <param name="pageSize">Dimensione della pagina</param>
        public SearchViewModel(string searchTerms, int pageSize)
        {
            this.PageSize = pageSize;
        }

    }
}
