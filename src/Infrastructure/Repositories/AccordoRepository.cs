using Domain.Model;
using Domain.Model.Utilities;
using Infrastructure.Context;
using Infrastructure.Repositories.VisibilityHandlers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Repositories
{
    using Domain;
    // Inserito qui per risolvere ambiguità negli extension methods FirstOrDefaultAsync
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Repository per l'entità Accordo.
    /// </summary>
    public class AccordoRepository : IRepository<Accordo, Guid>
    {
        /// <summary>
        /// Factory per il db context degli accordi.
        /// </summary>
        private readonly IDbContextFactory<AccordoContext> _dbContextFactory;

        private readonly IStrutturaService _strutturaService;

        public AccordoRepository(IDbContextFactory<AccordoContext> dbContextFactory, 
            IStrutturaService strutturaService)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _strutturaService = strutturaService ?? throw new ArgumentNullException(nameof(strutturaService));
        }

        public async Task<int> InsertAsync(Accordo accordo, CancellationToken cancellationToken)
        {
            // Genera l'id
            accordo.Id = Guid.NewGuid();

            await using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.Accordi.AddAsync(accordo, cancellationToken);

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> UpdateAsync(Accordo accordo, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            //Attività accordo
            var attivitaAccordo = dbContext.AttivitaAccordo.AsQueryable().Where(aa => accordo.Id.Equals(aa.AccordoId));
            dbContext.AttivitaAccordo.RemoveRange(attivitaAccordo);

            dbContext.Update(accordo);

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            dbContext.Accordi.Remove(new Accordo { Id = id });

            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Accordo> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            return dbContext.Accordi.Include(x => x.ListaAttivita).Include(x => x.StoricoStato).Include(x => x.Transmission).AsQueryable().Where(x => x.Id == id).FirstOrDefault();
        }
        public async Task<SearchResult<Accordo, Guid>> FindAsync(Guid userUid, Expression<Func<Accordo, bool>> whereExpression = null, RoleAndKeysClaimEnum role = RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, CancellationToken cancellationToken = default)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();

            var queryResults = CreateFilterQuery(dbContext, whereExpression, userUid, role);

            return new SearchResult<Accordo, Guid>(
                // Include nella select, le informazioni di trasmissione.
                await queryResults.Select(a => a.Accordo).Include(a => a.Transmission).ToListAsync(cancellationToken),
                (await queryResults.FirstOrDefaultAsync(cancellationToken))?.Total ?? 0);
        }

        public async Task<SearchResult<Accordo, Guid>> FindAsync(Guid userId, int page, int pageSize, Expression<Func<Accordo, bool>> whereExpression = null, RoleAndKeysClaimEnum role = RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, CancellationToken cancellationToken = default)
        {
            var itemsToSkip = (page - 1) * pageSize;

            await using var dbContext = _dbContextFactory.CreateDbContext();

            var queryResults = CreateFilterQuery(dbContext, whereExpression, userId, role)
                .Skip(itemsToSkip)
                .Take(pageSize);

            return new SearchResult<Accordo, Guid>(
                await queryResults.Select(a => a.Accordo).Include(a => a.StoricoStato.Where(ss => ss.Stato == StatoAccordo.Sottoscritto)).ToListAsync(cancellationToken),
                (await queryResults.FirstOrDefaultAsync(cancellationToken))?.Total ?? 0);
        }

        public async Task<ICollection<Guid>> UpdateByPropertyAsync<TProperty>(Func<Accordo, PropertyInfo> propertyInfo, TProperty actualValue, TProperty newValue, Expression<Func<Accordo, bool>> whereExpression = null, CancellationToken cancellationToken = default)
        {
            // Recupera le informazioni sulla proprietà da aggiornare.
            var property = propertyInfo(new Accordo());
            if (property == null)
            {
                throw new ArgumentException("Accordo non contiene la proprietà specificata.");
            }

            // Se il tipo della proprietà è diverso da TProperty, lancia eccezione.
            if (!property.PropertyType.Equals(typeof(TProperty)))
            {
                throw new ArgumentException($"Tipo {typeof(TProperty).Name} non conforme con il tipo {property.PropertyType.Name} della proprietà {property.Name}");
            }

            // Recupera tutti gli accordi con la proprietà valorizzata con oldValue ed estrae Id e
            // proprietà convertendoli in Accordo.
            await using var dbContext = _dbContextFactory.CreateDbContext();
            var accordi = dbContext.Accordi.AsQueryable();
            if (whereExpression != null)
            {
                accordi = accordi.Where(whereExpression);
            }

            accordi = accordi.Where($"{property.Name} = @0", actualValue).Select<Accordo>($"new (Id, {property.Name})");

            // Aggiorna il valore della proprietà ed imposta la proprietà come modificata
            var accordiCollection = await accordi.ToListAsync(cancellationToken);
            accordiCollection.ForEach(accordo =>
            {

                dbContext.Entry(accordo).Property(property.Name).CurrentValue = newValue;
                dbContext.Entry(accordo).Property(property.Name).IsModified = true;

                dbContext.Entry(accordo).Property("RevisioneAccordo").CurrentValue = false;
                dbContext.Entry(accordo).Property("RevisioneAccordo").IsModified = true;
            });

            // Aggiorna il database.
            await dbContext.SaveChangesAsync(cancellationToken);

            // Restituisce gli identificativi degli accordi aggiornati
            return accordiCollection.Select(a => a.Id).ToList();

        }

        /// <summary>
        /// Costruisce il filtro sugli accordi.
        /// </summary>
        /// <param name="dbContext">DbContext per l'interfacciamento con il database</param>
        /// <param name="whereExpression">Filteri da applicare agli accordi.</param>
        /// <param name="userUid">Identificativo dell'utente che effettua la ricerca</param>
        /// <param name="role">Ruolo con cui ricercare i risultati</param>
        /// <returns>Queryable sugli accordi</returns>
        private IQueryable<AccordoWithCount> CreateFilterQuery(AccordoContext dbContext, Expression<Func<Accordo, bool>> whereExpression, Guid userUid, RoleAndKeysClaimEnum role)
        {
            var accordi = dbContext.Accordi.AsQueryable();
            if (whereExpression != null)
            {
                accordi = dbContext.Accordi.Where(whereExpression);

            }

            //Applica il filtro sulla visibilità degli accordi, in base al ruolo.
            accordi = VisibilityHandlerFactory<Accordo, Guid>.GetVisibilityHandler(role.ToString()).Filter(accordi, userUid, _strutturaService);

            return accordi
                .OrderByDescending(a => a.Codice)
                .Select(a => new AccordoWithCount
                {
                    Accordo = a,
                    Total = accordi.Count()
                });
        }

        public async Task<Guid> CloneEntity(Guid entityId, bool revisioneAccordo, CancellationToken cancellationToken)
        {
            // Identificativo del nuovo accordo.
            var newId = Guid.NewGuid();

            // L'operazione deve essere effettuata in transazione.
            await using var dbContext = _dbContextFactory.CreateDbContext();

            // I database sono configurati con trategia di retry che non compatible con l'inserimento
            // in unit of work
            await dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                // Recupera l'accordo.
                var accordoToClone = await this.GetAsync(entityId, cancellationToken);

                // Se l'accordo non è rinnovabile, scatena eccezione
                if (!accordoToClone.Rinnovabile && !revisioneAccordo)
                {
                    throw new LavoroAgileException("Accordo non rinnovabile");
                }

                // Se l'accordo ha già un child, non è possibile clonarlo.
                if (!Guid.Empty.Equals(accordoToClone.ChildId))
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new LavoroAgileException("Questo accordo ha già un discendente.");
                }

                // Clona l'accordo, imposta gli identificativi padre-figlio, ripulisce l'accordo nuovo e riporta l'accordo nuovo in bozza.
                var clonedAccordo = (Accordo)accordoToClone.Clone();

                clonedAccordo.ParentId = accordoToClone.Id;
                accordoToClone.ChildId = clonedAccordo.Id;

                //Se trattasi di un rinnovo
                if (!revisioneAccordo)
                {
                    clonedAccordo.DataFineAccordoPrecedente = accordoToClone.DataFineUtc;
                    clonedAccordo.PianificazioneDateAccordo = String.Empty;
                }

                //Se trattasi di una revisione anticipata
                if (revisioneAccordo)
                {
                    clonedAccordo.DataInizioUtc = DateTime.UtcNow;
                    clonedAccordo.DataFineUtc = accordoToClone.DataFineUtc;
                    clonedAccordo.RevisioneAccordo = revisioneAccordo;
                    clonedAccordo.ValutazioneEsitiAccordoPrecedente = null;
                    accordoToClone.RevisioneAccordo = revisioneAccordo;
                }

                // Aggiorna l'accordo originale e salva il nuovo accordo.
                dbContext.Accordi.Update(accordoToClone);
                dbContext.Accordi.Add(clonedAccordo);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                newId = clonedAccordo.Id;
            });

            return newId;
        }

        /// <summary>
        /// Classe interna utilizzata per memorizzare accordi e numero totale di accordi.
        /// </summary>
        internal class AccordoWithCount
        {
            public int Total { get; init; }
            public Accordo Accordo { get; init; }
        }

    }
}
