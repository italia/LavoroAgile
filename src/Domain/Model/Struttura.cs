using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model
{
    public class Struttura : Entity<Guid>
    {
        public string StrutturaCompleta { get; private set; }

        public int NumeroLivelli { get; private set; }

        public string StrutturaLiv1 { get; init; } = null;

        public string StrutturaLiv2 { get; init; } = null;

        public string StrutturaLiv3 { get; init; } = null;

        public bool DirettaCollaborazione { get; init; }

        public Dirigente CapoStruttura { get; init; }

        public Dirigente CapoIntermedio { get; init; }

        public Dirigente DirigenteResponsabile { get; set; }

        public Dirigente ResponsabileAccordo { get; private set; }

        public Referente ReferenteInterno { get; private set; }

        public Struttura() { }

        /// <summary>
        /// Inizializza una nuova struttura.
        /// </summary>
        /// <param name="strutturaLiv1">Nome della struttura di primo livello</param>
        /// <param name="capoStruttura">Dirigente della struttura di primo livello</param>
        /// <param name="strutturaLiv2">Nome della struttura di secondo livello</param>
        /// <param name="capoIntermedio">Dirigente della struttura di secondo livello</param>
        /// <param name="strutturaLiv3">Nome della struttura di terzo livello</param>
        /// <param name="dirigenteResponsabile">Dirigente della struttura di terzo livello</param>
        public Struttura(string strutturaLiv1,
                        Dirigente capoStruttura,
                        string strutturaLiv2,
                        Dirigente capoIntermedio,
                        string strutturaLiv3,
                        Dirigente dirigenteResponsabile)
        {
            if (!string.IsNullOrWhiteSpace(strutturaLiv1))
                StrutturaLiv1 = strutturaLiv1;
            CapoStruttura = capoStruttura;

            if (!string.IsNullOrWhiteSpace(strutturaLiv2))
                StrutturaLiv2 = strutturaLiv2;
            CapoIntermedio = capoIntermedio;

            if (!string.IsNullOrWhiteSpace(strutturaLiv3))
                StrutturaLiv3 = strutturaLiv3;
            DirigenteResponsabile = dirigenteResponsabile;

            SetResponsabileAccordo();
            SetStrutturaCompleta();
        }

        /// <summary>
        /// Imposta le informazioni sul referente interno.
        /// </summary>
        /// <param name="referenteInterno">Profilo del responsabile interno.</param>
        public void SetReferenteInterno(Referente referenteInterno)
        {
            ReferenteInterno = referenteInterno;
        }

        /// <summary>
        /// Calcola il responsabile dell'accordo.
        /// </summary>
        private void SetResponsabileAccordo()
        {
            // Il responsabile dell'accordo è il responsabile del livello più basso compilato.
            if (!string.IsNullOrWhiteSpace(StrutturaLiv3))
            {
                ResponsabileAccordo = (Dirigente)DirigenteResponsabile.Clone();
            }
            else if (!string.IsNullOrWhiteSpace(StrutturaLiv2))
            {
                ResponsabileAccordo = (Dirigente)CapoIntermedio.Clone();
            }
            else
            {
                ResponsabileAccordo = (Dirigente)CapoStruttura.Clone();
            }

        }

        /// <summary>
        /// Calcola la descrizione completa della struttura.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void SetStrutturaCompleta()
        {
            // La struttura di livello 1 non può essere nulla.
            if (string.IsNullOrWhiteSpace(StrutturaLiv1))
            {
                throw new ArgumentNullException(nameof(StrutturaLiv1));
            }

            StrutturaCompleta = StrutturaLiv1;
            NumeroLivelli = 1;

            if (!string.IsNullOrWhiteSpace(this.StrutturaLiv2))
            {
                this.StrutturaCompleta += "/" + StrutturaLiv2;
                this.NumeroLivelli++;
            }

            if (!string.IsNullOrWhiteSpace(this.StrutturaLiv3))
            {
                this.StrutturaCompleta += "/" + StrutturaLiv3;
                this.NumeroLivelli++;
            }
        }

        /// <summary>
        /// Indica se questa struttura è uguale a <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Struttura da confrontare.</param>
        /// <returns>true se le strutture sono uguali.</returns>
        public override bool Equals(object obj)
        {
            // Due strutture sono uguali se hanno stesse descrizioni dei livelli
            // e stesse informazioni sui dirigenti
            if (obj is not Struttura)
            {
                return false;
            }

            var s = obj as Struttura;
            return StrutturaLiv1 == s.StrutturaLiv1 &&
                CapoStruttura?.NomeCognome == s.CapoStruttura?.NomeCognome &&
                CapoStruttura?.Email == s.CapoStruttura?.Email &&
                StrutturaLiv2 == s.StrutturaLiv2 &&
                CapoIntermedio?.NomeCognome == s.CapoIntermedio?.NomeCognome &&
                CapoIntermedio?.Email == s.CapoIntermedio?.Email &&
                StrutturaLiv3 == s.StrutturaLiv3 &&
                DirigenteResponsabile?.NomeCognome == s.DirigenteResponsabile?.NomeCognome &&
                DirigenteResponsabile?.Email == s.DirigenteResponsabile?.Email;

        }

        /// <summary>
        /// Verifica se due strutture sono identiche.
        /// </summary>
        /// <param name="s1">Prima struttura</param>
        /// <param name="s2">Seconda struttura</param>
        /// <returns>true se le strutture sono uguali</returns>
        public static bool operator ==(Struttura s1, Struttura s2)
        {
            // I due oggetti sono uguali se sono entrambi nulli o se Equals applicato
            // ai due elementi è true
            // s1 | s2 | result
            // ----------------
            // 0  | 0  | 1
            // 0  | 1  | 0
            // 1  | 0  | 0
            // 1  | 1  | Equals



            return (s1 is null && s2 is null) || (s1?.Equals(s2) ?? false);
        }

        /// <summary>
        /// Verifica se due strutture sono diverse.
        /// </summary>
        /// <param name="s1">Prima struttura</param>
        /// <param name="s2">Seconda struttura</param>
        /// <returns>true se le strutture sono diverse</returns>
        public static bool operator !=(Struttura s1, Struttura s2) => !(s1 == s2);

        /// <summary>
        /// Strutture gestite con una integrazione esterna, solo primo livello = true
        /// Strutture gestite internamente da db, solo primo livello = false
        /// </summary>
        [NotMapped]
        public bool OnlyFirstLevel { get; set; } = false;
    }
}
