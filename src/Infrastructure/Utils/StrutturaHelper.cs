using Domain.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Utils
{
    /// <summary>
    /// Funzioni di utilità per le strutture
    /// </summary>
    public static class StrutturaHelper
    {
        /// <summary>
        /// Verifica l'esistenza di una struttura basandosi sui nomi dei suoi livelli e quindi
        /// la crea se non esiste, o la aggiorna nel caso in cui siano cambiate le informazioni
        /// sui dirigenti.
        /// </summary>
        /// <param name="struttura">Struttura da lavorare</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Identificativo della struttura individuata/create/aggiornata.</returns>
        public static async Task<Guid> CheckCreateOrUpdate(this Struttura struttura, IStrutturaRepository<Struttura, Guid> strutturaRepository, CancellationToken cancellationToken = default)
        {
            // Ricerca la struttura
            var s = await strutturaRepository.FindAsync(
                s => s.StrutturaLiv1.Equals(struttura.StrutturaLiv1)
                && s.StrutturaLiv2.Equals(struttura.StrutturaLiv2)
                && s.StrutturaLiv3.Equals(struttura.StrutturaLiv3), cancellationToken: cancellationToken);

            // Se la struttura non esiste, la censisce e ne recupera l'id
            if (s.Entities.Count == 0)
            {
                await strutturaRepository.InsertAsync(struttura, cancellationToken);
                s = await strutturaRepository.FindAsync(s => s.StrutturaLiv1.Equals(struttura.StrutturaLiv1)
                    && s.StrutturaLiv2.Equals(struttura.StrutturaLiv2)
                    && s.StrutturaLiv3.Equals(struttura.StrutturaLiv3), cancellationToken: cancellationToken);
            }

            // Se la struttura è diversa da quella storicizzata, aggiorna le informazioni sui
            // responsabili.
            if (s.Entities.First() != struttura)
            {
                struttura.Id = s.Entities.First().Id;
                await strutturaRepository.UpdateAsync(struttura, cancellationToken);

            }

            // Restituisce l'identificativo della struttura.
            return s.Entities.First().Id;
        }
    }
}
