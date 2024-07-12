using System.Threading;
using System.Threading.Tasks;
using Domain.Model;
using Elsa.Scripting.Liquid.Messages;
using Fluid;
using MediatR;
using static Domain.Model.ApprovatoreAccordiInStato;

namespace Infrastructure.Workflow.Scripting.Liquid
{
    /// <summary>
    /// Registrazione degli oggetti .NET da rendere disponibili per il valutatore Liquid
    /// </summary>
    public class ConfigureLiquidEngine : INotificationHandler<EvaluatingLiquidExpression>
    {
        public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
        {
            var memberAccessStrategy = notification.TemplateContext.Options.MemberAccessStrategy;

            memberAccessStrategy.Register<Accordo>();
            memberAccessStrategy.Register<ApprovatoreAccordiInStato>();
            memberAccessStrategy.Register<ApprovatoreAccordiInStatoAccordo>();

            return Task.CompletedTask;
        }
    }
}
