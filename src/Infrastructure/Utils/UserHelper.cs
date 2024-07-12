using Domain.Model;
using Domain.Model.Identity;
using Esprima.Ast;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Helpers
{
    /// <summary>
    /// Classe con metodi di utilità sulla gestione utenti
    /// </summary>
    public static class UserHelper
    {
        /// <summary>
        /// Cerca i profili dei dirigenti afferenti ad una struttura in base alla username (= email) 
        /// e li crea nel caso in cui non dovessero essere già presenti nella base dati.
        /// </summary>
        /// <param name="serviceScopeFactory">Factory per il recupero di istanze di servizi scoped</param>
        /// <param name="accordo">Accordo da cui recuperare le informazioni sui dirigenti.</param>
        /// <returns>Campo id dei dirigenti popolato con l'identificativo recuperato da base dati.</returns>
        /// <remarks>Attenzione! Salvare l'accordo per memorizzare gli identificativi dei profili individuati/creati</remarks>
        public static async Task FindOrCreateManagersByUsernameAsync(this Accordo accordo, IServiceScopeFactory serviceScopeFactory)
        {
            var users = new List<PeopleCommon>()
            {
                accordo.CapoStruttura,
                accordo.CapoIntermedio,
                accordo.DirigenteResponsabile,
                accordo.ResponsabileAccordo,
                accordo.ReferenteInterno
            };

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();

                // Il controllo deve essere effettuato solo per i dipendenti con mail valorizzata.
                // Chi chiama questo metodo non fa una verifica sull'eventuale presenza di una figura, quindi
                // nel caso in cui una figura manchi, arriverà un utente senza mail (ad esempio capo intermedio
                // non presente -> mail vuota)
                // Dato che è probabile la duplicazione di figure, effettua anche una unique sull'email
                var filteredUsers = users.Where(u => !string.IsNullOrWhiteSpace(u.Email)).GroupBy(u => u.Email).Select(gu => gu.First());
                // Carica i profili degli utenti
                var userProfiles = filteredUsers.FindUsersByEmailAsync(userManager);

                // Se qualche profilo non è stato trovato, lo crea.
                var notFoundUsers = filteredUsers.Where(au => !userProfiles.Any(u => u.Email.Equals(au.Email)))?.ToList() ?? new List<PeopleCommon>();

                // Elenco degli eventuali utenti per cui fallisce la creazione (perché esiste già un account)
                var alreadyExistingUsers = new List<AppUser>();
                foreach (var user in notFoundUsers)
                {
                    var result = await userManager.CreateAsync(new AppUser(user.NomeCognome, user.Email));
                    // Non viene più sollevata l'eccezione perché il controllo è stato spostato da
                    // check sulla Username a check sulla Email per risolvere i casi degli account che hanno
                    // Username valorizzata con quella di dominio ed Email valorizzata con quella che arriva
                    // dal provider esterno. In questi casi potrebbe succedere che l'account esiste già.
                    // Nel caso in cui l'utenza esista già, la ricarica in base alla username
                    if (!result.Succeeded && result.Errors.Any(e => e.Code.Equals("DuplicateUserName")))
                    {
                        //throw new Exception("Errore durante la creazione del profilo utente");
                        alreadyExistingUsers.Add(user.FindUsersByUsernameAsync(userManager));
                    }
                }

                // Se sono stati creati profili, ricarica i dettagli in modo da avere anche gli id
                // dei nuovi profili
                if (notFoundUsers.Any())
                {
                    userProfiles = users.FindUsersByEmailAsync(userManager);
                }

                // Verifica se nel frattempo non è stato creato/modificato un account (in questo caso
                // uscirebbe nella user profiles
                alreadyExistingUsers = alreadyExistingUsers.Where(aeu => !userProfiles.ToList().Any(u => u.Email.Equals(aeu.Email, StringComparison.OrdinalIgnoreCase)))?.ToList();
                var cleanedUsers = userProfiles.ToList();
                if (alreadyExistingUsers?.Count > 0)
                {
                    cleanedUsers.AddRange(alreadyExistingUsers);
                }

                users.ForEach(user =>
                {
                    // Se esistono più account per l'utente, prende il primo non loccato.
                    // Nel caso in cui siano tutti loccati, associa il primo.
                    var userId = cleanedUsers.FirstOrDefault(u => !u.LockoutEnd.HasValue && (u.Email.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase) || u.UserName.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase)))?.Id ?? Guid.Empty;
                    if (userId.Equals(Guid.Empty))
                    {
                        userId = cleanedUsers.FirstOrDefault(u => u.Email.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase) || u.UserName.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase))?.Id ?? Guid.Empty;
                    }
                    user.Id = userId;
                });

            }
        }

        /// <summary>
        /// Ricerca ed eventualmente crea una persona associata alla segreteria tecnica.
        /// </summary>
        /// <param name="user">User da cercare ed eventualmente creare.</param>
        /// <param name="serviceScopeFactory">Factory dei servizi</param>
        /// <returns>Campo id della persona individuata/censita</returns>
        /// <exception cref="Exception"></exception>
        /// <remarks>Attenzione! Salvare l'oggetto user per memorizzare l'id del profilo individuato/creati</remarks>
        public static async Task FindOrCreateSegreteriaTecnicaByUsernameAsync(this SegreteriaTecnica user, IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();

                var userAsArray = new[] { user.ToReferente() };

                // Carica il profili degli utenti
                var userProfile = userAsArray.FindUsersByEmailAsync(userManager);

                // Se il profilo non esiste lo crea.
                if (userProfile.Count() == 0)
                {
                    var result = await userManager.CreateAsync(new AppUser(user.NomeCompleto, user.EmailUtente));
                    // Solleva un'eccezione se l'utente esiste già
                    if (!result.Succeeded && !result.Errors.Any(e => e.Code.Equals("DuplicateUserName")))
                    {
                        throw new Exception("Errore durante la creazione del profilo utente");
                    }
                    userProfile = userAsArray.FindUsersByEmailAsync(userManager);
                }
                user.UserProfileId = userProfile.FirstOrDefault()?.Id ?? Guid.Empty;

            }
        }

        /// <summary>
        /// Ricerca ed eventualmente crea i dirigenti associati alla struttura.
        /// </summary>
        /// <param name="struttura">Struttura di cui ricercare ed eventualmente creare i profili utente.</param>
        /// <param name="serviceScopeFactory">Factory dei servizi</param>
        /// <returns>Campo id delle persone individuate/censite</returns>
        /// <exception cref="Exception"></exception>
        /// <remarks>Attenzione! Salvare l'oggetto user per memorizzare l'id del profilo individuato/creati</remarks>
        public static async Task FindOrCreateStrutturaManagerByUsernameAsync(this Struttura struttura, IServiceScopeFactory serviceScopeFactory)
        {
            var accordo = new Accordo()
            {
                CapoStruttura = struttura.CapoStruttura,
                CapoIntermedio = struttura.CapoIntermedio,
                DirigenteResponsabile = struttura.DirigenteResponsabile,
                ResponsabileAccordo = struttura.ResponsabileAccordo,
                ReferenteInterno = struttura.ReferenteInterno
            };

            await accordo.FindOrCreateManagersByUsernameAsync(serviceScopeFactory);
        }

        /// <summary>
        /// Recupera il profilo di un insieme di utenti.
        /// </summary>
        /// <param name="appUsers">Elenco degli utenti di cui recuperare il profilo.</param>
        /// <param name="userManager">Gestore degli utenti.</param>
        /// <returns>Dettaglio dei profili utenti.</returns>
        private static IQueryable<AppUser> FindUsersByEmailAsync(this IEnumerable<PeopleCommon> appUsers, UserManager<AppUser> userManager) => userManager.Users.Where(u => appUsers.Select(a => a.Email).Contains(u.Email) && !u.LockoutEnd.HasValue);

        /// <summary>
        /// Recupera il profilo di un utente ricercandolo per username
        /// </summary>
        /// <param name="appUser">Utente da recuperare</param>
        /// <param name="userManager">Gestore degli utenti</param>
        /// <returns>Dettaglio del profilo utente</returns>
        private static AppUser FindUsersByUsernameAsync(this PeopleCommon appUser, UserManager<AppUser> userManager) => userManager.Users.First(u => appUser.Email.Contains(u.UserName));


    }
}
