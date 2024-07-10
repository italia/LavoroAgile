using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class AttivitaAccordo
    {
        public int Id { get; set; }

        public Guid AccordoId { get; set; }

        public int Index { get; set; }

        public string Attivita { get; set; }

        public string Risultati { get; set; }

        public string DenominazioneIndicatore { get; set; }

        public string TipologiaIndicatore { get; set; }

        public string OperatoreLogicoIndicatoreTesto { get; set; }

        public string TestoTarget { get; set; }

        public string TestoTargetRaggiunto { get; set; }

        public string OperatoreLogicoIndicatoreNumeroAssoluto { get; set; }

        public string NumeroAssolutoTarget { get; set; }

        public string NumeroAssolutoRaggiunto { get; set; }

        public string NumeroAssolutoDaTarget { get; set; }

        public string NumeroAssolutoATarget { get; set; }

        public string PercentualeIndicatoreDenominazioneNumeratore { get; set; }

        public string PercentualeIndicatoreDenominazioneDenominatore { get; set; }

        public string OperatoreLogicoIndicatorePercentuale { get; set; }

        public string PercentualeTarget { get; set; }
        public string PercentualeDenominatoreTargetRaggiunto { get; set; }
        public string PercentualeNumeratoreTargetRaggiunto { get; set; }

        public string PercentualeTargetRaggiunto { get; set; }

        public string PercentualeDaTarget { get; set; }

        public string PercentualeATarget { get; set; }

        public string OperatoreLogicoIndicatoreData { get; set; }

        public DateTime? DataTarget { get; set; }

        public DateTime? DataTargetRaggiunto { get; set; }

        public DateTime? DataDaTarget { get; set; }

        public DateTime? DataATarget { get; set; }

        public AttivitaAccordo Clone()
        {
            var attivita = (AttivitaAccordo)base.MemberwiseClone();
            attivita.Id = 0;
            attivita.AccordoId = Guid.Empty;
            return attivita;
        }
    }
}
