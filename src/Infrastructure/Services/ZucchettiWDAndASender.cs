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
using System.Threading.Tasks;
using ZucchettiCensimentoAttivita;
using ZucchettiGiorLavorServiceReference;

namespace Infrastructure.Services
{
    /// <summary>
    /// Implementa l'invio delle giornate lavorative e delle attività
    /// verso Zucchetti.
    /// </summary>
    public class ZucchettiWDAndASender : ISendWorkingDaysAndActivities
    {
        /// <summary>
        /// Riferimento al logger per il servizio.
        /// </summary>
        private readonly ILogger<ZucchettiWDAndASender> _logger;

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
        /// Riferimento al servizio per la cancellazione delle informazioni su accordi.
        /// </summary>
        private readonly ZucchettiWDAndADeleter _zucchettiWDAndADeleter;

        /// <summary>
        /// Servizi di utilità comune.
        /// </summary>
        private readonly ZucchettiCommonServices _zucchettiCommonServices;

        /// <summary>
        /// Configurazione serializer JSON per formattazione compatibile UTF-8.
        /// </summary>
        private readonly static JsonSerializerOptions javascriptJsonSerializerOptions = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        public bool Enabled => this._zucchettiServiceSettings.Valid;

        /// <summary>
        /// Inizializza un nuovo <see cref="ZucchettiWDAndASender"/>.
        /// </summary>
        /// <param name="settings">Impostazioni del servizio.</param
        /// <param name="logger">Riferimento al logger</param>
        /// <param name="personalDataProviderService">Servizio per il recupero delle informazioni utente</param>
        /// <param name="zucchettiWDAndADeleter">Servizio per la cancellazione delle informazioni</param>
        public ZucchettiWDAndASender(IOptions<ZucchettiServiceSettings> settings, ILogger<ZucchettiWDAndASender> logger, IPersonalDataProviderService personalDataProviderService, IDeleteWorkingDaysAndRestrictActivities zucchettiWDAndADeleter, ZucchettiCommonServices zucchettiCommonServices) =>
            (_zucchettiServiceSettings, _zucchettiPersonalDataProviderService, _logger, _zucchettiWDAndADeleter, _zucchettiCommonServices) =
            (settings?.Value, personalDataProviderService as ZucchettiPersonalDataProviderService, logger, zucchettiWDAndADeleter as ZucchettiWDAndADeleter, zucchettiCommonServices);

        public async Task SendActivities(WorkingActivityTransmission workingActivityTransmission)
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

            // Se ci sono informazioni su un accordo precedente
            if (workingActivityTransmission.OldAccordo != null)
            {
                await _zucchettiWDAndADeleter.RestrictActivities(workingActivityTransmission.OldAccordo);

            }

            // Generazione del codice della macro-categoria delle attività specifiche di accordo.
            var specActCode = $"{workingActivityTransmission.Code}-001";

            // Invia le informazioni sulle attività.
            await SendActivities(workingActivityTransmission.UserId, workingActivityTransmission.Code, specActCode, workingActivityTransmission.StartDate, workingActivityTransmission.EndDate, workingActivityTransmission.Activities, workingActivityTransmission.WorkingDaysCount);

            // Linka fra di loro le attività.
            await _zucchettiCommonServices.LinkActivities(workingActivityTransmission.UserId, workingActivityTransmission.Code, specActCode, workingActivityTransmission.Activities.Select(a => a.Code).ToList());

            // Invio della relazione utente-commessa.
            await _zucchettiCommonServices.SendUserWorklistAssociation(workingActivityTransmission.UserId, idEmployee, workingActivityTransmission.Code, workingActivityTransmission.StartDate, workingActivityTransmission.EndDate);

        }

        /// <summary>
        /// Registrato ai messaggi sul canale SendDays, si occupa di gestire l'invio delle
        /// giornate lavorative a Zucchetti.
        /// </summary>
        /// <param name="workingDays"></param>
        /// <returns></returns>
        public async Task SendWorkingDays(WorkingDaysTransmission workingDays)
        {
            // Se l'integrazione non è attiva, non procede
            if (!this.Enabled)
            {
                return;
            }

            // Se ci sono informazioni su un accordo precedente, deve prima procedere con la cancellazione
            // delle sue giornate.
            if (workingDays.OldAccordo != null)
            {
                await this._zucchettiWDAndADeleter.DeleteWorkingDays(workingDays.OldAccordo);
            }

            // Recupera l'identificativo del sottoscrittore dell'accordo.
            var idEmployee = await _zucchettiPersonalDataProviderService.GetIdEmployee(workingDays.UserId);

            // Se non è stato ricevuto un ed valido, non prosegue.
            if (string.IsNullOrWhiteSpace(idEmployee))
            {
                throw new LavoroAgileException("Codice dipendente non valido");
            }

            // Invio delle singole giornate
            var errors = new List<string>();
            foreach (var workingDay in workingDays.WorkingDays)
            {
                errors.AddRange(await SendWorkingDay(workingDays.UserId, idEmployee, workingDay));
            }

            // Se si è verificato almeno un errore, l'array errors conterrà almeno un elemento.
            // In questo caso calcola lo unique degli errori e restituisce l'elenco di errori
            // separati da virgola.
            if (errors.Any())
            {
                throw new LavoroAgileException(string.Join(", ", errors.Distinct()));
            }

        }

        public async Task DeleteWorkingDays(WorkingDaysTransmission workingDays)
        {
            // Se l'integrazione non è attiva, non procede
            if (!this.Enabled)
            {
                return;
            }

            await this._zucchettiWDAndADeleter.DeleteWorkingDays(workingDays);

        }

        #region Invocazioni servizi Zucchetti

        /// <summary>
        /// Invia una singola giornata lavorativa a Zucchetti.
        /// </summary>
        /// <param name="uid">Identificativo del dipendente</param>
        /// <param name="idEmployee">Codice dipendente 0-filled a 7 caratteri.</param>
        /// <param name="date">Data da inviare.</param>
        /// <returns>Lista con i messaggi di esito dell'invio (sarà vuota nel caso in cui l'invio è stato effettuato correttamente, conterrà un errore nel caso opposto).</returns>
        private async Task<IEnumerable<string>> SendWorkingDay(string uid, string idEmployee, string date)
        {
            // Servizio fsendjustify
            var binding = hfwe_fsendjustifyWSClient.GetDefaultBinding();
            var endpoint = new EndpointAddress(this._zucchettiServiceSettings.SendJustifyUrl);
            using (var service = new hfwe_fsendjustifyWSClient(binding, endpoint))
            {
                try
                {
                    // Le date a sistema sono salvate nel formato dd/MM/yyyy, Zucchetti le vuole
                    // nel formato yyyy-mm-dd.
                    var convertedDate = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                    var response = await service.hfwe_fsendjustify_RunAsync(
                        _zucchettiServiceSettings.WUsername,
                        _zucchettiServiceSettings.WPassword,
                        _zucchettiServiceSettings.Company,
                        _zucchettiServiceSettings.SmartWorkingCode,
                        _zucchettiServiceSettings.Company,
                        idEmployee,
                        convertedDate,
                        convertedDate,
                        "00:00",
                        "00:00",
                        0,
                        _zucchettiServiceSettings.SmartWorkingReason,
                        "N",
                        "N",
                        "S",
                        "N",
                        "N",
                        "N",
                        string.Empty);

                    var result = JsonSerializer.Deserialize<SendDaysReturn>(response.Body.@return);

                    // Se l'esito è KO ed il codice errore è 0715, significa che esiste già
                    // un giustificativo identico per quella data, se è 0756 significa che non è consentito
                    // inserire giustificativi per quel giorno (ad esempio perché è un festivo), se è 0744 significa
                    // che è stata superata la scadenza per effettuare la richiesta
                    // quindi lo considera come ok
                    if (result.status == "KO" && result.errorCode != "0715" && result.errorCode != "0756" && result.errorCode != "0744")
                    {
                        _logger.LogError(new LavoroAgileException(result.errorDescription), $"Servizio invio giornate lavorative zucchetti per utenza {uid}.");
                        return new List<string> { result.errorDescription };
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio invio giornate lavorative zucchetti per utenza {uid}.");
                    return new List<string> { "Errore non identificato nell'interazione con il servizio" };
                }

            }

            return new List<string>();
        }

        /// <summary>
        /// Invia l'elenco delle attività e il record dell'accordo.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente</param>
        /// <param name="code">Codice accordo.</param>
        /// <param name="relatedActCode">Codice da assegnare alle attività specifiche di accordo.</param>
        /// <param name="startDate">Data di inizio dell'accordo</param>
        /// <param name="endDate">Data di fine dell'accordo</param>
        /// <param name="activityDetails">Dettagli delle attività da inviare.</param>
        /// <param name="workingDaysCount">Numero di giorni lavorativi</param>
        /// <returns></returns>
        private async Task SendActivities(string uid, string code, string relatedActCode, string startDate, string endDate, List<WorkingActivityTransmission.ActivityDetails> activityDetails, int workingDaysCount)
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
                        entity1_mstd = activityDetails?.OrderBy(a => a.Code).Select(a => new ZTAttivita { company = "000000", entity1 = a.Code, dsent = a.Description, dtstartvl = convertedStartDate, dtendvl = convertedEndDate }).ToList(),

                    };

                    var result = await service.htht_fwsztsanag_RunAsync(
                        _zucchettiServiceSettings.WUsername,
                        _zucchettiServiceSettings.WPassword,
                        _zucchettiServiceSettings.Company,
                        JsonSerializer.Serialize(anagrafica, javascriptJsonSerializerOptions),
                        "N",
                        "S");

                    var response = JsonSerializer.Deserialize<CreateActivitiesReturn>(result.Body.@return);

                    // Se l'operazione è andata male, solleva eccezione.
                    if (response.status == "KO")
                    {
                        _logger.LogError(new LavoroAgileException(response.message), $"Servizio invio attività zucchetti per utenza {uid}.");
                        throw new LavoroAgileException(response.message);
                    }

                    // Se ci sono errori, restiuisce una eccezione nel caso in cui
                    // ci sia almeno un errore diverso da "Chiave già utilizzata.".
                    if (response.errors != null && response.errors.Any(e => e.msg != "Chiave già utilizzata."))
                    {
                        _logger.LogError(new LavoroAgileException(JsonSerializer.Serialize(response.errors, javascriptJsonSerializerOptions)), $"Servizio invio attività zucchetti per utenza {uid}.");

                        throw new LavoroAgileException(response.errors.First(e => e.msg != "Chiave già utilizzata.").msg);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio invio attività zucchetti per utenza {uid}.");
                    throw;
                }
            }
        }

        #endregion

    }

}