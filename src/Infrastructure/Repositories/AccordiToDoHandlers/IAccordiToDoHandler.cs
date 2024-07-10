using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.AccordiToDoHandlers
{
    public interface IAccordiToDoHandler
    {
        IQueryable<Accordo> AccordiToDoFilter(IQueryable<Accordo> collection);

        IQueryable<Accordo> AccordiToDoValutazioniFilter(IQueryable<Accordo> collection, Guid uid);
    }
}
