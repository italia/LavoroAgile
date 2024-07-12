using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class StrutturaContext : DbContext
    {
        public DbSet<Struttura> Strutture { get; set; }

        public DbSet<SegreteriaTecnica> SegreteriaTecnica { get; set; }

        public StrutturaContext(DbContextOptions<StrutturaContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Struttura>().OwnsOne(
                s => s.CapoStruttura,
                ci =>
                {
                    ci.Property(p => p.Id).HasColumnName("IdCapoStruttura");
                    ci.Property(p => p.NomeCognome).HasColumnName("CapoStruttura");
                    ci.Property(p => p.Email).HasColumnName("EmailCapoStruttura");
                });

            builder.Entity<Struttura>().OwnsOne(
                s => s.CapoIntermedio,
                ci =>
                {
                    ci.Property(p => p.Id).HasColumnName("IdCapoIntermedio");
                    ci.Property(p => p.NomeCognome).HasColumnName("CapoIntermedio");
                    ci.Property(p => p.Email).HasColumnName("EmailCapoIntermedio");
                });

            builder.Entity<Struttura>().OwnsOne(
                s => s.DirigenteResponsabile,
                ci =>
                {
                    ci.Property(p => p.Id).HasColumnName("IdDirigenteResponsabile");
                    ci.Property(p => p.NomeCognome).HasColumnName("DirigenteResponsabile");
                    ci.Property(p => p.Email).HasColumnName("EmailDirigenteResponsabile");
                });

            builder.Entity<Struttura>().OwnsOne(
                s => s.ResponsabileAccordo,
                ci =>
                {
                    ci.Property(p => p.Id).HasColumnName("IdResponsabileAccordo");
                    ci.Property(p => p.NomeCognome).HasColumnName("ResponsabileAccordo");
                    ci.Property(p => p.Email).HasColumnName("EmailResponsabileAccordo");
                });

            builder.Entity<Struttura>().OwnsOne(
                s => s.ReferenteInterno,
                ci =>
                {
                    ci.Property(p => p.Id).HasColumnName("IdReferenteInterno");
                    ci.Property(p => p.NomeCognome).HasColumnName("ReferenteInterno");
                    ci.Property(p => p.Email).HasColumnName("EmailReferenteInterno");
                });
        }
    }
}
