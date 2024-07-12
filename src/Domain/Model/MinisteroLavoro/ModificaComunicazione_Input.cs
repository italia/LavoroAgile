
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0", IsNullable = false)]
public partial class ModificaComunicazione_Input
{

    private string codiceComunicazioneField;

    private ModificaComunicazione_InputSezioneRapportoLavoro sezioneRapportoLavoroField;

    private ModificaComunicazione_InputSezioneAccordoSmartWorking sezioneAccordoSmartWorkingField;

    private ModificaComunicazione_InputSezioneSoggettoAbilitato sezioneSoggettoAbilitatoField;

    private string codTipologiaComunicazioneField;

    /// <remarks/>
    public string CodiceComunicazione
    {
        get
        {
            return this.codiceComunicazioneField;
        }
        set
        {
            this.codiceComunicazioneField = value;
        }
    }

    /// <remarks/>
    public ModificaComunicazione_InputSezioneRapportoLavoro SezioneRapportoLavoro
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
    public ModificaComunicazione_InputSezioneAccordoSmartWorking SezioneAccordoSmartWorking
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
    public ModificaComunicazione_InputSezioneSoggettoAbilitato SezioneSoggettoAbilitato
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
public partial class ModificaComunicazione_InputSezioneRapportoLavoro
{

    private string codTipologiaRapportoLavoroField;

    private string posizioneINAILField;

    private string tariffaINAILField;

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
public partial class ModificaComunicazione_InputSezioneAccordoSmartWorking
{

    private System.DateTime dataSottoscrizioneAccordoField;

    private System.DateTime dataInizioPeriodoField;

    private System.DateTime dataFinePeriodoField;

    private string tipologiaDurataPeriodoField;

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
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
public partial class ModificaComunicazione_InputSezioneSoggettoAbilitato
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