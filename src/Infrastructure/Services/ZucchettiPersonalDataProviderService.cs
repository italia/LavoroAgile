using Domain.Model;
using Domain.Settings;
using Infrastructure.Repositories;
using Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading.Tasks;
using ZucchettiAnagServiceReference;

// Estende la visibilità dei metodi all'assembly degli unit test.
[assembly: InternalsVisibleTo("Tests")]
namespace Infrastructure.Services
{
    /// <summary>
    /// Implementa la comunicazione con il servizio di anagrafica Zucchetti.
    /// </summary>
    public class ZucchettiPersonalDataProviderService : IPersonalDataProviderService
    {
        /// <summary>
        /// Riferimento al logger per il servizio.
        /// </summary>
        private readonly ILogger<ZucchettiPersonalDataProviderService> _logger;

        /// <summary>
        /// Impostazione per la connessione ai servizi Zucchetti.
        /// </summary>
        private readonly ZucchettiServiceSettings _zucchettiServiceSettings;

        /// <summary>
        /// Repository per la gestione delle strutture.
        /// </summary>
        private readonly IStrutturaRepository<Struttura, Guid> _strutturaRepository;

        /// <summary>
        /// Inizializza un nuovo <see cref="ZucchettiPersonalDataProviderService"/>.
        /// </summary>
        /// <param name="settings">Impostazioni del servizio.</param>
        /// <param name="logger">Riferimento al logger</param>
        public ZucchettiPersonalDataProviderService(IOptions<ZucchettiServiceSettings> settings, IStrutturaRepository<Struttura, Guid> strutturaRepository, ILogger<ZucchettiPersonalDataProviderService> logger)
        {
            this._zucchettiServiceSettings = settings?.Value;
            _strutturaRepository = strutturaRepository;
            this._logger = logger;
        }

        public bool Enabled => this._zucchettiServiceSettings.Valid;

        public async Task<Dipendente> GetUserDataAsync(string uid)
        {
            // Tenta di recuperare le informazioni sull'utente.
            var userData = await this.InvokeZAnagAsync(uid);

            // Se ha ricevuto dati, li mappa con le informazioni utente.
            if (userData != null)
            {
                var user = new Dipendente
                {
                    Nome = userData.NOME,
                    Cognome = userData.COGNOME,
                    NomeCognome = $"{userData.NOME} {userData.COGNOME}",
                    CodiceFiscale = userData.CF,
                    Sesso = userData.SESSO,
                    LuogoDiNascita = userData.LUOGO_NASCITA,
                    Email = userData.EMAIL,
                    PosizioneGiuridica = userData.POSIZIONE_GIURIDICA,
                    CategoriaFasciaRetributiva = userData.FASCIA_RETRIBUTIVA,
                    
                    Struttura = new Struttura(userData.DIPARTIMENTO, new Dirigente(userData.ANNAME3, userData.ANSURNAM3, userData.ANEMAIL3),
                        userData.UFFICIO, new Dirigente(userData.ANNAME2, userData.ANSURNAM2, userData.ANEMAIL2),
                        userData.SERVIZIO, new Dirigente(userData.ANNAME1, userData.ANSURNAM1, userData.ANEMAIL1))
                };

                // Verifica ed eventualmente crea la struttura di riferimento
                user.Struttura.Id = await user.Struttura.CheckCreateOrUpdate(_strutturaRepository);

                // Prova a leggere la data di nascita.
                DateTime dataNascita;
                if (DateTime.TryParse(userData.DATA_NASCITA, out dataNascita))
                {
                    user.DataDiNascita = dataNascita;
                }

                return user;
            }

            return default;

        }

        /// <summary>
        /// Recupera l'id del dipendente.
        /// </summary>
        /// <param name="uid">Identificativo utente.</param>
        /// <returns>Matricola dell'utente.</returns>
        public async Task<string> GetIdEmployee(string uid)
        {
            // Invoca il servizio e restituisce il codice rapporto (che da documentazione
            // viene indicato come idEmployee).
            return (await InvokeZAnagAsync(uid))?.CODICE_RAPPORTO;
        }

        /// <summary>
        /// Interroga il servizio di anagrafica zucchetti per prelevare le informazioni sull'utente
        /// </summary>
        /// <param name="uid">Identificativo dell'utente di cui recuperare le informazioni</param>
        /// <returns>Dettaglio utente.</returns>
        private async Task<ws_vqr_lavoroagile_TabularQueryResponseStructure> InvokeZAnagAsync(string uid)
        {
            // Se i parametri di connessione verso zucchetti non sono valorizzati,
            // non tenta l'interrogazione.
            if (_zucchettiServiceSettings == null || !_zucchettiServiceSettings.Valid)
            {
                return default;
            }

            // Inizializzazione binding ed endpoint.
            var binding = ws_vqr_lavoroagileWSClient.GetDefaultBinding();
            var endpoint = new EndpointAddress(this._zucchettiServiceSettings.AnagUrl);
            using (var service = new ws_vqr_lavoroagileWSClient(binding, endpoint))
            {
                try
                {
                    var response = await service.ws_vqr_lavoroagile_TabularQueryAsync(this._zucchettiServiceSettings.RUsername, this._zucchettiServiceSettings.RPassword, this._zucchettiServiceSettings.Company, uid);
                    var result = response?.Body?.Records.FirstOrDefault();

                    // Se è stato restituito un elemento, verifica la corrispondenza fra numero
                    // di livelli e numero di "riporti".
                    if (result != null)
                    {
                        CleanResponse(result);

                        result = this.CheckLevels(result);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Servizio anagrafica zucchetti");
                    return default;
                }
            }

        }

        /// <summary>
        /// Pulisce la respose.
        /// </summary>
        /// <param name="result">Response da pulire</param>
        /// <remarks>
        /// Zucchetti invia le informazioni con una serie di spazi prima e dopo il valore reale
        /// e per le info di struttura, invia - quando questa non è valorizzata.
        /// </remarks>
        private void CleanResponse(ws_vqr_lavoroagile_TabularQueryResponseStructure result)
        {
            // Effettua una pulizia preventiva delle descrizioni dei livelli (
            // arrivano con degli spazi ai lati ed i livelli non specificati, sono
            // indicati con un carattere -
            result.DIPARTIMENTO = result.DIPARTIMENTO?.Trim();
            result.SERVIZIO = result.SERVIZIO?.Trim();
            result.UFFICIO = result.UFFICIO.Trim();
            result.NOME = result.NOME?.Trim();
            result.COGNOME = result.COGNOME?.Trim();
            result.CF = result.CF?.Trim();
            result.SESSO = result.SESSO?.Trim();
            result.LUOGO_NASCITA = result.LUOGO_NASCITA?.Trim();
            result.DATA_NASCITA = result.DATA_NASCITA?.Trim();
            result.EMAIL = result.EMAIL?.Trim();
            result.POSIZIONE_GIURIDICA = result.POSIZIONE_GIURIDICA?.Trim();
            result.FASCIA_RETRIBUTIVA = result.FASCIA_RETRIBUTIVA?.Trim();
            result.ANNAME3 = result.ANNAME3?.Trim();
            result.ANNAME2 = result.ANNAME2?.Trim();
            result.ANNAME1 = result.ANNAME1?.Trim();
            result.ANSURNAM1 = result.ANSURNAM1?.Trim();
            result.ANSURNAM2 = result.ANSURNAM2?.Trim();
            result.ANSURNAM3 = result.ANSURNAM3?.Trim();
            result.ANEMAIL1 = result.ANEMAIL1?.Trim();
            result.ANEMAIL2 = result.ANEMAIL2?.Trim();
            result.ANEMAIL3 = result.ANEMAIL3?.Trim();

            if (!string.IsNullOrWhiteSpace(result.DIPARTIMENTO) && result.DIPARTIMENTO.Equals("-"))
            {
                result.DIPARTIMENTO = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(result.SERVIZIO) && result.SERVIZIO.Equals("-"))
            {
                result.SERVIZIO = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(result.UFFICIO) && result.UFFICIO.Equals("-"))
            {
                result.UFFICIO = string.Empty;
            }

        }

        /// <summary>
        /// Verifica la corrispondenza fra numero di livelli ricevuti da Zucchetti e numero di
        /// riporti approvativi.
        /// </summary>
        /// <param name="result">Dati restituiti da Zucchetti.</param>
        /// <returns>Dati restituiti da Zucchetti eventualmente adeguati.</returns>
        /// <remarks>
        /// Caso d'uso: un capo servizio vuole avviare l'iter per il proprio accordo di lavoro agile.
        /// In questo caso, il servizio riporta 3 livelli per la struttura ma 2 approvatori in quanto
        /// il capo servizio non può approvarsi da solo l'accordo.
        /// </remarks>
        internal protected ws_vqr_lavoroagile_TabularQueryResponseStructure CheckLevels(ws_vqr_lavoroagile_TabularQueryResponseStructure result)
        {
            //La sequenza delle approvazioni è ANNAME3 - ANNAME2 - ANNAME1

            //3 Livelli con tutti i responsabili valorizzati non fa niente
            //return result;
            //ANNAME1 - Primo riporto - Responsabile servizio
            //ANNAME2 - Secondo riporto - Responsabile ufficio
            //ANNAME3 - Terzo riporto - Responsabile dipartimento

            //2 Livelli con tutti i responsabili valorizzati deve spostare il primo 
            // riporto a livello 2 ed il terzo riporto a livello 3, cosicché riesca a
            // inizializzare correttamente la gerarchia.
            //ANNAME1 - Primo riporto - Responsabile ufficio
            //ANNAME2 - Secondo riporto - Responabile dipartimento
            //ANNAME3 - Terzo riporto - nullo
            if (!string.IsNullOrWhiteSpace(result.DIPARTIMENTO) && !string.IsNullOrWhiteSpace(result.UFFICIO) && string.IsNullOrWhiteSpace(result.SERVIZIO))
            {
                result.ANNAME3 = result.ANNAME2;
                result.ANSURNAM3 = result.ANSURNAM2;
                result.ANEMAIL3 = result.ANEMAIL2;

                result.ANNAME2 = result.ANNAME1;
                result.ANSURNAM2 = result.ANSURNAM1;
                result.ANEMAIL2 = result.ANEMAIL1;

                result.ANNAME1 = string.Empty;
                result.ANSURNAM1 = string.Empty;
                result.ANEMAIL1 = string.Empty;

            }

            // 2 Livelli con tutti i responsabili valorizzati ma manca l'ufficio
            // in questo caso il secondo riporto va portato a terzo.
            // ANNAME1 - Primo riporto - Responsabile servizio
            // ANNAME2 - Secondo riporto - Responabile dipartimento
            // ANNAME3 - Terzo riporto - nullo
            if (!string.IsNullOrWhiteSpace(result.DIPARTIMENTO) && string.IsNullOrWhiteSpace(result.UFFICIO) && !string.IsNullOrWhiteSpace(result.SERVIZIO))
            {
                result.ANNAME3 = result.ANNAME2;
                result.ANSURNAM3 = result.ANSURNAM2;
                result.ANEMAIL3 = result.ANEMAIL2;

                result.ANNAME2 = string.Empty;
                result.ANSURNAM2 = string.Empty;
                result.ANEMAIL2 = string.Empty;

            }


            //3 Livelli con responsabile servizio mancante
            //ANNAME1 - Primo riporto - Responsabile ufficio
            //ANNAME2 - Secondo riporto - Responsabile dipartimento
            //ANNAME3 - Terzo riporto - nullo

            if (!string.IsNullOrWhiteSpace(result.UFFICIO) && 
                !string.IsNullOrWhiteSpace(result.SERVIZIO) && 
                string.IsNullOrWhiteSpace(result.ANNAME3))
            {
                result.ANNAME3 = result.ANNAME2;
                result.ANSURNAM3 = result.ANSURNAM2;
                result.ANEMAIL3 = result.ANEMAIL2;

                result.ANNAME2 = result.ANNAME1;
                result.ANSURNAM2 = result.ANSURNAM1;
                result.ANEMAIL2 = result.ANEMAIL1;

                result.ANNAME1 = result.NOME;
                result.ANSURNAM1 = result.COGNOME;
                result.ANEMAIL1 = result.EMAIL;
            }

            //2 Livelli con responsabile servizio mancante
            //ANNAME1 - Primo riporto - Responsabile dipartimento
            //ANNAME2 - Secondo riporto - nullo
            //ANNAME3 - Terzo riporto - nullo

            if (!string.IsNullOrWhiteSpace(result.UFFICIO) &&
                string.IsNullOrWhiteSpace(result.SERVIZIO) &&
                string.IsNullOrWhiteSpace(result.ANNAME2))
            {
                result.ANNAME3 = result.ANNAME1;
                result.ANSURNAM3 = result.ANSURNAM1;
                result.ANEMAIL3 = result.ANEMAIL1;

                result.ANNAME2 = result.NOME;
                result.ANSURNAM2 = result.COGNOME;
                result.ANEMAIL2 = result.EMAIL;
            }

            //2 Livelli con responsabile ufficio mancante
            //ANNAME1 - Primo riporto - Responsabile dipartimento
            //ANNAME2 - Secondo riporto - nullo (in questi casi per un bug zucchetti arriva valorizzato uguale a responsabile dipartimento)
            //ANNAME3 - Terzo riporto - nullo

            if (string.IsNullOrWhiteSpace(result.UFFICIO) &&
                !string.IsNullOrWhiteSpace(result.SERVIZIO) &&
                result.ANNAME2.Equals(result.ANNAME1))
            {
                result.ANNAME3 = result.ANNAME1;
                result.ANSURNAM3 = result.ANSURNAM1;
                result.ANEMAIL3 = result.ANEMAIL1;

                result.ANNAME1 = result.NOME;
                result.ANSURNAM1 = result.COGNOME;
                result.ANEMAIL1 = result.EMAIL;
            }

            //1 Livello
            //Questo è il caso delle strutture apicali che come suggerito da Zucchetti sono identificate dall'ID_DIPARTIMENTO che inizia con 1
            //In questo caso creiamo una struttura fittizia identificata dalla descrizione che riporta fra parentesi il nome del dirigente responsabile
            //Il responsabile accordo ed il capo struttura saranno uguali

            if (result.ID_DIPARTIMENTO.StartsWith("1") && String.IsNullOrEmpty(result.UFFICIO) && String.IsNullOrEmpty(result.SERVIZIO))
            {
                result.DIPARTIMENTO = result.DIPARTIMENTO + " (" + result.ANNAME1 + " " + result.ANSURNAM1 + ")";
                result.ANNAME3 = result.ANNAME1;
                result.ANSURNAM3 = result.ANSURNAM1;
                result.ANEMAIL3 = result.ANEMAIL1;
            }

            return result;

        }
    }
}
