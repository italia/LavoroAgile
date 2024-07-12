using System;

namespace Domain.Model
{
    public class SegreteriaTecnica : Entity<Guid>
    {
        public string NomeCompleto { get; set; }

        public string EmailUtente { get; set; }

        public Guid UserProfileId { get; set; } = Guid.Empty;

        public SegreteriaTecnica() { }

        public Referente ToReferente() => new Referente(NomeCompleto, string.Empty, EmailUtente);
    }
}
