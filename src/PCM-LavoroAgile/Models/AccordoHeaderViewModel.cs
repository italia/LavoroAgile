using Domain.Model;
using System;

namespace PCM_LavoroAgile.Models
{
    /// <summary>
    /// Informazioni minimali si un accordo.
    /// </summary>
    public class AccordoHeaderViewModel
    {
        /// <summary>
        /// Identificativo dell'accordo.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Codice dell'accordo.
        /// </summary>
        public string DescriptiveCode { get; set; }

        /// <summary>
        /// Data di sottoscrizione dell'accordo.
        /// </summary>
        public DateTime? DataSottoscrizione { get; set; }

        /// <summary>
        /// Data inizio accordo in UTC.
        /// </summary>
        public DateTime DataInizioUtc { get; set; }

        /// <summary>
        /// Data fine accordo in UTC.
        /// </summary>
        public DateTime DataFineUtc { get; set; }

        /// <summary>
        /// Stato dell'accordo.
        /// </summary>
        public StatoAccordo Stato { get; set; }

        /// <summary>
        /// Indica che un accordo è in corso
        /// </summary>
        public bool InCorso { get; set; }

        /// <summary>
        /// Identificativo del dirigente responsabile di questo accordo.
        /// </summary>
        public string ResponsabileAccordo { get; set; }

        /// <summary>
        /// Id del responsabile dell'accordo.
        /// </summary>
        public Guid IdResponsabileAccordo { get; set; }

        /// <summary>
        /// Identificativo del capo intermedio di questo accordo.
        /// </summary>
        public string CapoIntermedio { get; set; }

        /// <summary>
        /// Identificativo del capo struttura di questo accordo.
        /// </summary>
        public string CapoStruttura { get; set; }

        /// <summary>
        /// Nome e cognome del proponente.
        /// </summary>
        public string NomeCognome { get; set; }

        /// <summary>
        /// Data recesso dell'accordo.
        /// </summary>
        public DateTime? DataRecesso { get; set; }

        /// <summary>
        /// Identificativo dell'accordo figlio (presente in caso di rinnovo)
        /// </summary>
        public Guid ChildId { get; set; }

        /// <summary>
        /// Indica se si tratta di un rinnovo.
        /// </summary>
        public bool IsRinnovo { get; set; }

        /// <summary>
        /// Indica se un rinnovo di accordo è rinnovabile
        /// </summary>
        /// <remarks>L'accordo è rinnovabile nel caso in cui ci si trova entro le due
        /// settimane antecedenti la fine del contratto precedente e se c'è una valutazione positiva
        /// Questi controlli vengono effettuati nella mappatura dell'accordo con il risultato
        /// di ricerca.
        /// </remarks>
        public bool Rinnovabile { get; set; }

        /// <summary>
        /// Indica l'esito della valutazione
        /// </summary>
        public bool isValutazionePositiva { get; set; }

        /// <summary>
        /// Data note dipendente
        /// </summary>
        public DateTime? DataNotaDipendente { get; set; }

        /// <summary>
        /// Data note responsabile accordo
        /// </summary>
        public DateTime? DataNotaDirigente { get; set; }

        /// <summary>
        /// Informazioni sullo stato di trasmissione.
        /// </summary>
        public TransmissionStatusViewModel Transmission { get; set; }

        /// <summary>
        /// Mail Responsabile accordo
        /// </summary>
        public string EmailResponsabileAccordo { get; set; }

    }

}
