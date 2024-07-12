using Domain.Model;
using ObjectsComparer;

namespace Infrastructure.Factories
{
    /// <summary>
    /// Definisce la factory per l'inizializzazione del comparer per gli accordi
    /// </summary>
    public class AccordoComparersFactory : ComparersFactory
    {
        public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, BaseComparer parentComparer = null)
        {
            if (typeof(T) == typeof(Accordo))
            {
                var comparer = new Comparer<Accordo>(settings, parentComparer, this);
                comparer.IgnoreMember(() => new Accordo().ChildId);
                comparer.IgnoreMember(() => new Accordo().Codice);
                comparer.IgnoreMember(() => new Accordo().CreationDate);
                comparer.IgnoreMember(() => new Accordo().DataFineAccordoPrecedente);
                comparer.IgnoreMember(() => new Accordo().DataFineUtc);
                comparer.IgnoreMember(() => new Accordo().DataInizioUtc);
                comparer.IgnoreMember(() => new Accordo().DataNotaDipendente);
                comparer.IgnoreMember(() => new Accordo().DataNotaDirigente);
                comparer.IgnoreMember(() => new Accordo().DataRecesso);
                comparer.IgnoreMember(() => new Accordo().DataSottoscrizione);
                comparer.IgnoreMember(() => new Accordo().EditTime);
                comparer.IgnoreMember(() => new Accordo().InCorso);
                comparer.IgnoreMember(() => new Accordo().InvioNotificaNoteCondivise);
                comparer.IgnoreMember(() => new Accordo().isValutazionePositiva);
                comparer.IgnoreMember(() => new Accordo().NotaDipendente);
                comparer.IgnoreMember(() => new Accordo().NotaDirigente);
                comparer.IgnoreMember(() => new Accordo().NoteCondivise);
                comparer.IgnoreMember(() => new Accordo().NoteRefereteInterno);
                comparer.IgnoreMember(() => new Accordo().NoteSegreteriaTecnica);
                comparer.IgnoreMember(() => new Accordo().ParentId);
                comparer.IgnoreMember(() => new Accordo().PrimoAccordo);
                comparer.IgnoreMember(() => new Accordo().Recidibile);
                comparer.IgnoreMember(() => new Accordo().Rinnovabile);
                comparer.IgnoreMember(() => new Accordo().Stato);
                comparer.IgnoreMember(() => new Accordo().StoricoStato);
                comparer.IgnoreMember(() => new Accordo().DirigenteResponsabile);
                comparer.IgnoreMember(() => new Accordo().UidStrutturaUfficioServizio);
                comparer.IgnoreMember(() => new Accordo().ValutazioneEsitiAccordoPrecedente);
                comparer.IgnoreMember(() => new Accordo().VistoSegreteriaTecnica);
                comparer.IgnoreMember(() => new Accordo().RevisioneAccordo);
                comparer.IgnoreMember(() => new Accordo().DerogaPianificazioneDate);
                comparer.IgnoreMember(() => new Accordo().Transmission);
                comparer.IgnoreMember(() => new Accordo().ListaAttivita);
                comparer.IgnoreMember(() => new Accordo().NumLivelliStruttura);
                comparer.IgnoreMember(() => new Accordo().PianificazioneGiorniAccordo);
                comparer.IgnoreMember(() => new Accordo().PianificazioneDateAccordo);

                // Anche se specificate sopra, il comparer non le esclude (credo
                // per il fatto che siano definite a livello di classe superiore).
                comparer.IgnoreMember("Id");
                comparer.IgnoreMember("CreationDate");
                comparer.IgnoreMember("EditTime");
                comparer.IgnoreMember("ParentId");
                comparer.IgnoreMember("ChildId");



                return (IComparer<T>)comparer;
            }

            return base.GetObjectsComparer<T>(settings, parentComparer);
        }
    }
}
