using Domain.Model;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Workflow.Activities
{
    [Action(Category = "Lavoro agile", Description = "Passa in Recesso, gli accordi che hanno raggiunto la data di recesso anticipato.", DisplayName = "Verifica recesso accordo")]
    public class VerifyRecessoActivity : Activity
    {
        /// <summary>
        /// Riferimento al servizio per la gestione degli accordi.
        /// </summary>
        private readonly IAccordoService _accordoService;

        /// <summary>
        /// Nota da associare al cambiamento di stato.
        /// </summary>
        [ActivityInput(Hint = "Nota da associare al cambiamento di stato")]
        public string Nota { get; set; }

        /// <summary>
        /// Autore del passaggio dello stato a Recesso.
        /// </summary>
        /// <remarks>Di default è impostato a Sistema</remarks>
        [ActivityInput(Hint = "Autore del passaggio allo stato di recesso", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string Autore { get; set; } = "Sistema";

        [ActivityOutput(Hint = "Collezione degli accordi modificati")]
        public ICollection<Guid> Accordi { get; set; }

        /// <summary>
        /// Inizializza un nuovo <see cref="UpdateStatoAccordoActivity"/>
        /// </summary>
        /// <param name="accordoService">Riferimento al servizio di gestione degli accordi.</param>
        public VerifyRecessoActivity(IAccordoService accordoService)
        {
            this._accordoService = accordoService ?? throw new ArgumentNullException(nameof(accordoService));
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            // Aggiorna gli stati degli accordi
            Accordi = await _accordoService.UpdateAccordiToRecesso(Nota, Autore,  context.CancellationToken);

            // Logga gli identificativi degli accordi modificati
            context.JournalData.Add("Accordi aggiornati", Accordi);
            
            return Done();
        }

    }
}
