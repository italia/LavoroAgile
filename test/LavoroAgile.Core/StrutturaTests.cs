using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LavoroAgile.Core
{
    /// <summary>
    /// Verifica il corretto comportamento della classe Struttura.
    /// </summary>
    public class StrutturaTests
    {
        /// <summary>
        /// Verifica la corretta composizione della descrizione completa della struttura
        /// </summary>
        [Fact]
        public void TestDescrizioneCompleta3Livelli()
        {
            var struttura = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Capo intermedio", Email = "capo.intermedio@mail.it" },
                "Livello3", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            var expected = "Livello1/Livello2/Livello3";

            Assert.Equal(expected, struttura.StrutturaCompleta);

        }

        /// <summary>
        /// Verifica la corretta composizione della descrizione completa della struttura
        /// </summary>
        [Fact]
        public void TestDescrizioneCompleta2Livelli()
        {
            var struttura = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Capo intermedio", Email = "capo.intermedio@mail.it" },
                null, null);

            var expected = "Livello1/Livello2";

            Assert.Equal(expected, struttura.StrutturaCompleta);

        }

        /// <summary>
        /// Verifica la corretta composizione della descrizione completa della struttura
        /// </summary>
        [Fact]
        public void TestDescrizioneCompleta2LivelliNoIntermedio()
        {
            var struttura = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                null, null, 
                "Livello3", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            var expected = "Livello1/Livello3";

            Assert.Equal(expected, struttura.StrutturaCompleta);

        }

        /// <summary>
        /// Verifica la corretta composizione della descrizione completa della struttura
        /// </summary>
        [Fact]
        public void TestDescrizioneCompleta1Livello()
        {
            var struttura = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                null, null,
                null, null);

            var expected = "Livello1";

            Assert.Equal(expected, struttura.StrutturaCompleta);

        }

        /// <summary>
        /// Verifica la corretta determinazione del responsabile accordo
        /// </summary>
        [Fact]
        public void TestRA3Livelli()
        {
            var struttura = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Capo intermedio", Email = "capo.intermedio@mail.it" },
                "Livello3", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            string expectedName = "Dirigente responsabile", expextedMail = "dirigente.responsabile@mail.it";

            Assert.Equal(expectedName, struttura.ResponsabileAccordo.NomeCognome);
            Assert.Equal(expextedMail, struttura.ResponsabileAccordo.Email);

        }

        /// <summary>
        /// Verifica la corretta determinazione del responsabile accordo
        /// </summary>
        [Fact]
        public void TestRA2Livelli()
        {
            var struttura = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Capo intermedio", Email = "capo.intermedio@mail.it" },
                null, null);

            string expectedName = "Capo intermedio", expextedMail = "capo.intermedio@mail.it";

            Assert.Equal(expectedName, struttura.ResponsabileAccordo.NomeCognome);
            Assert.Equal(expextedMail, struttura.ResponsabileAccordo.Email);

        }

        /// <summary>
        /// Verifica la corretta determinazione del responsabile accordo
        /// </summary>
        [Fact]
        public void TestRA2LivelliNoIntermedio()
        {
            var struttura = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                null, null,
                "Livello3", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            string expectedName = "Dirigente responsabile", expextedMail = "dirigente.responsabile@mail.it";

            Assert.Equal(expectedName, struttura.ResponsabileAccordo.NomeCognome);
            Assert.Equal(expextedMail, struttura.ResponsabileAccordo.Email);

        }

        /// <summary>
        /// Verifica la corretta determinazione del responsabile accordo
        /// </summary>
        [Fact]
        public void TestRA1Livello()
        {
            var struttura = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                null, null,
                null, null);

            string expectedName = "Capo struttura", expextedMail = "capo.struttura@mail.it";

            Assert.Equal(expectedName, struttura.ResponsabileAccordo.NomeCognome);
            Assert.Equal(expextedMail, struttura.ResponsabileAccordo.Email);

        }

        /// <summary>
        /// Verifica il comportamento dell'operatore ==
        /// </summary>
        [Fact]
        public void TestEqualsOperator()
        {
            var struttura1 = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Capo intermedio", Email = "capo.intermedio@mail.it" },
                "Livello3", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            var struttura2 = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Capo intermedio", Email = "capo.intermedio@mail.it" },
                "Livello3", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            Assert.True(struttura1 == struttura2);

        }

        /// <summary>
        /// Verifica il comportamento dell'operatore ==
        /// </summary>
        [Fact]
        public void TestUnequalsOperator()
        {
            var struttura1 = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Capo intermedio", Email = "capo.intermedio@mail.it" },
                "Livello3", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            var struttura2 = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Modificato", Email = "capo.intermedio@mail.it" },
                "Modificato", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            Assert.False(struttura1 == struttura2);

        }

        /// <summary>
        /// Verifica correttezza operatore == in presenza di un operatore nullo
        /// </summary>
        [Fact]
        public void TestEqualsWithNullOperator1()
        {
            Struttura struttura1 = null;

            var struttura2 = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Modificato", Email = "capo.intermedio@mail.it" },
                "Modificato", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            Assert.False(struttura1 == struttura2);

        }

        /// <summary>
        /// Verifica correttezza operatore == in presenza di un operatore nullo
        /// </summary>
        [Fact]
        public void TestEqualsWithNullOperator2()
        {
            var struttura1 = new Struttura(
                "Livello1", new Dirigente { NomeCognome = "Capo struttura", Email = "capo.struttura@mail.it" },
                "Livello2", new Dirigente { NomeCognome = "Capo intermedio", Email = "capo.intermedio@mail.it" },
                "Livello3", new Dirigente { NomeCognome = "Dirigente responsabile", Email = "dirigente.responsabile@mail.it" });

            Struttura struttura2 = null;

            Assert.False(struttura1 == struttura2);

        }

        /// <summary>
        /// Verifica correttezza operatore == in presenza di due nulli
        /// </summary>
        [Fact]
        public void TestEqualsWithNullOperators()
        {
            Struttura struttura1 = null, struttura2 = null;

            Assert.True(struttura1 == struttura2);

        }

    }
}
