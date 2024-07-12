using Domain.Model;
using Elsa.Scripting.JavaScript.Services;
using System;
using System.Collections.Generic;
using static Domain.Model.ApprovatoreAccordiInStato;

namespace Infrastructure.Workflow.Scripting.JavaScript
{
    /// <summary>
    /// Registrazione degli oggetti .NET da rendere disponibili nell'intellisense.
    /// </summary>
    public class CustomTypeDefinitionProvider : TypeDefinitionProvider
    {
        public override IEnumerable<Type> CollectTypes(TypeDefinitionContext context)
        {
            yield return typeof(Accordo);
            yield return typeof(ApprovatoreAccordiInStato);
            yield return typeof(ApprovatoreAccordiInStatoAccordo);
        }
    }
}
