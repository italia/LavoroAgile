using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Contratto per un repository che offra servizi di gestione dello storico di uno stato.
    /// </summary>
    /// <typeparam name="TKey">Tipo della chiave dell'entità</typeparam>
    /// <typeparam name="TStatoEnum">Tipo dell'enumerazione utilizzata per rappresentare lo stato.</typeparam>
    public interface IStoricoRepository<TKey, TStatoEnum> where TStatoEnum : Enum
    {
        /// <summary>
        /// Aggiunge un passaggio di stato allo storico di un insieme di accordi.
        /// </summary>
        /// <param name="entityIds">Collezione degli identificativi delle entità cui associare la voce di storico.</param>
        /// <param name="status">Valore dello stato da storicizzare.</param>
        /// <param name="note">Note associate al cambio di stato.</param>
        /// <param name="autore">Autore del passaggio di stato.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddStatoToStorico(ICollection<Guid> entityIds, TStatoEnum status, string note, string autore, CancellationToken cancellationToken);

        /// <summary>
        /// Eliminazione dello storico di un accordo
        /// </summary>
        /// <param name="idAccordo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteStoricoAccordo(Guid idAccordo, CancellationToken cancellationToken);
    }
}
