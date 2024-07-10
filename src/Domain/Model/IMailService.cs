using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Model
{
    /// <summary>
    /// Definisce l'intefaccia del servizio di gestione email
    /// </summary>
    public interface IMailService
    {
        Task SendEmailAsync(Email email);
    }
}
