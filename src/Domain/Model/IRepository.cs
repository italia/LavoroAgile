using Domain.Model.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IRepository<T, TKey> where T : Entity<TKey>
    {
        Task<int> InsertAsync(T item, CancellationToken cancellationToken = default);
        Task<int> UpdateAsync(T item, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        Task<T> GetAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Effettua una ricerca di elementi di tipo <typeparamref name="T"/>.
        /// </summary>
        /// <param name="whereExpression">Espressione per la specifica delle condizioni che devono essere soddisfatte dagli elementi restituiti.</param>
        /// <param name="userUid">Uid dell'utente che sta effettuando la ricerca.</param>
        /// <param name="role">Ruolo da utilizzare per la ricerca</param>
        /// <returns><see cref="SearchResult{EntityType}"/> con gli elementi che soddisfano i filtri specificati.</returns>
        Task<SearchResult<T, TKey>> FindAsync(Guid userUid, Expression < Func<T, bool>> whereExpression = null, RoleAndKeysClaimEnum role = RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, CancellationToken cancellationToken = default);

        /// <summary>
        /// Effettua una ricerca paginata di elementi di tipo <typeparamref name="T"/>.
        /// </summary>
        /// <param name="whereExpression">Espressione per la specifica delle condizioni che devono essere soddisfatte dagli elementi restituiti.</param>
        /// <param name="userUid">Uid dell'utente che sta effettuando la ricerca.</param>
        /// <param name="role">Ruolo da utilizzare per la ricerca</param>
        /// <param name="page">Numero della pagina da restituire in base 1</param>
        /// <param name="pageSize">Dimensione della pagina</param>
        /// <returns><see cref="SearchResult{EntityType}"/> con gli elementi che soddisfano i filtri specificati.</returns>
        Task<SearchResult<T, TKey>> FindAsync(Guid userUid, int page, int pageSize, Expression<Func<T, bool>> whereExpression = null, RoleAndKeysClaimEnum role = RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, CancellationToken cancellationToken = default);

        /// <summary>
        /// Valorizza con <paramref name="newValue"/> la proprietà <paramref name="propertyInfo"/> di tutti gli oggetti 
        /// <typeparamref name="T"/> che hanno <paramref name="propertyInfo"/> valorizzata con <paramref name="actualValue"/>.
        /// </summary>
        /// <typeparam name="TProperty">Tipo della proprietà da valorizzare.</typeparam>
        /// <param name="propertyInfo">Informazioni sulla proprietà da valorizzare.</param>
        /// <param name="actualValue">Valore con cui è attualmente valorizzata <paramref name="propertyInfo"/></param>
        /// <param name="newValue">Nuovo valore da assegnare a <paramref name="propertyInfo"/>.</param>
        /// <param name="whereExpression">Eventuale filtro per la selezione degli elementi.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Elenco delle chiavi degli elementi modificati</returns>
        Task<ICollection<TKey>> UpdateByPropertyAsync<TProperty>(Func<T, PropertyInfo> propertyInfo, TProperty actualValue, TProperty newValue, Expression<Func<T, bool>> whereExpression = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clona una entità
        /// </summary>
        /// <param name="entityId">Identificativo dell'entità da clonare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Identificativo della nuova entità</returns>
        Task<TKey> CloneEntity(TKey entityId, bool revisioneAccordo, CancellationToken cancellationToken);
    }
}
