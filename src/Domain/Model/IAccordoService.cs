using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Definisce l'intefaccia del servizio di gestione accordi.
    /// </summary>
    public interface IAccordoService
    {
        /// <summary>
        /// Recupera il dettaglio di un accordo in corso.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente per cui recuperare l'accordo in corso.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Dettaglio dell'accordo in corso o null nel caso in cui non vi siano accordi in corso.</returns>
        Task<Accordo> GetAccordoInCorsoAsync(Guid uid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera il numero di accordi da lavorare per uno specifico utente in uno specifico ruolo.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente per cui recuperare gli accordi in lavorazioni.</param>
        /// <param name="role">Ruolo in cui sta operando l'utente.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Numero di accordi da lavorare.</returns>
        Task<int> GetAccordiToDoAsync(Guid uid, string role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Inizializza la richiesta di creazione accordo.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente che vuole registrare un accordo.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Accordo inizializzato</returns>
        Task<Accordo> InitCreateAccordoAsync(Guid uid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Crea un accordo.
        /// </summary>
        /// <param name="accordo">Informazioni sull'accordo da creare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CreateAccordoAsync(Accordo accordo, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera il dettaglio di un accordo.
        /// </summary>
        /// <param name="idAccordo">Identificativo dell'accordo da recuperare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Dettaglio dell'accordo.</returns>
        Task<Accordo> GetAccordoAsync(Guid idAccordo, CancellationToken cancellationToken = default);

        /// <summary>
        /// Aggiorna un accordo.
        /// </summary>
        /// <param name="accordo">Informazioni sull'accordo da creare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> UpdateAccordoAsync(Accordo accordo, CancellationToken cancellationToken, bool verificaNumeroMassimoGiornate = true);

        /// Effettua una ricerca di accordi.
        /// </summary>
        /// <param name="userId">Uid dell'utente che effettua la ricerca</param>
        /// <param name="userRole">Ruolo dell'utente che effettua la ricerca.</param>
        /// <param name="searchModel">Parametri di ricerca</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Elenco degli accordi che soddisfano i criteri di ricerca.</returns>
        Task<SearchResult<Accordo, Guid>> FindAsync(Guid userId, string userRole, AccordoSearch searchModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recede da un accordo.
        /// </summary>
        /// <param name="userId">Identificativo dell'utente che richiede l'operazione.</param>
        /// <param name="accordoId">Identificativo dell'accordo.</param>
        /// <param name="recessoDate">Data di recesso.</param>
        /// <param name="note">Note</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RecediAsync(Guid userId, Guid accordoId, DateTime? recessoDate, string note, CancellationToken cancellationToken);

        /// <summary>
        /// Verifica tutti gli accordi nello stato RecessoPianificato che hanno raggiunto la data di recesso 
        /// e li passa in Recesso
        /// </summary>
        /// <param name="note">Eventuali note da associare al cambio di stato.</param>
        /// <param name="autore">Autore del recesso</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lista degli identificativi degli accordi aggiornati</returns>
        Task<ICollection<Guid>> UpdateAccordiToRecesso(string note, string autore, CancellationToken cancellationToken);

        /// <summary>
        /// Rinnova un accordo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo da rinnovare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Identificativo del nuovo accordo</returns>
        Task<Guid> RinnovaAccordoAsync(Guid id, bool revisioneAccordo, CancellationToken cancellationToken);

        /// <summary>
        /// Elimina un accordo.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo da eliminare.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAccordoAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Calcola la differenza fra due accordi.
        /// </summary>
        /// <param name="accordo1">Identificativo dell'accordo 1</param>
        /// <param name="accordo2">Identificativo dell'accordo 2</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lista delle differenze fra gli accordi.</returns>
        Task<IEnumerable<Difference>> GetDifferenzeAccordiAsync(Guid accordo1, Guid accordo2, CancellationToken cancellationToken);

        /// <summary>
        /// Verifica una serie di condizioni post cambio stato
        /// </summary>
        /// <param name="accordo"></param>
        /// <param name="cancellationToken"></param>
        Task VerificaCondizioniPostCambioStato(Guid idAccordo, CancellationToken cancellationToken);

        /// <summary>
        /// Chiude l'accordo precedente non più in corso
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ChiusuraAccordoPrecedente(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Recede da un accordo per giustificato motivo
        /// </summary>
        /// <param name="userId">Identificativo dell'utente che richiede l'operazione.</param>
        /// <param name="accordoId">Identificativo dell'accordo.</param>        
        /// <param name="dateRecessoGiustificatoMotivo">Date recesso.</param>        
        /// <param name="note">Note</param>
        /// <param name="giustificatoMotivo">Motivo del recesso</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RecediGiustificatoMotivoAsync(Guid userId, Guid accordoId, DateTime dateRecessoGiustificatoMotivo, string note, string giustificatoMotivo, CancellationToken cancellationToken);

        /// <summary>
        /// Esegue le operazioni di amminsitrazione sull'accordo
        /// </summary>
        /// <param name="idAccordo"></param>
        /// <param name="operazione"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteAmmOperation(Guid idAccordo, OperazioniAmmAccordo operazione, CancellationToken cancellationToken);

        /// <summary>
        /// Recupera gli accordi da valutare del responsabile
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> GetToDoValutazioniAsync(Guid uid, string role, CancellationToken cancellationToken);

        /// <summary>
        /// Recupera tutti gli accordi per uno specifico ruolo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRole"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<Accordo, Guid>> GetAccordiForRole(Guid userId, string userRole, CancellationToken cancellationToken = default);


        /// <summary>
        /// Rifiuta la richiesta di approvazione di una deroga dell'accordo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRole"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RifiutaDeroga(Guid userId, Guid accordoId, CancellationToken cancellationToken);
    }
}
