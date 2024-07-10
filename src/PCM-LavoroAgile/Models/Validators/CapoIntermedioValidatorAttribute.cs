using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models.Validators
{
    public class CapoIntermedioValidatorAttribute : DirigenteValidatorAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var accordo = (AccordoViewModel)validationContext.ObjectInstance;
            if (accordo.NumLivelliStruttura != null && int.Parse(accordo.NumLivelliStruttura) == 3) 
            {
                return base.IsValid(value, validationContext);
            }
            return ValidationResult.Success;
        }
    }
}
