using Domain.ExtensionMethods;
using Domain.Model;
using Infrastructure.Utils;
using PCM_LavoroAgile.Models.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PCM_LavoroAgile.Models
{
    /// <summary>
    /// View model dell'accordo.
    /// </summary>
    public class AccordoViewModel : IValidatableObject
    {
        /// <summary>
        /// Valorizzato a true, indica che l'accordo è stato prepopolato con
        /// dati provenienti dalle anagrafiche esterne.
        /// </summary>
        public bool InitializedFromExtenalService { get; set; } = false;

        /// <summary>
        /// Descrizione del livello 1.
        /// </summary>
        public string Livello1 { get; set; }

        /// <summary>
        /// Descrizione del livello 2.
        /// </summary>
        public string Livello2 { get; set; }

        /// <summary>
        /// Descrizione del livello 3.
        /// </summary>
        public string Livello3 { get; set; }

        public Guid Id { get; set; } = Guid.Empty;

        public DateTime CreationDate { get; set; }

        public DateTime EditTime { get; set; }

        public bool Ripianificare { get; set; }

        #region ANAGRAFICA

        public DipendenteViewModel Dipendente { get; set; }
        
        [Required(ErrorMessage = "Campo obbligatorio")]
        public string StrutturaUfficioServizio { get; set; }
        public string UidStrutturaUfficioServizio { get; set; }

        public string NumLivelliStruttura { get; set; }

        [DirigenteValidator]
        public DirigenteViewModel CapoStruttura { get; set; }

        [CapoIntermedioValidator]
        public DirigenteViewModel CapoIntermedio { get; set; }

        [DirigenteValidator]
        public DirigenteViewModel ResponsabileAccordo { get; set; }

        [DirigenteValidator]
        public ReferenteViewModel ReferenteInterno { get; set; }

        public DirigenteViewModel DirigenteResponsabile { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [DataType(DataType.Date)]
        public DateTime? DataPresaServizio { get; set; }

        #endregion ANAGRAFICA

        #region CRITERI DI PRIORITA

        public bool Priorita_1 { get; set; }

        public bool Priorita_2 { get; set; }

        #endregion CRITERI DI PRIORITA

        #region CARATTERISTICHE DELL'ACCORDO

        /// <summary>
        /// Codice solo numerico dell'accordo.
        /// </summary>
        public int Codice { get; set; }

        /// <summary>
        /// Codice completo dell'accordo.
        /// </summary>
        public string DescriptiveCode { get; set; }

        public bool PrimoAccordo { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataFineAccordoPrecedente { get; set; }

        public string ValutazioneEsitiAccordoPrecedente { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [DataType(DataType.Date)]
        public DateTime? DataInizioUtc { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        [DataType(DataType.Date)]
        public DateTime? DataFineUtc { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string Modalita { get; set; }

        public List<string> PianificazioneGiorniAccordo { get; set; }

        public bool DerogaPianificazioneDate { get; set; }

        public string PianificazioneDateAccordo { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public List<string> FasceDiContattabilita { get; set; }

        [Required(ErrorMessage = "Campo obbligatorio")]
        public string StrumentazioneUtilizzata { get; set; }

        public DateTime? DataSottoscrizione { get; set; }

        #endregion CARATTERISTICHE DELL'ACCORDO

        #region ORGANIZZAZIONE DEL LAVORO

        public List<AttivitaAccordoViewModel> ListaAttivita { get; set; } = new List<AttivitaAccordoViewModel>();

        #endregion ORGANIZZAZIONE DEL LAVORO

        #region GIUSTIFICATO MOTIVO DI RECESSO

        public string GiustificatoMotivoDiRecesso { get; set; }
        #endregion GIUSTIFICATO MOTIVO DI RECESSO

        #region INFORMATIVE

        public bool FormazioneLavoroAgile { get; set; }

        public bool SaluteESicurezza { get; set; }

        public bool AccessoVPN { get; set; }

        public bool PrivacyEConsensoTrattamentoDati { get; set; }

        #endregion INFORMATIVE

        #region STATO DELLA RICHIESTA

        public StatoAccordo Stato { get; set; }

        /// <summary>
        /// Storico dei passaggi di stato.
        /// </summary>
        public ICollection<StoricoStatoViewModel> StoricoStato { get; set; } = new List<StoricoStatoViewModel>();

        /// <summary>
        /// Indica che un accordo è in corso
        /// </summary>
        public bool InCorso { get; set; }

        #endregion STATO DELLA RICHIESTA        

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
        public bool checkValutazionePositiva { get; set; }
        public bool checkValutazioneNegativa { get; set; }

        /// <summary>
        /// Data di recesso dell'accordo.
        /// </summary>
        public DateTime? DataRecesso { get; set; }

        /// <summary>
        /// Identificativo del parent dell'accordo.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Identificativo del figlio dell'accordo.
        /// </summary>
        public Guid ChildId { get; set; }

        /// <summary>
        /// Per questo accordo è stato richiesto un rinnovo se esiste un figlio.
        /// </summary>
        public bool RinnovoRichiesto => !Guid.Empty.Equals(ChildId);

        #endregion INFORMAZIONI AGGIUNTIVE PER LA VALUTAZIONE DELL'ACCORDO

        #region REVISIONE ACCORDO
        public bool RevisioneAccordo { get; set; }

        #endregion REVISIONE ACCORDO

        #region Info di trasmissione a sistemi esterni

        /// <summary>
        /// Stato trasmissione.
        /// </summary>
        public TransmissionStatusViewModel Transmission { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Verifico la validità delle email dei responsabili e del referente interno
            if (!RegexUtilities.IsValidEmail(CapoStruttura?.Email))
                yield return new ValidationResult("Inserire una email valida", new[] { $"{nameof(CapoStruttura)}.{nameof(CapoStruttura.Email)}" });

            if (NumLivelliStruttura != null && int.Parse(NumLivelliStruttura) == 3 && !RegexUtilities.IsValidEmail(CapoIntermedio?.Email))
                yield return new ValidationResult("Inserire una email valida", new[] { $"{nameof(CapoIntermedio)}.{nameof(CapoIntermedio.Email)}" });

            if (!RegexUtilities.IsValidEmail(ResponsabileAccordo?.Email))
                yield return new ValidationResult("Inserire una email valida", new[] { $"{nameof(ResponsabileAccordo)}.{nameof(ResponsabileAccordo.Email)}" });
            
            if (!RegexUtilities.IsValidEmail(ReferenteInterno?.Email))
                yield return new ValidationResult("Inserire una email valida", new[] { $"{nameof(ReferenteInterno)}.{nameof(ReferenteInterno.Email)}" });

            //Verifica che la data fine sia maggiore della data inizio
            if (DataFineUtc <= DataInizioUtc)
            {
                yield return new ValidationResult("La data fine deve essere maggiore della data inizio", new[] { nameof(DataFineUtc) });
            }

            //Verifica che l'intervallo di tempo fra data fine e data inizio non superi i 6 mesi
            if (DataFineUtc > DataInizioUtc.Value.AddMonths(6).AddDays(-1))
            {
                yield return new ValidationResult("L'intervallo DataFine - DataInizio non può superare i 6 mesi", new[] { nameof(DataFineUtc) });
            }

            //Verifica che la data di presa servizio sia inferiore alla data inizio accordo
            if(DataPresaServizio.Value.Date > DataInizioUtc.Value.Date)
            {
                yield return new ValidationResult("La data presa servizio non può essere maggiore della data inizio accordo", new[] { nameof(DataPresaServizio) });
            }

            //Verifico le opzioni in base alla modalità di pianificazione
            switch (Modalita)
            {
                case "OrdinariaUnGiorno":
                    if (PianificazioneGiorniAccordo == null)
                    {
                        yield return new ValidationResult("Campo obbligatorio", new[] { nameof(PianificazioneGiorniAccordo) });
                    }
                    else if (PianificazioneGiorniAccordo.Count != 1)
                    {
                        yield return new ValidationResult("Selezionare un giorno", new[] { nameof(PianificazioneGiorniAccordo) });
                    }
                    break;

                case "OrdinariaDueGiorni":
                    if (PianificazioneGiorniAccordo == null)
                    {
                        yield return new ValidationResult("Campo obbligatorio", new[] { nameof(PianificazioneGiorniAccordo) });
                    }
                    else if (PianificazioneGiorniAccordo.Count != 2)
                    {
                        yield return new ValidationResult("Selezionare due giorni", new[] { nameof(PianificazioneGiorniAccordo) });
                    }
                    break;

                case "Eccezionale":
                    //Verifica che siano presenti delle date di pianificazione
                    if (String.IsNullOrEmpty(PianificazioneDateAccordo))
                    {
                        yield return new ValidationResult("Campo obbligatorio", new[] { nameof(PianificazioneDateAccordo) });
                    }
                    else
                    {
                        //Verifica che le date ricadono nell'intervallo DataInizio - DataFine
                        if (DataInizioUtc.VerificaPianificazione(DataFineUtc, PianificazioneDateAccordo))
                            yield return new ValidationResult("Le date devono ricadere nell'intervallo - " + DataInizioUtc.Value.ToString("dd/MM/yyyy") + " - " + DataFineUtc.Value.ToString("dd/MM/yyyy"), new[] { nameof(PianificazioneDateAccordo) });

                        //In caso di deroga il controllo del numero di giorni a settimana non deve essere applicato
                        if (!DerogaPianificazioneDate)
                        {
                            //Verifica che le date pianificate non prevedano più di due giorni a settimana
                            if (DataInizioUtc.VerificaNumeroGiorniSettimana(PianificazioneDateAccordo))
                                yield return new ValidationResult("Non è possibile pianificare più di due giorni a settimana", new[] { nameof(PianificazioneDateAccordo) });
                        }
                    }
                    break;
            }

            //Verifica della scelta di almeno 3 fasce di contattabilità
            if (FasceDiContattabilita != null && FasceDiContattabilita.Count < 3)
                yield return new ValidationResult("Selezionare almeno tre fasce di contattabilità", new[] { nameof(FasceDiContattabilita) });

            //Verifica che sia stata inserita almeno una Attività
            if (ListaAttivita.Count == 0)
            {
                yield return new ValidationResult("E' necessario inserire almeno una attività", new[] { nameof(ListaAttivita) });
            }

            //Verifica valorizzazione campi attività
            if(VerificaCampiAttivita())
            {
                yield return new ValidationResult("Tutti i campi delle attività sono obbligatori", new[] { nameof(ListaAttivita) });
            }

            //Verifica accettazione informative
            if (!FormazioneLavoroAgile)
                yield return new ValidationResult("Accettazione obbligatoria", new[] { nameof(FormazioneLavoroAgile) });
            if (!SaluteESicurezza)
                yield return new ValidationResult("Accettazione obbligatoria", new[] { nameof(SaluteESicurezza) });
            if (!AccessoVPN)
                yield return new ValidationResult("Accettazione obbligatoria", new[] { nameof(AccessoVPN) });
            if (!PrivacyEConsensoTrattamentoDati)
                yield return new ValidationResult("Accettazione obbligatoria", new[] { nameof(PrivacyEConsensoTrattamentoDati) });

            //Eventualemente con la possibilità di editare il responsabile accordo l'utente ha modificato i dati di quest'ultimo
            //Si allinea il dirigente responsabile con queste informazioni
            if (!ResponsabileAccordo.Equals(DirigenteResponsabile))
            {
                DirigenteResponsabile = ResponsabileAccordo;
            }
        }

        /// <summary>
        /// Verifica se le informazioni sulla struttura sono state inizializzate.
        /// </summary>
        /// <returns>true se le informazioni sulla struttura sono state inizializzate.</returns>
        public bool IsStrutturaInitialized()
        {
            return !(string.IsNullOrWhiteSpace(Livello1) && string.IsNullOrWhiteSpace(Livello2) && string.IsNullOrWhiteSpace(Livello3));
        }

        /// <summary>
        /// Varifica campi attività
        /// </summary>
        /// <returns></returns>
        private bool VerificaCampiAttivita()
        {
            bool result = false;
            
            foreach (AttivitaAccordoViewModel attivitaAccordoViewModel in ListaAttivita)
            {
                if (!String.IsNullOrEmpty(attivitaAccordoViewModel.Attivita) &&
                !String.IsNullOrEmpty(attivitaAccordoViewModel.Risultati) &&
                !String.IsNullOrEmpty(attivitaAccordoViewModel.DenominazioneIndicatore))
                {
                    switch (attivitaAccordoViewModel.TipologiaIndicatore.ToUpper())
                    {
                        case "TESTO":
                            if (String.IsNullOrEmpty(attivitaAccordoViewModel.TestoTarget))
                            {
                                result = true;
                            }
                            break;
                        case "PERCENTUALE":
                            if (!String.IsNullOrEmpty(attivitaAccordoViewModel.PercentualeIndicatoreDenominazioneNumeratore) &&
                                !String.IsNullOrEmpty(attivitaAccordoViewModel.PercentualeIndicatoreDenominazioneDenominatore))
                            {

                                if (attivitaAccordoViewModel.OperatoreLogicoIndicatorePercentuale == "compreso tra")
                                {
                                    if (String.IsNullOrEmpty(attivitaAccordoViewModel.PercentualeDaTarget) ||
                                        String.IsNullOrEmpty(attivitaAccordoViewModel.PercentualeATarget))
                                    {
                                        result = true;
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(attivitaAccordoViewModel.PercentualeTarget))
                                    {
                                        result = true;
                                    }
                                }
                            }
                            else
                            {
                                result = true;
                            }
                            break;
                        case "DATA":
                            if (attivitaAccordoViewModel.OperatoreLogicoIndicatoreData == "compreso tra")
                            {
                                if (attivitaAccordoViewModel.DataDaTarget == null ||
                                    attivitaAccordoViewModel.DataATarget == null)
                                {
                                    result = true;
                                }
                            }
                            else
                            {
                                if (attivitaAccordoViewModel.DataTarget == null)
                                {
                                    result = true;
                                }
                            }
                            break;
                        case "NUMEROASSOLUTO":
                            if (attivitaAccordoViewModel.OperatoreLogicoIndicatoreNumeroAssoluto == "compreso tra")
                            {
                                if (String.IsNullOrEmpty(attivitaAccordoViewModel.NumeroAssolutoDaTarget) ||
                                    String.IsNullOrEmpty(attivitaAccordoViewModel.NumeroAssolutoATarget))
                                {
                                    result = true;
                                }
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(attivitaAccordoViewModel.NumeroAssolutoTarget))
                                {
                                    result = true;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }        
    }
}
