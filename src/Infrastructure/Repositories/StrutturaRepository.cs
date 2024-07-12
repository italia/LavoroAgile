using Domain.Model;
using Domain.Model.Utilities;
using Infrastructure.Context;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    // Inserito qui per risolvere ambiguità negli extension methods FirstOrDefaultAsync
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Reflection;

    public class StrutturaRepository : IStrutturaRepository<Struttura, Guid>
    {
        /// <summary>
        /// Factory per il db context della struttura.
        /// </summary>
        private readonly IDbContextFactory<StrutturaContext> _dbContextFactory;

        public StrutturaRepository(IDbContextFactory<StrutturaContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<int> InsertAsync(Struttura struttura, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.Strutture.AddAsync(struttura, cancellationToken);

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> UpdateAsync(Struttura struttura, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Update(struttura);

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Strutture.Remove(new Struttura { Id = id });

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Struttura> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Strutture.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<SearchResult<Struttura, Guid>> FindAsync(Expression<Func<Struttura, bool>> whereExpression = null, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            var queryResults = CreateFilterQuery(dbContext, whereExpression);

            return new SearchResult<Struttura, Guid>(
                await queryResults.Select(s => s.Struttura).ToListAsync(cancellationToken),
                (await queryResults.FirstOrDefaultAsync(cancellationToken))?.Total ?? 0);
        }

        public async Task<SearchResult<Struttura, Guid>> FindAsync(int page, int pageSize, Expression<Func<Struttura, bool>> whereExpression = null, CancellationToken cancellationToken = default)
        {
            var itemsToSkip = (page - 1) * pageSize;

            await using var dbContext = _dbContextFactory.CreateDbContext();

            var queryResults = CreateFilterQuery(dbContext, whereExpression)
                .Skip(itemsToSkip)
                .Take(pageSize);

            return new SearchResult<Struttura, Guid>(
                await queryResults.Select(s => s.Struttura).ToListAsync(cancellationToken),
                (await queryResults.FirstOrDefaultAsync(cancellationToken))?.Total ?? 0);

        }

        /// <summary>
        /// Costruisce il filtro sulle strutture.
        /// </summary>
        /// <param name="dbContext">DbContext per l'interfacciamento con il database</param>
        /// <param name="whereExpression">Filteri da applicare alle strutture.</param>
        /// <param name="userUid">Identificativo dell'utente che effettua la ricerca</param>
        /// <param name="role">Ruolo con cui ricercare i risultati</param>
        /// <returns>Queryable sulle strutture</returns>
        private IQueryable<StrutturaWithCount> CreateFilterQuery(StrutturaContext dbContext, Expression<Func<Struttura, bool>> whereExpression)
        {
            var strutture = dbContext.Strutture.AsQueryable();
            if (whereExpression != null)
            {
                strutture = dbContext.Strutture.Where(whereExpression);

            }

            return strutture
                .OrderBy(s => s.CreationDate)
                .Select(s => new StrutturaWithCount
                {
                    Struttura = s,
                    Total = strutture.Count()
                });
        }

        public Task<ICollection<Guid>> UpdateByPropertyAsync<TProperty>(Func<Struttura, PropertyInfo> propertyInfo, TProperty actualValue, TProperty newValue, Expression<Func<Struttura, bool>> whereExpression = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CloneEntity(Guid entityId, bool revisioneAccordo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Classe interna utilizzata per memorizzare strutture e numero totale di strutture.
        /// </summary>
        internal class StrutturaWithCount
        {
            public int Total { get; init; }
            public Struttura Struttura { get; init; }
        }
    }
}
