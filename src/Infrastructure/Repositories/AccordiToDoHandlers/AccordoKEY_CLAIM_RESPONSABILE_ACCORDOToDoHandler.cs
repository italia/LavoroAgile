using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.AccordiToDoHandlers
{
    public class AccordoKEY_CLAIM_RESPONSABILE_ACCORDOToDoHandler : IAccordiToDoHandler
    {
        public IQueryable<Accordo> AccordiToDoFilter(IQueryable<Accordo> collection)
        {
            return collection.Where(a => a.Stato == StatoAccordo.DaSottoscrivereRA ||
                                        a.Stato == StatoAccordo.DaApprovareRA);
        }

        public IQueryable<Accordo> AccordiToDoValutazioniFilter(IQueryable<Accordo> collection, Guid uid)
        {
            return collection.Where(a => a.DataNotaDipendente != null && 
                                        a.DataNotaDirigente == null &&
                                        !a.Dipendente.Id.Equals(uid));
        }

    }
}
