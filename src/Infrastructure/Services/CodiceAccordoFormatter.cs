using Domain.Model;
using Domain.Settings;
using Microsoft.Extensions.Options;
using NetBox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    /// <summary>
    /// Formattatore per il codice accordo.
    /// </summary>
    public class CodiceAccordoFormatter
    {
        /// <summary>
        /// Formattazione da applicare al codice dell'accordo.
        /// </summary>
        private readonly string _format;

        /// <summary>
        /// Inizializza un nuovo <see cref="CodiceAccordoFormatter"/>.
        /// </summary>
        /// <param name="settings">Impostazioni di formattazione.</param>
        public CodiceAccordoFormatter(IOptions<DescriptiveCodeSettings> settings)
        {
            _format = settings.Value.DescriptiveCodeFormat;

        }

        /// <summary>
        /// Applica le regole di formattazione al valore passato in input
        /// </summary>
        /// <param name="accordo">Accordo da cui prelevare le informazioni di formattazione.</param>
        /// <returns></returns>
        public string Format(Accordo accordo)
        {
            // Se l'accordo è nullo, torna una stringa nulla.
            if (accordo == null)
            {
                return string.Empty;
            }

            // Se non è stato valorizzato il codice, restituisce il codice così com'è.
            if (string.IsNullOrWhiteSpace(_format))
            {
                return accordo.Codice.ToString();
            }

            // Recupera i formattatori all'interno della stringa di formato.
            var parameters = Regex.Matches(_format, @"\{(.+?)\}");

            // Disctionary di associazione placeholder - valore.
            var placeholdersValues = new Dictionary<string, string>();

            // Scorre i parametri, li valuta e ne salva il riferimento nel dictionary.
            foreach (Match parameter in parameters)
            {
                placeholdersValues[parameter.Groups[0].ToString()] = this.GetValue(accordo, parameter.Groups[1].ToString());
            }

            // Costruisce il codice.
            var codice = _format;
            foreach (var parameter in placeholdersValues)
            {
                codice = codice.Replace(parameter.Key, parameter.Value);
            }
            
            return codice;

        }

        /// <summary>
        /// Recupera il valore per lo specifico parametro.
        /// </summary>
        /// <param name="accordo">Accordo da cui prelevare il valore.</param>
        /// <param name="parameter">Parametro da estrarre.</param>
        /// <returns>Valore associato al campo.</returns>
        private string GetValue(Accordo accordo, string parameter)
        {
            // Recupero del nome del campo (se il parametro contiene |, il nome del parametro
            // si trova prima di |.
            var splittedParams = parameter.Split("|");

            // Verifica se il campo esiste nell'accordo altrimenti torna una stringa vuota.
            var accordoProperty = accordo.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals(splittedParams[0]));
            if (accordoProperty == null)
            {
                return string.Empty;
            }

            // Nel caso in cui siano stati specificati due parametri, ci si aspetta che
            // la property sia una data, quindi applica la formattazione.
            if (splittedParams.Length == 2 && (accordoProperty.PropertyType.Equals(typeof(DateTime)) || accordoProperty.PropertyType.Equals(typeof(DateTime?))))
            {
                return ((DateTime?)accordoProperty.GetValue(accordo))?.ToString(splittedParams[1]);
            }

            // Negli altri casi, ritorna il valore così com'è.
            return accordoProperty.GetValue(accordo)?.ToString();

        }

    }
}
