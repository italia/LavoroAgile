using Domain.Model;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implementa la gesione dello storico degli stati per gli accordi.
    /// </summary>
    public class StoricoAccordoRepository : IStoricoRepository<Guid, StatoAccordo>
    {
        /// <summary>
        /// Factory per il db context degli accordi.
        /// </summary>
        private readonly IDbContextFactory<AccordoContext> _dbContextFactory;

        /// <summary>
        /// Inizializza un nuovo <see cref="StoricoAccordoRepository"/>.
        /// </summary>
        /// <param name="dbContextFactory">Context factory per gli accordi.</param>
        public StoricoAccordoRepository(IDbContextFactory<AccordoContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task AddStatoToStorico(ICollection<Guid> entityIds, StatoAccordo status, string note, string autore, CancellationToken cancellationToken)
        {
            if (entityIds == null || entityIds.Count == 0)
            {
                throw new ArgumentNullException(nameof(entityIds));

            }

            // Preleva tutti gli identificativi "validi".
            var validIds = entityIds.Where(id => !Guid.Empty.Equals(id));
            if (validIds.Count() == 0)
            {
                throw new ArgumentNullException(nameof(entityIds));

            }

            // Aggiunge una voce allo storico.
            await using var dbContext = _dbContextFactory.CreateDbContext();
            foreach (var id in validIds)
            {
                dbContext.StoricoStato.Add(new StoricoStato(note, status, autore, id));
            }
            await dbContext.SaveChangesAsync(cancellationToken);

        }

        public async Task DeleteStoricoAccordo(Guid idAccordo, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            var storicoStati = dbContext.StoricoStato.AsQueryable().Where(st => idAccordo.Equals(st.AccordoId));
            
            dbContext.StoricoStato.RemoveRange(storicoStati);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

    }

}
