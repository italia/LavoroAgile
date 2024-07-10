using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Servizio di gestione del personale di segreteria tecnica.
    /// </summary>
    public interface ISegreteriaTecnicaService
    {
        /// <summary>
        /// Crea un elemento nella segreteria tecnica.
        /// </summary>
        /// <param name="segreteriaTecnica">Informazioni sulla persona della segreteria tecnica.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Dettaglio della persona assegnata alla segreteria tecnica.</returns>
        Task<SegreteriaTecnica> CreateSegreteriaTecnicaAsync(SegreteriaTecnica segreteriaTecnica, CancellationToken cancellationToken = default);

    }
}
