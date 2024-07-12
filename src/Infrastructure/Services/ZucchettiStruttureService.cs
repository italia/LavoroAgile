using Domain.Helpers;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ZucchettiStruttureService : IStrutturaService
    {
        private readonly IStrutturaRepository<Struttura, Guid> _strutturaRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ZucchettiStruttureService(IStrutturaRepository<Struttura, Guid> strutturaRepository, IServiceScopeFactory serviceScopeFactory)
        {
            this._strutturaRepository = strutturaRepository ?? throw new ArgumentNullException(nameof(strutturaRepository));
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task<Struttura> InitCreateStrutturaAsync(CancellationToken cancellationToken = default)
        {
            Struttura struttura = new Struttura();

            //Valorizzato a true perchè la struttura è gestita da una integrazione esterna
            struttura.OnlyFirstLevel = true;

            return Task.FromResult(struttura);
        }

        public async Task InsertStrutturaAsync(Struttura struttura, CancellationToken cancellationToken = default)
        {
            await struttura.FindOrCreateStrutturaManagerByUsernameAsync(_serviceScopeFactory);
            await _strutturaRepository.InsertAsync(struttura, cancellationToken);
        }

        public async Task UpdateStrutturaAsync(Struttura struttura, CancellationToken cancellationToken = default)
        {
            //Essendo in un regime di integrazione esterna delle strutture, è possibile gestire solo il referente interno 
            Struttura strutturaToUpdate = await _strutturaRepository.GetAsync(struttura.Id, cancellationToken);
            strutturaToUpdate.SetReferenteInterno(struttura.ReferenteInterno);

            await strutturaToUpdate.FindOrCreateStrutturaManagerByUsernameAsync(_serviceScopeFactory);
            await _strutturaRepository.UpdateAsync(strutturaToUpdate, cancellationToken);
        }

        public async Task DeleteStrutturaAsync(Guid id, CancellationToken cancellationToken)
        {
            await _strutturaRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<Struttura> GetStrutturaAsync(Guid idStruttura, CancellationToken cancellationToken = default)
        {
            Struttura struttura = await _strutturaRepository.GetAsync(idStruttura, cancellationToken);

            //Valorizzato a true perchè la struttura è gestita da una integrazione esterna
            struttura.OnlyFirstLevel = true;

            return struttura;
        }

        public async Task<SearchResult<Struttura, Guid>> FindStrutturaAsync(Expression<Func<Struttura, bool>> whereExpression = null, CancellationToken cancellationToken = default)
        {
            return await _strutturaRepository.FindAsync(whereExpression, cancellationToken);
        }

    }
}
