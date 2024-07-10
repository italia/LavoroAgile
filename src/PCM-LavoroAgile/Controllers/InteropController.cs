using Domain.Model;
using Domain.Model.ExternalCommunications;
using Domain.Model.MinisteroLavoro.Response;
using Domain.Settings;
using DotNetCore.CAP;
using Infrastructure.Services.Factories;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    /// <summary>
    /// Controller per la gestione delle chiamate di 
    /// interoperabilità.
    /// </summary>
    public class InteropController : Controller
    {
        /// <summary>
        /// Riferimento al servizio di invio dati verso Zucchetti.
        /// </summary>
        private readonly ISendWorkingDaysAndActivities _zucchettiWDAndASender;

        /// <summary>
        /// Riferimento al servizio di logging su database dello stato.
        /// </summary>
        private readonly ISendWDAndAStatusLogger _sendWDAndAStatusLogger;

        /// <summary>
        /// Riferimento al servizio per la gestione degli accordi.
        /// </summary>
        private readonly IAccordoService _accordoService;

        /// <summary>
        /// Riferimento al bus CAP.
        /// </summary>
        private readonly ICapPublisher _capBus;

        /// <summary>
        /// Riferimento alla factory di supporto alla creazione dei pacchetti dati da inviare.
        /// </summary>
        private readonly WorkingDaysAndActivitiesDataFactory _workingDaysAndActivitiesDataFactory;

        private readonly ILogger<InteropController> _logger;

        /// <summary>
        /// Riferimento al servizio di eliminazione dati verso Zucchetti.
        /// </summary>
        private readonly IDeleteWorkingDaysAndRestrictActivities _zucchettiWDAndADeleter;

        /// <summary>
        /// Riferimento al servizio per la gestione email
        /// </summary>
        private readonly IMailService _mailService;

        /// <summary>
        /// Riferimento al servizio per la gestione delle chiamate al Ministero del Lavoro
        /// </summary>
        private readonly IMinisteroLavoroService _ministeroLavoroService;

        /// <summary>
        /// Riferimento al servizio di logging su database dello stato Ministero del Lavoro.
        /// </summary>
        private readonly IMinisteroLavoroStatusLogger _ministeroLavoroStatusLogger;

        private readonly MinisteroLavoroServicesSettings _ministeroLavoroServicesSettings;

        /// <summary>
        /// Inizializza un nuovo <see cref="InteropController"/>.
        /// </summary>
        /// <param name="zucchettiWDAndASender">Riferimento al servizio di invio dati.</param>
        /// <param name="sendWDAndAStatusLogger">Riferimento al servizio di logging stato invio.</param>
        /// <param name="capBus">Riferimento al servizio di messaggistica.</param>
        /// <param name="workingDaysAndActivitiesDataFactory">Riferimento alla factory per la preparazione delle strutture dati per invio gionrate e attività</param>
        /// <param name="accordoService">Riferimento al servizio di gestione accordi.</param>
        public InteropController(ISendWorkingDaysAndActivities zucchettiWDAndASender, 
                                ICapPublisher capBus, 
                                WorkingDaysAndActivitiesDataFactory workingDaysAndActivitiesDataFactory, 
                                IAccordoService accordoService, 
                                ISendWDAndAStatusLogger sendWDAndAStatusLogger, 
                                ILogger<InteropController> logger,
                                IDeleteWorkingDaysAndRestrictActivities zucchettiWDAndADeleter,
                                IMailService mailService, 
                                IMinisteroLavoroService ministeroLavoroService,
                                IMinisteroLavoroStatusLogger ministeroLavoroStatusLogger,
                                IOptions<MinisteroLavoroServicesSettings> ministeroLavoroServicesSettings)
            => (_zucchettiWDAndASender, 
            _sendWDAndAStatusLogger, 
            _capBus, 
            _workingDaysAndActivitiesDataFactory, 
            _accordoService, 
            _logger,
            _zucchettiWDAndADeleter,
            _mailService,
            _ministeroLavoroService,
            _ministeroLavoroStatusLogger,
            _ministeroLavoroServicesSettings) = 
            (zucchettiWDAndASender ?? throw new ArgumentNullException(nameof(zucchettiWDAndASender)), 
            sendWDAndAStatusLogger ?? throw new ArgumentNullException(nameof(sendWDAndAStatusLogger)),
            capBus ?? throw new ArgumentNullException(nameof(capBus)), 
            workingDaysAndActivitiesDataFactory ?? throw new ArgumentNullException(nameof(workingDaysAndActivitiesDataFactory)), 
            accordoService, 
            logger,
            zucchettiWDAndADeleter,
            mailService,
            ministeroLavoroService,
            ministeroLavoroStatusLogger,
            ministeroLavoroServicesSettings?.Value);

        #region Endpoint CAP

        [NonAction]
        [CapSubscribe(CAPChannels.SendActivities)]
        public async Task SendActivity(WorkingActivityTransmission workingActivityTransmission)
        {
            try
            {
                await _zucchettiWDAndASender.SendActivities(workingActivityTransmission);
                await _sendWDAndAStatusLogger.SetActivitiesStatus(workingActivityTransmission.Id, true, String.Empty, CancellationToken.None);

            }
            catch (Exception ex)
            {
                await _sendWDAndAStatusLogger.SetActivitiesStatus(workingActivityTransmission.Id, false, ex.Message, CancellationToken.None);
                throw;
            }
        }

        [NonAction]
        [CapSubscribe(CAPChannels.SendDays)]
        public async Task SendWorkingDays(WorkingDaysTransmission workingDays)
        {
            try
            {
                await _zucchettiWDAndASender.SendWorkingDays(workingDays);
                await _sendWDAndAStatusLogger.SetWorkingDaysStatus(workingDays.Id, true, String.Empty, CancellationToken.None);

            }
            catch (Exception ex)
            {
                await _sendWDAndAStatusLogger.SetWorkingDaysStatus(workingDays.Id, false, ex.Message, CancellationToken.None);
                throw;
            }
        }

        #endregion

        #region Endpoint resend

        /// <summary>
        /// Reinvia i dati sulle giornate lavorative.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo di riferimento.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task ReSendWorkingDays(Guid id, CancellationToken cancellationToken)
        {
            // Recupera l'accordo di riferimento.
            var accordo = await _accordoService.GetAccordoAsync(id, cancellationToken);
            if (accordo != null)
            {
                var wdt = _workingDaysAndActivitiesDataFactory.GetWorkingDaysTransmission(accordo);
                await _capBus.PublishAsync(CAPChannels.SendDays, wdt);
            }
        }

        /// <summary>
        /// Reinvia i dati sulle giornate lavorative.
        /// </summary>
        /// <param name="id">Identificativo dell'accordo di riferimento.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task ReSendWorkingActivities(Guid id, CancellationToken cancellationToken)
        {
            // Recupera l'accordo di riferimento.
            var accordo = await _accordoService.GetAccordoAsync(id, cancellationToken);
            if (accordo != null)
            {
                var wat = _workingDaysAndActivitiesDataFactory.GetWorkingActivitiesTransmission(accordo);
                await _capBus.PublishAsync(CAPChannels.SendActivities, wat);
            }
        }

        #endregion

        #region Chiusura Accordo
        /// <summary>
        /// Chiude l'accordo precedente non più in corso
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [NonAction]
        [CapSubscribe(CAPChannels.ChiusuraAccordoPrecedente)]
        public async Task ChiusuraAccordoPrecedente(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _accordoService.ChiusuraAccordoPrecedente(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante la chiusura dell'accordo id:" + id, ex);
                throw new Exception("Errore durante la chiusura dell'accordo id:" + id, ex);
            }
        }
        #endregion

        [NonAction]
        [CapSubscribe(CAPChannels.DeleteDays)]
        public async Task DeleteWorkingDays(WorkingDaysTransmission workingDays, CancellationToken cancellationToken)
        {
            try
            {
                await _zucchettiWDAndADeleter.DeleteWorkingDays(workingDays);
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante l'eliminazione delle giornate dell'accordo id:" + workingDays.Id, ex);
                throw new Exception("Errore durante l'eliminazione delle giornate dell'accordo id:" + workingDays.Id, ex);
            }
        }

        [NonAction]
        [CapSubscribe(CAPChannels.DeleteDaysRemediation)]
        public async Task DeleteDaysRemediation(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _zucchettiWDAndADeleter.CalculateAndDeleteWorkingDaysAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante l'eliminazione di emergenza delle giornate dell'accordo id:" + id, ex);
                throw;
            }
        }

        [NonAction]
        [CapSubscribe(CAPChannels.SendEmail)]
        public async Task SendEmail(Email email)
        {
            try
            {
                await _mailService.SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore durante l'invio dell'email", ex);
                throw new Exception("Errore durante l'invio dell'email", ex);
            }
        }

        [NonAction]
        [CapSubscribe(CAPChannels.NuovaComunicazioneMinisteroLavoro)]
        public async Task NuovaComunicazioneMinisteroLavoro(Guid idAccordo, CancellationToken cancellationToken)
        {
            try
            {
                //Verifica la validità della configurazione
                if (!_ministeroLavoroServicesSettings.Valid)
                    return;

                ResponseRestEsito responseRestEsito = await _ministeroLavoroService.CreaComunicazione(idAccordo, cancellationToken);

                if (responseRestEsito != null && responseRestEsito.Esito != null)
                {
                    if (responseRestEsito.Esito.Count == 1 && responseRestEsito.Esito[0].codice.Equals("E100"))
                    {
                        await _ministeroLavoroStatusLogger.SetNuovaComunicazioneStatus(idAccordo, true, null, cancellationToken);
                    }
                    else
                    {
                        string errore = String.Empty;
                        foreach (Esito e in responseRestEsito.Esito)
                        {
                            errore += "Accordo id: " + idAccordo + " CodiceEsito: " + responseRestEsito.Esito[0].codice + " Esito: " + responseRestEsito.Esito[0].messaggio;
                        }
                        await _ministeroLavoroStatusLogger.SetNuovaComunicazioneStatus(idAccordo, false, errore, cancellationToken);
                    }
                }
                else
                {
                    await _ministeroLavoroStatusLogger.SetNuovaComunicazioneStatus(idAccordo, false, "Esito nullo chiamata CreaComunicazione per accordo id: " + idAccordo, cancellationToken);                    
                }
            }
            catch (Exception ex)
            {
                await _ministeroLavoroStatusLogger.SetNuovaComunicazioneStatus(idAccordo, false, "Errore durante l'invio di una nuova comunicazione al Ministero del Lavoro per accordo id: " + idAccordo, cancellationToken);
                _logger.LogError("Errore durante l'invio di una nuova comunicazione al Ministero del Lavoro", ex);
                throw new Exception("Errore durante l'invio di una nuova comunicazione al Ministero del Lavoro", ex);
            }
        }

        [NonAction]
        [CapSubscribe(CAPChannels.RecessoComunicazioneMinisteroLavoro)]
        public async Task RecessoComunicazioneMinisteroLavoro(Guid idAccordo, CancellationToken cancellationToken)
        {
            try
            {
                //Verifica la validità della configurazione
                if (!_ministeroLavoroServicesSettings.Valid)
                    return;

                Accordo accordo = await _accordoService.GetAccordoAsync(idAccordo);

                ResponseRestEsito responseRestEsito = await _ministeroLavoroService.RecediComunicazione(idAccordo, accordo.DataFineUtc, cancellationToken);

                if (responseRestEsito != null && responseRestEsito.Esito != null)
                {
                    if (responseRestEsito.Esito.Count == 1 && responseRestEsito.Esito[0].codice.Equals("E100"))
                    {
                        await _ministeroLavoroStatusLogger.SetRecessoComunicazioneStatus(idAccordo, true, null, cancellationToken);
                    }
                    else
                    {
                        string errore = String.Empty;
                        foreach (Esito e in responseRestEsito.Esito)
                        {
                            errore += "Accordo id: " + idAccordo + " CodiceEsito: " + responseRestEsito.Esito[0].codice + " Esito: " + responseRestEsito.Esito[0].messaggio;
                        }
                        await _ministeroLavoroStatusLogger.SetRecessoComunicazioneStatus(idAccordo, false, errore, cancellationToken);
                    }
                }
                else
                {
                    await _ministeroLavoroStatusLogger.SetRecessoComunicazioneStatus(idAccordo, false, "Esito nullo chiamata RecessoComunicazione per accordo id: " + idAccordo, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                await _ministeroLavoroStatusLogger.SetRecessoComunicazioneStatus(idAccordo, false, "Errore durante l'invio di un recesso comunicazione al Ministero del Lavoro per accordo id: " + idAccordo, cancellationToken);
                _logger.LogError("Errore durante l'invio di un recesso comunicazione al Ministero del Lavoro", ex);
                throw new Exception("Errore durante l'invio di un recesso comunicazione al Ministero del Lavoro", ex);
            }
        }
    }
}
