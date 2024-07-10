using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PCM_LavoroAgile.Models.Validators
{
    /// <summary>
    /// Applicato ad un oggetto dirigente, verifica che ne siano valorizzati nome ed email.
    /// </summary>
    public class DirigenteValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dirigente = value as DirigenteViewModel;

            var invalidMembers = new List<string>();
            
            if (string.IsNullOrWhiteSpace(dirigente?.NomeCognome))
            {
                invalidMembers.Add(nameof(dirigente.NomeCognome));
            }
            if (string.IsNullOrWhiteSpace(dirigente?.Email))
            {
                invalidMembers.Add(nameof(dirigente.Email));
            }

            if (invalidMembers.Any())
            {
                return new ValidationResult("Campo obbligatorio", invalidMembers);
            }

            return ValidationResult.Success;
        }
    }
}
