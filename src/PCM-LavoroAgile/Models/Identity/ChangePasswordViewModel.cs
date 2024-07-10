using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models.Identity
{
    /// <summary>
    /// View model del cambio password.
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// Password attuale dell'utente.
        /// </summary>
        [Required(ErrorMessage = "Password obbligatoria")]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Nuova password.
        /// </summary>
        [Required(ErrorMessage = "Password obbligatoria")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Password di conferma.
        /// </summary>
        [Required(ErrorMessage = "Password obbligatoria")]
        public string ConfirmPassword { get; set; }

    }

}
