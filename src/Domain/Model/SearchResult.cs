using System.Collections.Generic;

namespace Domain.Model
{
    /// <summary>
    /// Rappresenta il risultato di una ricerca.
    /// </summary>
    /// <typeparam name="EntityType">Tipo degli oggetti restituiti dalla ricerca</typeparam>
    public class SearchResult<EntityType, TKey> where EntityType : Entity<TKey>
    {
        /// <summary>
        /// Lista delle entità.
        /// </summary>
        public List<EntityType> Entities { get; init; }

        /// <summary>
        /// Numero totale di elementi restituiti dalla ricerca (utile in caso di paginazione).
        /// </summary>
        public int TotalElements { get; init; }

        /// <summary>
        /// Inizializza un nuovo <see cref="SearchResult{EntityType}"/>.
        /// </summary>
        /// <param name="entities">Lista delle entità restitute dalla ricerca.</param>
        /// <param name="totalElements">Numero totale di elementi</param>
        public SearchResult(List<EntityType> entities, int totalElements)
        {
            Entities = entities;
            TotalElements = totalElements;
        }
    }
}
