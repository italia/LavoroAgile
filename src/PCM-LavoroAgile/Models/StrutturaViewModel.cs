using Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models
{
    public class StrutturaViewModel : IValidatableObject
    {
        public Guid Id { get; set; }

        public string Author { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EditTime { get; set; }

        public byte[] Timestamp { get; set; }

        public string StrutturaCompleta { get; set; }

        public string StrutturaLiv1 { get; set; }

        public string StrutturaLiv2 { get; set; }

        public string StrutturaLiv3 { get; set; }

        public bool DirettaCollaborazione { get; set; }

        public DirigenteViewModel CapoStruttura { get; set; }

        public DirigenteViewModel CapoIntermedio { get; set; }

        public DirigenteViewModel DirigenteResponsabile { get; set; }

        public DirigenteViewModel ResponsabileAccordo { get; set; }

        public ReferenteViewModel ReferenteInterno { get; set; }

        /// <summary>
        /// Strutture gestite con una integrazione esterna, solo primo livello = true
        /// Strutture gestite internamente da db, solo primo livello = false
        /// </summary>
        public bool OnlyFirstLevel { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(OnlyFirstLevel)
            {
                //Se le strutture sono gestite son una integrazione esterna,
                //va verificata solo l'obbligatorietà della struttura di primo livello e dei dati del referente interno
                if (string.IsNullOrWhiteSpace(StrutturaLiv1))
                    yield return new ValidationResult("Campo obbligatorio", new[] { nameof(StrutturaLiv1) });
                
                //Per motivi organizzativi al momento l'obbligatorietà del referente interno è stata rilassata
                //if (string.IsNullOrWhiteSpace(ReferenteInterno))
                //    yield return new ValidationResult("Campo obbligatorio", new[] { nameof(ReferenteInterno) });
                //if (string.IsNullOrWhiteSpace(EmailReferenteInterno))
                //    yield return new ValidationResult("Campo obbligatorio", new[] { nameof(EmailReferenteInterno) });
                //if(!RegexUtilities.IsValidEmail(EmailReferenteInterno))
                //    yield return new ValidationResult("Inserire una email valida", new[] { nameof(EmailReferenteInterno) });
            }
            else
            {
                //Se le strutture sono gestine internamente, vanno verificati tutte le obbligatorietà

                //Livello 1, Dati capo struttura, Dati responsabile accordo e Dati Referente interno sono obbligatori
                if (string.IsNullOrWhiteSpace(StrutturaLiv1))
                    yield return new ValidationResult("Campo obbligatorio", new[] { nameof(StrutturaLiv1) });

                if (string.IsNullOrWhiteSpace(CapoStruttura?.NomeCognome))
                    yield return new ValidationResult("Campo obbligatorio", new[] { $"{nameof(CapoStruttura)}.{nameof(CapoStruttura.NomeCognome)}" });
                if (string.IsNullOrWhiteSpace(CapoStruttura?.Email))
                    yield return new ValidationResult("Campo obbligatorio", new[] { $"{nameof(CapoStruttura)}.{nameof(CapoStruttura.Email)}" });
                if (!RegexUtilities.IsValidEmail(CapoStruttura?.Email))
                    yield return new ValidationResult("Inserire una email valida", new[] { $"{nameof(CapoStruttura)}.{nameof(CapoStruttura.Email)}" });

                if (!string.IsNullOrWhiteSpace(StrutturaLiv2))
                {
                    if (string.IsNullOrWhiteSpace(CapoIntermedio?.NomeCognome))
                        yield return new ValidationResult("Campo obbligatorio", new[] { $"{nameof(CapoIntermedio)}.{nameof(CapoIntermedio.NomeCognome)}" });
                    if (string.IsNullOrWhiteSpace(CapoStruttura?.Email))
                        yield return new ValidationResult("Campo obbligatorio", new[] { $"{nameof(CapoIntermedio)}.{nameof(CapoIntermedio.Email)}" });
                    if (!RegexUtilities.IsValidEmail(CapoStruttura?.Email))
                        yield return new ValidationResult("Inserire una email valida", new[] { $"{nameof(CapoIntermedio)}.{nameof(CapoIntermedio.Email)}" });
                }

                if (string.IsNullOrWhiteSpace(ResponsabileAccordo?.NomeCognome))
                    yield return new ValidationResult("Campo obbligatorio", new[] { $"{nameof(ResponsabileAccordo)}.{nameof(ResponsabileAccordo.NomeCognome)}" });
                if (string.IsNullOrWhiteSpace(ResponsabileAccordo?.Email))
                    yield return new ValidationResult("Campo obbligatorio", new[] { $"{nameof(ResponsabileAccordo)}.{nameof(ResponsabileAccordo.Email)}" });
                if (!RegexUtilities.IsValidEmail(ResponsabileAccordo?.Email))
                    yield return new ValidationResult("Inserire una email valida", new[] { $"{nameof(ResponsabileAccordo)}.{nameof(ResponsabileAccordo.Email)}" });

                //Per motivi organizzativi al momento l'obbligatorietà del referente interno è stata rilassata
                //if (string.IsNullOrWhiteSpace(ReferenteInterno))
                //    yield return new ValidationResult("Campo obbligatorio", new[] { nameof(ReferenteInterno) });
                //if (string.IsNullOrWhiteSpace(EmailReferenteInterno))
                //    yield return new ValidationResult("Campo obbligatorio", new[] { nameof(EmailReferenteInterno) });
                //if (!RegexUtilities.IsValidEmail(EmailReferenteInterno))
                //    yield return new ValidationResult("Inserire una email valida", new[] { nameof(EmailReferenteInterno) });                
            }            
        }
    }
}
