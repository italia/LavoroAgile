using Domain.Helpers;
using Domain.Model;
using Domain.Model.Utilities;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class DatabaseStruttureService : IStrutturaService
    {
        private readonly IStrutturaRepository<Struttura, Guid> _strutturaRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DatabaseStruttureService(IStrutturaRepository<Struttura, Guid> strutturaRepository, IServiceScopeFactory serviceScopeFactory)
        {
            this._strutturaRepository = strutturaRepository ?? throw new ArgumentNullException(nameof(strutturaRepository));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public async Task<Struttura> InitCreateStrutturaAsync(CancellationToken cancellationToken = default)
        {
            Struttura struttura = new Struttura();

            //Valorizzato a false perchè la struttura è gestita internamente su DB
            struttura.OnlyFirstLevel = false;

            return struttura;
        }

        public async Task InsertStrutturaAsync(Struttura struttura, CancellationToken cancellationToken = default)
        {
            struttura.DirigenteResponsabile = (Dirigente)struttura.ResponsabileAccordo.Clone();
            await struttura.FindOrCreateStrutturaManagerByUsernameAsync(_serviceScopeFactory);
            await _strutturaRepository.InsertAsync(struttura, cancellationToken);
        }

        public async Task UpdateStrutturaAsync(Struttura struttura, CancellationToken cancellationToken = default)
        {
            struttura.DirigenteResponsabile = (Dirigente)struttura.ResponsabileAccordo.Clone();
            await struttura.FindOrCreateStrutturaManagerByUsernameAsync(_serviceScopeFactory);
            await _strutturaRepository.UpdateAsync(struttura, cancellationToken);
        }

        public async Task DeleteStrutturaAsync(Guid id, CancellationToken cancellationToken)
        {
            await _strutturaRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<Struttura> GetStrutturaAsync(Guid idStruttura, CancellationToken cancellationToken = default)
        {
            Struttura struttura = await _strutturaRepository.GetAsync(idStruttura, cancellationToken);

            //Valorizzato a true perchè la struttura è gestita da una integrazione esterna
            struttura.OnlyFirstLevel = false;

            return struttura;
        }

        public async Task<SearchResult<Struttura, Guid>> FindStrutturaAsync(Expression<Func<Struttura, bool>> whereExpression = null, CancellationToken cancellationToken = default)
        {
            return await _strutturaRepository.FindAsync(whereExpression, cancellationToken);
        }

    }
}
