using Domain.Model;
using Domain.Model.ExternalCommunications;
using Domain.Model.Identity;
using Domain.Settings;
using Infrastructure.BackgoundServices;
using Infrastructure.Identity;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Savorboard.CAP.InMemoryMessageQueue;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Infrastructure
{
    public static class ServicesRegistrationExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IStrutturaRepository<Struttura, Guid>, StrutturaRepository>();

#if UseFaker
            serviceCollection.AddSingleton<IRepository<Accordo, Guid>, Repositories.Fakes.AccordoRepositoryFake>();
#else
            serviceCollection.AddSingleton<IRepository<Accordo, Guid>, AccordoRepository>();
#endif

            serviceCollection.AddSingleton<ISegreteriaTecnicaRepository<SegreteriaTecnica, Guid>, SegreteriaTecnicaRepository>();
            serviceCollection.AddSingleton<IStoricoRepository<Guid, StatoAccordo>, StoricoAccordoRepository>();

            serviceCollection.AddSingleton<ISimpleRepository<TransmissionStatus>, TransmissionRepository>();

            serviceCollection.AddSingleton<IAccordoService, AccordoService>();
            serviceCollection.AddSingleton<ISegreteriaTecnicaService, SegreteriaTecnicaService>();

            serviceCollection.AddSingleton<WorkingDaysAndActivitiesDataFactory>();
            serviceCollection.AddSingleton<IPersonalDataProviderService, ZucchettiPersonalDataProviderService>();
            serviceCollection.AddSingleton<ISendWorkingDaysAndActivities, ZucchettiWDAndASender>();
            serviceCollection.AddSingleton<IDeleteWorkingDaysAndRestrictActivities, ZucchettiWDAndADeleter>();
            serviceCollection.AddSingleton<ZucchettiCommonServices>();
            serviceCollection.AddSingleton<ISendWDAndAStatusLogger, DatabaseSendWDAndAStatusLogger>();

            serviceCollection.AddScoped<IDocumentsService, DocumentsService>();
            serviceCollection.AddScoped<IWorkflowService, WorkflowService>();
            serviceCollection.AddScoped<IAccountService, AccountService>();

            // Registrazione del formattatore del codice accordo.
            serviceCollection.AddSingleton<CodiceAccordoFormatter>();

            // Registrazione servizio gestione email
            serviceCollection.AddSingleton<IMailService, MailService>();

            //Registrazione servizi Ministero del lavoro
            serviceCollection.AddSingleton<IMinisteroLavoroStatusLogger, DataBaseMinisteroLavoroStatusLogger>();
            serviceCollection.AddSingleton<IMinisteroLavoroService, MinisteroLavoroService>();

            // Registrazione delle configurazioni
            serviceCollection.Configure<ZucchettiServiceSettings>(configuration.GetSection("ZucchettiServiceSettings"));
            serviceCollection.Configure<DescriptiveCodeSettings>(configuration);
            serviceCollection.Configure<DocumentsServiceSettings>(configuration.GetSection("DocumentsServiceSettings"));
            serviceCollection.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            serviceCollection.Configure<MinisteroLavoroServicesSettings>(configuration.GetSection("MinisteroLavoroServicesSettings"));
            serviceCollection.Configure<AdminUserSettings>(configuration.GetSection(AdminUserSettings.Position));


            //Registrazione dinamica del sevizio di gestione strutture a partire dal parametro di configurazione "StruttureService"
            //"Infrastructure.Services.DatabaseStruttureService, Infrastructure" - Le strutture sono gestite internamente su DB
            //"Infrastructure.Services.ZucchettiStruttureService, Infrastructure" - Le strutture sono gestite tramite servizi esposti dall'ERP Zucchetti
            //Per una eventuale implementazione di un ulteriore servizio per le strutture, estendere l'interfaccia "IStruttureService" e configurare il parametro dell'appsettings.json
            Type type = Type.GetType(configuration.GetSection("StruttureService").Value);
            serviceCollection.AddSingleton(typeof(IStrutturaService), type);

            // Servizio di background per l'aggiornamento del database.
            if (!string.IsNullOrWhiteSpace(configuration["MigrationJobEnabled"]))
            {
                bool jobEnabled = false;
                bool.TryParse(configuration["MigrationJobEnabled"], out jobEnabled);
                if (jobEnabled)
                {
                    serviceCollection.AddHostedService<RunMigrations>();

                }

            }

            // Servizio CAP per la gestione della transazione distribuita
            // (utilizzata nell'integrazione con Zucchetti).
            serviceCollection.AddCap(x =>
            {
                // Sql server come sistema di storage.
                x.UseSqlServer(configuration.GetConnectionString("CAPConnection"));

                // Memoria come trasporto.
                x.UseInMemoryMessageQueue();

                // Carica impostazione CAP.
                var capSettings = new CAPSettings();
                configuration.GetSection(CAPSettings.Position).Bind(capSettings);

                // Tempo fra un tentativo ed il successivo a partire dal quarto tentativo di invio
                // (da documentazione i primi 3 tentativi sono immediati).
                x.FailedRetryInterval = capSettings.FailedRetryInterval;

                // Numero massimo di tentativi.
                x.FailedRetryCount = capSettings.FailedRetryCount;

            });
            

            return serviceCollection;
        }

    }
}
