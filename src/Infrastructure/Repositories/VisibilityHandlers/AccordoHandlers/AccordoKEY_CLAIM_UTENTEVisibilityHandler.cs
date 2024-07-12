using Domain.Model;
using System;
using System.Linq;

namespace Infrastructure.Repositories.VisibilityHandlers.AccordoHandlers
{
    /// <summary>
    /// Gestisce i filtri di visilità degli accordi per l'utente normale.
    /// L'utente deve poter visualizzare unicamente i suoi accordi.
    /// </summary>
    public class AccordoKEY_CLAIM_UTENTEVisibilityHandler : IVisibilityHandler<Accordo, Guid>
    {
        /// <summary>
        /// Applica il filtro sull'utente.
        /// </summary>
        /// <param name="collection">Collezione degli accordi da filtrare.</param>
        /// <param name="uid">Identificativo dell'utente.</param>
        /// <returns>Queryable con la clausola di filtro per l'utente normale.</returns>
        public IQueryable<Accordo> Filter(IQueryable<Accordo> collection, Guid uid, IStrutturaService strutturaService)
        {
            return collection.Where(a => a.Dipendente.Id.Equals(uid));
        }
    }
}
