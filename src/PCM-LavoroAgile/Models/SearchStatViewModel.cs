using System.Collections.Generic;
using Domain.Model;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models;
using PCM_LavoroAgile.Models.Search;
using System.Linq;
using Domain.Model.Utilities;
using System;

namespace PCM_LavoroAgile.Models.Search
{
    /// <summary>
    /// Collezione dei risultati di ricerca.
    /// </summary>
    public class SearchStatViewModel
    {
        /// <summary>
        /// Inizializza un nuovo <see cref="SearchStatViewModel"/>.
        /// </summary>
        /// <param name="totalElements">Numero totale di elementi restituiti dalla ricerca.</param>
        /// <param name="searchResults">Risultati di ricerca</param>
        //public SearchStatViewModel(List<Accordo> _searchResults,string ruolo)
        public SearchStatViewModel(List<AccordoViewModel> _searchResults, string ruolo)
        {
            totalElements = _searchResults.Count();

            Acc_sottoscritti = _searchResults.Where(s => s.Stato == StatoAccordo.Sottoscritto ||
                                                     s.Stato == StatoAccordo.InCorso ||
                                                    s.Stato == StatoAccordo.AccordoConProposteModifica ||
                                                    s.Stato == StatoAccordo.AccordoConcluso ||
                                                    s.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente || 
                                                    s.Stato == StatoAccordo.Recesso || 
                                                    s.Stato == StatoAccordo.RecessoPianificato
                                                    ).Count();
            //bugfixSimone 
            Acc_PrimoAccordo = _searchResults.Where(s => s.PrimoAccordo && (s.Stato == StatoAccordo.Sottoscritto ||
                                                     s.Stato == StatoAccordo.InCorso ||
                                                    s.Stato == StatoAccordo.AccordoConProposteModifica ||
                                                    s.Stato == StatoAccordo.AccordoConcluso ||
                                                    s.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente ||
                                                    s.Stato == StatoAccordo.Recesso ||
                                                    s.Stato == StatoAccordo.RecessoPianificato)).Count();
            //bugfixSimone calcolo il numero di giorni attraverso il campo PianificazioneDateAccordo
          
            if (Acc_sottoscritti > 0)
            {
                int giorni = String.Join(",", _searchResults.Where(s => s.PianificazioneDateAccordo is not null && (s.Stato == StatoAccordo.Sottoscritto || s.Stato == StatoAccordo.InCorso || s.Stato == StatoAccordo.AccordoConProposteModifica || s.Stato == StatoAccordo.AccordoConcluso || s.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente || s.Stato == StatoAccordo.Recesso || s.Stato == StatoAccordo.RecessoPianificato)).Select(a => a.PianificazioneDateAccordo)).Split(",").Length;
                Acc_TotGiornateLavoro = giorni;
                Acc_MediaGiornateLavoro = Decimal.Round(giorni / Convert.ToDecimal(Acc_sottoscritti),2);
            }
            else
            {
                Acc_TotGiornateLavoro = 0; Acc_MediaGiornateLavoro = 0;
            }
            //SUM(DataFineUtc - DataInizioUtc) WHERE Stato = Sottoscritto
            //SUM(DataFineUtc - DataInizioUtc) / TOT

            Acc_NoValidateResponsabileAccordo = _searchResults.Where(s => s.Stato == StatoAccordo.RifiutataRA).Count();
            Acc_NonValidateCapoIntermedio = _searchResults.Where(s => s.Stato == StatoAccordo.RifiutataCI).Count();
            Acc_NoValidateCapoStruttura = _searchResults.Where(s => s.Stato == StatoAccordo.RifiutataCS).Count();
            
            Acc_AttesaResponsabileAccordo = _searchResults.Where(s => s.Stato == StatoAccordo.DaApprovareRA).Count();
            Acc_AttesaCapoIntermedio = _searchResults.Where(s => s.Stato == StatoAccordo.DaApprovareCI).Count();
            Acc_AttesaCapoStruttura = _searchResults.Where(s => s.Stato == StatoAccordo.DaApprovareCS).Count();
            Acc_AttesaRichiestaIntegrazione = _searchResults.Where(s => s.Stato == StatoAccordo.RichiestaModificaRA ||
                                                    s.Stato == StatoAccordo.RichiestaModificaCI ||
                                                    s.Stato == StatoAccordo.RichiestaModificaCS
                                                    ).Count();
            Acc_AttesaSottoscrizione = _searchResults.Where(s => s.Stato == StatoAccordo.DaSottoscrivereRA ||
                                                    s.Stato == StatoAccordo.DaSottoscrivereP
                                                    ).Count();

            if (Acc_AttesaSottoscrizione > 0)
            {
                int giorni = String.Join(",", _searchResults.Where(s => s.PianificazioneDateAccordo is not null && (s.Stato == StatoAccordo.DaSottoscrivereRA || s.Stato == StatoAccordo.DaSottoscrivereP)).Select(a => a.PianificazioneDateAccordo)).Split(",").Length;
                Acc_TotGiornateLavoroAttesa = giorni;
                Acc_MediaGiornateLavoroAttesa = Decimal.Round(giorni / Convert.ToDecimal(Acc_AttesaSottoscrizione), 2);
            }
            else
            {
                Acc_TotGiornateLavoroAttesa = 0; Acc_MediaGiornateLavoroAttesa = 0;
            }
            role = ruolo;

            //bugfixSimone statistiche aggiuntive relative al cruscotto della Segreteria Tecnica Accordi Sottoiscritti in poi e avviati per la prima volta
            if (role.Equals(RoleAndKeysClaimEnum.KEY_CLAIM_SEGRETERIA_TECNICA.ToDescriptionString()))
            {
                St_Esaminati = _searchResults.Where(s => 
                                                        (s.Stato == StatoAccordo.Sottoscritto || s.Stato == StatoAccordo.InCorso || s.Stato == StatoAccordo.AccordoConProposteModifica || s.Stato == StatoAccordo.AccordoConcluso || s.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente) 
                                                        && s.PrimoAccordo 
                                                        && s.VistoSegreteriaTecnica).Count();
                St_EsaminatiValidatisenzaAnnotazioni = _searchResults.Where(s => 
                                                        (s.Stato == StatoAccordo.Sottoscritto || s.Stato == StatoAccordo.InCorso || s.Stato == StatoAccordo.AccordoConProposteModifica || s.Stato == StatoAccordo.AccordoConcluso || s.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente) 
                                                        && s.PrimoAccordo 
                                                        && s.VistoSegreteriaTecnica 
                                                        && s.NoteSegreteriaTecnica == null).Count();
                St_EsaminatiValidaticonAnnotazioni = _searchResults.Where(s =>
                                                                            (s.Stato == StatoAccordo.Sottoscritto || s.Stato == StatoAccordo.InCorso || s.Stato == StatoAccordo.AccordoConProposteModifica || s.Stato == StatoAccordo.AccordoConcluso || s.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente)
                                                                            && s.PrimoAccordo && s.VistoSegreteriaTecnica && s.NoteSegreteriaTecnica != null).Count();
                St_InAttesa = _searchResults.Where(s =>
                                                    (s.Stato == StatoAccordo.Sottoscritto || s.Stato == StatoAccordo.InCorso || s.Stato == StatoAccordo.AccordoConProposteModifica || s.Stato == StatoAccordo.AccordoConcluso || s.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente)
                                                    && s.PrimoAccordo && !s.VistoSegreteriaTecnica).Count();
                St_conAnnotazioniReferentiInterni = _searchResults.Where(s =>
                                                                            (s.Stato == StatoAccordo.Sottoscritto || s.Stato == StatoAccordo.InCorso || s.Stato == StatoAccordo.AccordoConProposteModifica || s.Stato == StatoAccordo.AccordoConcluso || s.Stato == StatoAccordo.AccordoConclusoCambioStrutturaDipendente)
                                                                            && s.PrimoAccordo && s.NoteRefereteInterno != null).Count();

                //Quando il nuovo requisito sulle richieste di modifica di un accordo sarà implementato, vanno ricalcolati:
                //- accordi che hanno una richiesta di modifica "Approvata"
                //- accordo che hanno un richiesta di modifica "RichiestaEApprovataDaDirigenteResponsabile"

                
            }

        }

        /// <summary>
        /// Numero di elementi totali restituiti dalla ricerca.
        /// </summary>
        public int Acc_sottoscritti { get; set; }
        public int Acc_PrimoAccordo { get; set; }
        public int Acc_TotGiornateLavoro { get; set; }
        public Decimal Acc_MediaGiornateLavoro { get; set; }
       
        public int Acc_NoValidateResponsabileAccordo { get; set; }
        public int Acc_NonValidateCapoIntermedio { get; set; }
        public int Acc_NoValidateCapoStruttura { get; set; }
        public int Acc_Attesa { get; set; }
        public int Acc_AttesaResponsabileAccordo { get; set; }
        public int Acc_AttesaCapoIntermedio { get; set; }
        public int Acc_AttesaCapoStruttura { get; set; }
        public int Acc_AttesaRichiestaIntegrazione { get; set; }
        public int Acc_AttesaSottoscrizione { get; set; }
        public int Acc_TotGiornateLavoroAttesa { get; set; }
        public Decimal Acc_MediaGiornateLavoroAttesa { get; set; }

        public int St_Esaminati { get; set; }
        public int St_EsaminatiValidatisenzaAnnotazioni { get; set; }
        public int St_EsaminatiValidaticonAnnotazioni { get; set; }
        public int St_InAttesa { get; set; }
        public int St_conAnnotazioniReferentiInterni { get; set; }
        public int St_ModificatiDipendente { get; set; }
        public int St_ModificatiDirigente { get; set; }
        public int St_ModificatiDirigenteMotivate { get; set; }

        public string role { get; set; }
        public int totalElements { get; set; }
    }
}
