using Domain;
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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ZucchettiCancGiorServiceReference;
using ZucchettiCensimentoAttivita;
using ZucchettiGetGiorServiceReference;
using ZucchettiRelazioneAttivita;

namespace Infrastructure.Services
{
    /// <summary>
    /// Servizi Zucchetti di utilità comune.
    /// </summary>
    public class ZucchettiCommonServices
    {
        /// <summary>
        /// Riferimento al logger per il servizio.
        /// </summary>
        private readonly ILogger<ZucchettiCommonServices> _logger;

        /// <summary>
        /// Impostazione per la connessione ai servizi Zucchetti.
        /// </summary>
        private readonly ZucchettiServiceSettings _zucchettiServiceSettings;

        /// <summary>
        /// Configurazione serializer JSON per formattazione compatibile UTF-8.
        /// </summary>
        private readonly static JsonSerializerOptions javascriptJsonSerializerOptions = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        /// <summary>
        /// Inizializza un nuovo <see cref="ZucchettiCommonServices"/>.
        /// </summary>
        /// <param name="settings">Configurazione della connessione verso Zucchetti.</param>
        /// <param name="logger">Logger per il servizio</param>
        public ZucchettiCommonServices(IOptions<ZucchettiServiceSettings> settings, ILogger<ZucchettiCommonServices> logger)
            => (_zucchettiServiceSettings, _logger) = (settings?.Value, logger);

        /// <summary>
        /// Cancella l'associazione utente-commessa.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente</param>
        /// <param name="code">Codice accordo.</param>
        /// <returns></returns>
        public async Task DeleteActivityUserAssociation(string uid, string code)
        {
            // Servizio fwsztsanag
            var binding = htht_fwsztsanagWSClient.GetDefaultBinding();
            var endpoint = new EndpointAddress(this._zucchettiServiceSettings.CreateActivitiesUrl);
            using (var service = new htht_fwsztsanagWSClient(binding, endpoint))
            {
                try
                {

                    // Cancellazione dell'associazione utente-commessa
                    var ass = new ZTAnagrafica
                    {
                        attr_mstd = new List<ZTAccordoUtente>
                        {
                            new ZTAccordoUtente
                            {
                                company = "000000", 
                                ident = int.Parse(code).ToString("000000000000000"), 
                                dtstartvl = "1800-01-01", 
                                dtendvl = "2999-12-31", 
                                compdiscr = _zucchettiServiceSettings.Company, 
                                flstatus = "d1" 
                            } 
                        }
                    };

                    var result = await service.htht_fwsztsanag_RunAsync(
                        _zucchettiServiceSettings.WUsername,
                        _zucchettiServiceSettings.WPassword,
                        _zucchettiServiceSettings.Company,
                        JsonSerializer.Serialize(ass, javascriptJsonSerializerOptions),
                        "N",
                        "S");

                    var response = JsonSerializer.Deserialize<CreateActivitiesReturn>(result.Body.@return);

                    // Se l'operazione è andata male, solleva eccezione.
                    if (response.status == "KO")
                    {
                        _logger.LogError(new LavoroAgileException(response.message), $"Servizio cancellazione associazione utente-commessa zucchetti per utenza {uid}.");
                        throw new LavoroAgileException(response.message);
                    }

                    // Se ci sono errori, restiuisce una eccezione
                    if (response.errors?.Count > 0)
                    {
                        _logger.LogError(new LavoroAgileException(JsonSerializer.Serialize(response.errors, javascriptJsonSerializerOptions)), $"Servizio cancellazione associazione utente-commessa zucchetti per utenza {uid}.");

                        throw new LavoroAgileException(response.errors.First().msg);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio cancellazione associazione utente-commessa zucchetti per utenza {uid}.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Rende invisibile l'associazione utente-commessa.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente</param>
        /// <param name="code">Codice accordo.</param>
        /// <param name="startDate">Data di inizio dell'accordo</param>
        /// <param name="endDate">Data di fine dell'accordo</param>
        /// <returns></returns>
        public async Task SetActivityUserAssociationInvisible(string uid, string code, string startDate, string endDate)
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

                    // Nasconde la relazione utente-commessa
                    var ass = new ZTAnagrafica
                    {
                        attr_mstd = new List<ZTAccordoUtente> { new ZTAccordoUtente { company = "000000", ident = int.Parse(code).ToString("000000000000000"), dtstartvl = convertedStartDate, dtendvl = convertedEndDate, compdiscr = _zucchettiServiceSettings.Company, tpattr = "N" } }
                    };

                    var result = await service.htht_fwsztsanag_RunAsync(
                        _zucchettiServiceSettings.WUsername,
                        _zucchettiServiceSettings.WPassword,
                        _zucchettiServiceSettings.Company,
                        JsonSerializer.Serialize(ass, javascriptJsonSerializerOptions),
                        "N",
                        "S");

                    var response = JsonSerializer.Deserialize<CreateActivitiesReturn>(result.Body.@return);

                    // Se l'operazione è andata male, solleva eccezione.
                    if (response.status == "KO")
                    {
                        _logger.LogError(new LavoroAgileException(response.message), $"Servizio impostazione associazione utente-commessa a invisibile zucchetti per utenza {uid}.");
                        throw new LavoroAgileException(response.message);
                    }

                    // Se ci sono errori, restiuisce una eccezione nel caso in cui
                    // ci sia almeno un errore diverso da "Attribuzione già definita per il periodo.".
                    if (response.errors != null && response.errors.Any(e => e.msg != "Attribuzione già definita per il periodo"))
                    {
                        _logger.LogError(new LavoroAgileException(JsonSerializer.Serialize(response.errors, javascriptJsonSerializerOptions)), $"Servizio impostazione associazione utente-commessa a invisibile zucchetti per utenza {uid}.");

                        throw new LavoroAgileException(response.errors.First(e => e.msg != "Attribuzione già definita per il periodo").msg);
                    }

                    // Se è stato restituito un errore "Attribuzione già definita per il periodo",
                    // elimina l'associazione e ritenta.
                    if (response.errors != null && response.errors.Any(e => e.msg.Equals("Attribuzione già definita per il periodo")))
                    {
                        await DeleteActivityUserAssociation(uid, code);
                        await SetActivityUserAssociationInvisible(uid, code, startDate, endDate);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio impostazione associazione utente-commessa a invisibile zucchetti per utenza {uid}.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Collega le attività alle macro-categorie ed all'accordo.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente.</param>
        /// <param name="code">Codice accordo.</param>
        /// <param name="relatedActCode">Codice della macro-categoria Attività di accordo.</param>
        /// <param name="activityCode">Lista dei codici della attività da linkare.</param>
        /// <returns></returns>
        public async Task LinkActivities(string uid, string code, string relatedActCode, List<string> activityCode)
        {
            // Servizio fwsztsrelstruct
            var binding = htht_fwsztsrelstructWSClient.GetDefaultBinding();
            var endpoint = new EndpointAddress(this._zucchettiServiceSettings.CreateRelUrl);
            using (var service = new htht_fwsztsrelstructWSClient(binding, endpoint))
            {
                try
                {
                    // Creazione legame attività libera - commessa (attività dell'accordo - macro-categoria "Attività di accordo".
                    var links = activityCode.Select(a => new ZTRelation { company = "000000", entk4 = a, joborder = relatedActCode, activity = code }).ToList();

                    // Creazione legame commessa - accordo (macro-categorie - accordo)
                    // Attività non programmabili
                    links.Add(new ZTRelation { company = "000000", joborder = _zucchettiServiceSettings.CodeActNotProgrammable, activity = code });
                    // Attività di formazione
                    links.Add(new ZTRelation { company = "000000", joborder = _zucchettiServiceSettings.CodeActTraining, activity = code });
                    
                    // Attività specifiche di accordo
                    links.Add(new ZTRelation { company = "000000", joborder = relatedActCode, activity = code });

                    JsonSerializerOptions options = new()
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };


                    var result = await service.htht_fwsztsrelstruct_RunAsync(
                        _zucchettiServiceSettings.WUsername,
                        _zucchettiServiceSettings.WPassword,
                        _zucchettiServiceSettings.Company,
                        JsonSerializer.Serialize(links, options));

                    var response = JsonSerializer.Deserialize<CreateRelReturn>(result.Body.@return);

                    // Se l'operazione è andata male, solleva eccezione.
                    if (response.status == "KO")
                    {
                        _logger.LogError(new LavoroAgileException(response.message), $"Servizio link attività zucchetti per utenza {uid}.");
                        throw new LavoroAgileException(response.message);
                    }

                    // Se ci sono errori, restiuisce una eccezione.
                    if (response.rowserrors?.Count > 0)
                    {
                        _logger.LogError(new LavoroAgileException(JsonSerializer.Serialize(response.rowserrors, javascriptJsonSerializerOptions)), $"Servizio link attività zucchetti per utenza {uid}.");
                        throw new LavoroAgileException("Errore nella creazione delle relazioni fra entità");

                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio link attività zucchetti per utenza {uid}.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Invia a Zucchetti l'associativa utente-commessa, rendendola non assegnabile al di fuori
        /// dell'intervallo di assegnazione.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente</param>
        /// <param name="idEmployee">Id dell'utente</param>
        /// <param name="code">Codice accordo.</param>
        /// <param name="startDate">Data di inizio dell'accordo</param>
        /// <param name="endDate">Data di fine dell'accordo</param>
        /// <returns></returns>
        public async Task SendUserWorklistAssociation(string uid, string idEmployee, string code, string startDate, string endDate)
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
                    var ass = new ZTAnagrafica
                    {
                        // Associazione utente-commessa (dato che come codice si utilizza il codice accordo
                        // che è numerico, Zucchetti lo interpreta come numero e lo vule zero-filled a 15 cifre.
                        // Commessa visible solo nell'intervallo dell'accordo, invisibile prima e dopo.
                        attr_mstd = new List<ZTAccordoUtente>
                        {
                            new ZTAccordoUtente { company = "000000", ident = int.Parse(code).ToString("000000000000000"), dtstartvl = convertedStartDate, dtendvl = convertedEndDate, compdiscr = _zucchettiServiceSettings.Company, discr = idEmployee }
                        }

                    };

                    var result = await service.htht_fwsztsanag_RunAsync(
                        _zucchettiServiceSettings.WUsername,
                        _zucchettiServiceSettings.WPassword,
                        _zucchettiServiceSettings.Company,
                        JsonSerializer.Serialize(ass, javascriptJsonSerializerOptions),
                        "N",
                        "S");

                    var response = JsonSerializer.Deserialize<CreateActivitiesReturn>(result.Body.@return);

                    // Se l'operazione è andata male, solleva eccezione.
                    if (response.status == "KO")
                    {
                        _logger.LogError(new LavoroAgileException(response.message), $"Servizio associazione utente-commessa zucchetti per utenza {uid}.");
                        throw new LavoroAgileException(response.message);
                    }

                    // Se ci sono errori, restiuisce una eccezione nel caso in cui
                    // ci sia almeno un errore diverso da "Attribuzione già definita per il periodo.".
                    if (response.errors != null && response.errors.Any(e => e.msg != "Attribuzione già definita per il periodo"))
                    {
                        _logger.LogError(new LavoroAgileException(JsonSerializer.Serialize(response.errors, javascriptJsonSerializerOptions)), $"Servizio associazione utente-commessa zucchetti per utenza {uid}.");

                        throw new LavoroAgileException(response.errors.First(e => e.msg != "Attribuzione già definita per il periodo").msg);
                    }

                    // Se è stato restituito un errore "Attribuzione già definita per il periodo",
                    // elimina l'associazione e ritenta.
                    if (response.errors != null && response.errors.Any(e => e.msg.Equals("Attribuzione già definita per il periodo")))
                    {
                        await DeleteActivityUserAssociation(uid, code);
                        await SendUserWorklistAssociation(uid, idEmployee, code, startDate, endDate);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio associazione utente-commessa zucchetti per utenza {uid}.");

                    // Elimina l'associazione e la imposta come invisibile
                    await DeleteActivityUserAssociation(uid, code);
                    await SetActivityUserAssociationInvisible(uid, code, startDate, endDate);

                    throw;
                }
            }
        }

        /// <summary>
        /// Recupera i giustificativi compresi fra un intervallo di date per un utente e ne restituisce
        /// gli identificativi delle sole giornate rientranti nell'elenco delle giornate di interesse.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente.</param>
        /// <param name="startDate">Estremo inferiore di ricerca</param>
        /// <param name="endDate">Estremo superiore</param>
        /// <param name="filterDate">Insieme delle date di interesse</param>
        /// <returns>Tupla con l'elenco degli identificativi associati alle date da cancellare e dall'elenco delle data corrispondenti.</returns>
        public virtual async Task<(IEnumerable<double> daysId, IEnumerable<DateTime> daysDate)> GetWorkingDaysId(string uid, DateTime startDate, DateTime endDate, IEnumerable<DateTime> filterDate = null)
        {
            // Servizio ws_vqr_hrw_codici_richieste
            var binding = ws_vqr_hrw_codici_richiesteWSClient.GetDefaultBinding();
            var endpoint = new EndpointAddress(this._zucchettiServiceSettings.GetSWDaysUrl);
            using (var service = new ws_vqr_hrw_codici_richiesteWSClient(binding, endpoint))
            {
                try
                {
                    var convertedStartDate = startDate.ToString("yyyyMMdd");
                    var convertedEndDate = endDate.ToString("yyyyMMdd");

                    var response = await service.ws_vqr_hrw_codici_richieste_TabularQueryAsync(
                        _zucchettiServiceSettings.RUsername,
                        _zucchettiServiceSettings.RPassword,
                        _zucchettiServiceSettings.Company,
                        uid,
                        _zucchettiServiceSettings.WorklistToSearchCodes,
                        convertedStartDate,
                        convertedEndDate);

                    // Dell'elenco di giornate restituite, filtra quelle che abbiano data inizio
                    // contenute nella collezione delle date di filtro e ne restituisce l'id.
                    var records = filterDate != null ? response.Body?.Records?.Where(x => filterDate.Contains(DateTime.ParseExact(x.DADATA, "dd-MM-yyyy", CultureInfo.InvariantCulture)))
                        : response.Body?.Records;
                    return (
                        records.Select(x => x.CODRICHIE),
                        records.Select(x => DateTime.ParseExact(x.DADATA, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                        );

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio ricerca giornate lavorate zucchetti per utenza {uid}.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Elimina una giornata lavorativa.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente per cui cancellare la giornata.</param>
        /// <param name="workingDayId">Identificativo della giornata lavorativa da cancellare.</param>
        /// <returns></returns>
        public virtual async Task DeleteWorkingDay(string uid, double workingDayId)
        {
            // Servizio hfwe_fdeletereq
            var binding = hfwe_fdeletereqWSClient.GetDefaultBinding();
            var endpoint = new EndpointAddress(this._zucchettiServiceSettings.DeleteWorkingDaysUrl);
            using (var service = new hfwe_fdeletereqWSClient(binding, endpoint))
            {
                try
                {
                    var response = await service.hfwe_fdeletereq_RunAsync(
                        _zucchettiServiceSettings.WUsername,
                        _zucchettiServiceSettings.WPassword,
                        _zucchettiServiceSettings.Company,
                        1,
                        workingDayId);

                    var result = JsonSerializer.Deserialize<DeleteDayReturn>(response.Body.@return);

                    // Se l'esito è KO, e l'errore è diverso da 0501 (corrispondente a cancellazione non possibile), solleva eccezione.
                    if (result.status == "KO" && result.errorCode != "0501")
                    {
                        _logger.LogError(new LavoroAgileException(result.errorDescription), $"Servizio cancellazione giornate lavorative zucchetti per utenza {uid}.");
                        throw new LavoroAgileException(result.errorDescription);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Servizio cancellazione giornate lavorative zucchetti per utenza {uid}.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Genera due liste di date nel formato dd/MM/yyyy: la prima con date precedenti
        /// o uguali alla data <paramref name="upperLimit"/>; la seconda con date successive.
        /// </summary>
        /// <param name="dates">Lista di date da filtrare.</param>
        /// <param name="upperLimit">Limite da utilizzare per determinare la data oltre la quale considerare una data come da eliminare</param>
        /// <returns>Record con array delle date da presevare e da eliminare</returns>
        public static (IEnumerable<string> toMaintain, IEnumerable<string> toRemove) GetDatesToDelete(IEnumerable<string> dates, DateTime upperLimit)
        {
            //Date pianificate, nuove date pianificate, date da eliminare
            var datePianificateConverted = dates?.Select(date => DateTime.ParseExact(s: date.Trim(), format: "dd/MM/yyyy", provider: null));

            var toDelete = datePianificateConverted?.Where(x => x > upperLimit);

            return (
                datePianificateConverted?.Except(toDelete)?.Select(x => x.ToString("dd/MM/yyyy")),
                toDelete?.Select(x => x.ToString("dd/MM/yyyy"))
                );
        }

    }
}
