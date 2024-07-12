using Domain.Model.Identity;
using Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Context.Extensions;

/// <summary>
/// Extensions per il prepopolamento del database
/// </summary>
internal static class DatabaseSeeder
{
    /// <summary>
    /// Prepopola il database con dati essenziali per poter avviare un'istanza 
    /// vergine del sistema.
    /// </summary>
    /// <param name="modelBuilder">API per l'interazione con il builder del modello</param>
    /// <param name="adminUserSettings">Informazioni sull'utente admin</param>
    public static void SeedDatabase(this ModelBuilder modelBuilder, AdminUserSettings adminUserSettings)
    {
        // Utente admin
        var adminUser = new AppUser
        {
            Id = Guid.Parse("a98136a1-2e49-4e3b-af96-b5f7d8a43eea"),
            UserName = adminUserSettings.Username,
            NormalizedUserName = adminUserSettings.Username.ToUpper(),
            FullName = "Admin",
            Email = adminUserSettings.Username,
            NormalizedEmail = adminUserSettings.Username.ToUpper(),
            AccessFailedCount = 0,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            SecurityStamp = "fcf95cae-72a5-46a7-b96c-08f8bb3d4db3",
            ConcurrencyStamp = "562acd7f-1c59-4939-9680-238ec182b58d"
        };
        var passwordHasher = new PasswordHasher<AppUser>();
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminUserSettings.InitialPassword);
        modelBuilder.Entity<AppUser>().HasData(adminUser);

        // Ruoli
        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("e69ad520-ef3b-46a9-990b-3e6d1fb3facb"),
                Name = "Administrator",
                NormalizedName = "Administrator".ToUpper()
            });

        // Associazione utente-ruolo
        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>
            {
                RoleId = Guid.Parse("e69ad520-ef3b-46a9-990b-3e6d1fb3facb"),
                UserId = adminUser.Id
            });

    }

}
