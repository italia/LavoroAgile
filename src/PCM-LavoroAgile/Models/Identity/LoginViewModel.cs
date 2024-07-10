using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models.Identity
{
    public class LoginViewModel
    {
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Campo obbligatorio")]
            //[EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Campo obbligatorio")]
            //[DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Ricordami")]
            public bool RememberMe { get; set; }

            public bool LoginAD { get; set; }
        }
    }
}
