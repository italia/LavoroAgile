using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    /// <summary>
    /// Rappresenta lo storico di un passaggio di stato di un accordo.
    /// </summary>
    public class StoricoStato
    {
        /// <summary>
        /// Identificativo della riga di audit sul passaggio di stato.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }

        /// <summary>
        /// Stato per cui si sta registrando l'audit.
        /// </summary>
        public StatoAccordo Stato { get; init; }

        /// <summary>
        /// Note sul passaggio di stato.
        /// </summary>
        public string Note { get; init; }

        /// <summary>
        /// Autore del cambiamento di stato.
        /// </summary>
        public string Autore { get; set; }

        /// <summary>
        /// Timestamp della registrazione.
        /// </summary>
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Identificativo dell'accordo cui si riferisce questa voce di storico.
        /// </summary>
        public Guid AccordoId { get; init; }

        /// <summary>
        /// Inizializza un nuovo <see cref="StoricoStato"/>.
        /// </summary>
        public StoricoStato()
        {

        }

        /// <summary>
        /// Inizializza un nuovo <see cref="StoricoStato"/>.
        /// </summary>
        /// <param name="note">Note sull'assegnazione di stato.</param>
        /// <param name="statoAccordo">Stato da storizzare</param>
        /// <param name="autore">Autore del passaggio di stato</param>
        public StoricoStato(string note, StatoAccordo statoAccordo, string autore)
            : this()
        {
            Note = note;
            Stato = statoAccordo;
            this.Autore = autore;
        }

        /// <summary>
        /// Inizializza un nuovo <see cref="StoricoStato"/>.
        /// </summary>
        /// <param name="note">Note sull'assegnazione di stato.</param>
        /// <param name="statoAccordo">Stato da storizzare</param>
        /// <param name="autore">Autore del cambimento di stato.</param>
        /// <param name="accordoId">Identificativo dell'accordo cui si riferisce questa voce di storico.</param>
        public StoricoStato(string note, StatoAccordo statoAccordo, string autore, Guid accordoId)
            : this(note, statoAccordo, autore)
        {
            AccordoId = accordoId;
        }
    }
}
