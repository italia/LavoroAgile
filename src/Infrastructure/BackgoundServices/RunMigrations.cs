using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.BackgoundServices
{
    /// <summary>
    /// Job per l'aggiornamento del database.
    /// </summary>
    public class RunMigrations : IHostedService
    {
        /// <summary>
        /// Riferimenti alle factory dei context da aggiornare.
        /// </summary>
        private readonly IDbContextFactory<AccordoContext> _accordiContextFactory;
        private readonly IDbContextFactory<StrutturaContext> _struttureContextFactory;

        // Riferimento al provider dei servizi (necessaria per risolvere il context
        // dell'identity che è scoped).
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Inizializza il job di aggiornamento database.
        /// </summary>
        /// <param name="accordiContextFactory">Factory del context degli accordi.</param>
        /// <param name="struttureContextFactory">Factory del context delle strutture.</param>
        /// <param name="identityDbContext">Context dell'identity.</param>
        public RunMigrations(
            IDbContextFactory<AccordoContext> accordiContextFactory,
            IDbContextFactory<StrutturaContext> struttureContextFactory,
            IServiceProvider serviceProvider) =>
                (_accordiContextFactory, _struttureContextFactory, _serviceProvider) =
                (accordiContextFactory, struttureContextFactory, serviceProvider);

        /// <summary>
        /// Avvia la migrazione.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var accordoDbContext = _accordiContextFactory.CreateDbContext();
            await accordoDbContext.Database.MigrateAsync(cancellationToken);
            await accordoDbContext.DisposeAsync();

            await using var struttureDbContext = _struttureContextFactory.CreateDbContext();
            await struttureDbContext.Database.MigrateAsync(cancellationToken);
            await struttureDbContext.DisposeAsync();

            // Avvia uno scope ed aggiorna il database dell'identity
            using var scope = _serviceProvider.CreateScope();
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            await identityContext.Database.MigrateAsync(cancellationToken);
            await identityContext.DisposeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
