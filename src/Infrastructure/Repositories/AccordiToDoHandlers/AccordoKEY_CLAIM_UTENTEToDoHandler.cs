using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.AccordiToDoHandlers
{
    public class AccordoKEY_CLAIM_UTENTEToDoHandler : IAccordiToDoHandler
    {
        public IQueryable<Accordo> AccordiToDoFilter(IQueryable<Accordo> collection)
        {

            return collection.Where(a => a.Stato == StatoAccordo.DaSottoscrivereP ||
                                        a.Stato == StatoAccordo.RichiestaModificaCI ||
                                        a.Stato == StatoAccordo.RichiestaModificaRA ||
                                        a.Stato == StatoAccordo.RichiestaModificaCS);
        }

        public IQueryable<Accordo> AccordiToDoValutazioniFilter(IQueryable<Accordo> collection, Guid uid)
        {
            //Per questo ruolo non sono applicate ricerche su accordi da valutare, quindi viene restituita una lista vuota
            return new List<Accordo>().AsQueryable();
        }
    }
}
