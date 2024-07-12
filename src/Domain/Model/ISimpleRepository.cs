using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Definisce il contratto di un repository semplficato.
    /// </summary>
    /// <remarks>Utile per quei casi in cui ci sono entità senza chiave (ad esempio le trasmissioni)</remarks>
    public interface ISimpleRepository<T> where T : class
    {
        /// <summary>
        /// Recupera una entità a partire dal suo id.
        /// </summary>
        /// <param name="id">Identificativo dell'entità da recuperare.</param>
        /// <returns>Entità recuperata.</returns>
        Task<T> GetOne(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Salva una entità.
        /// </summary>
        /// <param name="entity">Entità da salvare o aggiornare.</param>
        /// <param name="updater">
        /// Delegato da invocare per la definizione delle modifiche 
        /// da effettuare sull'entità. Il delegato riceve la coppia 
        /// (oggettoRecuperatoDalDB, oggettoDaSalvare) e restituisce una istanza dell'entità.</param>
        /// <returns></returns>
        Task Upsert(T entity, Expression<Func<T, T, T>> updater, CancellationToken cancellationToken);
    
    }

}
