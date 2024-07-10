using Domain.Model.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IStrutturaService
    {
        Task<Struttura> InitCreateStrutturaAsync(CancellationToken cancellationToken = default);

        Task InsertStrutturaAsync(Struttura struttura, CancellationToken cancellationToken = default);
        
        Task UpdateStrutturaAsync(Struttura struttura, CancellationToken cancellationToken = default);

        Task DeleteStrutturaAsync(Guid id, CancellationToken cancellationToken);

        Task<Struttura> GetStrutturaAsync(Guid idStruttura, CancellationToken cancellationToken = default);

        Task<SearchResult<Struttura, Guid>> FindStrutturaAsync(Expression<Func<Struttura, bool>> whereExpression = null, CancellationToken cancellationToken = default);

    }
}
