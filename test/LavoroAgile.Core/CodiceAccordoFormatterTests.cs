using Domain.Model;
using Domain.Settings;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace LavoroAgile.Core
{
    /// <summary>
    /// Classe di test per il formattatore del codice accordo.
    /// </summary>
    public class CodiceAccordoFormatterTests
    {
        /// <summary>
        /// Verifica che il formattatore torni una stringa vuota nel caso in cui
        /// non venga passato alcun accordo.
        /// </summary>
        [Fact]
        public void FormatCodice_NoAccordo()
        {
            // Prepare
            var formatter = new CodiceAccordoFormatter(Options.Create(new DescriptiveCodeSettings { DescriptiveCodeFormat = "LA-{Codice}{DataSottoscrizione|-dd-MM-yyyy}" }));

            // Act
            var formatted = formatter.Format(null);

            // Test
            Assert.Equal(string.Empty, formatted);
        }

        /// <summary>
        /// Verifica che il formattatore torni il codice se non è stato configurato
        /// il formato di formattazione.
        /// </summary>
        [Fact]
        public void FormatCodice_NoFormat()
        {
            // Prepare
            var accordo = new Accordo();
            accordo.Codice = 13;
            accordo.StoricoStato = new[] { new StoricoStato(string.Empty, StatoAccordo.Sottoscritto, string.Empty) { Timestamp = new DateTime(2021, 12, 1) } };
            var formatter = new CodiceAccordoFormatter(Options.Create(new DescriptiveCodeSettings { DescriptiveCodeFormat = string.Empty }));

            // Act
            var formatted = formatter.Format(accordo);

            // Test
            Assert.Equal("13", formatted);
        }

        /// <summary>
        /// Verifica che il formattatore restituisca una stringa vuota se la proprietà
        /// non esiste nell'oggetto valutato.
        /// </summary>
        [Fact]
        public void FormatCodice_InvalidProperty()
        {
            // Prepare
            var accordo = new Accordo();
            accordo.Codice = 13;
            accordo.StoricoStato = new[] { new StoricoStato(string.Empty, StatoAccordo.Sottoscritto, string.Empty) { Timestamp = new DateTime(2021, 12, 1) } };
            var formatter = new CodiceAccordoFormatter(Options.Create(new DescriptiveCodeSettings { DescriptiveCodeFormat = "LA-{NotExists}{DataSottoscrizione|-dd-MM-yyyy}" }));

            // Act
            var formatted = formatter.Format(accordo);

            // Test
            Assert.Equal("LA--01-12-2021", formatted);
        }

        /// <summary>
        /// Verifica il corretto funzionamento del formattatore.
        /// </summary>
        [Fact]
        public void FormatCodice()
        {
            // Prepare
            var accordo = new Accordo();
            accordo.Codice = 13;
            accordo.StoricoStato = new[] { new StoricoStato(string.Empty, StatoAccordo.Sottoscritto, string.Empty) { Timestamp = new DateTime(2021, 12, 1) } };
            var formatter = new CodiceAccordoFormatter(Options.Create(new DescriptiveCodeSettings { DescriptiveCodeFormat = "LA-{Codice}{DataSottoscrizione|-dd-MM-yyyy}" }));

            // Act
            var formatted = formatter.Format(accordo);

            // Test
            Assert.Equal("LA-13-01-12-2021", formatted);

        }
    }
}
