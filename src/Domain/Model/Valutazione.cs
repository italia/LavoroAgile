using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domain.Model
{
    public class Valutazione 
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public Guid AccordoId { get; set; }

        public string NoteDipendente { get; set; }

        public DateTime? DataNoteDipendente { get; set; }

        public string NoteDirigente { get; set; }

        public DateTime? DataNoteDirigente { get; set; }

        public bool isApprovata { get; set; }


    }
}
