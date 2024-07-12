
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://mlps.gov.it/DataModels/InformationDelivery/SmartWorking/1.0", IsNullable = false)]
public partial class AnnullaComunicazione_Input
{

    private string codiceComunicazioneField;

    private AnnullaComunicazione_InputSezioneSoggettoAbilitato sezioneSoggettoAbilitatoField;

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
    public AnnullaComunicazione_InputSezioneSoggettoAbilitato SezioneSoggettoAbilitato
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
public partial class AnnullaComunicazione_InputSezioneSoggettoAbilitato
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

