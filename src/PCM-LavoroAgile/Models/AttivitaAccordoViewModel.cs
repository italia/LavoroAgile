using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Models
{
    public class AttivitaAccordoViewModel
    {
        public int Index { get; set; }

        public int Id { get; set; }

        public string Attivita { get; set; }

        public string Risultati { get; set; }

        public string DenominazioneIndicatore { get; set; }

        public string TipologiaIndicatore { get; set; }

        //Tipologia indicatore TESTO
        //public string TestoIndicatore { get; set; }

        public string OperatoreLogicoIndicatoreTesto { get; set; }

        public string TestoTarget { get; set; }
        public string TestoTargetRaggiunto { get; set; }

        //Tipologia NUMERO ASSOLUTO
        //public string NumeroAssolutoIndicatore { get; set; }

        public string OperatoreLogicoIndicatoreNumeroAssoluto { get; set; }

        public string NumeroAssolutoTarget { get; set; }
        public string NumeroAssolutoRaggiunto { get; set; }

        public string NumeroAssolutoDaTarget { get; set; }

        public string NumeroAssolutoATarget { get; set; }

        //Tipologia indicatore PERCENTUALE
        public string PercentualeIndicatoreDenominazioneNumeratore { get; set; }

        public string PercentualeIndicatoreDenominazioneDenominatore { get; set; }
        public string PercentualeDenominatoreTargetRaggiunto { get; set; }
        public string PercentualeNumeratoreTargetRaggiunto { get; set; }

        public string OperatoreLogicoIndicatorePercentuale { get; set; }

        public string PercentualeTarget { get; set; }
        public string PercentualeTargetRaggiunto { get; set; }

        public string PercentualeDaTarget { get; set; }

        public string PercentualeATarget { get; set; }

        //Tipologia indicatore DATA
        //public DateTime? DataIndicatore { get; set; }

        public string OperatoreLogicoIndicatoreData { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataTarget { get; set; }
        public DateTime? DataTargetRaggiunto { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataDaTarget { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataATarget { get; set; }

    }
}
