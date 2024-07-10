
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0", IsNullable = false)]
public partial class CreaComunicazione_Input
{

    private CreaComunicazione_InputSezioneDatoreLavoro sezioneDatoreLavoroField;

    private CreaComunicazione_InputSezioneLavoratore sezioneLavoratoreField;

    private CreaComunicazione_InputSezioneRapportoLavoro sezioneRapportoLavoroField;

    private CreaComunicazione_InputSezioneAccordoSmartWorking sezioneAccordoSmartWorkingField;

    private CreaComunicazione_InputSezioneSoggettoAbilitato sezioneSoggettoAbilitatoField;

    private string codTipologiaComunicazioneField;

    /// <remarks/>
    public CreaComunicazione_InputSezioneDatoreLavoro SezioneDatoreLavoro
    {
        get
        {
            return this.sezioneDatoreLavoroField;
        }
        set
        {
            this.sezioneDatoreLavoroField = value;
        }
    }

    /// <remarks/>
    public CreaComunicazione_InputSezioneLavoratore SezioneLavoratore
    {
        get
        {
            return this.sezioneLavoratoreField;
        }
        set
        {
            this.sezioneLavoratoreField = value;
        }
    }

    /// <remarks/>
    public CreaComunicazione_InputSezioneRapportoLavoro SezioneRapportoLavoro
    {
        get
        {
            return this.sezioneRapportoLavoroField;
        }
        set
        {
            this.sezioneRapportoLavoroField = value;
        }
    }

    /// <remarks/>
    public CreaComunicazione_InputSezioneAccordoSmartWorking SezioneAccordoSmartWorking
    {
        get
        {
            return this.sezioneAccordoSmartWorkingField;
        }
        set
        {
            this.sezioneAccordoSmartWorkingField = value;
        }
    }

    /// <remarks/>
    public CreaComunicazione_InputSezioneSoggettoAbilitato SezioneSoggettoAbilitato
    {
        get
        {
            return this.sezioneSoggettoAbilitatoField;
        }
        set
        {
            this.sezioneSoggettoAbilitatoField = value;
        }
    }

    /// <remarks/>
    public string CodTipologiaComunicazione
    {
        get
        {
            return this.codTipologiaComunicazioneField;
        }
        set
        {
            this.codTipologiaComunicazioneField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
public partial class CreaComunicazione_InputSezioneDatoreLavoro
{

    private string codiceFiscaleDatoreLavoroField;

    private string denominazioneDatoreLavoroField;

    /// <remarks/>
    public string CodiceFiscaleDatoreLavoro
    {
        get
        {
            return this.codiceFiscaleDatoreLavoroField;
        }
        set
        {
            this.codiceFiscaleDatoreLavoroField = value;
        }
    }

    /// <remarks/>
    public string DenominazioneDatoreLavoro
    {
        get
        {
            return this.denominazioneDatoreLavoroField;
        }
        set
        {
            this.denominazioneDatoreLavoroField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
public partial class CreaComunicazione_InputSezioneLavoratore
{

    private string codiceFiscaleLavoratoreField;

    private string nomeLavoratoreField;

    private string cognomeLavoratoreField;

    private System.DateTime dataNascitaLavoratoreField;

    private string codComuneNascitaLavoratoreField;

    /// <remarks/>
    public string CodiceFiscaleLavoratore
    {
        get
        {
            return this.codiceFiscaleLavoratoreField;
        }
        set
        {
            this.codiceFiscaleLavoratoreField = value;
        }
    }

    /// <remarks/>
    public string NomeLavoratore
    {
        get
        {
            return this.nomeLavoratoreField;
        }
        set
        {
            this.nomeLavoratoreField = value;
        }
    }

    /// <remarks/>
    public string CognomeLavoratore
    {
        get
        {
            return this.cognomeLavoratoreField;
        }
        set
        {
            this.cognomeLavoratoreField = value;
        }
    }

    /// <remarks/>
    public System.DateTime DataNascitaLavoratore
    {
        get
        {
            return this.dataNascitaLavoratoreField;
        }
        set
        {
            this.dataNascitaLavoratoreField = value;
        }
    }

    /// <remarks/>
    public string CodComuneNascitaLavoratore
    {
        get
        {
            return this.codComuneNascitaLavoratoreField;
        }
        set
        {
            this.codComuneNascitaLavoratoreField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
public partial class CreaComunicazione_InputSezioneRapportoLavoro
{

    private System.DateTime dataInizioRapportoLavoroField;

    private string codTipologiaRapportoLavoroField;

    private string posizioneINAILField;

    private string tariffaINAILField;

    /// <remarks/>
    public System.DateTime DataInizioRapportoLavoro
    {
        get
        {
            return this.dataInizioRapportoLavoroField;
        }
        set
        {
            this.dataInizioRapportoLavoroField = value;
        }
    }

    /// <remarks/>
    public string CodTipologiaRapportoLavoro
    {
        get
        {
            return this.codTipologiaRapportoLavoroField;
        }
        set
        {
            this.codTipologiaRapportoLavoroField = value;
        }
    }

    /// <remarks/>
    public string PosizioneINAIL
    {
        get
        {
            return this.posizioneINAILField;
        }
        set
        {
            this.posizioneINAILField = value;
        }
    }

    /// <remarks/>
    public string TariffaINAIL
    {
        get
        {
            return this.tariffaINAILField;
        }
        set
        {
            this.tariffaINAILField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
public partial class CreaComunicazione_InputSezioneAccordoSmartWorking
{

    private System.DateTime dataSottoscrizioneAccordoField;

    private System.DateTime dataInizioPeriodoField;

    private System.DateTime dataFinePeriodoField;

    private string tipologiaDurataPeriodoField;

    private string streamPDFField;

    /// <remarks/>
    public System.DateTime DataSottoscrizioneAccordo
    {
        get
        {
            return this.dataSottoscrizioneAccordoField;
        }
        set
        {
            this.dataSottoscrizioneAccordoField = value;
        }
    }

    /// <remarks/>
    public System.DateTime DataInizioPeriodo
    {
        get
        {
            return this.dataInizioPeriodoField;
        }
        set
        {
            this.dataInizioPeriodoField = value;
        }
    }

    /// <remarks/>
    public System.DateTime DataFinePeriodo
    {
        get
        {
            return this.dataFinePeriodoField;
        }
        set
        {
            this.dataFinePeriodoField = value;
        }
    }

    /// <remarks/>
    public string TipologiaDurataPeriodo
    {
        get
        {
            return this.tipologiaDurataPeriodoField;
        }
        set
        {
            this.tipologiaDurataPeriodoField = value;
        }
    }

    /// <remarks/>
    public string StreamPDF
    {
        get
        {
            return this.streamPDFField;
        }
        set
        {
            this.streamPDFField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
public partial class CreaComunicazione_InputSezioneSoggettoAbilitato
{

    private string codTipologiaSoggettoAbilitatoField;

    private string codiceFiscaleSoggettoAbilitatoField;

    /// <remarks/>
    public string CodTipologiaSoggettoAbilitato
    {
        get
        {
            return this.codTipologiaSoggettoAbilitatoField;
        }
        set
        {
            this.codTipologiaSoggettoAbilitatoField = value;
        }
    }

    /// <remarks/>
    public string CodiceFiscaleSoggettoAbilitato
    {
        get
        {
            return this.codiceFiscaleSoggettoAbilitatoField;
        }
        set
        {
            this.codiceFiscaleSoggettoAbilitatoField = value;
        }
    }
}

