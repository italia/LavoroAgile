using Domain.Model.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.AccordiToDoHandlers
{
    public class AccordiToDoHandlerFactory
    {

        public static IAccordiToDoHandler GetAccordiToDoHandler(RoleAndKeysClaimEnum role)
        {
            var typeName = $"{typeof(AccordiToDoHandlerFactory).Namespace}.Accordo{role.ToString()}ToDoHandler";
            return (IAccordiToDoHandler)Activator.CreateInstance(Type.GetType(typeName, true));            
        }
    }
}
