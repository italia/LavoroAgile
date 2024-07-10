using Domain.Model;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Workflow.Activities
{
    /// <summary>
    /// Definisce l'activity che consente di caricare il dettaglio di un accordo.
    /// </summary>
    [Action(Category = "Lavoro agile", Description = "Carica uno specifico accordo dalla base dati.", DisplayName = "Recupera dettaglio accordo")]
    public class GetAccordoActivity : Activity
    {
        /// <summary>
        /// Repository per l'accesso agli accordi.
        /// </summary>
        private readonly IRepository<Accordo, Guid> _repository;

        /// <summary>
        /// Inizializza ua nuova <see cref="GetAccordoActivity"/>.
        /// </summary>
        /// <param name="repository">Repository per l'accesso agli accordi.</param>
        public GetAccordoActivity(IRepository<Accordo, Guid> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Identificativo dell'accordo da caricare.
        /// </summary>
        [ActivityInput(
            Label = "ID Accordo",
            Hint = "ID dell'accordo da caricare.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public Guid IdAccordo { get; set; } = default!;

        /// <summary>
        /// Dettaglio dell'accordo.
        /// </summary>
        [ActivityOutput(
            Hint = "Dettaglio dell'accordo.")]
        public Accordo Output { get; set; } = default!;

        /// <summary>
        /// Carica il dettaglio di un accordo e lo imposta come output dell'activity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            Output = await _repository.GetAsync(IdAccordo, context.CancellationToken);

            return Done();
        }
    }
}
