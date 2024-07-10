using System.ComponentModel;

namespace Domain.Model
{
    /// <summary>
    /// Enumerazione dei nomi dei workflow.
    /// </summary>
    public enum WorkflowNames
    {
        ApprovazioneLavoroAgile,
        ApprovazioneLavoroAgileRidotto
    }

    /// <summary>
    /// Enumerazione dei segnali gestiti dal flusso di lavoro agile.
    /// </summary>
    public enum LavoroAgileSignals
    {
        ApprovatoCS,
        ApprovatoCI,
        ApprovatoRA,
        RifiutatoCS,
        RifiutatoCI,
        RifiutatoRA,
        RichiestaModifiche,
        InviaModificaCS,
        InviaModificaCI,
        InviaModificaRA,
        SottoscrittoRA,
        SottoscrittoP
    }

    /// <summary>
    /// Enumerazione degli operatori logici, uso la descrizione come stringa da far vedere a video.
    /// </summary>
    public enum OperatoriLogici
    {
        [Description("uguale a")]
        UgualeA,
        [Description("diverso da")]
        DiversoDA,
        [Description("minore di")]
        MinoreDI,
        [Description("minore uguale a")]
        MinoreUgualeA,
        [Description("maggiore di")]
        MaggioreDI,
        [Description("maggiore uguale a")]
        MaggioreUgualeA,
        [Description("compreso tra")]
        CompresoTRA,

    }

}
