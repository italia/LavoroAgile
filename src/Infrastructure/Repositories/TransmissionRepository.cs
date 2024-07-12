using Domain.Model;
using Domain.Model.ExternalCommunications;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implementa il repository per la gestione dello stato di trasmissione degli accordi.
    /// </summary>
    public class TransmissionRepository : ISimpleRepository<TransmissionStatus>
    {
        /// <summary>
        /// Factory per il db context degli accordi.
        /// </summary>
        private readonly IDbContextFactory<AccordoContext> _dbContextFactory;

        /// <summary>
        /// Inizializza un nuovo <see cref="TransmissionRepository"/>.
        /// </summary>
        /// <param name="dbContextFactory">Factory per la generazione del db context.</param>
        /// <exception cref="ArgumentNullException">Sollevata nel caso in cui non fosse inizializzata la factory</exception>
        public TransmissionRepository(IDbContextFactory<AccordoContext> dbContextFactory) =>
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

        /// <summary>
        /// Recupera le informazioni su una specifica trasmissione.
        /// </summary>
        /// <param name="id">Identificativo della trasmissione.</param>
        /// <returns>Trasmissione o null nel caso in cui non esista.</returns>
        public async Task<TransmissionStatus> GetOne(Guid id, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.AccordoTransmission.FirstOrDefaultAsync(t => t.AccordoId == id, cancellationToken);

        }

        /// <summary>
        /// Effettua inserimento o aggiornamento di una entità.
        /// </summary>
        /// <param name="entity">Entità da inserire/salvare.</param>
        public async Task Upsert(TransmissionStatus entity, Expression<Func<TransmissionStatus, TransmissionStatus, TransmissionStatus>> updater, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.AccordoTransmission
                .Upsert(entity)
                .WhenMatched(updater)
                .RunAsync(cancellationToken);

        }

    }

}
