using Domain.Model.ExternalCommunications;
using Domain.Model.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domain.Model
{
    /// <summary>
    /// Rappresenta un accordo
    /// </summary>
    public class Accordo : Entity<Guid>
    {
        #region ANAGRAFICA

        /// <summary>
        /// Valorizzato a true, indica che l'accordo è stato prepopolato con
        /// dati provenienti dalle anagrafiche esterne.
        /// </summary>
        [NotMapped]
        public bool InitializedFromExtenalService { get; set; } = false;

        /// <summary>
        /// Descrizione del livello 1.
        /// </summary>
        [NotMapped]
        public string Livello1 { get; set; }

        /// <summary>
        /// Descrizione del livello 2.
        /// </summary>
        [NotMapped]
        public string Livello2 { get; set; }

        /// <summary>
        /// Descrizione del livello 3.
        /// </summary>
        [NotMapped]
        public string Livello3 { get; set; }

        /// <summary>
        /// Profilo del dipendente.
        /// </summary>
        public Dipendente Dipendente { get; set; }

        public string StrutturaUfficioServizio { get; set; }

        public string UidStrutturaUfficioServizio { get; set; }

        public Dirigente CapoStruttura { get; set; }

        public Dirigente CapoIntermedio { get; set; }

        public Dirigente ResponsabileAccordo { get; set; }

        public Dirigente DirigenteResponsabile { get; set; }

        public int NumLivelliStruttura { get; set; }

        public Referente ReferenteInterno { get; set; }

        public DateTime? DataPresaServizio { get; set; }

        public string Telefono { get; set; }

        #endregion ANAGRAFICA

        #region CRITERI DI PRIORITA

        public bool Priorita_1 { get; set; }

        public bool Priorita_2 { get; set; }

        #endregion CRITERI DI PRIORITA

        #region CARATTERISTICHE DELL'ACCORDO

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codice { get; set; }

        public bool PrimoAccordo { get; set; }

        public DateTime? DataFineAccordoPrecedente { get; set; }

        public string ValutazioneEsitiAccordoPrecedente { get; set; }

        /// <summary>
        /// Data inizio dell'accordo in UTC.
        /// </summary>
        public DateTime DataInizioUtc { get; set; }

        /// <summary>
        /// Data fine dell'accordo in UTC.
        /// </summary>
        public DateTime DataFineUtc { get; set; }

        public string Modalita { get; set; }

        public string PianificazioneGiorniAccordo { get; set; }

        public bool DerogaPianificazioneDate { get; set; }

        public string PianificazioneDateAccordo { get; set; }

        public string FasceDiContattabilita { get; set; }

        public string StrumentazioneUtilizzata { get; set; }

        [NotMapped]
        public bool Ripianificare { get; set; }

        /// <summary>
        /// Data di sottoscrizione dell'accordo.
        /// </summary>
        [NotMapped]
        public DateTime? DataSottoscrizione => this.StoricoStato.FirstOrDefault(s => s.Stato == StatoAccordo.Sottoscritto)?.Timestamp;

        #endregion CARATTEREISTICHE DELL'ACCORDO

        #region ORGANIZZAZIONE DEL LAVORO

        public List<AttivitaAccordo> ListaAttivita { get; set; }

        #endregion ORGANIZZAZIONE DEL LAVORO

        #region INFORMATIVE

        public bool FormazioneLavoroAgile { get; set; }

        public bool SaluteESicurezza { get; set; }

        public bool AccessoVPN { get; set; }

        public bool PrivacyEConsensoTrattamentoDati { get; set; }

        #endregion INFORMATIVE

        #region STATO E STORICO DELLA RICHIESTA

        public StatoAccordo Stato { get; set; }

        /// <summary>
        /// Indica che un accordo è in corso
        /// </summary>
        /// <remarks>
        /// Un accordo è in corso se:
        /// - Si trova nello stato Sottoscritto e la data di oggi è compreso fra data inizio e data fine
        /// - Si trova nello stato RecessoPianificato
        /// </remarks>
        [NotMapped]
        public bool InCorso => DateTime.UtcNow >= DataInizioUtc.Date && DateTime.Now.Date <= DataFineUtc.Date &&
                    Stato != StatoAccordo.RifiutataRA &&
                    Stato != StatoAccordo.RifiutataCI &&
                    Stato != StatoAccordo.RifiutataCS &&
                    Stato != StatoAccordo.AccordoConclusoCambioStrutturaDipendente &&
                    Stato != StatoAccordo.Recesso;

        //private List<StoricoStato> _storicoStato = new List<StoricoStato>();

        //public IReadOnlyCollection<StoricoStato> StoricoStato => _storicoStato.AsReadOnly();

        /// <summary>
        /// Aggiunge una voce allo storico degli accordi.
        /// </summary>
        /// <param name="note">Note</param>
        /// <param name="statoAccordo">Stato da storicizzare</param>
        /// <param name="autore">Autore del passaggio di stato.</param>
        public void AddStoricoStato(string note, StatoAccordo statoAccordo, string autore)
        {
            StoricoStato.Add(new StoricoStato(note, statoAccordo, autore));
        }

        public ICollection<StoricoStato> StoricoStato { get; set; } = new List<StoricoStato>();

        #endregion STATO E STORICO DELLA RICHIESTA

        #region REFERENTE INTERNO

        public string NoteRefereteInterno { get; set; }

        #endregion REFERENTE INTERNO

        #region SEGRETERIA TECNICA

        public bool VistoSegreteriaTecnica { get; set; }

        public string NoteSegreteriaTecnica { get; set; }

        public string NoteCondivise { get; set; }

        public bool InvioNotificaNoteCondivise { get; set; }

        #endregion SEGRETERIA TECNICA

        #region INFORMAZIONI AGGIUNTIVE PER LA VALUTAZIONE DELL'ACCORDO

        public string NotaDipendente { get; set; }
        public DateTime? DataNotaDipendente { get; set; }
        public string NotaDirigente { get; set; }
        public DateTime? DataNotaDirigente { get; set; }
        public bool isValutazionePositiva { get; set; }

        #endregion INFORMAZIONI AGGIUNTIVE PER LA VALUTAZIONE DELL'ACCORDO

        #region Gestione recesso / revisione

        /// <summary>
        /// Indica se l'accordo è recidibile 
        /// </summary>
        public bool Recidibile => DateTime.Now.Date.AddDays(30) < DataFineUtc;

        /// <summary>
        /// Data di recesso dell'accordo.
        /// </summary>
        public DateTime? DataRecesso { get; set; }

        public string GiustificatoMotivoDiRecesso { get; set; }

        public bool RevisioneAccordo { get; set; }

        #endregion

        #region GESTIONE RINNOVO

        /// <summary>
        /// Indica se un accordo è rinnovabile
        /// </summary>
        /// <remarks>
        /// Un accordo è rinnovabile se:
        /// - E' in corso
        /// - L'utente non è inibito la creazione di un nuovo accordo (invece di andare sui claims
        ///   verifica se la valutazione è negativa)
        /// - La data attuale è minore dell'ultimo giorno del mese prima della scadenza dell'accordo.
        /// - Non è stato richiesto un recesso
        /// </remarks>
        public bool Rinnovabile => InCorso && !DataRecesso.HasValue && this.isValutazionePositiva; //&& new DateTime(DataFineUtc.Year, DataFineUtc.Month, 1).AddDays(-1) >= DateTime.UtcNow && !DataRecesso.HasValue && this.isValutazionePositiva;

        #endregion GESTIONE RINNOVO

        #region COMUNICAZIONE CON SISTEMI ESTERNI

        /// <summary>
        /// Stato della trasmissione accordo verso sistemi esterni.
        /// </summary>
        public virtual TransmissionStatus Transmission { get; set; }
        
        /// <summary>
        /// Codice comunicazione valorizzato con la response della chiamata d'invio nuova comunicazione al Ministero del lavoro
        /// </summary>
        public string CodiceComunicazioneMinisteroLavoro { get; set; }

        #endregion

        public override Entity<Guid> Clone()
        {
            var accordo = (Accordo)base.Clone();

            accordo.Codice = 0;
            // Data inizio parte dalla data di fine + 1
            accordo.DataInizioUtc = DataFineUtc.AddDays(1);
            accordo.DataFineUtc = DataFineUtc.AddDays(1);

            accordo.DataNotaDipendente = null;
            accordo.DataNotaDirigente = null;
            accordo.DataRecesso = null;
            accordo.CreationDate = DateTime.UtcNow;
            accordo.EditTime = accordo.CreationDate;
            accordo.InvioNotificaNoteCondivise = false;
            accordo.ValutazioneEsitiAccordoPrecedente = accordo.isValutazionePositiva ? "Positiva" : "Negativo";
            accordo.isValutazionePositiva = false;
            accordo.NotaDipendente = string.Empty;
            accordo.NotaDirigente = string.Empty;
            accordo.NoteCondivise = string.Empty;
            accordo.NoteRefereteInterno = string.Empty;
            accordo.NoteSegreteriaTecnica = string.Empty;
            accordo.PrimoAccordo = false;
            accordo.StoricoStato = null;
            accordo.VistoSegreteriaTecnica = false;
            accordo.Stato = StatoAccordo.Bozza;
            accordo.Id = Guid.NewGuid();
            accordo.Transmission = null;
            accordo.CodiceComunicazioneMinisteroLavoro = null;

            accordo.CapoStruttura = accordo.CapoStruttura.Clone() as Dirigente;
            accordo.CapoIntermedio = accordo.CapoIntermedio?.Clone() as Dirigente;
            accordo.ResponsabileAccordo = accordo.ResponsabileAccordo?.Clone() as Dirigente;
            accordo.DirigenteResponsabile = accordo.DirigenteResponsabile?.Clone() as Dirigente;
            accordo.ReferenteInterno = accordo.ReferenteInterno?.Clone() as Referente;
            accordo.Dipendente = accordo.Dipendente?.Clone() as Dipendente;

            var listaAttività = accordo.ListaAttivita;
            accordo.ListaAttivita = new List<AttivitaAccordo>();
            listaAttività.ForEach(a =>
            {
                accordo.ListaAttivita.Add(a.Clone());
            });
            return accordo;
        }
    }

    /// <summary>
    /// Enumerazioni degli stati in cui si può trovare un accordo.
    /// </summary>
    public enum StatoAccordo
    {
        [Description("Bozza")]
        Bozza,
        [Description("Da Approvare e Sottoscrivere – Responsabile Accordo")]
        DaApprovareRA,
        [Description("Da Approvare - Capo Intermedio")]
        DaApprovareCI,
        [Description("Da Approvare - Capo Struttura")]
        DaApprovareCS,
        [Description("Richiesta Modifica/Integrazione – Responsabile Accordo")]
        RichiestaModificaRA,
        [Description("Richiesta Modifica/Integrazione - Capo Intermedio")]
        RichiestaModificaCI,
        [Description("Richiesta Modifica/Integrazione - Capo Struttura")]
        RichiestaModificaCS,
        [Description("Rifiutata – Responsabile Accordo")]
        RifiutataRA,
        [Description("Rifiutata - Capo Intermedio")]
        RifiutataCI,
        [Description("Rifiutata - Capo Struttura")]
        RifiutataCS,
        [Description("Da Sottoscrivere – Responsabile Accordo")]
        DaSottoscrivereRA,
        [Description("Da Sottoscrivere - Proponente")]
        DaSottoscrivereP,
        [Description("Sottoscritto")]
        Sottoscritto,
        [Description("Accordo in corso")]
        InCorso,
        [Description("Accordo con proposte modifica in corso")]
        AccordoConProposteModifica,
        [Description("Accordo concluso")]
        AccordoConcluso,
        [Description("Accordo concluso anticipatamente per cambio struttura da parte del dipendente")]
        AccordoConclusoCambioStrutturaDipendente,
        [Description("Chiuso per recesso")]
        Recesso,
        [Description("Recesso pianificato")]
        RecessoPianificato,
        [Description("Chiuso per revisione accordo")]
        ChiusoPerRevisioneAccordo,
        [Description("In attesa di valutazione della Segr. Tecnica")]
        InAttesaValutazioneSegreteriaTecnica
    }

    /// <summary>
    /// Enumerazioni delle operazioni possibili su un accordo da amministrazione
    /// </summary>
    public enum OperazioniAmmAccordo
    {
        [Description("Eliminazione valutazioni accordo")]
        EliminazioneValutazioniAccordo = 1,
        [Description("Eliminazione valutazione responsabile accordo")]
        EliminazioneValutazioneResponsabileAccordo = 2,
        [Description("Eliminazione accordo")]
        EliminazioneAccordo = 3,
        [Description("Riportare in bozza accordo")]
        RiportareInBozzaAccordo = 4,
        /// <summary>
        /// Reinvio di giornate e attività a Zucchetti.
        /// </summary>
        ReinviareInformazioniAZucchetti = 5,
        /// <summary>
        /// Cancellazione delle giornate da Zucchetti.
        /// </summary>
        CancellazioneGiornate = 6,
        [Description("Reinvio comunicazione Ministero del Lavoro")]
        ReinvioComunicazioneMinisteroLavoro = 7
    }

}