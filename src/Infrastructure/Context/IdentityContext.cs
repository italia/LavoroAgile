using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Model.Identity;
using Infrastructure.Context.Extensions;
using Microsoft.Extensions.Options;
using Domain.Settings;

namespace Infrastructure.Context
{
    public class IdentityContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        private readonly AdminUserSettings _adminUserSettings;

        public IdentityContext(DbContextOptions<IdentityContext> options, IOptionsSnapshot<AdminUserSettings> adminUser)
            : base(options)
        {
            _adminUserSettings = adminUser.Value;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.SeedDatabase(_adminUserSettings);
        }
    }
}
