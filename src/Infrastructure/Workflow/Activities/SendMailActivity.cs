using Elsa.Activities.Email;
using Elsa.Activities.Email.Options;
using Elsa.Activities.Email.Services;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Serialization;
using Elsa.Services.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Workflow.Activities
{
    /// <summary>
    /// Estende l'activity send mail affinché non faccia fallire il workflow 
    /// nel caso in cui l'invio della mail non vada a buon fine.
    /// </summary>
    [Action(Category = "Lavoro agile", Description = "Invia una email senza far fallire il workflow nel caso in cui l'invio dovesse fallire", DisplayName = "Email")]
    public class SendMailActivity : SendEmail
    {
        public SendMailActivity(ISmtpService smtpService, IOptions<SmtpOptions> options, IHttpClientFactory httpClientFactory, IContentSerializer contentSerializer)
            : base(smtpService, options, httpClientFactory, contentSerializer)
        {
            
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            try
            {
                await base.OnExecuteAsync(context);
            }
            catch (Exception ex)
            {
                context.JournalData.Add("Errore", ex);
            }

            return Done();

        }

    }

}
