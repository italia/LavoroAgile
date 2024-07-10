using Domain.Model;
using Domain.Model.ExternalCommunications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    /// <summary>
    /// DbContext per gli accordi
    /// </summary>
    public class AccordoContext : DbContext
    {
        /// <summary>
        /// Elenco degli accordi.
        /// </summary>
        public DbSet<Accordo> Accordi { get; set; }

        /// <summary>
        /// Storico delle modifiche dello stato (aggiunto per aggiungere voci
        /// allo storico di più accordi contemporaneamente).
        /// </summary>
        public DbSet<StoricoStato> StoricoStato { get; set; }

        public DbSet<AttivitaAccordo> AttivitaAccordo { get; set; }

        public DbSet<Valutazione> Valutazione { get; set; }

        /// <summary>
        /// Elenco degli stati di trasmissione degli accordi.
        /// </summary>
        public DbSet<TransmissionStatus> AccordoTransmission { get; set; }

        /// <summary>
        /// Inizializza un nuovo <see cref="AccordoContext"/>.
        /// </summary>
        /// <param name="options"></param>
        public AccordoContext(DbContextOptions<AccordoContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Chiave alternativa sul codice dell'accordo (per consentirne la generazione automatica
            // da parte del DB).
            modelBuilder.Entity<Accordo>().HasAlternateKey(a => a.Codice);

            // Definizione relazione one-to-one senza chiave fra Accordo e Stato della trasmissione
            // AccordoId fa da foreign key con l'accordo.
            modelBuilder.Entity<TransmissionStatus>().ToTable(nameof(TransmissionStatus)).HasKey(t => t.AccordoId);
            modelBuilder.Entity<Accordo>().HasOne(a => a.Transmission).WithOne(t => t.Accordo);

            modelBuilder.Entity<Accordo>().OwnsOne(a => a.Dipendente,
                d =>
                {
                    d.Property(d => d.NomeCognome).HasColumnName("NomeCognome");
                    d.Property(d => d.DataDiNascita).HasColumnName("DataDiNascita");
                    d.Property(d => d.Sesso).HasColumnName("Sesso");
                    d.Property(d => d.LuogoDiNascita).HasColumnName("LuogoDiNascita");
                    d.Property(d => d.CodiceFiscale).HasColumnName("CodiceFiscale");
                    d.Property(d => d.Id).HasColumnName("IdDipendente");
                    d.Property(d => d.Email).HasColumnName("Email");
                    d.Property(d => d.PosizioneGiuridica).HasColumnName("PosizioneGiuridica");
                    d.Property(d => d.CategoriaFasciaRetributiva).HasColumnName("CategoriaFasciaRetributiva");

                });

            modelBuilder.Entity<Accordo>().OwnsOne(a => a.CapoStruttura,
                d =>
                {
                    d.Property(d => d.NomeCognome).HasColumnName("CapoStruttura");
                    d.Property(d => d.Email).HasColumnName("EmailCapoStruttura");
                    d.Property(d => d.Id).HasColumnName("IdCapoStruttura");

                });

            modelBuilder.Entity<Accordo>().OwnsOne(a => a.CapoIntermedio,
                d =>
                {
                    d.Property(d => d.NomeCognome).HasColumnName("CapoIntermedio");
                    d.Property(d => d.Email).HasColumnName("EmailCapoIntermedio");
                    d.Property(d => d.Id).HasColumnName("IdCapoIntermedio");

                });

            modelBuilder.Entity<Accordo>().OwnsOne(a => a.ResponsabileAccordo,
                d =>
                {
                    d.Property(d => d.NomeCognome).HasColumnName("ResponsabileAccordo");
                    d.Property(d => d.Email).HasColumnName("EmailResponsabileAccordo");
                    d.Property(d => d.Id).HasColumnName("IdResponsabileAccordo");

                });

            modelBuilder.Entity<Accordo>().OwnsOne(a => a.DirigenteResponsabile,
                d =>
                {
                    d.Property(d => d.NomeCognome).HasColumnName("DirigenteResponsabile");
                    d.Property(d => d.Email).HasColumnName("EmailDirigenteResponsabile");
                    d.Property(d => d.Id).HasColumnName("IdDirigenteResponsabile");

                });

            modelBuilder.Entity<Accordo>().OwnsOne(a => a.ReferenteInterno,
                d =>
                {
                    d.Property(d => d.NomeCognome).HasColumnName("ReferenteInterno");
                    d.Property(d => d.Email).HasColumnName("EmailReferenteInterno");
                    d.Property(d => d.Id).HasColumnName("IdReferenteInterno");

                });
        }
    }
}
