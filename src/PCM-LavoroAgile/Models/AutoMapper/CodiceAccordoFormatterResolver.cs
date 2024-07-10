using AutoMapper;
using Domain.Model;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Models.AutoMapper
{
    public class CodiceAccordoFormatterResolver : IValueResolver<Accordo, AccordoHeaderViewModel, string>, IValueResolver<Accordo, AccordoViewModel, string>
    {
        /// <summary>
        /// Formattatore del codice accordo.
        /// </summary>
        private readonly CodiceAccordoFormatter _codiceAccordoFormatter;

        public CodiceAccordoFormatterResolver(CodiceAccordoFormatter codiceAccordoFormatter)
        {
            _codiceAccordoFormatter = codiceAccordoFormatter;
        }

        public string Resolve(Accordo source, AccordoHeaderViewModel destination, string destMember, ResolutionContext context)
        {
            return _codiceAccordoFormatter.Format(source);
        }

        public string Resolve(Accordo source, AccordoViewModel destination, string destMember, ResolutionContext context)
        {
            return _codiceAccordoFormatter.Format(source);
        }
    }
}
