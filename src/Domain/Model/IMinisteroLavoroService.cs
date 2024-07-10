using Domain.Model.MinisteroLavoro.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IMinisteroLavoroService
    {
        Task<List<Accordo>> GetAccordiDaInizializzare(CancellationToken cancellationToken);

        Task<ResponseRestEsito> CreaComunicazione(Guid idAccordo, CancellationToken cancellationToken);

        Task<ResponseRestEsito> RecediComunicazione(Guid idAccordo, DateTime dataFinePeriodo, CancellationToken cancellationToken);

        Task<ResponseRestEsitoRicerca> RicercaComunicazione(string codiceFiscaleUtente, CancellationToken cancellationToken);

        Task<ResponseRestEsitoDettaglio> DettaglioComunicazione(string codiceComunicazione, CancellationToken cancellationToken);

        Task<ResponseRestEsito> AnnullaComunicazione(Guid idAccordo, CancellationToken cancellationToken);

        Task ReinvioComunicazioneMinisteroLavoro(Guid idAccordo, CancellationToken cancellationToken);
    }
}
