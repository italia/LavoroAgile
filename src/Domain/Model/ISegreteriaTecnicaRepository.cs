using Domain.Model.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    public interface ISegreteriaTecnicaRepository<T, TKey> where T : Entity<TKey>
    {

        Task<int> InsertAsync(T item, CancellationToken cancellationToken = default);
        Task<int> UpdateAsync(T item, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        Task<T> GetAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Effettua una ricerca di elementi di tipo <typeparamref name="T"/>.
        /// </summary>
        /// <param name="whereExpression">Espressione per la specifica delle condizioni che devono essere soddisfatte dagli elementi restituiti.</param>
        /// <param name="role">Ruolo da utilizzare per la ricerca</param>
        /// <returns><see cref="SearchResult{EntityType}"/> con gli elementi che soddisfano i filtri specificati.</returns>
        Task<SearchResult<T, TKey>> FindAsync(Expression<Func<T, bool>> whereExpression = null, RoleAndKeysClaimEnum role = RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, CancellationToken cancellationToken = default);

        /// <summary>
        /// Effettua una ricerca paginata di elementi di tipo <typeparamref name="T"/>.
        /// </summary>
        /// <param name="whereExpression">Espressione per la specifica delle condizioni che devono essere soddisfatte dagli elementi restituiti.</param>
        /// <param name="role">Ruolo da utilizzare per la ricerca</param>
        /// <param name="page">Numero della pagina da restituire in base 1</param>
        /// <param name="pageSize">Dimensione della pagina</param>
        /// <returns><see cref="SearchResult{EntityType}"/> con gli elementi che soddisfano i filtri specificati.</returns>
        Task<SearchResult<T, TKey>> FindAsync(int page, int pageSize, Expression<Func<T, bool>> whereExpression = null, RoleAndKeysClaimEnum role = RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, CancellationToken cancellationToken = default);
        
    }
}
