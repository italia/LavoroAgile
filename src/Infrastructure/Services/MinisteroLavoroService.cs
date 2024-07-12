using Domain.Model;
using Domain.Model.MinisteroLavoro.Request;
using Domain.Model.MinisteroLavoro.Response;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace Infrastructure.Services
{
    public class MinisteroLavoroService : IMinisteroLavoroService
    {
        private readonly ILogger<MinisteroLavoroService> _logger;

        private readonly MinisteroLavoroServicesSettings _ministeroLavoroServicesSettings;

        private readonly IRepository<Accordo, Guid> _repository;

        private readonly IPersonalDataProviderService _personalDataProviderService;

        private readonly DocumentsServiceSettings _documentsServiceSettings;

        private readonly IMinisteroLavoroStatusLogger _ministeroLavoroStatusLogger;

        public MinisteroLavoroService(ILogger<MinisteroLavoroService> logger,
            IOptions<MinisteroLavoroServicesSettings> ministeroLavoroServicesSettings,
            IRepository<Accordo, Guid> repository,
            IPersonalDataProviderService personalDataProviderService,
            IOptions<DocumentsServiceSettings> documentsServiceSettings,
            IMinisteroLavoroStatusLogger ministeroLavoroStatusLogger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._ministeroLavoroServicesSettings = ministeroLavoroServicesSettings?.Value;
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _personalDataProviderService = personalDataProviderService ?? throw new ArgumentNullException(nameof(personalDataProviderService));
            _documentsServiceSettings = documentsServiceSettings?.Value;
            _ministeroLavoroStatusLogger = ministeroLavoroStatusLogger;
        }

        public async Task<ResponseRestGetToken> GetToken()
        {
            RestClient _client = new RestClient();
            ResponseRestGetToken responseRestGetToken = null;

            var requestGetToken = new RestRequest(_ministeroLavoroServicesSettings.GetToken, Method.Post);
            requestGetToken.AddHeader("Authorization", "Basic " + _ministeroLavoroServicesSettings.Authorization);
            requestGetToken.AddHeader("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            requestGetToken.AddStringBody("grant_type=client_credentials&scope=" + _ministeroLavoroServicesSettings.ScopeGetToken, "text/plain");

            try
            {            
                var responseGetToken = await _client.ExecutePostAsync(requestGetToken);
                responseRestGetToken = JsonSerializer.Deserialize<ResponseRestGetToken>(responseGetToken.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch(Exception ex)
            {
                _logger.LogError("Errore chiamata GetToken: " + ex.Message);
            }

            return responseRestGetToken;
        }

        public async Task<List<Accordo>> GetAccordiDaInizializzare(CancellationToken cancellationToken)
        {
            IQueryable<Accordo> accordi = (await _repository.FindAsync(Guid.Empty, a => a.Stato == StatoAccordo.Sottoscritto && 
                a.DataInizioUtc <= System.DateTime.Now && 
                a.DataFineUtc >= System.DateTime.Now &&
                a.CodiceComunicazioneMinisteroLavoro == null, Domain.Model.Utilities.RoleAndKeysClaimEnum.KEY_CLAIM_SEGRETERIA_TECNICA )).Entities.AsQueryable(); ;

            return accordi.ToList();
        }

        public async Task<ResponseRestEsito> CreaComunicazione(Guid idAccordo, CancellationToken cancellationToken)
        {
            ResponseRestGetToken responseRestGetToken = await this.GetToken();
            ResponseRestEsito responseRestEsito = null;

            //Recupero l'accordo di interesse
            Accordo accordo = await _repository.GetAsync(idAccordo, cancellationToken);

            //Creo la request
            RequestRestCreaComunicazione requestRestCreaComunicazione = new RequestRestCreaComunicazione();
            requestRestCreaComunicazione.Comunicazione = new List<Domain.Model.MinisteroLavoro.Request.Comunicazione>();
            Domain.Model.MinisteroLavoro.Request.Comunicazione comunicazione = new Domain.Model.MinisteroLavoro.Request.Comunicazione();
            comunicazione.id = accordo.Codice;
            comunicazione.codTipologiaComunicazione = "I";
                
            comunicazione.SezioneDatoreLavoro = new SezioneDatoreLavoro();
            comunicazione.SezioneDatoreLavoro.codiceFiscaleDatoreLavoro = _ministeroLavoroServicesSettings.CodiceFiscaleDatoreLavoro;
            comunicazione.SezioneDatoreLavoro.denominazioneDatoreLavoro = _ministeroLavoroServicesSettings.DenominazioneDatoreLavoro;
                
            comunicazione.SezioneLavoratore = new SezioneLavoratore();
            comunicazione.SezioneLavoratore.codiceFiscaleLavoratore = accordo.Dipendente.CodiceFiscale;
            await SetNomeCognomeLavoratore(accordo, comunicazione);
            comunicazione.SezioneLavoratore.dataNascitaLavoratore = accordo.Dipendente.DataDiNascita.Value.ToString("yyyy-MM-dd");
            comunicazione.SezioneLavoratore.codComuneNascitaLavoratore = accordo.Dipendente.CodiceFiscale.Substring(11, 4);

            comunicazione.SezioneRapportoLavoro = new SezioneRapportoLavoro();
            comunicazione.SezioneRapportoLavoro.dataInizioRapportoLavoro = accordo.DataPresaServizio.Value.ToString("yyyy-MM-dd");
            SetCodiceTipologiaRapportoLavoro(accordo, comunicazione);
            comunicazione.SezioneRapportoLavoro.posizioneINAIL = _ministeroLavoroServicesSettings.PosizioneINAIL;
            comunicazione.SezioneRapportoLavoro.tariffaINAIL = _ministeroLavoroServicesSettings.TariffaINAIL;

            comunicazione.SezioneAccordoSmartWorking = new SezioneAccordoSmartWorking();
            comunicazione.SezioneAccordoSmartWorking.dataSottoscrizioneAccordo = accordo.DataSottoscrizione.Value.ToString("yyyy-MM-dd");
            comunicazione.SezioneAccordoSmartWorking.dataInizioPeriodo = accordo.DataInizioUtc.ToString("yyyy-MM-dd");
            comunicazione.SezioneAccordoSmartWorking.dataFinePeriodo = accordo.DataFineUtc.ToString("yyyy-MM-dd");
            comunicazione.SezioneAccordoSmartWorking.tipologiaDurataPeriodo = "TD";

            requestRestCreaComunicazione.Comunicazione.Add(comunicazione);

            try
            {
                //Effettuo la chiamata ignorando eventuali errori di certificato SSL
                var options = new RestClientOptions() { RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true };

                RestClient _client = new RestClient(options);
                var requestCreaComunicazione = new RestRequest(_ministeroLavoroServicesSettings.CreaComunincazione, Method.Post);
                requestCreaComunicazione.AddHeader("Authorization", responseRestGetToken.token_type + " " + responseRestGetToken.access_token);
                requestCreaComunicazione.AddHeader("Content-Type", "application/json");
                string jsonRequestRestCreaComunicazione = JsonSerializer.Serialize<RequestRestCreaComunicazione>(requestRestCreaComunicazione, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                requestCreaComunicazione.AddBody(jsonRequestRestCreaComunicazione);
                
                var responseCreaComunicazione = await _client.ExecutePostAsync<RequestRestCreaComunicazione>(requestCreaComunicazione);

                //Verifico l'esito della chiamata
                if (responseCreaComunicazione.IsSuccessful && !String.IsNullOrEmpty(responseCreaComunicazione.Content))
                {
                    responseRestEsito = JsonSerializer.Deserialize<ResponseRestEsito>(responseCreaComunicazione.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    //Verifico eventuali codici di errore interni al servizio
                    if (responseRestEsito.Esito.Count == 1 && responseRestEsito.Esito[0].codice.Equals("E100") && !String.IsNullOrEmpty(responseRestEsito.Esito[0].codiceComunicazione))
                    {
                        //Aggiorno il CodiceComunicazione dell'accordo
                        accordo.CodiceComunicazioneMinisteroLavoro = responseRestEsito.Esito[0].codiceComunicazione;
                        await _repository.UpdateAsync(accordo);
                    }                        
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Errore chiamata CreaComunicazione: " + ex.Message);
            }

            return responseRestEsito;
        }

        public async Task<ResponseRestEsito> RecediComunicazione(Guid idAccordo, DateTime dataFinePeriodo, CancellationToken cancellationToken)
        {
            ResponseRestGetToken responseRestGetToken = await this.GetToken();
            ResponseRestEsito responseRestEsito = new ResponseRestEsito();

            //Recupero l'accordo di interesse
            Accordo accordo = await _repository.GetAsync(idAccordo, cancellationToken);

            //Creo la request
            RequestRestRecediComunicazione requestRestRecediComunicazione = new RequestRestRecediComunicazione();
            requestRestRecediComunicazione.RecediComunicazione = new List<RecediComunicazione>();
            RecediComunicazione recediComunicazione = new RecediComunicazione();
            recediComunicazione.id = accordo.Codice;
            recediComunicazione.DataFinePeriodo = dataFinePeriodo.ToString("yyyy-MM-dd");
            recediComunicazione.CodiceComunicazione = accordo.CodiceComunicazioneMinisteroLavoro;
            recediComunicazione.CodTipologiaComunicazione = "R";

            requestRestRecediComunicazione.RecediComunicazione.Add(recediComunicazione);

            try
            {
                //Effettuo la chiamata ignorando eventuali errori di certificato SSL
                var options = new RestClientOptions() { RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true };

                RestClient _client = new RestClient(options);
                var requestRecediComunicazione = new RestRequest(_ministeroLavoroServicesSettings.RecediComunicazione, Method.Post);
                requestRecediComunicazione.AddHeader("Authorization", responseRestGetToken.token_type + " " + responseRestGetToken.access_token);
                requestRecediComunicazione.AddHeader("Content-Type", "application/json");
                string jsonRequestRestRecediComunicazione = JsonSerializer.Serialize<RequestRestRecediComunicazione>(requestRestRecediComunicazione, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                requestRecediComunicazione.AddBody(jsonRequestRestRecediComunicazione);

                var responseRecediComunicazione = await _client.ExecutePostAsync<RequestRestAnnullaComunicazione>(requestRecediComunicazione);

                //Verifico l'esito della chiamata
                if (responseRecediComunicazione.IsSuccessful && !String.IsNullOrEmpty(responseRecediComunicazione.Content))
                {
                    responseRestEsito = JsonSerializer.Deserialize<ResponseRestEsito>(responseRecediComunicazione.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    //Verifico eventuali codici di errore interni al servizio
                    if (responseRestEsito.Esito.Count == 1 && responseRestEsito.Esito[0].codice.Equals("E100") && !String.IsNullOrEmpty(responseRestEsito.Esito[0].codiceComunicazione))
                    {
                        //Aggiorno il CodiceComunicazione dell'accordo
                        accordo.CodiceComunicazioneMinisteroLavoro = responseRestEsito.Esito[0].codiceComunicazione;
                        await _repository.UpdateAsync(accordo);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Errore chiamata RecediComunicazione: " + ex.Message);
            }

            return responseRestEsito;
        }

        public async Task<ResponseRestEsitoRicerca> RicercaComunicazione(string codiceFiscaleUtente, CancellationToken cancellationToken)
        {
            ResponseRestGetToken responseRestGetToken = await this.GetToken();
            ResponseRestEsitoRicerca responseRestEsitoRicerca = new ResponseRestEsitoRicerca();

            try
            {
                //Effettuo la chiamata ignorando eventuali errori di certificato SSL
                var options = new RestClientOptions() { RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true };

                RestClient _client = new RestClient(options);
                var requestRicercaComunicazione = new RestRequest(_ministeroLavoroServicesSettings.RicercaComunicazione, Method.Get);
                requestRicercaComunicazione.AddHeader("Authorization", responseRestGetToken.token_type + " " + responseRestGetToken.access_token);
                requestRicercaComunicazione.AddParameter("CFLavoratore", codiceFiscaleUtente);

                var responseRicercaComunicazione = await _client.ExecuteGetAsync(requestRicercaComunicazione);

                //Verifico l'esito della chiamata
                if (responseRicercaComunicazione.IsSuccessful && !String.IsNullOrEmpty(responseRicercaComunicazione.Content))
                {
                    responseRestEsitoRicerca = JsonSerializer.Deserialize<ResponseRestEsitoRicerca>(responseRicercaComunicazione.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore chiamata RicercaComunicazione: " + ex.Message);
            }

            return responseRestEsitoRicerca;
        }

        public async Task<ResponseRestEsitoDettaglio> DettaglioComunicazione(string codiceComunicazione, CancellationToken cancellationToken)
        {
            ResponseRestGetToken responseRestGetToken = await this.GetToken();
            ResponseRestEsitoDettaglio responseRestEsitoDettaglio = new ResponseRestEsitoDettaglio();

            try
            {
                //Effettuo la chiamata ignorando eventuali errori di certificato SSL
                var options = new RestClientOptions() { RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true };

                RestClient _client = new RestClient(options);
                var requestDettaglioComunicazione = new RestRequest(_ministeroLavoroServicesSettings.DettaglioComunicazione, Method.Get);
                requestDettaglioComunicazione.AddHeader("Authorization", responseRestGetToken.token_type + " " + responseRestGetToken.access_token);
                requestDettaglioComunicazione.AddParameter("CodiceComunicazione", codiceComunicazione);

                var responseDettaglioComunicazione = await _client.ExecuteGetAsync(requestDettaglioComunicazione);

                //Verifico l'esito della chiamata
                if (responseDettaglioComunicazione.IsSuccessful && !String.IsNullOrEmpty(responseDettaglioComunicazione.Content))
                {
                    responseRestEsitoDettaglio = JsonSerializer.Deserialize<ResponseRestEsitoDettaglio>(responseDettaglioComunicazione.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore chiamata DettaglioComunicazione: " + ex.Message);
            }

            return responseRestEsitoDettaglio;
        }

        public async Task<ResponseRestEsito> AnnullaComunicazione(Guid idAccordo, CancellationToken cancellationToken)
        {
            ResponseRestGetToken responseRestGetToken = await this.GetToken();
            ResponseRestEsito responseRestEsito = new ResponseRestEsito();

            //Recupero l'accordo di interesse
            Accordo accordo = await _repository.GetAsync(idAccordo, cancellationToken);

            //Creo la request
            RequestRestAnnullaComunicazione requestRestAnnullaComunicazione = new RequestRestAnnullaComunicazione();
            requestRestAnnullaComunicazione.AnnullaComunicazione = new List<AnnullaComunicazione>();
            AnnullaComunicazione annullaComunicazione = new AnnullaComunicazione();
            annullaComunicazione.id = accordo.Codice;
            annullaComunicazione.CodiceComunicazione = accordo.CodiceComunicazioneMinisteroLavoro;
            annullaComunicazione.CodTipologiaComunicazione = "A";

            requestRestAnnullaComunicazione.AnnullaComunicazione.Add(annullaComunicazione);

            try
            {
                //Effettuo la chiamata ignorando eventuali errori di certificato SSL
                var options = new RestClientOptions() { RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true };

                RestClient _client = new RestClient(options);
                var requestAnnullaComunicazione = new RestRequest(_ministeroLavoroServicesSettings.AnnullaComunicazione, Method.Post);
                requestAnnullaComunicazione.AddHeader("Authorization", responseRestGetToken.token_type + " " + responseRestGetToken.access_token);
                requestAnnullaComunicazione.AddHeader("Content-Type", "application/json");
                string jsonRequestRestAnnullaComunicazione = JsonSerializer.Serialize<RequestRestAnnullaComunicazione>(requestRestAnnullaComunicazione, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                requestAnnullaComunicazione.AddBody(jsonRequestRestAnnullaComunicazione);

                var responseAnnullaComunicazione = await _client.ExecutePostAsync<RequestRestAnnullaComunicazione>(requestAnnullaComunicazione);

                //Verifico l'esito della chiamata
                if (responseAnnullaComunicazione.IsSuccessful && !String.IsNullOrEmpty(responseAnnullaComunicazione.Content))
                {
                    responseRestEsito = JsonSerializer.Deserialize<ResponseRestEsito>(responseAnnullaComunicazione.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    //Verifico eventuali codici di errore interni al servizio
                    if (responseRestEsito.Esito.Count == 1 && responseRestEsito.Esito[0].codice.Equals("E100") && !String.IsNullOrEmpty(responseRestEsito.Esito[0].codiceComunicazione))
                    {
                        //Annullo il CodiceComunicazione dell'accordo
                        accordo.CodiceComunicazioneMinisteroLavoro = null;
                        await _repository.UpdateAsync(accordo);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Errore chiamata AnnullaComunicazione: " + ex.Message);
            }

            return responseRestEsito;
        }


        private async Task SetNomeCognomeLavoratore(Accordo accordo, Domain.Model.MinisteroLavoro.Request.Comunicazione comunincazione)
        {
            try
            {
                // Recupera le informazioni sull'utente dall'anagrafica esterna.
                var userData = await this._personalDataProviderService.GetUserDataAsync(accordo.Dipendente.Email);

                //Se le informazioni recuperate da zucchetti non sono nulle vengono usate per compilare Nome e Congome
                //In caso contario si gestisce uno split sul campo unico dell'accordo NomeCognome
                if (userData != null && !String.IsNullOrEmpty(userData.Nome) && !String.IsNullOrEmpty(userData.Cognome))
                {
                    comunincazione.SezioneLavoratore.nomeLavoratore = userData.Nome;
                    comunincazione.SezioneLavoratore.cognomeLavoratore = userData.Cognome;
                }
                else
                {
                    string[] nomeCognome = accordo.Dipendente.NomeCognome.Split(" ");
                    if (nomeCognome.Length == 2)
                    {
                        comunincazione.SezioneLavoratore.nomeLavoratore = nomeCognome[0];
                        comunincazione.SezioneLavoratore.cognomeLavoratore = nomeCognome[1];
                    }
                    else
                    {
                        comunincazione.SezioneLavoratore.nomeLavoratore = accordo.Dipendente.NomeCognome;
                        comunincazione.SezioneLavoratore.cognomeLavoratore = accordo.Dipendente.NomeCognome;
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Errore SetNomeCognomeLavoratore CreaComunicazione: " + ex.Message);
            }
        }

        private void SetCodiceTipologiaRapportoLavoro(Accordo accordo, Domain.Model.MinisteroLavoro.Request.Comunicazione comunincazione)
        {
            try
            {
                //Recupera il file in cui sono censiti tutti i codici fiscali degli utenti PNRR che hanno CodTipologiaRapportoLavoro = A02
                WebClient client = new WebClient();
                List<string> cfContrattistiPNRR = new List<string>();

                Stream dataFileTxt = client.OpenRead(_documentsServiceSettings.BaseUrl + "/Documents/CF_Contrattisti_PNRR.txt");
                using (StreamReader sr = new StreamReader(dataFileTxt))
                {
                    while (sr.Peek() >= 0)
                    {
                        cfContrattistiPNRR.Add(sr.ReadLine());
                    }
                }

                if (cfContrattistiPNRR != null && cfContrattistiPNRR.Contains(accordo.Dipendente.CodiceFiscale))
                    comunincazione.SezioneRapportoLavoro.codTipologiaRapportoLavoro = "A02";
                else
                    comunincazione.SezioneRapportoLavoro.codTipologiaRapportoLavoro = "A01";
            }
            catch(Exception ex)
            {
                _logger.LogError("Errore recupero file Contrattisti PNRR CreaComunicazione: " + ex.Message);
            }
        }

        /// <summary>
        /// Questo metodo non effettua un semplice reinvio di una comunicazione, ma opera come segue:
        /// - Prova l'invio di una nuova comunicazione se va bene termina
        /// - In caso di errori ricerca tutte le comunicazioni per lo specifico utente tramite codice fiscale
        /// - Se presenti ne effettua per ognuna un recesso
        /// - Ritenta l'invio della nuova comunicazione che a questo punto dovrebbe andare bene
        /// - In caso negativo traccia comunque l'errore/i
        /// </summary>
        /// <param name="idAccordo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ReinvioComunicazioneMinisteroLavoro(Guid idAccordo, CancellationToken cancellationToken)
        {
            Accordo a = await _repository.GetAsync(idAccordo, cancellationToken);

            try
            {
                //Effettuo la chiamata al servizio
                ResponseRestEsito responseRestEsito = await CreaComunicazione(a.Id, cancellationToken);

                //Verifico l'esito della chiamata, va tenuto presente che anche in caso di errori gestiti il servizio restitutisce un successo in chiamata
                if (responseRestEsito != null && responseRestEsito.Esito != null)
                {
                    //Verifico eventuali errori gestiti in base ai codici esito recuperati dalla chiatamata
                    if (responseRestEsito.Esito.Count == 1 && responseRestEsito.Esito[0].codice.Equals("E100"))
                    {
                        //Chiamata andata a buon fine peristo in lavoro agile l'esito
                        await _ministeroLavoroStatusLogger.SetNuovaComunicazioneStatus(a.Id, true, null, cancellationToken);
                        _logger.LogInformation("Accordo codice: " + a.Codice + " CodiceEsito: " + responseRestEsito.Esito[0].codice + " Esito: " + responseRestEsito.Esito[0].messaggio + " CodiceComunicazione: " + responseRestEsito.Esito[0].codiceComunicazione);
                    }
                    else
                    {
                        //Chiamata anadata a buon fine che ha restituito degli errori gestiti
                        //Effettuo una ricerca di tutti gli accordi eventalmente trasmessi al ministero, associati al codice fiscale dell'utente
                        ResponseRestEsitoRicerca responseRestEsitoRicerca = await RicercaComunicazione(a.Dipendente.CodiceFiscale, cancellationToken);

                        if (responseRestEsitoRicerca != null && responseRestEsitoRicerca.Comunicazioni != null && responseRestEsitoRicerca.Comunicazioni.Count > 0)
                        {
                            _logger.LogInformation("CodiceFiscale: " + a.Dipendente.CodiceFiscale + " trovate comunicazioni precedenti ancora in corso per questo utente");
                            //Se sono state trovate comunicazioni per il codice fiscale dell'utente procedo al recesso di ognuna di esse
                            foreach (Domain.Model.MinisteroLavoro.Response.Comunicazioni c in responseRestEsitoRicerca.Comunicazioni)
                            {
                                ResponseRestEsito responseRestEsitoRecedi = await RecediComunicazione(a.Id, a.DataFineUtc, cancellationToken);
                                _logger.LogInformation("Recesso comunicazione CodiceComunicazione: " + c.codiceComunicazione);
                            }                           
                        }

                        //A questo punto riprovo l'invio della nuova comunicazione
                        responseRestEsito = await CreaComunicazione(a.Id, cancellationToken);

                        //Verifico l'esito della chiamata, va tenuto presente che anche in caso di errori gestiti il servizio restitutisce un successo in chiamata
                        if (responseRestEsito != null && responseRestEsito.Esito != null)
                        {
                            //Verifico eventuali errori gestiti in base ai codici esito recuperati dalla chiatamata
                            if (responseRestEsito.Esito.Count == 1 && responseRestEsito.Esito[0].codice.Equals("E100"))
                            {
                                //Chiamata andata a buon fine peristo in lavoro agile l'esito
                                await _ministeroLavoroStatusLogger.SetNuovaComunicazioneStatus(a.Id, true, null, cancellationToken);
                                _logger.LogInformation("Accordo codice: " + a.Codice + " CodiceEsito: " + responseRestEsito.Esito[0].codice + " Esito: " + responseRestEsito.Esito[0].messaggio + " CodiceComunicazione: " + responseRestEsito.Esito[0].codiceComunicazione);
                            }
                            else
                            {
                                string errore = String.Empty;
                                foreach (Esito e in responseRestEsito.Esito)
                                {
                                    errore += "Accordo id: " + idAccordo + " CodiceEsito: " + responseRestEsito.Esito[0].codice + " Esito: " + responseRestEsito.Esito[0].messaggio;
                                }
                                await _ministeroLavoroStatusLogger.SetNuovaComunicazioneStatus(idAccordo, false, errore, cancellationToken);
                            }
                        }
                    }
                }
                else
                {
                    await _ministeroLavoroStatusLogger.SetRecessoComunicazioneStatus(idAccordo, false, "Esito nullo chiamata ReinvioComunicazione per accordo id: " + idAccordo, cancellationToken);
                    _logger.LogInformation("Esito nullo chiamata ReinvioComunicazione per accordo id: " + idAccordo);
                }
            }
            catch (Exception ex)
            {
                await _ministeroLavoroStatusLogger.SetRecessoComunicazioneStatus(idAccordo, false, "Errore chiamata ReinvioComunicazione per accordo id: " + idAccordo, cancellationToken);
                _logger.LogInformation("Errore chiamata ReinvioComunicazione per accordo id: " + idAccordo + " " + ex.Message);
            }
        }
    }
}
