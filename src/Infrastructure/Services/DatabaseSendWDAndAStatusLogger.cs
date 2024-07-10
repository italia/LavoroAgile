using Domain.Model;
using Domain.Model.ExternalCommunications;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    /// <summary>
    /// Implementa il salvataggio dello stato di invio su database.
    /// </summary>
    public class DatabaseSendWDAndAStatusLogger : ISendWDAndAStatusLogger
    {
        /// <summary>
        /// Riferimento al repository per la gestione delle trasmissioni.
        /// </summary>
        private readonly ISimpleRepository<TransmissionStatus> _simpleRepository;

        public DatabaseSendWDAndAStatusLogger(ISimpleRepository<TransmissionStatus> simpleRepository)
            => _simpleRepository = simpleRepository ?? throw new ArgumentNullException(nameof(simpleRepository));

        public async Task SetActivitiesStatus(Guid accordoId, bool sentSuccessfully, string sendError, CancellationToken cancellationToken)
        {

            await _simpleRepository.Upsert(new TransmissionStatus(accordoId, workingActivitiesSentSuccessfully: sentSuccessfully, workingActivitiesSendError: sendError, lastWorkingActivitiesSentDate: DateTime.UtcNow),
                (dbT, newT) => new TransmissionStatus(accordoId)
                {
                    WorkingActivitiesSentSuccessfully = newT.WorkingActivitiesSentSuccessfully,
                    WorkingActivitiesSendError = newT.WorkingActivitiesSendError,
                    LastWorkingActivitiesSentDate = newT.LastWorkingActivitiesSentDate
                },
                cancellationToken);

        }

        public async Task SetWorkingDaysStatus(Guid accordoId, bool sentSuccessfully, string sendError, CancellationToken cancellationToken)
        {
            await _simpleRepository.Upsert(new TransmissionStatus(accordoId, sentSuccessfully, sendError, DateTime.UtcNow),
                (dbT, newT) => new TransmissionStatus(accordoId)
                {
                    WorkingDaysSentSuccessfully = newT.WorkingDaysSentSuccessfully,
                    WorkingDaysSendError = newT.WorkingDaysSendError,
                    LastWorkingDaysSentDate = newT.LastWorkingDaysSentDate
                },
                cancellationToken);
        }
    }
}
