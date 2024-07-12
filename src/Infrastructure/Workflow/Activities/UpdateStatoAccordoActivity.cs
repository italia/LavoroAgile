using Domain.Model;
using Domain.Model.ExternalCommunications;
using DotNetCore.CAP;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Infrastructure.Services;
using Infrastructure.Services.Factories;
using Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Workflow.Activities
{
    /// <summary>
    /// Definisce l'activity che consente di caricare il dettaglio di un accordo.
    /// </summary>
    [Action(Category = "Lavoro agile", Description = "Aggiorna lo stato di un accordo sulla base dati.", DisplayName = "Aggiorna stato accordo")]
    public class UpdateStatoAccordoActivity : Activity
    {
        /// <summary>
        /// Repository per l'accesso agli accordi.
        /// </summary>
        private readonly IRepository<Accordo, Guid> _repository;

        /// <summary>
        /// Repository per lo storico degli accordi.
        /// </summary>
        private readonly IStoricoRepository<Guid, StatoAccordo> _storicoRepository;

        /// <summary>
        /// Riferimento al bus CAP.
        /// </summary>
        private readonly ICapPublisher _capBus;

        /// <summary>
        /// Riferimento al servizio per la gestione degli accordi.
        /// </summary>
        private readonly IAccordoService _accordoService;

        /// <summary>
        /// Riferimento alla factory di supporto alla creazione dei pacchetti dati da inviare.
        /// </summary>
        private readonly WorkingDaysAndActivitiesDataFactory _workingDaysAndActivitiesDataFactory;

        /// <summary>
        /// Inizializza una nuova <see cref="UpdateStatoAccordoActivity"/>.
        /// </summary>
        /// <param name="repository">Repository per l'accesso agli accordi.</param>
        /// <param name="capPublisher">Riferimento al bus CAP.</param>
        /// <param name="accordoService">Riferimento al servizio di gestione degli accordi</param>
        /// <param name="storicoRepository">Riferimento al servizio di gestione dello storico.</param>
        /// <param name="workingDaysAndActivitiesDataFactory">Riferimento alla factory per la generazione dei pacchetti dati</param>
        public UpdateStatoAccordoActivity(IRepository<Accordo, Guid> repository, ICapPublisher capPublisher, IStoricoRepository<Guid, StatoAccordo> storicoRepository, IAccordoService accordoService, WorkingDaysAndActivitiesDataFactory workingDaysAndActivitiesDataFactory)
        {
            _repository = repository;
            _capBus = capPublisher;
            _storicoRepository = storicoRepository;
            _accordoService = accordoService;
            _workingDaysAndActivitiesDataFactory = workingDaysAndActivitiesDataFactory;
        }

        /// <summary>
        /// Identificativo dell'accordo da aggiornare.
        /// </summary>
        [ActivityInput(
            Label = "ID Accordo",
            Hint = "ID dell'accordo da caricare.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public Guid IdAccordo { get; set; } = default!;

        /// <summary>
        /// Stato del documento.
        /// </summary>
        [ActivityInput(
            Label = "Stato",
            Hint = "Stato da impostare.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public StatoAccordo Stato { get; set; } = StatoAccordo.Bozza;

        /// <summary>
        /// Autore del cambiamento di stato.
        /// </summary>
        [ActivityInput(
            Label = "Autore",
            Hint = "Autore del cambio di stato",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string Autore { get; set; }

        /// <summary>
        /// Eventuali note sull'assegnazione di stato.
        /// </summary>
        [ActivityInput(
            Label = "Note",
            Hint = "Note sull'assegnazione di stato.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public string Note { get; set; }

        /// <summary>
        /// Carica il dettaglio di un accordo, ne aggiorna lo stato e lo salva.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var accordo = await _repository.GetAsync(IdAccordo, context.CancellationToken);
            accordo.Stato = this.Stato;
            accordo.AddStoricoStato(this.Note, this.Stato, this.Autore);

            await _repository.UpdateAsync(accordo, context.CancellationToken);

            //Verifica condizioni post sottoscrizione
            await _accordoService.VerificaCondizioniPostCambioStato(IdAccordo, context.CancellationToken);

            //Se l'accordo è sottoscritto pubblica il messaggio per l'eventuale chiusura del precedente accordo
            if (this.Stato == StatoAccordo.Sottoscritto)
                _capBus.Publish(CAPChannels.ChiusuraAccordoPrecedente, accordo.Id);

            // Se lo stato è sottoscritto, pubblica i messaggi per inviare attività e giornate lavorative.
            // Sarebbe meglio farlo in transazione, ma al momento i repos offrono metodi per farlo.
            if (this.Stato == StatoAccordo.Sottoscritto)
            {
                // Request per l'invio delle attività e delle giornate.
                var wdt = _workingDaysAndActivitiesDataFactory.GetWorkingDaysTransmission(accordo);
                var wat = _workingDaysAndActivitiesDataFactory.GetWorkingActivitiesTransmission(accordo);
                
                // Se c'è un accordo padre chiuso per revisione accordo si deve procedere con una
                // cancellazione delle giornate non fruite del vecchio e
                // conseguente restringimento della validità delle attività del vecchio.
                if (!Guid.Empty.Equals(accordo.ParentId))
                {
                    var accordoPadre = await _repository.GetAsync(accordo.ParentId, context.CancellationToken);

                    var infoAccordoPrecedente = this.CheckGiornate(accordo, accordoPadre);

                    wat.OldAccordo = infoAccordoPrecedente?.Item1;
                    wdt.OldAccordo = infoAccordoPrecedente?.Item2;

                }

                _capBus.Publish(CAPChannels.SendActivities, wat);
                _capBus.Publish(CAPChannels.SendDays, wdt);

            }

            //Se l'accordo è sottoscritto, pubblica in messaggio per l'invio di una nuova comunicazione al Ministero del Lavoro
            if (this.Stato == StatoAccordo.Sottoscritto)
                _capBus.Publish(CAPChannels.NuovaComunicazioneMinisteroLavoro, accordo.Id);
            
            return Done();
        }

        /// <summary>
        /// Calcola le attività da restringere e l'elenco delle date da eliminare
        /// per l'accordo vecchio.
        /// </summary>
        /// <param name="accordo">Nuovo accordo</param>
        /// <param name="accordoPadre">Vecchio accordo.</param>
        /// <returns></returns>
        private Tuple<WorkingActivityTransmission, WorkingDaysTransmission> CheckGiornate(Accordo accordo, Accordo accordoPadre)
        {
            // Se almeno uno degli accordi è nullo o se l'accordo padre non è nello stato
            // chiuso per revisione accordo, non va lavorato.
            if (accordo == null || accordoPadre == null || accordoPadre.Stato != StatoAccordo.ChiusoPerRevisioneAccordo)
            {
                return null;
            }

            // Crea l'oggetto con le informazioni sulle modifiche da fare alle attività
            // dell'accordo padre.
            var wat = _workingDaysAndActivitiesDataFactory.GetWorkingActivitiesTransmission(accordoPadre);

            // Crea l'oggetto con le informazioni sulle eliminazioni di giornate da fare per
            // l'accordo padre.
            var wdt = _workingDaysAndActivitiesDataFactory.GetWorkingDaysTransmission(accordoPadre);

            // L'elenco delle date da eliminare equivale alle date dell'accordo
            // padre cui devono essere eliminate tutte quelle maggiori della data di
            var dateAccordoPadre = accordoPadre.PianificazioneDateAccordo
                .Split(',');

            // Calcola il numero totale di giornate prima della pulizia.
            var totalDays = dateAccordoPadre.Count();

            dateAccordoPadre = dateAccordoPadre.Select(x => x.Trim())
                // Converte a data
                .Select(x => DateTime.ParseExact(x, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                // Seleziona tutte le date superiori alla data fine
                .Where(x => x > accordoPadre.DataFineUtc)
                // Riporta a string
                .Select(x => x.ToString("dd/MM/yyyy"))?.ToArray();

            wdt.WorkingDays = dateAccordoPadre.ToList() ?? new List<string>();

            // Aggiorna il numero di giornate lavorative
            wat.WorkingDaysCount = totalDays - wdt.WorkingDays.Count;

            return Tuple.Create(wat, wdt);

        }
    }
}
