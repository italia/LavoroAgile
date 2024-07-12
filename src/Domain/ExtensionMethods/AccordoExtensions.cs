using Domain.Model;
using Domain.Model.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.ExtensionMethods
{
    public static class AccordoExtensions
    {
        /// <summary>
        /// Valorizza l'anagrafica di un accordo chiamando il servizio dell'ERP
        /// </summary>
        /// <param name="accordo"></param>
        /// <param name="dipendente">Profilo del dipendente autenticato.</param>
        /// <param name="_personalDataProviderService"></param>
        /// <param name="_strutturaService"></param>
        /// <param name="userManager"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Accordo> SetAnagrafica(this Accordo accordo,
                                            Dipendente dipendente,
                                            IPersonalDataProviderService _personalDataProviderService,
                                            IStrutturaService _strutturaService,
                                            UserManager<AppUser> userManager,
                                            CancellationToken cancellationToken = default)
        {
            // Recupera le informazioni sull'utente dall'anagrafica esterna.
            var userData = await _personalDataProviderService.GetUserDataAsync(dipendente.Email);

            // Se il servizio ha restituito dati, mappa le informazioni sull'accordo e quindi
            // recupera e mappa le informazioni sul referente interno.
            if (userData != null)
            {
                accordo.InitializedFromExtenalService = true;
                accordo.Dipendente = userData;
                accordo.StrutturaUfficioServizio = userData.Struttura?.StrutturaCompleta;
                accordo.Livello1 = userData.Struttura?.StrutturaLiv1;
                accordo.Livello2 = userData.Struttura?.StrutturaLiv2;
                accordo.Livello3 = userData.Struttura?.StrutturaLiv3;
                accordo.UidStrutturaUfficioServizio = userData.Struttura?.Id.ToString();
                accordo.CapoStruttura = userData.Struttura.CapoStruttura;
                accordo.CapoIntermedio = userData.Struttura.CapoIntermedio;
                accordo.ResponsabileAccordo = userData.Struttura.ResponsabileAccordo;
                accordo.DirigenteResponsabile = userData.Struttura.DirigenteResponsabile;

                // Se la struttura è valorizzata ed è specificata quella di primo livello, individua 
                // il referente interno.
                if (!string.IsNullOrWhiteSpace(userData.Struttura?.StrutturaLiv1))
                {
                    var struttura = (await _strutturaService.FindStrutturaAsync(s => s.StrutturaLiv1.Equals(userData.Struttura.StrutturaLiv1), cancellationToken: cancellationToken))?.Entities?.FirstOrDefault();
                    accordo.ReferenteInterno = struttura?.ReferenteInterno ?? new Referente();
                }

                if (userData.Struttura != null)
                {
                    accordo.NumLivelliStruttura = userData.Struttura.NumeroLivelli;
                }

                //Verifica del caso particolare, l'utente che opera è il dirigente responsabile del servizio, quindi il responsabile dell'accordo.
                //In questo caso il responsabile dell'accordo deve essere impostato come il primo responsabile utile,
                //va preseo il campo intermedio o il capo struttura a seconda dei livelli della struttua.
                if (dipendente.Email.Equals(userData.Struttura.ResponsabileAccordo.Email))
                {
                    switch (userData.Struttura.NumeroLivelli)
                    {
                        //Struttura a due livelli, il responsabile dell'accordo diventa il capo intermedio
                        case 3:
                            accordo.ResponsabileAccordo = (Dirigente)userData.Struttura?.CapoIntermedio.Clone();
                            break;
                        //Struttura ad un livello, il responsabile dell'accordo diventa il capo struttura
                        case 2:
                            accordo.ResponsabileAccordo = (Dirigente)userData.Struttura?.CapoStruttura.Clone();
                            break;
                    }
                }
            }

            // Se è attiva l'integrazione con il servizio di anagrafica esterna, imposta l'accordo 
            // come inizializzato da fonte esterna in modo da rendere non editabile la sezione
            // anagrafica.
            accordo.InitializedFromExtenalService = _personalDataProviderService.Enabled;

            // Se il provider di identità non ha restituito le informazioni
            // anagrafiche, valorizza le info del dipendente con informazioni minimali (nome, cognome, id, email)
            if (accordo.Dipendente is null)
            {
                accordo.Dipendente = dipendente;
            }
            accordo.Dipendente.Id = dipendente.Id;

            return accordo;
        }
    }
}
