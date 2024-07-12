using Domain;
using Domain.ExtensionMethods;
using Domain.Helpers;
using Domain.Model;
using Domain.Model.ExternalCommunications;
using Domain.Model.Identity;
using Domain.Model.Utilities;
using Domain.Settings;
using DotNetCore.CAP;
using Infrastructure.Factories;
using Infrastructure.Repositories.AccordiToDoHandlers;
using Infrastructure.Services.Factories;
using Infrastructure.Utils;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    /// <summary>
    /// Implementa la classe di servizio per la gestione degli accordi.
    /// </summary>
    public class AccordoService : IAccordoService
    {
        /// <summary>
        /// Repository per l'accesso ai dati sugli accordi.
        /// </summary>
        private readonly IRepository<Accordo, Guid> _repository;

        /// <summary>
        /// Repository per l'accesso ai dati sullo storico degli accori.
        /// </summary>
        private readonly IStoricoRepository<Guid, StatoAccordo> _storicoRepository;

        /// <summary>
        /// Servizio di comunicazione con le anagrafiche esterne.
        /// </summary>
        private readonly IPersonalDataProviderService _personalDataProviderService;

        private readonly IStrutturaService _strutturaService;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly ILogger<AccordoService> _logger;

        /// <summary>
        /// Riferimento alla factory di supporto alla creazione dei pacchetti dati da inviare.
        /// </summary>
        private readonly WorkingDaysAndActivitiesDataFactory _workingDaysAndActivitiesDataFactory;

        /// <summary>
        /// Riferimento al bus CAP.
        /// </summary>
        private readonly ICapPublisher _capBus;

        /// <summary>
        /// Riferimento settings mail
        /// </summary>
        private readonly MailSettings _mailSettings;

        private readonly IMinisteroLavoroService _ministeroLavoroService;

        /// <summary>
        /// Istanzia un nuovo <see cref="AccordoService"/>
        /// </summary>
        /// <param name="repository">Istanza del repository per l'accesso ai dati sugli accordi.</param>
        /// <param name="storicoRepository">Istanza del repository per l'accesso ai dati sullo storico stati</param>
        /// <param name="personalDataProviderService">Istanza del servizio per l'interazione con le anagrafiche esterne.</param>
        /// <param name="strutturaService">Istanza del servizio per l'accesso ai dati delle strutture.</param>
        public AccordoService(IRepository<Accordo, Guid> repository,
            IStoricoRepository<Guid, StatoAccordo> storicoRepository,
            IPersonalDataProviderService personalDataProviderService,
            IStrutturaService strutturaService,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<AccordoService> logger,
            WorkingDaysAndActivitiesDataFactory workingDaysAndActivitiesDataFactory,
            ICapPublisher capPublisher,
            IOptions<MailSettings> mailSettings,
            IMinisteroLavoroService ministeroLavoroService)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._storicoRepository = storicoRepository ?? throw new ArgumentNullException(nameof(storicoRepository));
            this._personalDataProviderService = personalDataProviderService ?? throw new ArgumentNullException(nameof(personalDataProviderService));
            this._strutturaService = strutturaService ?? throw new ArgumentNullException(nameof(strutturaService));
            this._serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._workingDaysAndActivitiesDataFactory = workingDaysAndActivitiesDataFactory;
            this._capBus = capPublisher;
            this._mailSettings = mailSettings?.Value;
            _ministeroLavoroService = ministeroLavoroService;
        }

        public async Task<Accordo> GetAccordoInCorsoAsync(Guid uid, CancellationToken cancellationToken)
        {
            return (await _repository.FindAsync(
                uid,
                a => DateTime.UtcNow >= a.DataInizioUtc.Date && DateTime.UtcNow.Date <= a.DataFineUtc.Date &&
                    a.Stato != StatoAccordo.RifiutataRA &&
                    a.Stato != StatoAccordo.RifiutataCI &&
                    a.Stato != StatoAccordo.RifiutataCS &&
                    a.Stato != StatoAccordo.AccordoConclusoCambioStrutturaDipendente &&
                    a.Stato != StatoAccordo.Recesso,
                cancellationToken: cancellationToken)).Entities?.FirstOrDefault();
        }

        /// <summary>
        /// Inizializza la richiesta di creazione accordo.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente che vuole registrare un accordo.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Accordo inizializzato</returns>
        public async Task<Accordo> InitCreateAccordoAsync(Guid uid, CancellationToken cancellationToken = default)
        {
            //Accordo da restituire
            Accordo accordo = new Accordo();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();

                var dipendente = await userManager.Users.FirstOrDefaultAsync(u => u.Id == uid);
                if (dipendente is null)
                {
                    throw new Exception("Impossibile recuperare il profilo utente");
                }

                //Valorizzazione dell'anagrafica
                await accordo.SetAnagrafica(dipendente.ToDipendente(), _personalDataProviderService, _strutturaService, userManager, cancellationToken);

            }

            return accordo;
        }

        public async Task<int> GetAccordiToDoAsync(Guid uid, string role, CancellationToken cancellationToken)
        {
            //Recupera tutti gli accordi sui quali ha visilibiltà il mio ruolo in base alle strutture di appartenenza
            IQueryable<Accordo> accordi = (await _repository.FindAsync(uid, role: role.ToEnum<RoleAndKeysClaimEnum>(), cancellationToken: cancellationToken)).Entities.AsQueryable();

            //Applica il filtro sulle cose da fare in base al ruolo
            accordi = AccordiToDoHandlerFactory.GetAccordiToDoHandler(role.ToEnum<RoleAndKeysClaimEnum>()).AccordiToDoFilter(accordi);

            return accordi.Count();
        }

        public async Task CreateAccordoAsync(Accordo accordo, CancellationToken cancellationToken)
        {
            // Se esiste un accordo in corso, non è possibile crearne un altro.
            if (await this.GetAccordoInCorsoAsync(accordo.Dipendente.Id, cancellationToken) != null)
            {
                throw new LavoroAgileException("Esiste già un accordo in corso.");
            }

            //Valorizzazione delle giornate di lavoro agile
            accordo.PianificazioneDateAccordo = CalcolaGiornateAccordo(accordo.Id, accordo.Modalita,
                                                                        accordo.DataInizioUtc,
                                                                        accordo.DataFineUtc,
                                                                        accordo.PianificazioneGiorniAccordo,
                                                                        accordo.PianificazioneDateAccordo);

            // Verifica ed eventualmente crea i profili utente per i dirigenti della
            // struttura cui è collegato l'accordo.
            await accordo.FindOrCreateManagersByUsernameAsync(_serviceScopeFactory);

            //Creazione dell'accordo
            await _repository.InsertAsync(accordo, cancellationToken);
        }

        public async Task<Accordo> GetAccordoAsync(Guid idAccordo, CancellationToken cancellationToken)
        {
            var accordo = await _repository.GetAsync(idAccordo, cancellationToken);

            // Se è attiva l'integrazione con il servizio di anagrafica esterna, imposta l'accordo 
            // come inizializzato da fonte esterna in modo da rendere non editabile la sezione anagrafica.
            accordo.InitializedFromExtenalService = this._personalDataProviderService.Enabled;

            //Verifica se l'accordo è da ripianificare
            VerificaRipianificazioneAccordoSottoscrizioneProponente(accordo);

            return accordo;
        }

        public async Task<int> UpdateAccordoAsync(Accordo accordo, CancellationToken cancellationToken, bool verificaNumeroMassimoGiornate = true)
        {
            //Valorizzazione delle giornate di lavoro agile
            accordo.PianificazioneDateAccordo = CalcolaGiornateAccordo(accordo.Id, accordo.Modalita,
                                                                        accordo.DataInizioUtc,
                                                                        accordo.DataFineUtc,
                                                                        accordo.PianificazioneGiorniAccordo,
                                                                        accordo.PianificazioneDateAccordo);

            // Aggiorna data modifica accordo
            accordo.EditTime = DateTime.UtcNow;

            // Verifica ed eventualmente crea i profili utente per i dirigenti della
            // struttura cui è collegato l'accordo.
            await accordo.FindOrCreateManagersByUsernameAsync(_serviceScopeFactory);

            return await _repository.UpdateAsync(accordo, cancellationToken);
        }

        public async Task<SearchResult<Accordo, Guid>> FindAsync(Guid userId, string userRole, AccordoSearch searchModel, CancellationToken cancellationToken = default)
        {
            var filter = PredicateBuilder.New<Accordo>();

            // Filtro su codice accordo.
            if (!string.IsNullOrWhiteSpace(searchModel.Codice))
            {
                // Pulizia codice, preleva solo la componente numerica.
                var codice = 0;
                int.TryParse(new String(searchModel.Codice.Where(Char.IsDigit).ToArray()), out codice);
                filter = filter.And(a => a.Codice == codice);
            }

            // Filtro su data inizio
            if (searchModel.DataDa.HasValue)
            {
                filter = filter.And(a => a.DataInizioUtc >= searchModel.DataDa);

            }

            // Filtro su data inizio
            if (searchModel.DataA.HasValue)
            {
                filter = filter.And(a => a.DataInizioUtc <= searchModel.DataA);

            }

            //Filtro su Accordi da Valutare
            //Un accordo e' da valutare se vi e' la DataNotaDipendente (che viene inizializzata quando il dipendente salva la sua richiesta)
            //e non c'e' la dataNotaDirigente che viene salvata dopo che il dirigente salva la sua valutazione
            //Inoltre va escluso l'eventuale accordo con richiesta di valutazione del responsabile stesso
            if (searchModel.daValutare)
            {
                filter = filter.And(a => a.DataNotaDipendente != null);
                filter = filter.And(a => a.DataNotaDirigente == null);
                filter = filter.And(a => !a.Dipendente.Id.Equals(userId));
            }

            // Filtro sugli stati. Se non ne sono stati selezionati, li seleziona tutti altrimenti
            // la ricerca non restituirebbe risultati.
            // Lo stato InCorso non è uno stato reale, ma calcolato, quindi se è
            // stato selezionato "InCorso" viene tolto dai filtri di stato.
            var hasInCorso = false;
            if (searchModel.Stati.Contains(StatoAccordo.InCorso))
            {
                searchModel.Stati.Remove(StatoAccordo.InCorso);
                searchModel.Stati.Add(StatoAccordo.Sottoscritto);
                hasInCorso = true;
            }
            if (searchModel.Stati.Any())
            {
                filter = filter.And(a => searchModel.Stati.Contains(a.Stato));
            }
            else
            {
                var stati = (StatoAccordo[])Enum.GetValues(typeof(StatoAccordo));
                filter = filter.And(a => stati.Contains(a.Stato));
            }

            // Se sono stati richiesti gli accordi in corso, aggiunge la condizione
            if (hasInCorso)
            {
                filter = filter.Or(a => a.Stato == StatoAccordo.RecessoPianificato || (a.Stato == StatoAccordo.Sottoscritto && DateTime.UtcNow.Date >= a.DataInizioUtc.Date && DateTime.UtcNow.Date <= a.DataFineUtc.Date));
            }

            //Filtro sul visto della segreteria tecnica 
            if (searchModel.VistoSegreteriaTecnica)
            {
                filter = filter.And(a => a.VistoSegreteriaTecnica == true);
            }

            //Filtro sul proponente dell'accordo
            if (!String.IsNullOrEmpty(searchModel.Proponente))
            {
                filter = filter.And(a => a.Dipendente.NomeCognome.ToLower().Contains(searchModel.Proponente.ToLower()));
            }

            //Filtro sul dipartimento (Segreteria Tecnica)
            if (!String.IsNullOrEmpty(searchModel.Dipartimento))
            {
                filter = filter.And(a => a.StrutturaUfficioServizio.ToLower().Contains(searchModel.Dipartimento.ToLower()));
            }

            return (await _repository.FindAsync(whereExpression: filter, userUid: userId, role: userRole.ToEnum<RoleAndKeysClaimEnum>(), page: searchModel.Page, pageSize: searchModel.PageSize, cancellationToken: cancellationToken));

        }

        public async Task RecediAsync(Guid userId, Guid accordoId, DateTime? recessoDate, string note, CancellationToken cancellationToken)
        {
            if (Guid.Empty.Equals(accordoId))
            {
                throw new ArgumentNullException(nameof(accordoId));
            }
            if (!recessoDate.HasValue)
            {
                throw new ArgumentNullException(nameof(recessoDate));
            }

            // Carica il dettaglio dell'accordo
            var accordo = await _repository.GetAsync(accordoId, cancellationToken);
            if (accordo == null)
            {
                throw new LavoroAgileException("Accordo non trovato");
            }

            // Se l'accordo è recidibile, ne passa lo stato a Recesso, rigenera la documentazione
            // ricalcola le date dell'accordo, invia un messaggio per la cancellazione delle
            // giornate ed aggiorna la data fine dell'accordo.
            if (accordo.Recidibile)
            {
                accordo.DataRecesso = recessoDate;
                accordo.DataFineUtc = recessoDate.Value;
                accordo.AddStoricoStato(note, StatoAccordo.RecessoPianificato, accordo.Dipendente.NomeCognome);
                accordo.Stato = StatoAccordo.RecessoPianificato;

                var newDates = ZucchettiCommonServices.GetDatesToDelete(accordo.PianificazioneDateAccordo.Split(","), recessoDate.Value);
                accordo.PianificazioneDateAccordo = String.Empty;
                if (newDates.toMaintain?.Count() > 0)
                {
                    accordo.PianificazioneDateAccordo = String.Join(", ", newDates.toMaintain);
                }

                await _repository.UpdateAsync(accordo, cancellationToken);
                if (newDates.toRemove.Count() > 0)
                {
                    WorkingDaysTransmission wdt = new WorkingDaysTransmission(accordo.Dipendente.Email);
                    wdt.Id = accordo.Id;
                    wdt.WorkingDays = newDates.toRemove.ToList();
                    _capBus.Publish(CAPChannels.DeleteDays, wdt);
                }

                //Viene pubblicato il messaggio per la comunicazione di recesso dell'accordo al Ministero del Lavoro
                _capBus.Publish(CAPChannels.RecessoComunicazioneMinisteroLavoro, accordo.Id);
            }
        }

        public async Task<ICollection<Guid>> UpdateAccordiToRecesso(string note, string autore, CancellationToken cancellationToken)
        {
            // Per evitare legami al nome della proprietà degli stati, preleva la prima proprietà
            // con tipo StatoAccordo.
            var statoProperty = typeof(Accordo).GetProperties().FirstOrDefault(p => p.PropertyType.Equals(typeof(StatoAccordo)));
            if (statoProperty == null)
            {
                throw new ArgumentException("Nessuna proprietà di tipo StatoAccordo trovata nell'oggetto Accordo");
            }

            // Aggiorna gli stati degli accordi.
            var updated = await _repository.UpdateByPropertyAsync(a => a.GetType().GetProperty(statoProperty.Name), actualValue: StatoAccordo.RecessoPianificato, newValue: StatoAccordo.Recesso, whereExpression: a => a.DataRecesso.HasValue && a.DataRecesso.Value.Date <= DateTime.UtcNow.Date);

            // Aggiunge una voce allo storico di tutti gli accordi modificati
            if (updated.Count > 0)
            {
                await _storicoRepository.AddStatoToStorico(updated, StatoAccordo.Recesso, note, autore, cancellationToken);

            }

            return updated;

        }

        public async Task<Guid> RinnovaAccordoAsync(Guid id, bool revisioneAccordo, CancellationToken cancellationToken)
        {
            //Clone dell'accordo
            Guid idAccordo = await _repository.CloneEntity(id, revisioneAccordo, cancellationToken);

            //Recupero dell'accordo
            Accordo accordo = await _repository.GetAsync(idAccordo, cancellationToken);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();

                //Valorizzazione dell'anagrafica con richiamo all'ERP
                await accordo.SetAnagrafica(accordo.Dipendente, _personalDataProviderService, _strutturaService, userManager, cancellationToken);

            }

            // Verifica ed eventualmente crea i profili utente per i dirigenti della
            // struttura cui è collegato l'accordo.
            await accordo.FindOrCreateManagersByUsernameAsync(_serviceScopeFactory);

            //Aggiornamento dell'accordo
            await _repository.UpdateAsync(accordo, cancellationToken);

            //In caso di revisione richiama un update sel service che effettua il ricalcolo delle giornate 
            if (revisioneAccordo)
            {
                //Accordo accordo = await _repository.GetAsync(idAccordo, cancellationToken);
                await UpdateAccordoAsync(accordo, cancellationToken);
            }

            return idAccordo;
        }

        public async Task DeleteAccordoAsync(Guid id, CancellationToken cancellationToken)
        {
            // Recupera l'accordo
            var accordo = await _repository.GetAsync(id, cancellationToken);

            // Se l'accordo è in uno stato diverso dalla bozza, non può essere cancellato.
            if (accordo.Stato != StatoAccordo.Bozza)
            {
                throw new LavoroAgileException("Impossibile cancellare l'accordo perché in uno stato diverso da Bozza");
            }

            // Svuota i riferimento all'accordo dall'eventuale padre e resetta la proprietà di revisione accordo
            await _repository.UpdateByPropertyAsync<Guid>(a => a.GetType().GetProperty("ChildId"), id, Guid.Empty, cancellationToken: cancellationToken);

            // Cancella l'accordo.
            await _repository.DeleteAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Difference>> GetDifferenzeAccordiAsync(Guid accordo1, Guid accordo2, CancellationToken cancellationToken)
        {
            if (Guid.Empty.Equals(accordo1))
            {
                throw new ArgumentNullException(nameof(accordo1));
            }
            if (Guid.Empty.Equals(accordo2))
            {
                throw new ArgumentNullException(nameof(accordo2));
            }

            // Recupera i dettagli degli accordi.
            var accordo1Details = await _repository.GetAsync(accordo1, cancellationToken);
            if (accordo1Details == null)
            {
                throw new LavoroAgileException($"Accordo {accordo1} non trovato");
            }

            var accordo2Details = await _repository.GetAsync(accordo2, cancellationToken);
            if (accordo2Details == null)
            {
                throw new LavoroAgileException($"Accordo {accordo2} non trovato");
            }

            // Recupera comparatore per gli accordi e compara i due oggetti.
            var factory = new AccordoComparersFactory();
            var comparer = factory.GetObjectsComparer<Accordo>();

            IEnumerable<Difference> differences;
            comparer.Compare(accordo1Details, accordo2Details, out differences);

            return differences;

        }

        /// <summary>
        /// Questo metodo serve a calcolare le giornate di lavoro agile di uno specifico accordo.
        /// Indipendentemente dalla modalità scelta per il lavoro agile (un giorno a settimana, due giorni a settimana, libera),
        /// vengono calcolate le giornate (date) di lavoro agile.
        /// Le giornate sono restituite come stringa di date separate da "," utile a valorizzare il campo "PianificazioneDateAccordo" 
        /// sia dell'accordo che della richiesta di modifica accordo.
        /// </summary>
        private string CalcolaGiornateAccordo(Guid idAccordo, string modalita,
                                            DateTime dataInizioAccordo,
                                            DateTime dataFineAccordo,
                                            string pianificazioneGiorniAccordo,
                                            string pianificazioneDateAccordo)
        {
            DateTime startDate = dataInizioAccordo;
            DateTime endDate = dataFineAccordo;

            var dates = new List<DateTime>();
            string result = String.Empty;

            try
            {
                switch (modalita.ToUpper())
                {
                    case "ORDINARIAUNGIORNO":
                        for (var dt = startDate.Date; dt <= endDate.Date; dt = dt.AddDays(1))
                        {
                            if (dt.ToString("dddd") == pianificazioneGiorniAccordo.ToLower())
                                dates.Add(dt);
                        }
                        result = string.Join(", ", dates.Select(x => x.ToString("dd/MM/yyyy")));
                        break;

                    case "ORDINARIADUEGIORNI":
                        string[] giorniSelezionati = pianificazioneGiorniAccordo.Split(",");
                        for (var dt = startDate.Date; dt <= endDate.Date; dt = dt.AddDays(1))
                        {
                            if (dt.ToString("dddd") == giorniSelezionati[0].ToLower() || dt.ToString("dddd") == giorniSelezionati[1].ToLower())
                                dates.Add(dt);
                        }
                        result = string.Join(", ", dates.Select(x => x.ToString("dd/MM/yyyy")));
                        break;

                    case "ECCEZIONALE":
                        List<DateTime> listPianificazioneDate = pianificazioneDateAccordo.Split(",").Select(date => DateTime.Parse(date)).ToList();

                        foreach (DateTime dataPianificata in listPianificazioneDate)
                        {
                            if (dataPianificata.Date >= dataInizioAccordo.Date && dataPianificata.Date <= dataFineAccordo.Date)
                                dates.Add(dataPianificata);
                        }
                        result = string.Join(", ", dates.Select(x => x.ToString("dd/MM/yyyy")));
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore CalcolaGiornateAccordo - accordo id: " + idAccordo + " " + ex.Message);
            }
            return result;
        }

        public async Task VerificaCondizioniPostCambioStato(Guid idAccordo, CancellationToken cancellationToken)
        {
            Accordo accordo = await _repository.GetAsync(idAccordo, cancellationToken);

            //Se lo stato è sottoscritto e la data inizio dell'accordo è minore della data di sottoscrizione
            //Va reimpostata la data di inizio accordo a quella di sottoscrizione 
            //Per la modalità eccezionale viene richiesta una ripianificazione prima quindi le date saranno corrette
            //Per le altre modalità il salvataggio farà scattare un ricalcolo corretto delle giornate
            try
            {
                if (accordo.Stato == StatoAccordo.Sottoscritto && accordo.DataInizioUtc.Date < DateTime.UtcNow.Date)
                {
                    //accordo.DataInizioUtc = DateTime.UtcNow;
                    accordo.DataInizioUtc = accordo.DataSottoscrizione.Value.Date;

                    //In caso di modalità non eccezionali si effettua il ricalcolo delle giornate altrimenti si impostano solo le date
                    //perchè in caso di modalità eccezionale eventualmente le date pianificate non cadevano nel nuovo intervallo 
                    //è stata richiesta una ripianificazione
                    if (!accordo.Modalita.ToUpper().Equals("ECCEZIONALE"))
                        await this.UpdateAccordoAsync(accordo, cancellationToken, false);
                    else
                        await _repository.UpdateAsync(accordo, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore verifica sottoscrizione dopo data inizio - accordo id: " + accordo.Id + " " + ex.Message);
            }

            //Se l'accordo è una revisione accordo deve mettere il padre nello stato di "ChiusoPerRevisioneAccordo"
            //ed impostare la data fine del padre con la data inizio del nuovo accordo meno un giorno
            //Contemporaneamente per l'accordo figlio va invalidata la proprietà di revisione accordo
            try
            {
                if (accordo.Stato == StatoAccordo.Sottoscritto && accordo.RevisioneAccordo)
                {
                    var accordoPadre = await _repository.GetAsync(accordo.ParentId, cancellationToken);
                    var accordoFiglio = await _repository.GetAsync(accordo.Id, cancellationToken);
                    accordoPadre.Stato = StatoAccordo.ChiusoPerRevisioneAccordo;
                    accordoPadre.AddStoricoStato(string.Empty, StatoAccordo.ChiusoPerRevisioneAccordo, "Sistema");

                    //Se la data calcolata per la fine dell'accordo padre è maggiore della sua data inizio allora viene impostata,
                    //altrimenti la data fine dell'accordo padre corrisponderà con la sua data inizio
                    DateTime dataFineAccordoPadreCalcolata = accordoFiglio.DataInizioUtc.AddDays(-1);
                    if (dataFineAccordoPadreCalcolata > accordoPadre.DataInizioUtc)
                        accordoPadre.DataFineUtc = dataFineAccordoPadreCalcolata;
                    else
                        accordoPadre.DataFineUtc = accordoPadre.DataInizioUtc;

                    await _repository.UpdateAsync(accordoPadre, cancellationToken);

                    accordoFiglio.RevisioneAccordo = false;
                    await _repository.UpdateAsync(accordoFiglio, cancellationToken);

                    //Viene pubblicato il messaggio per la comunicazione di recesso dell'accordo padre al Ministero del Lavoro
                    _capBus.Publish(CAPChannels.RecessoComunicazioneMinisteroLavoro, accordoPadre.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore verifica accordo revisionato chiusura padre - accordo id: " + accordo.Id + " " + ex.Message);
            }

            //Se l'accordo è una revisione accordo ed è nello stato RIFIUTATO RESPONSABILE ACCORDO o RIFIUTATO CAPO INTERMEDIO o RIFIUTATO CAPO STRUTTURA
            //allora se presente va interrotta la relazione padre figlio liberando l'accordo padre.
            //In questa condizione chiaramente l'accordo rifiutato rimane orfano nello stato di rifiuto.
            try
            {
                if ((accordo.Stato == StatoAccordo.RifiutataRA ||
                    accordo.Stato == StatoAccordo.RifiutataCI ||
                    accordo.Stato == StatoAccordo.RifiutataCS) &&
                    !Guid.Empty.Equals(accordo.ParentId))
                {
                    var accordoPadre = await _repository.GetAsync(accordo.ParentId, cancellationToken);
                    var accordoFiglio = await _repository.GetAsync(accordo.Id, cancellationToken);
                    accordoPadre.ChildId = Guid.Empty;
                    accordoPadre.RevisioneAccordo = false;

                    await _repository.UpdateAsync(accordoPadre, cancellationToken);

                    accordoFiglio.RevisioneAccordo = false;
                    accordoFiglio.ParentId = Guid.Empty;
                    await _repository.UpdateAsync(accordoFiglio, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore verifica accordo revisionato interruzione relazione padre figlio - accordo id: " + accordo.Id + " " + ex.Message);
            }
        }

        private void VerificaRipianificazioneAccordoSottoscrizioneProponente(Accordo accordo)
        {
            if (accordo.DataInizioUtc.Date < DateTime.UtcNow.Date &&
                accordo.Modalita.ToUpper().Equals("ECCEZIONALE") &&
                accordo.Stato == StatoAccordo.DaSottoscrivereP)
            {
                if (DateTime.UtcNow.VerificaPianificazione(accordo.DataFineUtc, accordo.PianificazioneDateAccordo))
                {
                    accordo.DataInizioUtc = DateTime.UtcNow;
                    accordo.Ripianificare = true;
                }
            }
        }

        /// <summary>
        /// Chiude l'accordo precedente non più in corso
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ChiusuraAccordoPrecedente(Guid id, CancellationToken cancellationToken)
        {
            Accordo accordo = await _repository.GetAsync(id);

            //Verifica dell'esistenza di accordi precedenti SOTTOSCRITTI
            //con data fine minore o uguale della data inizio di questo nuovo accordo sottoscritto
            List<Accordo> accordiPrecedentiSottoscritti = (await _repository.FindAsync(
                                                                    accordo.Dipendente.Id,
                                                                    a => a.Stato == StatoAccordo.Sottoscritto &&
                                                                    a.DataFineUtc.Date <= accordo.DataInizioUtc.Date,
                                                                    RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE
                                                                    )).Entities.ToList<Accordo>();

            //In caso positivo gli accordi vengono messi in uno stato di CONCLUSO
            if (accordiPrecedentiSottoscritti != null && accordiPrecedentiSottoscritti.Count() > 0)
            {
                foreach (Accordo a in accordiPrecedentiSottoscritti)
                {
                    Accordo accordoDaConcludere = await _repository.GetAsync(a.Id, cancellationToken);
                    accordoDaConcludere.Stato = StatoAccordo.AccordoConcluso;
                    await _repository.UpdateAsync(accordoDaConcludere);
                }
            }
        }

        public async Task RecediGiustificatoMotivoAsync(Guid userId, Guid accordoId, DateTime dataRecessoGiustificatoMotivo, string note, string giustificatoMotivo, CancellationToken cancellationToken)
        {
            //accordoId non valorizzato
            if (Guid.Empty.Equals(accordoId))
            {
                throw new ArgumentNullException(nameof(accordoId));
            }

            //Carica il dettaglio dell'accordo
            var accordo = await _repository.GetAsync(accordoId, cancellationToken);
            if (accordo == null)
            {
                throw new LavoroAgileException("Accordo non trovato");
            }

            // Separa date da mantenere da date da cancellare
            var newDates = ZucchettiCommonServices.GetDatesToDelete(accordo.PianificazioneDateAccordo.Split(","), dataRecessoGiustificatoMotivo.Date);

            //Aggiorna lo stato, la data di recesso, la data fine dell'accordo, lo storico e le date pianificate
            try
            {
                accordo.Stato = StatoAccordo.Recesso;
                accordo.DataRecesso = dataRecessoGiustificatoMotivo.Date;
                accordo.DataFineUtc = dataRecessoGiustificatoMotivo.Date;
                accordo.PianificazioneDateAccordo = String.Empty;
                if (newDates.toMaintain?.Count() > 0)
                {
                    accordo.PianificazioneDateAccordo = String.Join(", ", newDates.toMaintain);
                }

                //Note complete, alle note nello storico si aggiunge il giustificato motivo per il recesso
                string noteComplete = note + " - Giustificato motivo di recesso: " + giustificatoMotivo;

                //Aggiornamento dello storico
                if (userId.Equals(accordo.Dipendente.Id))
                    accordo.AddStoricoStato(noteComplete, StatoAccordo.Recesso, accordo.Dipendente.NomeCognome);
                else
                    accordo.AddStoricoStato(noteComplete, StatoAccordo.Recesso, accordo.ResponsabileAccordo.NomeCognome);

                //Aggiornamento dell'accordo
                await _repository.UpdateAsync(accordo, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante l'aggiornamento dell'accordo in recesso - accordo id: " + accordo.Id + " " + ex.Message);
                throw new LavoroAgileException("Problemi durante l'aggiornamento dell'accordo.");
            }

            //Pubblica il messaggio per comunicare a zucchetti l'eliminazione delle date
            try
            {
                if (newDates.toRemove.Count() > 0)
                {
                    WorkingDaysTransmission wdt = new WorkingDaysTransmission(accordo.Dipendente.Email);
                    wdt.Id = accordo.Id;
                    wdt.WorkingDays = newDates.toRemove.ToList();
                    _capBus.Publish(CAPChannels.DeleteDays, wdt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante la pubblicazione del messaggio CAP per la cancellazione delle date dell'accordo in recesso - accordo id: " + accordo.Id + " " + ex.Message);
            }

            //Pubblica il messaggio per l'invio delle comunicazioni al responsabile accordo ed al proponente
            try
            {
                if (accordo.Dipendente.Id.Equals(userId))
                {
                    //Recesso del proponente, notifica al responsabile con proponente in CC
                    Email notificaResponsabile = new Email();
                    notificaResponsabile.destinatari.Add(accordo.ResponsabileAccordo.Email);
                    notificaResponsabile.destinatariCC.Add(accordo.Dipendente.Email);
                    notificaResponsabile.oggetto = "Notifica recesso accordo nr. " + accordo.Codice;
                    notificaResponsabile.testoEmail = String.Format(_mailSettings.TestoEmailNotificaRecesso, accordo.Dipendente.NomeCognome, accordo.Dipendente.Email, dataRecessoGiustificatoMotivo.Date, accordo.Codice);

                    _capBus.Publish(CAPChannels.SendEmail, notificaResponsabile);
                }
                else
                {
                    //Recesso del responsabile, notifica al proponente con responsabile in CC
                    Email notificaProponente = new Email();
                    notificaProponente.destinatari.Add(accordo.Dipendente.Email);
                    notificaProponente.destinatariCC.Add(accordo.ResponsabileAccordo.Email);
                    notificaProponente.oggetto = "Notifica recesso accordo nr. " + accordo.Codice;
                    notificaProponente.testoEmail = String.Format(_mailSettings.TestoEmailNotificaRecesso, accordo.ResponsabileAccordo.NomeCognome, accordo.ResponsabileAccordo.Email, dataRecessoGiustificatoMotivo.Date, accordo.Codice);

                    _capBus.Publish(CAPChannels.SendEmail, notificaProponente);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante la pubblicazione del messaggio CAP per l'invio delle notifiche dell'accordo in recesso - accordo id: " + accordo.Id + " " + ex.Message);
            }

            //Viene pubblicato il messaggio per la comunicazione di recesso dell'accordo al Ministero del Lavoro
            try
            {
                _capBus.Publish(CAPChannels.RecessoComunicazioneMinisteroLavoro, accordo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante la pubblicazione del messaggio CAP per la comunicazione di recesso al Ministero del Lavoro - accordo id: " + accordo.Id + " " + ex.Message);
            }
        }

        public async Task ExecuteAmmOperation(Guid idAccordo, OperazioniAmmAccordo operazione, CancellationToken cancellationToken)
        {
            Accordo accordo = await _repository.GetAsync(idAccordo, cancellationToken);

            switch (operazione)
            {
                case OperazioniAmmAccordo.EliminazioneValutazioniAccordo:
                    accordo.DataNotaDipendente = null;
                    accordo.DataNotaDirigente = null;
                    accordo.NotaDipendente = null;
                    accordo.NotaDirigente = null;

                    //Update accordo
                    await _repository.UpdateAsync(accordo, cancellationToken);
                    break;

                case OperazioniAmmAccordo.EliminazioneValutazioneResponsabileAccordo:
                    accordo.DataNotaDirigente = null;
                    accordo.NotaDirigente = null;

                    //Update accordo
                    await _repository.UpdateAsync(accordo, cancellationToken);
                    break;

                case OperazioniAmmAccordo.RiportareInBozzaAccordo:
                    //Questa operazione è consentita solo su accordo in determinati stati
                    if (accordo.Stato == StatoAccordo.Sottoscritto ||
                        accordo.Stato == StatoAccordo.InCorso ||
                        accordo.Stato == StatoAccordo.AccordoConProposteModifica ||
                        accordo.Stato == StatoAccordo.AccordoConcluso ||
                        accordo.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente ||
                        accordo.Stato == StatoAccordo.Recesso ||
                        accordo.Stato == StatoAccordo.RecessoPianificato ||
                        accordo.Stato == StatoAccordo.ChiusoPerRevisioneAccordo
                    )
                    {
                        throw new LavoroAgileException("L'operazione selezionata non è possibile per l'accordo in questo stato");
                    }

                    //Update stato
                    accordo.Stato = StatoAccordo.Bozza;

                    //Update accordo
                    await _repository.UpdateAsync(accordo, cancellationToken);

                    //Eliminazione storico
                    await _storicoRepository.DeleteStoricoAccordo(accordo.Id, cancellationToken);

                    //Eliminazione WF Instance
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _workFlowService = scope.ServiceProvider.GetService<IWorkflowService>();
                        await _workFlowService.DeleteInstance(accordo.Id.ToString(), cancellationToken);
                    }
                    break;

                case OperazioniAmmAccordo.EliminazioneAccordo:
                    //Questa operazione è consentita solo su accordo in determinati stati
                    if (accordo.Stato == StatoAccordo.Sottoscritto ||
                        accordo.Stato == StatoAccordo.InCorso ||
                        accordo.Stato == StatoAccordo.AccordoConProposteModifica ||
                        accordo.Stato == StatoAccordo.AccordoConcluso ||
                        accordo.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente ||
                        accordo.Stato == StatoAccordo.Recesso ||
                        accordo.Stato == StatoAccordo.RecessoPianificato ||
                        accordo.Stato == StatoAccordo.ChiusoPerRevisioneAccordo
                    )
                    {
                        throw new LavoroAgileException("L'operazione selezionata non è possibile per l'accordo in questo stato");
                    }

                    //Se l'accordo ha un padre sgancio il padre da questo accordo
                    if (accordo.ParentId != Guid.Empty)
                    {
                        Accordo accordoPadre = await _repository.GetAsync(accordo.ParentId, cancellationToken);

                        //Imposto i campi per sganciare l'accordo padre
                        accordoPadre.ChildId = Guid.Empty;
                        accordoPadre.RevisioneAccordo = false;

                        //Update accordo padre
                        await _repository.UpdateAsync(accordoPadre, cancellationToken);
                    }

                    //Se l'accordo ha un figlio sgancio il figlio da questo accordo
                    if (accordo.ChildId != Guid.Empty)
                    {
                        Accordo accordoFiglio = await _repository.GetAsync(accordo.ChildId, cancellationToken);

                        //Imposto i campi per sganciare l'accordo padre
                        accordoFiglio.ParentId = Guid.Empty;

                        //Update accordo figlio
                        await _repository.UpdateAsync(accordoFiglio, cancellationToken);
                    }

                    //Delete accordo
                    await _repository.DeleteAsync(idAccordo, cancellationToken);

                    //Eliminazione WF Instance
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _workFlowService = scope.ServiceProvider.GetService<IWorkflowService>();
                        await _workFlowService.DeleteInstance(accordo.Id.ToString(), cancellationToken);
                    }
                    break;
                case OperazioniAmmAccordo.ReinviareInformazioniAZucchetti:
                    // Operazione consentita solo se l'accordo è uno stato da sottoscritto in poi.
                    if (accordo.Stato == StatoAccordo.AccordoConcluso ||
                        accordo.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente ||
                        accordo.Stato == StatoAccordo.ChiusoPerRevisioneAccordo ||
                        accordo.Stato == StatoAccordo.InCorso ||
                        accordo.Stato == StatoAccordo.Recesso ||
                        accordo.Stato == StatoAccordo.RecessoPianificato ||
                        accordo.Stato == StatoAccordo.Sottoscritto
                    )
                    {
                        // Recupera l'accordo di riferimento.
                        accordo = await GetAccordoAsync(idAccordo, cancellationToken);
                        if (accordo != null)
                        {
                            // Reinvia giornate lavorate
                            var wdt = _workingDaysAndActivitiesDataFactory.GetWorkingDaysTransmission(accordo);
                            await _capBus.PublishAsync(CAPChannels.SendDays, wdt);

                            // Reinvia attività
                            var wat = _workingDaysAndActivitiesDataFactory.GetWorkingActivitiesTransmission(accordo);
                            await _capBus.PublishAsync(CAPChannels.SendActivities, wat);
                        }
                    }
                    break;
                case OperazioniAmmAccordo.CancellazioneGiornate:
                    // Operazione consentita se sono presenti informazioni di trasmissione.
                    if (accordo.Transmission != null)
                    {
                        await _capBus.PublishAsync(CAPChannels.DeleteDaysRemediation, accordo.Id, cancellationToken: cancellationToken);
                    }
                    break;

                case OperazioniAmmAccordo.ReinvioComunicazioneMinisteroLavoro:
                    if (accordo.Stato == StatoAccordo.Sottoscritto && accordo.InCorso)
                    {
                        await _ministeroLavoroService.ReinvioComunicazioneMinisteroLavoro(accordo.Id, cancellationToken);
                    }
                    break;

            }
        }

        public async Task<int> GetToDoValutazioniAsync(Guid uid, string role, CancellationToken cancellationToken)
        {
            int numeroAccordi = 0;

            //Recupera tutti gli accordi sui quali ha visilibiltà il mio ruolo in base alle strutture di appartenenza che sono da valutare per il responsabile accordo
            if (role.Equals(RoleAndKeysClaimEnum.KEY_CLAIM_RESPONSABILE_ACCORDO.ToDescriptionString()))
            {
                IQueryable<Accordo> accordi = (await _repository.FindAsync(uid, role: role.ToEnum<RoleAndKeysClaimEnum>(), cancellationToken: cancellationToken)).Entities.AsQueryable();

                //Applica il filtro sulle cose da fare in base al ruolo
                accordi = AccordiToDoHandlerFactory.GetAccordiToDoHandler(role.ToEnum<RoleAndKeysClaimEnum>()).AccordiToDoValutazioniFilter(accordi, uid);

                numeroAccordi = accordi.Count();
            }

            return numeroAccordi;
        }

        public async Task<SearchResult<Accordo, Guid>> GetAccordiForRole(Guid uid, string role, CancellationToken cancellationToken)
        {
            SearchResult<Accordo, Guid> accordi = (await _repository.FindAsync(uid, null, role.ToEnum<RoleAndKeysClaimEnum>(), cancellationToken));

            return accordi;
        }

        public async Task RifiutaDeroga(Guid userId, Guid accordoId, CancellationToken cancellationToken)
        {
            //AccordoId non valorizzato
            if (Guid.Empty.Equals(accordoId))
            {
                throw new ArgumentNullException(nameof(accordoId));
            }

            //Carica il dettaglio dell'accordo
            var accordo = await _repository.GetAsync(accordoId, cancellationToken);
            if (accordo == null)
            {
                throw new LavoroAgileException("Accordo non trovato");
            }

            //Annulla la pianificazione dell'accordo elo riporta nello stato di bozza
            try
            {
                accordo.Stato = StatoAccordo.Bozza;
                accordo.PianificazioneDateAccordo = String.Empty;

                //Aggiornamento dell'accordo
                await _repository.UpdateAsync(accordo, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante il rifiuto della deroga dell'accordo - accordo id: " + accordo.Id + " " + ex.Message);
                throw new LavoroAgileException("Problemi durante il rifiuto della deroga dell'accordo.");
            }              
        }
    }
}
