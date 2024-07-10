using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models.Identity
{
    public class CreateUserViewModel
    {
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Campo obbligatorio")]
            [StringLength(50, ErrorMessage = "Il {0} deve essere lungo almeno {2} e al massimo {1} caratteri.", MinimumLength = 6)]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Campo obbligatorio")]
            //[EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            //[Required(ErrorMessage = "Campo obbligatorio")]
            [StringLength(100, ErrorMessage = "Il {0} deve essere lungo almeno {2} e al massimo {1} caratteri.", MinimumLength = 6)]
            //[DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            //[DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "La password e la password di conferma non corrispondono.")]
            public string ConfirmPassword { get; set; }

            //[Required]
            [Display(Name = "User Role")]
            public string UserRole { get; set; }
        }
    }
}
