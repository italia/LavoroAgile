using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.VisibilityHandlers.AccordoHandlers
{
    public class AccordoKEY_CLAIM_SEGRETERIA_TECNICAVisibilityHandler : IVisibilityHandler<Accordo, Guid>
    {
        /// <summary>
        /// Applica il filtro sulla Segreteria Tecnica.
        /// </summary>
        /// <param name="collection">Collezione degli accordi da filtrare.</param>
        /// <param name="uid">Identificativo dell'utnete appartenente alla Segreteria Tecnica.</param>
        /// <returns>Queryable con la clausola di filtro per la Segreteria Tecnica.</returns>
        public IQueryable<Accordo> Filter(IQueryable<Accordo> collection, Guid uid, IStrutturaService strutturaService)
        {
            //I requisiti attuali prevedono che qualsiasi utente della segreteria tecnica può vedere tutti gli accordi
            return collection;
        }
    }
}
