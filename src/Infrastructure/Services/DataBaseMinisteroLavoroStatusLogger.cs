using Domain.Model;
using Domain.Model.ExternalCommunications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class DataBaseMinisteroLavoroStatusLogger : IMinisteroLavoroStatusLogger
    {
        /// <summary>
        /// Riferimento al repository per la gestione delle trasmissioni.
        /// </summary>
        private readonly ISimpleRepository<TransmissionStatus> _simpleRepository;

        public DataBaseMinisteroLavoroStatusLogger(ISimpleRepository<TransmissionStatus> simpleRepository)
           => _simpleRepository = simpleRepository ?? throw new ArgumentNullException(nameof(simpleRepository));

        public async Task SetNuovaComunicazioneStatus(Guid accordoId, bool sentSuccessfully, string sendError, CancellationToken cancellationToken)
        {
            await _simpleRepository.Upsert(new TransmissionStatus(accordoId, nuovaComunicazioneMinisteroLavoroSentSuccessfully: sentSuccessfully, nuovaComunicazioneMinisteroLavoroSendError: sendError, nuovaComunicazioneMinisteroLavoroLastSentDate: DateTime.UtcNow),
                (dbT, newT) => new TransmissionStatus(accordoId)
                {
                    NuovaComunicazioneMinisteroLavoroSentSuccessfully = newT.NuovaComunicazioneMinisteroLavoroSentSuccessfully,
                    NuovaComunicazioneMinisteroLavoroSendError = newT.NuovaComunicazioneMinisteroLavoroSendError,
                    NuovaComunicazioneMinisteroLavoroLastSentDate = newT.NuovaComunicazioneMinisteroLavoroLastSentDate
                },
                cancellationToken);

        }

        public async Task SetRecessoComunicazioneStatus(Guid accordoId, bool sentSuccessfully, string sendError, CancellationToken cancellationToken)
        {
            await _simpleRepository.Upsert(new TransmissionStatus(accordoId, recessoComunicazioneMinisteroLavoroSentSuccessfully: sentSuccessfully, recessoComunicazioneMinisteroLavoroSendError: sendError, recessoComunicazioneMinisteroLavoroLastSentDate: DateTime.UtcNow),
                (dbT, newT) => new TransmissionStatus(accordoId)
                {
                    RecessoComunicazioneMinisteroLavoroSentSuccessfully = newT.RecessoComunicazioneMinisteroLavoroSentSuccessfully,
                    RecessoComunicazioneMinisteroLavoroSendError = newT.RecessoComunicazioneMinisteroLavoroSendError,
                    RecessoComunicazioneMinisteroLavoroLastSentDate = newT.RecessoComunicazioneMinisteroLavoroLastSentDate
                },
                cancellationToken);
        }
    }
}
