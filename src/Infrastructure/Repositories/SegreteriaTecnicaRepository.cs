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

    public class SegreteriaTecnicaRepository : ISegreteriaTecnicaRepository<SegreteriaTecnica, Guid>
    {
        /// <summary>
        /// Factory per il db context della struttura.
        /// </summary>
        private readonly IDbContextFactory<StrutturaContext> _dbContextFactory;

        public SegreteriaTecnicaRepository(IDbContextFactory<StrutturaContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<int> InsertAsync(SegreteriaTecnica item, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.SegreteriaTecnica.AddAsync(item, cancellationToken);

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> UpdateAsync(SegreteriaTecnica item, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Update(item);

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.SegreteriaTecnica.Remove(new SegreteriaTecnica { Id = id });

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<SegreteriaTecnica> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.SegreteriaTecnica.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<SearchResult<SegreteriaTecnica, Guid>> FindAsync(int page, int pageSize, Expression<Func<SegreteriaTecnica, bool>> whereExpression = null, RoleAndKeysClaimEnum role = RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, CancellationToken cancellationToken = default)
        {
            var itemsToSkip = (page - 1) * pageSize;

            await using var dbContext = _dbContextFactory.CreateDbContext();

            var queryResults = CreateFilterQuery(dbContext, whereExpression, role)
                .Skip(itemsToSkip)
                .Take(pageSize);

            return new SearchResult<SegreteriaTecnica, Guid>(
                await queryResults.Select(s => s.SegreteriaTecnica).ToListAsync(cancellationToken),
                (await queryResults.FirstOrDefaultAsync(cancellationToken))?.Total ?? 0);
        }

        public async Task<SearchResult<SegreteriaTecnica, Guid>> FindAsync(Expression<Func<SegreteriaTecnica, bool>> whereExpression = null, RoleAndKeysClaimEnum role = RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            var queryResults = CreateFilterQuery(dbContext, whereExpression, role);

            return new SearchResult<SegreteriaTecnica, Guid>(
                await queryResults.Select(s => s.SegreteriaTecnica).ToListAsync(cancellationToken),
                (await queryResults.FirstOrDefaultAsync(cancellationToken))?.Total ?? 0);

        }

        /// <summary>
        /// Costruisce il filtro sui componenti della segreteria tecnica..
        /// </summary>
        /// <param name="dbContext">DbContext per l'interfacciamento con il database</param>
        /// <param name="whereExpression">Filteri da applicare ai componenti della segreteria tecnica.</param>
        /// <param name="role">Ruolo con cui ricercare i risultati</param>
        /// <returns>Queryable sui componenti della segreteria tecnica.</returns>
        private IQueryable<SegreteriaTecnicaWithCount> CreateFilterQuery(StrutturaContext dbContext, Expression<Func<SegreteriaTecnica, bool>> whereExpression, RoleAndKeysClaimEnum role)
        {
            var segTecnica = dbContext.SegreteriaTecnica.AsQueryable();
            if (whereExpression != null)
            {
                segTecnica = dbContext.SegreteriaTecnica.Where(whereExpression);

            }

            return segTecnica
                .OrderBy(s => s.CreationDate)
                .Select(s => new SegreteriaTecnicaWithCount
                {
                    SegreteriaTecnica = s,
                    Total = segTecnica.Count()
                });
        }

        //public Task<ICollection<Guid>> UpdateByPropertyAsync<TProperty>(Func<SegreteriaTecnica, PropertyInfo> propertyInfo, TProperty actualValue, TProperty newValue, Expression<Func<SegreteriaTecnica, bool>> whereExpression = null, CancellationToken cancellationToken = default)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Guid> CloneEntity(Guid entityId, bool revisioneAccordo, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Classe interna utilizzata per memorizzare segreteria tecnica e numero totale di componenti della segteria tecnica.
        /// </summary>
        internal class SegreteriaTecnicaWithCount
        {
            public int Total { get; init; }
            public SegreteriaTecnica SegreteriaTecnica { get; init; }
        }
    }
}
