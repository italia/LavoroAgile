using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Models
{
    public class SegreteriaTecnicaViewModel
    {
        public Guid Id { get; set; }

        public string Author { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EditTime { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [EmailAddress(ErrorMessage = "Specifica una email valida")]
        public string EmailUtente { get; set; }
    }
}
