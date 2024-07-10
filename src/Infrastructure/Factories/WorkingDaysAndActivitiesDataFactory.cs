using Domain.Model;
using Domain.Model.ExternalCommunications;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Infrastructure.Services.Factories
{
    /// <summary>
    /// Factory per la generazione delle strutture dati necessarie per
    /// l'invio di giornate lavorative e attività a sistemi esterni.
    /// </summary>
    public class WorkingDaysAndActivitiesDataFactory
    {
        /// <summary>
        /// Genera la struttura dati per l'invio delle informazioni sulle giornate lavorative.
        /// </summary>
        /// <param name="accordo">Accordo di riferimento.</param>
        /// <returns>Informazioni sulle giornate lavorative.</returns>
        /// <remarks>
        /// Se l'accordo per cui si stanno inviando le giornate è chiuso per revisione, il semplice
        /// reinvio di tutte le giornate, potrebbe provocare l'inserimento di giornate non desiderate
        /// perché l'elenco di date del vecchio accordo non vengono riadeguate a seguito della
        /// sottoscrizione del nuovo accordo, quindi si devono selezionare le sole date comprese
        /// nell'intervallo DataInizio - DataFine.
        /// </remarks>
        public WorkingDaysTransmission GetWorkingDaysTransmission(Accordo accordo)
        {
            return new WorkingDaysTransmission(accordo.Dipendente.Email)
            {
                Id = accordo.Id,
                WorkingDays = accordo.PianificazioneDateAccordo
                    // Splitta le date e rimuove spazi
                    .Split(',')
                    .Select(x => x.Trim())
                    // Converte a data
                    .Select(x => DateTime.ParseExact(x, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                    // Seleziona tutte le date comprese fra data inizio e data fine (estremi inclusi)
                    .Where(x => x >= accordo.DataInizioUtc && x <= accordo.DataFineUtc)
                    // Riporta a string
                    .Select(x => x.ToString("dd/MM/yyyy"))?.ToList() ?? new List<string>()
            };
        }

        /// <summary>
        /// Genera la struttura dati per l'invio delle informazioni sulle attività lavorative.
        /// </summary>
        /// <param name="accordo">Accordo di riferimento.</param>
        /// <returns>Informazioni sulle attività.</returns>
        public WorkingActivityTransmission GetWorkingActivitiesTransmission(Accordo accordo)
        {
            return new WorkingActivityTransmission
            {
                Id = accordo.Id,
                UserId = accordo.Dipendente.Email,
                Code = accordo.Codice.ToString(),
                Activities = accordo.ListaAttivita.Select(a =>
                    new WorkingActivityTransmission.ActivityDetails
                    {
                        Code = $"ATT-{a.Id}",
                        Description = a.Attivita
                    }).ToList(),
                StartDate = accordo.DataInizioUtc.ToString("dd/MM/yyyy"),
                EndDate = accordo.DataFineUtc.ToString("dd/MM/yyyy"),
                WorkingDaysCount = accordo.PianificazioneDateAccordo?.Split(',').Count() ?? 0
            };

        }
    }
}
