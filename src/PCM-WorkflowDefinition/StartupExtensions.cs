using Domain.Model.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCM_WorkflowDefinition
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssemblyName = typeof(Startup).Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Configurazione del database per la gestione identità
            services.AddDbContext<IdentityContext>(options =>
                options
                    .UseSqlServer(connectionString,
                    sqlServerOptionsAction =>
                    {
                        sqlServerOptionsAction.MigrationsAssembly(migrationsAssemblyName);
                        sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);

                    }));

            // Registra db factory per le strtture
            services.AddPooledDbContextFactory<StrutturaContext>(x => x.UseSqlServer(connectionString,
                    sqlServerOptionsAction =>
                    {
                        sqlServerOptionsAction.MigrationsAssembly(migrationsAssemblyName);
                        sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);

                    }));

            // Registra db factory per gli accordi
            services.AddPooledDbContextFactory<AccordoContext>(x => x.UseSqlServer(connectionString,
                    sqlServerOptionsAction =>
                    {
                        sqlServerOptionsAction.MigrationsAssembly(migrationsAssemblyName);
                        sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);

                    }));

            return services;
        }
        
        /// <summary>
        /// Aggiunge e configura il servizio di identity.
        /// </summary>
        /// <param name="services">Collezione dei servizi.</param>
        /// <param name="configuration">Configurazione applicativa.</param>
        /// <param name="validationInterval">Intervallo di validazione del cookie utente. Default 30 minuti.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration, int validationInterval = 1800000)
        {
            services.AddIdentity<AppUser, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            return services;
        }

    }
}
