using Domain.Model.Identity;
using Domain.Settings;
using Microsoft.Extensions.Options;
using System;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;

namespace Infrastructure.Identity
{
    /// <summary>
    /// Implementa l'interazione con l'Active Directory tramite le librerie System.DirectoryServices
    /// </summary>
    public class LdapService : ILdapService
    {
        /// <summary>
        /// Uri del domain controller da interrogare per autenticare un utente.
        /// </summary>
        private readonly string _domainControllerUri;

        /// <summary>
        /// Inizializza un nuovo <see cref="LdapService"/>.
        /// </summary>
        /// <param name="ldapSettingsOptions">Configurazione LDAP.</param>
        public LdapService(IOptions<LdapSettings> ldapSettingsOptions)
        {
            if (string.IsNullOrWhiteSpace(ldapSettingsOptions.Value?.Uri))
            {
                throw new ArgumentNullException(nameof(ldapSettingsOptions.Value.Uri));
            }
            _domainControllerUri = ldapSettingsOptions.Value?.Uri;
        }

        public bool Authenticate(string username, string password)
        {
            // Tenta prima con il sAMAccountName, quindi se fallisce tenta
            // con le network credentials
            if (AuthenticateWithDirectorySearcher(username, password))
            {
                return true;
            }

            return AuthenticateWithNetworkCredentials(username, password);

        }

        /// <summary>
        /// Autentica l'utente con il SearchDirectory (funziona con username uguale al sAMAccountName.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>true se l'autenticazione va a buon fine.</returns>
        internal bool AuthenticateWithDirectorySearcher(string username, string password)
        {
            bool ret;
            try
            {
                var de = new DirectoryEntry(_domainControllerUri, username, password);
                var dsearch = new DirectorySearcher(de);

                dsearch.FindOne();

                ret = true;
            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Autentica l'utente con le network credentials (funziona con gli UPN).
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>true se l'autenticazione va a buon fine.</returns>
        internal bool AuthenticateWithNetworkCredentials(string username, string password)
        {
            try
            {
                using (var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier((string)null, false, false)))
                {
                    var networkCredentials = new NetworkCredential(username, password);
                    ldapConnection.Credential = networkCredentials;
                    ldapConnection.AuthType = AuthType.Negotiate;
                    ldapConnection.Bind(networkCredentials);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
