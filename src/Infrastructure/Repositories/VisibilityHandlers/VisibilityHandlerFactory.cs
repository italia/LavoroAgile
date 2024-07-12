using Domain.Model;
using System;

namespace Infrastructure.Repositories.VisibilityHandlers
{
    /// <summary>
    /// Clsse factory responsabile della individuazione ed inizializzazione dell'handler di 
    /// gestione della visibilità.
    /// </summary>
    /// <typeparam name="T">Tipo dell'entità su cui applicare il filtro.</typeparam>
    public class VisibilityHandlerFactory<T, TKey> where T : Entity<TKey>
    {
        /// <summary>
        /// Recupera e restituisce l'handler identificato da <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">Identificativo dell'handler da istanziare.</param>
        /// <returns>Istanza dell'handler specificato</returns>
        /// <remarks>Per efficientare la ricerca dell'implementazione, il metodo effettua una ricerca 
        /// puntuale del tipo. Per far si che funzioni, l'handler viene ricercato nel namespace 
        /// Infrastructure.Repositories.VisibilityHandlers.NomeEntityHandlers e deve avere il nome
        /// EntityNameIdentifierVisibilityHandler.</remarks>
        public static IVisibilityHandler<T, TKey> GetVisibilityHandler(string identifier)
        {
            var typeName = $"{typeof(VisibilityHandlerFactory<T, TKey>).Namespace}.{typeof(T).Name}Handlers.{typeof(T).Name}{identifier}VisibilityHandler";
            return (IVisibilityHandler<T, TKey>)Activator.CreateInstance(Type.GetType(typeName, true));
        }
    }
}
