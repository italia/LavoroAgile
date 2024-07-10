using Domain.Model;
using LinqKit;
using System;
using System.Linq;

namespace Infrastructure.Repositories.VisibilityHandlers.AccordoHandlers
{
    /// <summary>
    /// Gestisce i filtri di visilità degli accordi per il capo struttura.
    /// L'utente deve poter visualizzare unicamente gli accordi di cui è responsabile.
    /// </summary>
    public class AccordoKEY_CLAIM_CAPO_STRUTTURAVisibilityHandler : IVisibilityHandler<Accordo, Guid>
    {
        /// <summary>
        /// Applica il filtro sull'utente.
        /// </summary>
        /// <param name="collection">Collezione degli accordi da filtrare.</param>
        /// <param name="uid">Identificativo dell'utente.</param>
        /// <returns>Queryable con la clausola di filtro per il capo struttura.</returns>
        /// <remarks>La visibiltà degli accordi è soggetta a: 
        /// 1. Gli accordi legati alla struttura di cui sono responsabile / capo intermedio / capo struttura / referente interno
        /// 2. Gli accordi per cui sono responsabile / capo intermedio / capo struttura / referente interno
        /// Questo tipo di condizioni sono necessarie per gestire i casi:
        /// 1. Cambio di struttura da parte di uno dei responsabili
        /// 2. Gestione del caso particolare in cui il dirigente risulata responsabile del suo stesso accordo
        /// (Dirigente che fa l'accordo per se stesso, stessa struttura)</remarks>
        public IQueryable<Accordo> Filter(IQueryable<Accordo> collection, Guid uid, IStrutturaService strutturaService)
        {
            var listaStrutture = strutturaService.FindStrutturaAsync(s => s.CapoStruttura.Id.Equals(uid)).GetAwaiter().GetResult();

            var predicate = PredicateBuilder.New<Accordo>();
            listaStrutture.Entities.ForEach(struttura =>
            {
                predicate = predicate.Or(p => p.UidStrutturaUfficioServizio == struttura.Id.ToString());
            });

            predicate = predicate.Or(a => a.CapoStruttura.Id.Equals(uid));

            predicate = predicate.And(a => a.Stato != StatoAccordo.Bozza);

            return collection.Where(predicate);
        }
    }
}
