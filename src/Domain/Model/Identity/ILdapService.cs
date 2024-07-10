namespace Domain.Model.Identity
{
    /// <summary>
    /// Definisce il contratto per l'implementazione dell'integrazione LDAP.
    /// </summary>
    public interface ILdapService
    {
        /// <summary>
        /// Autentica un utente sull'Active Directory.
        /// </summary>
        /// <param name="username">Username dell'utente.</param>
        /// <param name="password">Password dell'utente.</param>
        /// <returns>true se l'utente è stato autenticato con successo; false altrimenti.</returns>
        bool Authenticate(string username, string password);

    }
}
