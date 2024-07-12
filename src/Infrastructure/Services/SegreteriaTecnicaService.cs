using Domain;
using Domain.Helpers;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    /// <summary>
    /// Implementa il servizio per la gestione del personale di segreteria tecnica.
    /// </summary>
    public class SegreteriaTecnicaService : ISegreteriaTecnicaService
    {
        private readonly ISegreteriaTecnicaRepository<SegreteriaTecnica, Guid> _repositorySegreteriaTecnica;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SegreteriaTecnicaService(ISegreteriaTecnicaRepository<SegreteriaTecnica, Guid> segreteriaTecnica, IServiceScopeFactory serviceScopeFactory) => (_repositorySegreteriaTecnica, _serviceScopeFactory) = (segreteriaTecnica, serviceScopeFactory);

        public async Task<SegreteriaTecnica> CreateSegreteriaTecnicaAsync(SegreteriaTecnica segreteriaTecnica, CancellationToken cancellationToken = default)
        {
            // Inserisce la persona solo non è già presente.
            var st = await this._repositorySegreteriaTecnica.FindAsync(s => s.EmailUtente == segreteriaTecnica.EmailUtente, cancellationToken: cancellationToken);
            if (st?.TotalElements > 0)
            {
                throw new LavoroAgileException("Persona già inserita nella segreteria tecnica.");
            }

            await segreteriaTecnica.FindOrCreateSegreteriaTecnicaByUsernameAsync(_serviceScopeFactory);

            await _repositorySegreteriaTecnica.InsertAsync(segreteriaTecnica, cancellationToken);

            return segreteriaTecnica;

        }

    }
}
