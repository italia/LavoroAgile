using Domain.Model;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Domain.Model.ApprovatoreAccordiInStato;

namespace Infrastructure.Workflow.Activities
{
    /// <summary>
    /// Activity per il recupero degli accordi in uno specifico stato per
    /// uno specifico approvatore.
    /// </summary>
    [Action(Category = "Lavoro agile", Description = "Recupera gli identificativi degli accordi in uno specifico stato per una specifica figura approvatrice.", DisplayName = "Recupera accordi in stato approvativo.")]
    public class GetAccordiInStatoApprovativoActivity : Activity
    {
        /// <summary>
        /// Repository per l'accesso agli accordi.
        /// </summary>
        private readonly IRepository<Accordo, Guid> _repository;

        /// <summary>
        /// Inizializza ua nuova <see cref="GetAccordiInStatoApprovativoActivity"/>.
        /// </summary>
        /// <param name="repository">Repository per l'accesso agli accordi.</param>
        public GetAccordiInStatoApprovativoActivity(IRepository<Accordo, Guid> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Stato da recuperare.
        /// </summary>
        [ActivityInput(
            Label = "Stato",
            Hint = "Stato da recuperare.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid },
            Options = new[] {
                StatoAccordo.DaApprovareRA,
                StatoAccordo.DaApprovareCI,
                StatoAccordo.DaApprovareCS,
                StatoAccordo.DaSottoscrivereRA
            }
        )]
        public StatoAccordo Stato { get; set; } = StatoAccordo.DaApprovareRA;

        /// <summary>
        /// Dettaglio dell'associazione approvatore accordi.
        /// </summary>
        [ActivityOutput(
            Hint = "Dettaglio dell'accordo.")]
        public List<ApprovatoreAccordiInStato> Output { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            // Tira fuori accordi nello stato specifico.
            var accordiInStato = await this._repository.FindAsync(Guid.Empty, a => a.Stato == Stato, role: Domain.Model.Utilities.RoleAndKeysClaimEnum.KEY_CLAIM_SEGRETERIA_TECNICA);

            Output = new List<ApprovatoreAccordiInStato>();

            // In base allo stato richiesto, recupera dati per il responsabile corretto.
            switch (Stato)
            {
                case StatoAccordo.DaApprovareRA:
                case StatoAccordo.DaSottoscrivereRA:
                    this.Output.AddRange(accordiInStato.Entities.GroupBy(a => a.ResponsabileAccordo.Email)
                        .Select(l => new ApprovatoreAccordiInStato
                        {
                            ApproverEmail = l.Key,
                            ApproverName = l.FirstOrDefault()?.ResponsabileAccordo.NomeCognome,
                            Accordi = l.ToList().Select(a => new ApprovatoreAccordiInStatoAccordo { Id = a.Id, User = a.Dipendente.NomeCognome }).ToList()
                        }));
                    break;
                case StatoAccordo.DaApprovareCI:
                    this.Output.AddRange(accordiInStato.Entities.GroupBy(a => a.CapoIntermedio.Email)
                        .Select(l => new ApprovatoreAccordiInStato
                        {
                            ApproverEmail = l.Key,
                            ApproverName = l.FirstOrDefault()?.CapoIntermedio.NomeCognome,
                            Accordi = l.ToList().Select(a => new ApprovatoreAccordiInStatoAccordo { Id = a.Id, User = a.Dipendente.NomeCognome }).ToList()
                        }));
                    break;
                case StatoAccordo.DaApprovareCS:
                    this.Output.AddRange(accordiInStato.Entities.GroupBy(a => a.CapoStruttura.Email)
                        .Select(l => new ApprovatoreAccordiInStato
                        {
                            ApproverEmail = l.Key,
                            ApproverName = l.FirstOrDefault()?.CapoStruttura.NomeCognome,
                            Accordi = l.ToList().Select(a => new ApprovatoreAccordiInStatoAccordo { Id = a.Id, User = a.Dipendente.NomeCognome }).ToList()
                        }));
                    break;
                
            }

            context.JournalData.Add("Accordi individuati", this.Output);

            return Done();

        }

    }

}
