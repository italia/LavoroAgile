using Domain.Model;
using System;
using System.Linq;

namespace Infrastructure.Repositories.VisibilityHandlers
{
    /// <summary>
    /// Definisce l'interfaccia che deve implementare un filtro di visibilità.
    /// </summary>
    /// <typeparam name="T">Tipo dell'entità filtrata.</typeparam>
    public interface IVisibilityHandler<T, Tkey> where T : Entity<Tkey>
    {
        /// <summary>
        /// Filtra i dati di una collection di elementi di tipo <typeparamref name="T"/>.
        /// </summary>
        /// <param name="collection">Collezione di entità da filtrare.</param>
        /// <param name="uid">Identificativo dell'utente.</param>
        /// <returns>Queryable con l'espressione di filtro applicata.</returns>
        IQueryable<T> Filter(IQueryable<T> collection, Guid uid, IStrutturaService strutturaService);
    
    }
}
