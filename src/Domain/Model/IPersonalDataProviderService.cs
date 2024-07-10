using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Definisce il contratto che deve essere implementato da un servizio 
    /// fornitore di dati anagrafici.
    /// </summary>
    public interface IPersonalDataProviderService
    {
        /// <summary>
        /// Recupera i dati di un utente a partire dal suo identificativo.
        /// </summary>
        /// <param name="uid">Identificativo dell'utente di cui recuperare i dettagli.</param>
        /// <returns>Dettaglio dell'utente.</returns>
        public Task<Dipendente> GetUserDataAsync(string uid);

        /// <summary>
        /// Indica che l'integrazione è attiva.
        /// </summary>
        public bool Enabled { get; }
    }
}
