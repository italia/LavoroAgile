using Fluid.Parser;
using System;
using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models
{
    public class DipendenteViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string NomeCognome { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [DataType(DataType.Date)]
        public DateTime? DataDiNascita { get; set; }

        public string Sesso { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string LuogoDiNascita { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string CodiceFiscale { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [EmailAddress(ErrorMessage = "Specifica una email valida")]
        public string Email { get; set; }

        public string PosizioneGiuridica { get; set; }

        public string CategoriaFasciaRetributiva { get; set; }

        public override string ToString() => NomeCognome;
    }
}
