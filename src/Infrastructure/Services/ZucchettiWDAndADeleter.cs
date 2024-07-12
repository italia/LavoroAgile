using Domain;
using Domain.Model;
using Domain.Model.ExternalCommunications;
using Domain.Model.Zucchetti;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ZucchettiCensimentoAttivita;

namespace Infrastructure.Services
{
    /// <summary>
    /// Implementa il servizio di cancellazione giornate e giornate lavorative verso Zucchetti.
    /// </summary>
    public class ZucchettiWDAndADeleter : IDeleteWorkingDaysAndRestrictActivities
    {
        /// <summary>
        /// Riferimento al logger per il servizio.
        /// </summary>
        private readonly ILogger<ZucchettiWDAndADeleter> _logger;

        /// <summary>
        /// Impostazione per la connessione ai servizi Zucchetti.
        /// </summary>
        private readonly ZucchettiServiceSettings _zucchettiServiceSettings;

        /// <summary>
        /// Riferimento al servizio per il recupero informazioni utente. Utilizza l'istanza
        /// specifica di zucchetti, perché ha necessità di recuperare l'id employee.
        /// </summary>
        private readonly ZucchettiPersonalDataProviderService _zucchettiPersonalDataProviderService;

        /// <summary>
        /// Servizi di utilità comune.
        /// </summary>
        private readonly ZucchettiCommonServices _zucchettiCommonServices;

        /// <summary>
        /// Repository degli accordi.
        /// </summary>
        private readonly IRepository<Accordo, Guid> _accordoRepository;

        public bool Enabled => this._zucchettiServiceSettings.Valid;

        /// <summary>
        /// Configurazione serializer JSON per formattazione compatibile UTF-8.
        /// </summary>
        private readonly static JsonSerializerOptions javascriptJsonSerializerOptions = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        /// <summary>
        /// Inizializza un nuovo <see cref="ZucchettiWDAndADeleter"/>.
        /// </summary>
        /// <param name="settings">Impostazioni del servizio.</param
        /// <param name="logger">Riferimento al logger</param>
        /// <param name="personalDataProviderService">Servizio per il recupero dell'identificativo utente.</param>
        /// <param name="zucchettiCommonServices">Servizi di utilità comune.</param>
        /// <param name="accordoRepository">Repository per l'accesso alle informazioni sugli accordi.</param>
        public ZucchettiWDAndADeleter(IOptions<ZucchettiServiceSettings> settings, ILogger<ZucchettiWDAndADeleter> logger, IPersonalDataProviderService personalDataProviderService, ZucchettiCommonServices zucchettiCommonServices, IRepository<Accordo, Guid> accordoRepository)
            => (_zucchettiServiceSettings, _logger, _zucchettiPersonalDataProviderService, _zucchettiCommonServices, _accordoRepository) = (settings?.Value, logger, personalDataProviderService as ZucchettiPersonalDataProviderService, zucchettiCommonServices, accordoRepository);
        
        public async Task<IEnumerable<DateTime>> DeleteWorkingDays(WorkingDaysTransmission workingDays)
        {
            // Se l'integrazione non è attiva, non procede
            if (!this.Enabled)
            {
                return default;
            }

            // Recupera gli identificativi delle giornate da eliminare contenute all'interno dell'intervallo
            // di date specifico.
            var convertedDates = workingDays.WorkingDays.Select(x => DateTime.ParseExact(x.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));

            // Se la lista delle date non contiene dati, torna
            if (!convertedDates.Any())
            {
                return default;
            }

            var minDate = convertedDates.Min();
            var maxDate = convertedDates.Max();
            var (daysId, daysDate) = await _zucchettiCommonServices.GetWorkingDaysId(workingDays.UserId, minDate, maxDate, convertedDates);

            _logger.LogInformation($"Cancellazione giustificativi Zucchetti, trovate {daysId.Count()} giornate");

            // Cancellazione delle singole giornate
            foreach (var workingDay in daysId)
            {
                await _zucchettiCommonServices.DeleteWorkingDay(workingDays.UserId, workingDay);
            }

            return daysDate;

        }

        public async Task<IEnumerable<DateTime>> CalculateAndDeleteWorkingDaysAsync(Guid accordoId, CancellationToken cancellationToken)
        {
            if (!Enabled)
            {
                return default;
            }

            if (Guid.Empty.Equals(accordoId))
            {
                throw new ArgumentException(nameof(accordoId));
            }

            _logger.LogDebug("{0} - Avvio elaborazione", nameof(CalculateAndDeleteWorkingDaysAsync));

            // 1. Recupero del dettaglio dell'accordo accordoId
            var accordoDetails = await _accordoRepository.GetAsync(accordoId, cancellationToken);

            // 2. Costruisce elenco delle date degli accordi.
            var dates = (await this._accordoRepository.FindAsync(accordoDetails.Dipendente.Id, a => a.DataInizioUtc >= accordoDetails.DataInizioUtc, Domain.Model.Utilities.RoleAndKeysClaimEnum.KEY_CLAIM_UTENTE, cancellationToken))?.Entities
                .Select(a => a.PianificazioneDateAccordo.Split(",")).SelectMany(d => d)
                .Union(accordoDetails.PianificazioneDateAccordo.Split(","))
                .Select(x => DateTime.ParseExact(x.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));

            _logger.LogDebug("{0} - Individuate {1} date nel sistema.", nameof(CalculateAndDeleteWorkingDaysAsync), dates?.Count());

            // 3. Prelevo la data più piccola
            var minDate = dates.Min();

            // 4. Richiede a Zucchetti tutte le date a partire dalla data di inizio dell'accordo
            // da lavorare
            var zucchettiDates = await _zucchettiCommonServices.GetWorkingDaysId(accordoDetails.Dipendente.Email, minDate, DateTime.MaxValue);

            _logger.LogDebug("{0} - Individuate {1} date in Zucchetti.", nameof(CalculateAndDeleteWorkingDaysAsync), zucchettiDates.daysDate?.Count());

            // 5. Elimina dall'elenco di date di Zucchetti, quelle appartenenti agli accordi
            var datesToRemoves = zucchettiDates.daysDate?.Except(dates);

            _logger.LogDebug("{0} - Individuate {1} date da eliminare.", nameof(CalculateAndDeleteWorkingDaysAsync), datesToRemoves?.Count());

            // 6. Elimina giornate.
            var delResult =  await DeleteWorkingDays(new WorkingDaysTransmission
            {
                UserId = accordoDetails.Dipendente.Email,
                WorkingDays = datesToRemoves.Select(x => x.ToString("dd/MM/yyyy"))?.ToList()
            });

            return delResult;

        }

        public async Task RestrictActivities(WorkingActivityTransmission workingActivityTransmission)
        {
            // Se l'integrazione non è attiva, non procede
            if (!this.Enabled)
            {
                return;
            }

            // Recupera l'identificativo del sottoscrittore dell'accordo.
            var idEmployee = await _zucchettiPersonalDataProviderService.GetIdEmployee(workingActivityTransmission.UserId);

            // Se non è stato ricevuto un ed valido, non prosegue.
            if (string.IsNullOrWhiteSpace(idEmployee))
            {
                throw new LavoroAgileException("Codice dipendente non valido");
            }

            // Generazione del codice della macro-categoria delle attività specifiche di accordo.
            var specActCode = $"{workingActivityTransmission.Code}-001";

            // Aggiorna le informazioni sulle attività.
            await UpdateActivities(workingActivityTransmission.UserId, workingActivityTransmission.Code, specActCode, workingActivityTransmission.StartDate, workingActivityTransmission.EndDate, workingActivityTransmission.Activities, workingActivityTransmission.WorkingDaysCount);

            // Cancella la visibilità della commessa
            await _zucchettiCommonServices.DeleteActivityUserAssociation(workingActivityTransmission.UserId, workingActivityTransmission.Code);

            // Ri-linka l'utente alla commessa sul nuovo intervallo di date.
            // Invio della relazione utente-commessa.
            await _zucchettiCommonServices.SendUserWorklistAssociation(workingActivityTransmission.UserId, idEmployee, workingActivityTransmission.Code, workingActivityTransmission.StartDate, workingActivityTransmission.EndDate);

        }

        #region Invocazione servizi Zucchetti

        /// <summary>
        /// Modifica la durata dell'attività di accordo ad un nuovo intervallo.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente</param>
        /// <param name="code">Codice accordo.</param>
        /// <param name="relatedActCode">Codice da assegnare alle attività specifiche di accordo.</param>
        /// <param name="startDate">Data di inizio dell'accordo</param>
        /// <param name="endDate">Data di fine dell'accordo</param>
        /// <param name="activityDetails">Dettagli delle attività da inviare.</param>
        /// <param name="workingDaysCount">Numero di giornate lavorative.</param>
        /// <returns></returns>
        private async Task UpdateActivities(string uid, string code, string relatedActCode, string startDate, string endDate, List<WorkingActivityTransmission.ActivityDetails> activityDetails, int workingDaysCount)
        {
            // Servizio fwsztsanag
            var binding = htht_fwsztsanagWSClient.GetDefaultBinding();
            var endpoint = new EndpointAddress(this._zucchettiServiceSettings.CreateActivitiesUrl);
            using (var service = new htht_fwsztsanagWSClient(binding, endpoint))
            {
                try
                {
                    var convertedStartDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    var convertedEndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                    // Creazione dell'oggetto con le informazioni da inviare al servizio
                    // di censimento.
                    var anagrafica = new ZTAnagrafica
                    {
                        // Accordo.
                        activit_mstd = new List<ZTAccordo> { new ZTAccordo { company = "000000", activity = code, dsent = _zucchettiServiceSettings.SmartWorkingActivityDescription, dtstartvl = convertedStartDate, dtendvl = convertedEndDate, extcode = workingDaysCount.ToString() } },

                        // Commessa (crea solo quella per le attività specifiche di accordo).
                        jobordr_mstd = new List<ZTMacrocategoria> { new ZTMacrocategoria { company = "000000", joborder = relatedActCode, dsent = _zucchettiServiceSettings.SmartWorkingRelatedWorklistDescription, dtstartvl = convertedStartDate, dtendvl = convertedEndDate } },

                        // Attività (crea una attività per ogni attività specifica dell'accordo.
                        entity1_mstd = activityDetails?.Select(a => new ZTAttivita { company = "000000", entity1 = a.Code, dsent = a.Description, dtstartvl = convertedStartDate, dtendvl = convertedEndDate }).ToList(),

                    };

                    var result = await service.htht_fwsztsanag_RunAsync(
                        _zucchettiServiceSettings.WUsername,
                        _zucchettiServiceSettings.WPassword,
                        _zucchettiServiceSettings.Company,
                        JsonSerializer.Serialize(anagrafica, javascriptJsonSerializerOptions),
                        "S",
                        "S");

                    var response = JsonSerializer.Deserialize<CreateActivitiesReturn>(result.Body.@return);

                    // Se l'operazione è andata male, solleva eccezione.
                    if (response.status == "KO")
                    {
                        _logger.LogError(new LavoroAgileException(response.message), $"Servizio aggiornamento attività zucchetti per utenza {uid}.");
                        throw new LavoroAgileException(response.message);
                    }

                    // Se ci sono errori, restiuisce una eccezione
                    if (response.errors?.Count > 0)
                    {
                        _logger.LogError(new LavoroAgileException(JsonSerializer.Serialize(response.errors, javascriptJsonSerializerOptions)), $"Servizio aggiornamento attività zucchetti per utenza {uid}.");

                        throw new LavoroAgileException(response.errors.First().msg);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio aggiornamento attività zucchetti per utenza {uid}.");
                    throw;
                }
            }
        }

        #endregion
    }
}
