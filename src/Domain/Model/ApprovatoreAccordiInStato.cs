using System;
using System.Collections.Generic;

namespace Domain.Model
{
    /// <summary>
    /// Modella la correlazione fra approvatore e informazioni sugli
    /// accordi in un particolare stato.
    /// </summary>
    public class ApprovatoreAccordiInStato
    {
        /// <summary>
        /// Nome dell'approvatore.
        /// </summary>
        public string ApproverName { get; set; }

        /// <summary>
        /// Email dell'approvatore.
        /// </summary>
        public string ApproverEmail { get; set; }

        /// <summary>
        /// Lista degli accordi.
        /// </summary>
        public List<ApprovatoreAccordiInStatoAccordo> Accordi { get; set; }

        /// <summary>
        /// Modella le informazioni sull'accordo nello stato richiesto.
        /// </summary>
        public class ApprovatoreAccordiInStatoAccordo
        {
            /// <summary>
            /// Identificativo accordo.
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Nome del dipendente.
            /// </summary>
            public string User { get; set; }
        }
        
    }

}
