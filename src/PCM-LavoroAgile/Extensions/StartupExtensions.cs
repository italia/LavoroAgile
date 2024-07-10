using Domain.Model.Identity;
using Domain.Model.Utilities;
using Domain.Settings;
using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.SqlServer;
using Elsa.Providers.Workflows;
using FluentStorage;
using Infrastructure.Context;
using Infrastructure.Identity;
using Infrastructure.Workflow.Activities;
using Infrastructure.Workflow.Scripting.JavaScript;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace PCM_LavoroAgile.Extensions
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

            // Configurazione dell'intervallo di rivalidazione del security timestamp.
            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMilliseconds(validationInterval));

            //Aggiunta della Factory per i claims personalizzati
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AdditionalUserClaimsPrincipalFactory>();

            // Registra i servizi per l'autenticazione con LDAP.
            if (!string.IsNullOrWhiteSpace(configuration["Identity:Uri"]))
            {
                services.Configure<LdapSettings>(configuration.GetSection("Identity"));
                services.AddScoped<ILdapService, LdapService>();
                services.AddScoped<LdapSignInService>();

            }

            //Aggiunta delle policy di autorizzazione per ruoli e claims
            services.AddAuthorization(options =>
            {
                foreach (var roleAndKeysClaimEnum in (RoleAndKeysClaimEnum[])Enum.GetValues(typeof(RoleAndKeysClaimEnum)))
                {
                    var description = roleAndKeysClaimEnum.ToDescriptionString();
                    options.AddPolicy(description, policy => policy.RequireClaim(description));
                }

                // Policy che consente l'accesso al controller di ricerca degli accordi.
                options.AddPolicy("AccordiSearch", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Value == RoleAndKeysClaimEnum.CAPO_INTERMEDIO.ToDescriptionString() ||
                            c.Value == RoleAndKeysClaimEnum.UTENTE.ToDescriptionString())));

                // Policy che consente l'accesso alla action per la sottoscrizione ed il recesso dell'accordo.
                options.AddPolicy("AccordiRecediSottoscrivi", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Value == RoleAndKeysClaimEnum.RESPONSABILE_ACCORDO.ToDescriptionString() ||
                            c.Value == RoleAndKeysClaimEnum.UTENTE.ToDescriptionString())));

                // Policy che consente l'accesso alla action per approvazione/rifiuto/richiesta integrazione.
                options.AddPolicy("AccordiApprovaRifiutaIntegra", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Value == RoleAndKeysClaimEnum.RESPONSABILE_ACCORDO.ToDescriptionString() ||
                            c.Value == RoleAndKeysClaimEnum.CAPO_INTERMEDIO.ToDescriptionString() ||
                            c.Value == RoleAndKeysClaimEnum.CAPO_STRUTTURA.ToDescriptionString())));

                // Policy che consente l'utilizzo delle funzionalità di richiesta di modifica di un accordo
                options.AddPolicy("AccordiRichiestaModifica", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Value == RoleAndKeysClaimEnum.RESPONSABILE_ACCORDO.ToDescriptionString() ||
                            c.Value == RoleAndKeysClaimEnum.UTENTE.ToDescriptionString())));                
            });

            return services;
        }

        /// <summary>
        /// Aggiunge e configura il motore di workflow.
        /// </summary>
        /// <param name="services">Collezione dei servizi.</param>
        /// <param name="configuration">Configurazione applicativa.</param>
        /// <returns>Collezione dei servizi aggiornata.</returns>
        public static IServiceCollection AddElsaWorkflowServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJavaScriptTypeDefinitionProvider<CustomTypeDefinitionProvider>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var elsaSection = configuration.GetSection("Elsa");

            // Elsa services.
            services
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlServer(connectionString, migrationsAssemblyMarker: null))
                    .AddConsoleActivities()
                    .AddEmailActivities(configuration.GetSection("MailSettings").Bind)
                    .AddQuartzTemporalActivities()
                    .AddJavaScriptActivities()
                    .AddActivitiesFrom<GetAccordoActivity>()
                    .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                );

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();

            // Recupera il path dell'assembly corrente
            var currentAssemblyPath = Path.GetDirectoryName(typeof(StartupExtensions).Assembly.Location);

            // Configura lo storage per BlobStorageWorkflowProvider con una directory su disco in modo da
            // consentire il caricamento dei workflow da file json.
            services.Configure<BlobStorageWorkflowProviderOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(Path.Combine(currentAssemblyPath, "Workflows")));


            return services;

        }
    }
}
