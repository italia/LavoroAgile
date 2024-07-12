using System.Collections.Generic;

namespace PCM_LavoroAgile.Models.Search
{
    /// <summary>
    /// Collezione dei risultati di ricerca.
    /// </summary>
    public class SearchResultCollectionViewModel
    {
        /// <summary>
        /// Inizializza un nuovo <see cref="SearchResultCollectionViewModel"/>.
        /// </summary>
        /// <param name="totalElements">Numero totale di elementi restituiti dalla ricerca.</param>
        /// <param name="searchResults">Risultati di ricerca</param>
        /// <param name="role">Ruolo operativo con cui sono stati richiesti i risultati.</param>
        public SearchResultCollectionViewModel(int totalElements, List<AccordoHeaderViewModel> searchResults, string role)
        {
            this.TotalElements = totalElements;
            this.SearchResults = searchResults;
            this.Role = role;
        }

        public SearchResultCollectionViewModel()
        {

        }

        /// <summary>
        /// Numero di elementi totali restituiti dalla ricerca.
        /// </summary>
        public int TotalElements { get; init; }

        /// <summary>
        /// Pagina di risultati.
        /// </summary>
        public List<AccordoHeaderViewModel> SearchResults { get; init; }

        /// <summary>
        /// Ruolo applicativo con cui sono stati richiesti i risultati
        /// </summary>
        public string Role { get; init; }
    }
}
