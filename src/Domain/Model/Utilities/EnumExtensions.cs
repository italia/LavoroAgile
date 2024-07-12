using System;
using System.ComponentModel;
using System.Linq;

namespace Domain.Model.Utilities
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString<T>(this T val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.FirstOrDefault()?.Description ?? string.Empty;
        }

        /// <summary>
        /// Tenta di convertire una stringa in un elemento di una specifica enumeration, basandosi sul valore
        /// dell'attributo Description con cui è decorato il valore della enum.
        /// </summary>
        /// <typeparam name="T">Tipo dell'enumerazione verso cui tentare di convertire la stringa.</typeparam>
        /// <param name="value">Stringa da convertire</param>
        /// <returns>Il valore della enum decorato da un attributo <see cref="DescriptionAttribute"/> valorizzato con la stringa <paramref name="value"/>.</returns>
        /// <remarks>Nel caso in cui fallisce la conversione, viene scatenata una <see cref="InvalidCastException"/></remarks>
        public static T ToEnum<T>(this string value)
        {
            var res = ((T[])Enum.GetValues(typeof(T))).FirstOrDefault(r => r.ToDescriptionString().Equals(value));

            // Dato che l'enumeration non può essere nulla, il fallimento si ha nel caso in cui la riconversione
            // a stringa del valore individuato è la stringa ricevuta in parametro.
            if (!res.ToDescriptionString<T>().Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidCastException($"Impossibile trovare un valore dell'enumerazione '{ typeof(T).Name }' con descrizione '{value}'.");
            }

            return res;

        }
    }
}
