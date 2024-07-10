using Domain.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.VisibilityHandlers.AccordoHandlers
{
    public class AccordoKEY_CLAIM_REFERENTE_INTERNOVisibilityHandler : IVisibilityHandler<Accordo, Guid>
    {
        /// <summary>
        /// Applica il filtro sul referente interno.
        /// </summary>
        /// <param name="collection">Collezione degli accordi da filtrare.</param>
        /// <param name="uid">Identificativo del referente interno.</param>
        /// <returns>Queryable con la clausola di filtro per il referente interno.</returns>
        /// <remarks>La visibiltà degli accordi è soggetta a: 
        /// 1. Gli accordi legati alla struttura di cui sono responsabile / capo intermedio / capo struttura / referente interno
        /// 2. Gli accordi per cui sono responsabile / capo intermedio / capo struttura / referente interno
        /// Questo tipo di condizioni sono necessarie per gestire i casi:
        /// 1. Cambio di struttura da parte di uno dei responsabili
        /// 2. Gestione del caso particolare in cui il dirigente risulata responsabile del suo stesso accordo
        /// (Dirigente che fa l'accordo per se stesso, stessa struttura)</remarks>
        public IQueryable<Accordo> Filter(IQueryable<Accordo> collection, Guid uid, IStrutturaService strutturaService)
        {
            var listaStrutture = strutturaService.FindStrutturaAsync(s => s.ReferenteInterno.Id.Equals(uid)).GetAwaiter().GetResult();

            var predicate = PredicateBuilder.New<Accordo>();
            listaStrutture.Entities.ForEach(struttura =>
            {
                predicate = predicate.Or(p => p.UidStrutturaUfficioServizio == struttura.Id.ToString());
            });

            predicate = predicate.Or(a => a.ReferenteInterno.Id.Equals(uid));

            return collection.Where(predicate);
        }
    }
}
