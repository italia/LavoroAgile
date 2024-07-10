using Domain.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models
{
    /// <summary>
    /// Rappresenta il view model di una voce dello storico dei cambi di stato di un accordo.
    /// </summary>
    public class StoricoStatoViewModel
    {
        /// <summary>
        /// Stato cui si riferisce lo storico
        /// </summary>
        public StatoAccordo Stato { get; init; }

        /// <summary>
        /// Note sul passaggio di stato.
        /// </summary>
        public string Note { get; init; }

        /// <summary>
        /// Autore del cambiamento di stato.
        /// </summary>
        public string Autore { get; init; }

        /// <summary>
        /// Timestamp della registrazione.
        /// </summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
