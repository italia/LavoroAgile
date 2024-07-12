using System.Security;

namespace Domain.Settings
{
    /// <summary>
    /// Impostazioni r
    /// </summary>
    public sealed class AdminUserSettings
    {
        /// <summary>
        /// Nome della sezione di configurazione da cui recuperare le impostazioni per l'utente admin.
        /// </summary>
        public const string Position = "AdminUser";
        public string Username { get; set; }
        public string InitialPassword { get; set; }
    }
}
