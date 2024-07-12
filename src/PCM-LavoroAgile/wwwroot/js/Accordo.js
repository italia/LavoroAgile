function InitializeAccordo(endpointUrlGetStrutture, endpointUrlAddAttivita, userEmail) {
    //Inizializzazione date picker scelta singola
    $('#DataDiNascita, #DataFineAccordoPrecedente, #DataPresaServizio, #DataFineUtc').datepicker({
        changeYear: true,
        showButtonPanel: true,
        yearRange: '1900:2100',
    });

    // Nel caso in cui si tratti di un accordo in rinnovo, la data di inizio non
    // può essere precedente a quella di fine dell'accordo precedente. In questo caso
    // sarà popolato l'attributo data data-min-inzio-accodo, che sarà utilizzata per
    // impostare la data minima.
    if ($('#DataInizioUtc').data('data-min-inzio-accodo') == null || $('#DataInizioUtc').data('data-min-inzio-accodo') == '') {
        $('#DataInizioUtc').datepicker({
            changeYear: true,
            showButtonPanel: true,
            yearRange: '1900:2100',
            minDate: 0
        });
    }
    else {
        $('#DataInizioUtc').datepicker({
            changeYear: true,
            showButtonPanel: true,
            yearRange: '1900:2100',
            minDate: $('#DataInizioUtc').data('data-min-inzio-accodo')
        });
    }

    //Correzione bug che evita il refresh dopo le selezioni ed il ritorno errato al mese iniziale
    $.datepicker._selectDateOverload = $.datepicker._selectDate;
    $.datepicker._selectDate = function (id, dateStr) {
        var target = $(id);
        var inst = this._getInst(target[0]);
        inst.inline = true;
        $.datepicker._selectDateOverload(id, dateStr);
        inst.inline = false;
        if (target[0].multiDatesPicker != null) {
            target[0].multiDatesPicker.changed = false;
        } else {
            target.multiDatesPicker.changed = false;
        }
        this._updateDatepicker(inst);
    };

    //Quando viene selezionata una nuova data di inzio accordo e/o un nuovo valore per la durata mesi
    //vanno riaggiornati i valori maxDate e minDate di tutti i date picker conivolti
    $("#DataInizioUtc").on('change', function () {
        SetModalitaPianificazione();

        InitializeAttivita();
        //Reset campo pianificazione
        $('#PianificazioneDateAccordo').multiDatesPicker('resetDates', 'picked');
    });

    //Impostazione visualizzazione opzioni primo accordo
    //SetFieldsPrimoAccordo();

    //Impostazione visualizzazione opzioni modalita di pianificazione
    SetModalitaPianificazione();

    //Impostazione visualizzazione CapoIntermedio
    SetFieldsCapoIntermedio();

    //Inizializzazione sezione attività
    InitializeAttivita();

    //Valutazione selezione struttura di appartenenza e impostazione valori dei diversi responsabili
    $("#ddlStrutture").on('change', function () {
        $.ajax({
            type: 'GET',
            url: endpointUrlGetStrutture + '?idStruttura=' + $("#ddlStrutture").val(),
            success: function (struttura) {

                $("#StrutturaUfficioServizio").val(struttura.strutturaCompleta);
                $("#NumLivelliStruttura").val(struttura.numeroLivelli);
                $("#CapoStruttura").val(struttura.capoStruttura?.nomeCognome);
                $("#CapoStrutturaEmail").val(struttura.capoStruttura?.email);
                $("#CapoIntermedio").val(struttura.capoIntermedio?.nomeCognome);
                $("#CapoIntermedioEmail").val(struttura.capoIntermedio?.email);
                $("#ResponsabileAccordo").val(struttura.responsabileAccordo?.nomeCognome);
                $("#ResponsabileAccordoEmail").val(struttura.responsabileAccordo?.email);
                $("#ReferenteInterno").val(struttura.referenteInterno?.nomeCognome);
                $("#ReferenteInternoEmail").val(struttura.referenteInterno?.email);

                $("#DirigenteResponsabile").val(struttura.dirigenteResponsabile?.nomeCognome);
                $("#DirigenteResponsabile.Email").val(struttura.dirigenteResponsabile?.email);

                SetFieldsCapoIntermedio();

                // Aggiorna il valore della casella strutturaUfficioServizio allineandolo
                // con quello mostrato nella combo, in modo che se l'utente esce dalla modalità
                // di edit dell'anagrafica prima di salvare, vede il valore selezionato nella combo
                $('#strutturaUfficioServizio').val(struttura.strutturaCompleta);

                //Verifica del caso particolare, l'utente che opera è il dirigente responsabile del servizio, quindi il responsabile dell'accordo.
                //In questo caso il responsabile dell'accordo deve essere impostato come il primo responsabile utile,
                //va preseo il campo intermedio o il capo struttura a seconda dei livelli della struttua.
                if (userEmail != null && userEmail == $("#ResponsabileAccordoEmail").val()) {
                    switch ($("#NumLivelliStruttura").val()) {
                        case "3":
                            $("#ResponsabileAccordo").val(struttura.capoIntermedio?.nomeCognome);
                            $("#ResponsabileAccordo").val(struttura.capoIntermedio?.email);
                            break;
                        case "2":
                            $("#ResponsabileAccordo").val(struttura.capoStruttura?.nomeCognome);
                            $("#ResponsabileAccordoEmail").val(struttura.capoStruttura?.email);
                            break;
                    }
                }
            }
        });
    });

    //Valutazione modalità di pianificazione
    $("#ModalitaPianificazione").on('change', function () {
        SetModalitaPianificazione();
    });

    ////Valutazione opzione primo accordo
    //$('#PrimoAccordo').on('change', function () {
    //    SetFieldsPrimoAccordo();
    //});

    //Aggiunta Atività Accordo
    $("#addAttivita").on("click", function () {
        // Click sul pulsante 'Aggiungi attività'

        // Inizializza un oggetto AttivitaAccordoViewModel
        var attivitaAccordoViewModel = GetAttivitaAccordoViewModel();

        // Invoca endpoint per costuzione partial view della attività.
        if (verifyRequiredFileds(attivitaAccordoViewModel)) {
            $.ajax({
                async: true,
                data: attivitaAccordoViewModel,
                cache: false,
                type: "POST",
                url: endpointUrlAddAttivita,
                success: function (data) {
                    // Aggiunge riga alla tabella, sbianca i campi e resetta l'elemento correlato.
                    $('#tableAttivita > tbody').append(data);

                    //Inizializzazione sezione attività
                    InitializeAttivita();

                    //Reset form attività
                    ResetAttivita();
                }
            });

            $("#validationListaAttivita").html("");
        }
        else {
            $("#validationListaAttivita").html("Tutti i campi dell'attività sono obbligatori");
        }

        return false;
    });

    //Eliminazione Attività Accordo
    $("#tableAttivita").on("click", ".ibtnDel", function (event) {
        // Click sul pulsante elimina attivita.

        // Elimina la riga.
        $(this).closest("tr").remove();

        // Ricalcola gli indici.
        recalculateId();

        return false;
    });

    //Gestione alert deroga accordo
    initAlertDeroga();
}

//Riporta la form di inserimento nuova attività allo stato di default
function ResetAttivita() {

    //Default TESTO
    $("#DivIndicatoreTesto").show();
    $("#DivIndicatoreNumeroAssoluto").hide();
    $("#DivIndicatorePercentuale").hide();
    $("#DivIndicatoreData").hide();

    $("#Attivita").val(null);
    $("#Risultati").val(null);
    $("#DenominazioneIndicatore").val(null);
    $("#TipologiaIndicatore").val("Testo").trigger('change');
    $("#OperatoreLogicoIndicatoreTesto").val("uguale a").trigger('change');
    $("#TestoTarget").val(null);
}

//Inizializzazione form inserimento attività e sezioni di eventuali attività presenti
function InitializeAttivita() {

    //Inizializza la form inserimento nuova attività
    var lastIndex = recalculateId();

    SetFieldsIndicatoreTagertAttivita();
    SetFieldsDaAOperatoreLogicoAttivita();

    $("#TipologiaIndicatore").on('change', function () {
        SetFieldsIndicatoreTagertAttivita();
        SetFieldsDaAOperatoreLogicoAttivita();
    });

    $("#OperatoreLogicoIndicatoreNumeroAssoluto, #OperatoreLogicoIndicatorePercentuale, #OperatoreLogicoIndicatoreData")
        .on('change', function () {
            SetFieldsDaAOperatoreLogicoAttivita();
        });

    $('#DataTarget').datepicker('destroy');
    $('#DataTarget').datepicker({
        changeYear: true,
        showButtonPanel: true,
        yearRange: '1900:2100',
        minDate: calcolaDataInizioAccordo()        
    });

    $('#DataDaTarget').datepicker('destroy');
    $('#DataDaTarget').datepicker({
        changeYear: true,
        showButtonPanel: true,
        yearRange: '1900:2100',
        minDate: calcolaDataInizioAccordo()
    });

    $('#DataATarget').datepicker('destroy');
    $('#DataATarget').datepicker({
        changeYear: true,
        showButtonPanel: true,
        yearRange: '1900:2100',
        minDate: calcolaDataInizioAccordo()
    });

    //Inzializza le sezioni di eventuali attività già presenti
    for (i = lastIndex; i >= 0; i--) {
        SetFieldsIndicatoreTagertAttivita(i);
        SetFieldsDaAOperatoreLogicoAttivita(i);

        $("#TipologiaIndicatore" + i).on('change', function () {
            SetFieldsIndicatoreTagertAttivita($(this).attr('id').substr(-1));
            SetFieldsDaAOperatoreLogicoAttivita($(this).attr('id').substr(-1));
        });

        $("#OperatoreLogicoIndicatoreNumeroAssoluto" + i)
            .on('change', function () {
                SetFieldsDaAOperatoreLogicoAttivita($(this).attr('id').substr(-1));
            });

        $("#OperatoreLogicoIndicatorePercentuale" + i)
            .on('change', function () {
                SetFieldsDaAOperatoreLogicoAttivita($(this).attr('id').substr(-1));
            });

        $("#OperatoreLogicoIndicatoreData" + i)
            .on('change', function () {
                SetFieldsDaAOperatoreLogicoAttivita($(this).attr('id').substr(-1));
            });

        $('#DataTarget' + i).datepicker('destroy');
        $('#DataTarget' + i).datepicker({
            changeYear: true,
            showButtonPanel: true,
            yearRange: '1900:2100',
            minDate: calcolaDataInizioAccordo()            
        });

        $('#DataDaTarget' + i).datepicker('destroy');
        $('#DataDaTarget' + i).datepicker({
            changeYear: true,
            showButtonPanel: true,
            yearRange: '1900:2100',
            minDate: calcolaDataInizioAccordo()
        });

        $('#DataATarget' + i).datepicker('destroy');
        $('#DataATarget' + i).datepicker({
            changeYear: true,
            showButtonPanel: true,
            yearRange: '1900:2100',
            minDate: calcolaDataInizioAccordo()
        });
    }
}

function recalculateId() {
    // Per consentire il corretto funzionamento del controller ASP.NET,
    // è necessario che non ci siano buchi fra gli elementi della lista.
    // Per questo motivo recupera tutti i "name" degli oggetti contenuti
    // nelle righe della tabella e ne cambia il nome in modo da rispettare
    // la sequenzialità.
    // I nomi degli elementi, rispettano il pattern:
    // ListaAttivita[i].{Index|Attivita|Risultati|DenominazioneIndicatore|TipologiaIndicatore ecc.}

    var index = 0;
    var rows = $("[name^='ListaAttivita[");
    rows.each(function (i) {

        // Separa il primo punto
        let [first, ...attribute] = $(this).attr('name').split('.');
        attribute = attribute.join('.');
        var newName = 'ListaAttivita[' + index + '].' + attribute;

        //Imposta gli attributi name e id dell'elemento
        $(this).attr('name', newName);
        $(this).attr('id', attribute + index);

        // Se l'elemento è Index, aggiorna il valore ed aggiorna il valore inserito nella prima colonna della tabella.
        if (attribute === 'Index') {
            $(this).val(index);
            $(this).closest('tr').find('.attivitaIndex').text(index + 1 + '.');
        }

        //verifica il campo hidden "IndexStop" che indica la fine della sezione
        if (attribute === 'IndexStop') {
            index++;
        }
    });

    //Ricalcolo gli id dei Div delle diverse attività
    recalculateIdDiv();

    return index - 1;
}

//Ricalcolo gli id dei Div delle diverse attività
function recalculateIdDiv() {
    //Ricalcolo DivIndicatoreTesto
    var index = 0;
    var items = $("[id^='DivIndicatoreTesto");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivIndicatoreTesto" + index);
            index++;
        }
    });

    //Ricalcolo DivIndicatoreNumeroAssoluto
    index = 0;
    items = $("[id^='DivIndicatoreNumeroAssoluto");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivIndicatoreNumeroAssoluto" + index);
            index++;
        }
    });

    //Ricalcolo DivIndicatorePercentuale
    index = 0;
    items = $("[id^='DivIndicatorePercentuale");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivIndicatorePercentuale" + index);
            index++;
        }
    });

    //Ricalcolo DivIndicatoreData
    index = 0;
    items = $("[id^='DivIndicatoreData");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivIndicatoreData" + index);
            index++;
        }
    });

    //Ricalcolo DivNumeroAssolutoTarget
    index = 0;
    items = $("[id^='DivNumeroAssolutoTarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivNumeroAssolutoTarget" + index);
            index++;
        }
    });

    //Ricalcolo DivNumeroAssolutoDaTarget
    index = 0;
    items = $("[id^='DivNumeroAssolutoDaTarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivNumeroAssolutoDaTarget" + index);
            index++;
        }
    });

    //Ricalcolo DivNumeroAssolutoATarget
    index = 0;
    items = $("[id^='DivNumeroAssolutoATarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivNumeroAssolutoATarget" + index);
            index++;
        }
    });

    //Ricalcolo DivPercentualeTarget
    index = 0;
    items = $("[id^='DivPercentualeTarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivPercentualeTarget" + index);
            index++;
        }
    });

    //Ricalcolo DivPercentualeDaTarget
    index = 0;
    items = $("[id^='DivPercentualeDaTarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivPercentualeDaTarget" + index);
            index++;
        }
    });

    //Ricalcolo DivPercentualeATarget
    index = 0;
    items = $("[id^='DivPercentualeATarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivPercentualeATarget" + index);
            index++;
        }
    });

    //Ricalcolo DivDataTarget
    index = 0;
    items = $("[id^='DivDataTarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivDataTarget" + index);
            index++;
        }
    });
    //Ricalcolo DivDataDaTarget
    index = 0;
    items = $("[id^='DivDataDaTarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivDataDaTarget" + index);
            index++;
        }
    });
    //Ricalcolo DivDataATarget
    index = 0;
    items = $("[id^='DivDataATarget");
    items.each(function (i) {
        if (i > 0) {
            $(this).attr('id', "DivDataATarget" + index);
            index++;
        }
    });
}

////Impostazione visualizzazione opzioni primo accordo
//function SetFieldsPrimoAccordo() {
//    if ($('#PrimoAccordo').is(':checked')) {
//        $('#divDataFineAccordoPrecedente').hide();
//        $('#divValutazioneEsitiAccordoPrecedente').hide();
//    }
//    else {
//        $('#divDataFineAccordoPrecedente').show();
//        $('#divValutazioneEsitiAccordoPrecedente').show();
//    }
//}

//Impostazione visualizzazione opzioni modalità di pianificazione
function SetModalitaPianificazione() {
    if ($('#ModalitaPianificazione').val() == "Eccezionale") {
        $('#divPianificazioneDateAccordo').show();
        $('#divPianificazioneGiorniAccordo').hide();

        //Inizializzazione date picker scelta multipla
        $('#PianificazioneDateAccordo').multiDatesPicker('destroy');
        $('#PianificazioneDateAccordo').multiDatesPicker({
            changeYear: false,
            showButtonPanel: true
            //minDate: calcolaDataInizioAccordo()            
        });
    }
    else {
        $('#divPianificazioneDateAccordo').hide();
        $('#divPianificazioneGiorniAccordo').show();
        $('#PianificazioneDateAccordo').multiDatesPicker('resetDates', 'picked');
    }
}

//Impostazione visualizzazione campi Capo Intermedio
function SetFieldsCapoIntermedio() {
    if ($("#NumLivelliStruttura").val() != "" && $("#NumLivelliStruttura").val() < 3) {
        $("#divCapoIntermedio").hide();
        $("#divEmailCapoIntermedio").hide();
    }
    else {
        $("#divCapoIntermedio").show();
        $("#divEmailCapoIntermedio").show();
    }
}

//Impostazione visualizzazione campi Indicatore e Target Attività
//Il parametro lastIndex indica l'ultima attività eventualmente presente per un accordo
//Nel caso sia null, vuol dire che le operazioni vengono effettuate sulle sezione della form di inserimento
function SetFieldsIndicatoreTagertAttivita(lastIndex) {

    if (lastIndex == null) {
        lastIndex = '';
    }

    //Default TESTO
    $("#DivIndicatoreTesto" + lastIndex).show();
    $("#DivIndicatoreNumeroAssoluto" + lastIndex).hide();
    $("#DivIndicatorePercentuale" + lastIndex).hide();
    $("#DivIndicatoreData" + lastIndex).hide();

    if ($("#TipologiaIndicatore" + lastIndex).val() == "NumeroAssoluto") {
        $("#DivIndicatoreTesto" + lastIndex).hide();
        $("#DivIndicatoreNumeroAssoluto" + lastIndex).show();
        $("#DivIndicatorePercentuale" + lastIndex).hide();
        $("#DivIndicatoreData" + lastIndex).hide();
    }
    if ($("#TipologiaIndicatore" + lastIndex).val() == "Percentuale") {
        $("#DivIndicatoreTesto" + lastIndex).hide();
        $("#DivIndicatoreNumeroAssoluto" + lastIndex).hide();
        $("#DivIndicatorePercentuale" + lastIndex).show();
        $("#DivIndicatoreData" + lastIndex).hide();
    }
    if ($("#TipologiaIndicatore" + lastIndex).val() == "Data") {
        $("#DivIndicatoreTesto" + lastIndex).hide();
        $("#DivIndicatoreNumeroAssoluto" + lastIndex).hide();
        $("#DivIndicatorePercentuale" + lastIndex).hide();
        $("#DivIndicatoreData" + lastIndex).show();
    }
}

//Impostazione visualizzazione campi Da A operatore logico Attivita
//Il parametro lastIndex indica l'ultima attività eventualmente presente per un accordo
//Nel caso sia null, vuol dire che le operazioni vengono effettuate sulle sezione della form di inserimento
function SetFieldsDaAOperatoreLogicoAttivita(lastIndex) {
    if (lastIndex == null) {
        lastIndex = '';
    }

    TipologiaIndicatore = $("#TipologiaIndicatore" + lastIndex).val();

    if ($("#TipologiaIndicatore" + lastIndex).val() == TipologiaIndicatore &&
        $("#OperatoreLogicoIndicatore" + TipologiaIndicatore + lastIndex).val() == "compreso tra") {
        $("#Div" + TipologiaIndicatore + "Target" + lastIndex).hide();
        $("#Div" + TipologiaIndicatore + "DaTarget" + lastIndex).show();
        $("#Div" + TipologiaIndicatore + "ATarget" + lastIndex).show();
    }
    else {
        $("#Div" + TipologiaIndicatore + "Target" + lastIndex).show();
        $("#Div" + TipologiaIndicatore + "DaTarget" + lastIndex).hide();
        $("#Div" + TipologiaIndicatore + "ATarget" + lastIndex).hide();
    }
}

//Costruisce l'oggetto AttivitaAccordoViewModel in base alla tipologia di indicatore scelta
function GetAttivitaAccordoViewModel() {
    var attivitaAccordoViewModel;

    if ($("#TipologiaIndicatore").val() == "Testo") {
        attivitaAccordoViewModel = {
            index: $('.attivita-item').length,
            attivita: $("#Attivita").val(),
            risultati: $("#Risultati").val(),
            denominazioneIndicatore: $("#DenominazioneIndicatore").val(),
            tipologiaIndicatore: $("#TipologiaIndicatore").val(),
            //testoIndicatore: $("#TestoIndicatore").val(),
            operatoreLogicoIndicatoreTesto: $("#OperatoreLogicoIndicatoreTesto").val(),
            testoTarget: $("#TestoTarget").val()
        };
    }
    if ($("#TipologiaIndicatore").val() == "NumeroAssoluto") {
        attivitaAccordoViewModel = {
            index: $('.attivita-item').length,
            attivita: $("#Attivita").val(),
            risultati: $("#Risultati").val(),
            denominazioneIndicatore: $("#DenominazioneIndicatore").val(),
            tipologiaIndicatore: $("#TipologiaIndicatore").val(),
            //numeroAssolutoIndicatore: $("#NumeroAssolutoIndicatore").val(),
            operatoreLogicoIndicatoreNumeroAssoluto: $("#OperatoreLogicoIndicatoreNumeroAssoluto").val(),
            numeroAssolutoTarget: $("#NumeroAssolutoTarget").val()
        };
    }
    if ($("#TipologiaIndicatore").val() == "NumeroAssoluto" &&
        $("#OperatoreLogicoIndicatoreNumeroAssoluto").val() == "compreso tra") {
        attivitaAccordoViewModel = {
            index: $('.attivita-item').length,
            attivita: $("#Attivita").val(),
            risultati: $("#Risultati").val(),
            denominazioneIndicatore: $("#DenominazioneIndicatore").val(),
            tipologiaIndicatore: $("#TipologiaIndicatore").val(),
            //numeroAssolutoIndicatore: $("#NumeroAssolutoIndicatore").val(),
            operatoreLogicoIndicatoreNumeroAssoluto: $("#OperatoreLogicoIndicatoreNumeroAssoluto").val(),
            numeroAssolutoDaTarget: $("#NumeroAssolutoDaTarget").val(),
            numeroAssolutoATarget: $("#NumeroAssolutoATarget").val()
        };
    }
    if ($("#TipologiaIndicatore").val() == "Percentuale") {
        attivitaAccordoViewModel = {
            index: $('.attivita-item').length,
            attivita: $("#Attivita").val(),
            risultati: $("#Risultati").val(),
            denominazioneIndicatore: $("#DenominazioneIndicatore").val(),
            tipologiaIndicatore: $("#TipologiaIndicatore").val(),
            percentualeIndicatoreDenominazioneNumeratore: $("#PercentualeIndicatoreDenominazioneNumeratore").val(),
            percentualeIndicatoreDenominazioneDenominatore: $("#PercentualeIndicatoreDenominazioneDenominatore").val(),
            operatoreLogicoIndicatorePercentuale: $("#OperatoreLogicoIndicatorePercentuale").val(),
            percentualeTarget: $("#PercentualeTarget").val()
        };
    }
    if ($("#TipologiaIndicatore").val() == "Percentuale" &&
        $("#OperatoreLogicoIndicatorePercentuale").val() == "compreso tra") {
        attivitaAccordoViewModel = {
            index: $('.attivita-item').length,
            attivita: $("#Attivita").val(),
            risultati: $("#Risultati").val(),
            denominazioneIndicatore: $("#DenominazioneIndicatore").val(),
            tipologiaIndicatore: $("#TipologiaIndicatore").val(),
            //percentualeIndicatore: $("#PercentualeIndicatore").val(),
            percentualeIndicatoreDenominazioneNumeratore: $("#PercentualeIndicatoreDenominazioneNumeratore").val(),
            percentualeIndicatoreDenominazioneDenominatore: $("#PercentualeIndicatoreDenominazioneDenominatore").val(),
            operatoreLogicoIndicatorePercentuale: $("#OperatoreLogicoIndicatorePercentuale").val(),
            percentualeDaTarget: $("#PercentualeDaTarget").val(),
            percentualeATarget: $("#PercentualeATarget").val()
        };
    }
    if ($("#TipologiaIndicatore").val() == "Data") {
        attivitaAccordoViewModel = {
            index: $('.attivita-item').length,
            attivita: $("#Attivita").val(),
            risultati: $("#Risultati").val(),
            denominazioneIndicatore: $("#DenominazioneIndicatore").val(),
            tipologiaIndicatore: $("#TipologiaIndicatore").val(),
            //dataIndicatore: $("#DataIndicatore").val(),
            operatoreLogicoIndicatoreData: $("#OperatoreLogicoIndicatoreData").val(),
            dataTarget: $("#DataTarget").val()
        };
    }
    if ($("#TipologiaIndicatore").val() == "Data" &&
        $("#OperatoreLogicoIndicatoreData").val() == "compreso tra") {
        attivitaAccordoViewModel = {
            index: $('.attivita-item').length,
            attivita: $("#Attivita").val(),
            risultati: $("#Risultati").val(),
            denominazioneIndicatore: $("#DenominazioneIndicatore").val(),
            tipologiaIndicatore: $("#TipologiaIndicatore").val(),
            //dataIndicatore: $("#DataIndicatore").val(),
            operatoreLogicoIndicatoreData: $("#OperatoreLogicoIndicatoreData").val(),
            dataDaTarget: $("#DataDaTarget").val(),
            dataATarget: $("#DataATarget").val()
        };
    }

    return attivitaAccordoViewModel;
}

/** Inizializza i pulsanti di approvazione, rifiuto, richiesta integrazione e recedi */
function initApprovaRifiutaIntegraRecediButtons() {

    $('#approva-input, #rifiuta-input, #integra-input, #recedi-input').click(function (a) {
        openDialogApprovaRifiutaIntegraRecedi($(a.target).data("action"), $(a.target).data('data-fine'));
    });
}

/**
 * Inizializza ed apre la dialog per approvazione/rifiuto/richiesta integrazione/recesso
 * @param {string} action Azione da eseguire
 * @param {Date} endDate Data fine accordo (necessaria nel caso di recesso).
 */
function openDialogApprovaRifiutaIntegraRecedi(action, endDate) {
    // In base alla action, vanno abilitati i pulsanti.
    $('#approva-dialog-input, #rifiuta-dialog-input, #integra-dialog-input, #data-recesso-div, #impossibile-recedere-alert-div, #div-approva-rifiuta-integra-modal, #div-recesso-modal').attr('hidden', true);

    // Aggiorna il titolo della dialog.
    if (action != 'recedi') {
        $('#title-div').text(action);
    }
    else {
        $('#title-div').hide();
    }

    //Nel caso in cui l'azione sia Recesso, l'endpoint da interrogare è Recedi, in tutti gli altri casi è ApprovaRifiutaIntegra
    //Lasciata commentata qualora dovesse ritornare utile per evenuali nuove azioni da gestire
    //let formRef = $('#approval-form');
    //if (action == 'recedi') {
    //    formRef.attr('action', formRef.attr('action').replace('approvarifiutaintegra', 'recedi'));
    //} else {
    //    formRef.attr('action', formRef.attr('action').replace('recedi', 'approvarifiutaintegra'));
    //}

    switch (action) {
        case 'approva':
            $('#div-approva-rifiuta-integra-modal').attr('hidden', false);
            $('#approva-dialog-input').attr('hidden', false);
            break;
        case 'rifiuta':
            $('#div-approva-rifiuta-integra-modal').attr('hidden', false);
            $('#rifiuta-dialog-input').attr('hidden', false);
            break;
        case 'integra':
            $('#div-approva-rifiuta-integra-modal').attr('hidden', false);
            $('#integra-dialog-input').attr('hidden', false);
            break;
        case 'recedi':
            $('#div-recesso-modal').attr('hidden', false);
            
            // Calcolo data minima per effettuare recesso
            let minDateRecesso = new Date();
            minDateRecesso.setDate(minDateRecesso.getDate() + 30);

            // Mostra la casella di selezione della data di recesso
            $('#data-recesso-div').attr('hidden', false);

            // Attivazione del date picker e controllo delle date.
            $('#data-recesso-input').datepicker({
                inputFormat: ["dd/MM/yyyy"],
                outputFormat: 'dd/MM/yyyy',
                minDate: minDateRecesso,
                maxDate: new Date(endDate),
                changeYear: true,
                showButtonPanel: true,
                inline: true,

            });

            // Attivazione del date picker e controllo delle date per la data recesso giustificato motivo
            $('#data-recesso-giustificato-motivo').datepicker({
                inputFormat: ["dd/MM/yyyy"],
                outputFormat: 'dd/MM/yyyy',
                minDate: new Date(),
                maxDate: new Date(endDate),
                changeYear: true,
                showButtonPanel: true,
                inline: true,

            });

            // Imposta le date dei campi data
            $('#data-recesso-input').datepicker('setDate', minDateRecesso);
            $('#data-recesso-giustificato-motivo').datepicker('setDate', new Date());

            break;
        default:
    }

    $('#approva-rifiuta-integra-modal').modal('show');
}

function initDiffButton(endpoint) {
    $('#differenza-accordi-input').click(function (elem) {

        // Nasconde la sezione "Nessuna modifica" e svuota la tabella
        $('#no-diff').attr('hidden', true);
        $('#diff-table > table').empty();

        let elemRef = $(elem.target);
        $.ajax({
            type: 'GET',
            url: endpoint + `?accordo1=${elemRef.data('id')}&accordo2=${elemRef.data('parent-id')}`,
            success: function (differences) {

                // Se non ci sono differenze da mostrare, mostra l'alert "Nessuna modifica"
                if (differences == null || differences.length === 0) {
                    $('#no-diff').attr('hidden', false);
                } else {

                    // Altrimenti costruisce la tabella con le differenze.
                    let diffTable = $('<table class="table table-bordered table-ellipsis">');
                    diffTable.append(`<thead><tr><th scope="col">Proprietà</th><th scope="col">Nuovo</th><th scope="col">Vecchio</th></tr></thead><tbody>`);

                    differences.forEach(function (value) {

                        // Se la proprietà è ListaAttivita, non deve essere mostrata.
                        if (value.memberPath === 'ListaAttivita') {
                            return;
                        }

                        // L'individuazione dell'etichetta avviene in base al nome dell'elemento
                        // cui si riferisce la modifica. Il name va pulito da Value postfisso,
                        // in modo da trovare i campi nullable e, nel caso in cui sia presente
                        // un [, significa che si tratta di un array e quindi la ricerca deve
                        // essere effettuata sul template (deve quindi essere presa in considerazione
                        // solo l'ultima parte del memberPath
                        let fieldName = value.memberPath.split('.Value').shift();
                        if (fieldName.includes('[')) {
                            fieldName = fieldName.split('.').slice(-1);
                        }
                        let elemRef = $(`[name='${fieldName}']`);

                        // Se valorizzata, la label viene prelevata dall'attributo aria-label
                        // altrimenti cerca la label più vicina 
                        let label = elemRef.attr('aria-label');
                        if (undefined == label) {
                            label = elemRef.siblings("label").text();
                        }

                        // Recupero del valore associato al campo (di default sono i valori
                        // presenti in value, ma nel caso in cui l'input sia una select, cerca
                        // il testo associato all'option con lo specifico valore; nel caso di 
                        // booleano trasforma in Selezionato/Non selezionato
                        let valueOld = cleanValue(value.value2), valueNew = cleanValue(value.value1);
                        if (elemRef.is('select')) {
                            valueOld = elemRef.find(`[value='${valueOld}']`).text();
                            valueNew = elemRef.find(`[value='${valueNew}']`).text();
                        }

                        // Conversione dell'eventuale booleano.
                        valueOld = getBoolValueAsString(valueOld);
                        valueNew = getBoolValueAsString(valueNew);

                        diffTable.append(`<tr><td>${label}</td><td>${valueNew}</td><td>${valueOld}</td></tr>`);
                    });

                    $('#diff-table').append(diffTable);

                }


            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                window.location = "/home/error";

            }
        });

    });
}

/**
 * Pulisce un valore.
 * @param {any} value Valore da pulire.
 */
function cleanValue(value) {
    let cleanedValue = value;

    let dateTimeRegex = /^(\d{2})\/(\d{2})\/(\d{4}) 00:00:00$/;

    // Se il campo è una data, elimina la parte temporale.
    if (cleanedValue && cleanedValue.match(dateTimeRegex)) {
        cleanedValue = cleanedValue.split(' ').shift();
    }

    return cleanedValue;
}

function verifyRequiredFileds(attivitaAccordoViewModel) {
    var result = true;

    if (attivitaAccordoViewModel.attivita != null && attivitaAccordoViewModel.attivita != "" &&
        attivitaAccordoViewModel.risultati != null && attivitaAccordoViewModel.risultati != "" &&
        attivitaAccordoViewModel.denominazioneIndicatore != null && attivitaAccordoViewModel.denominazioneIndicatore != "") {

        switch (attivitaAccordoViewModel.tipologiaIndicatore.toUpperCase()) {
            case 'TESTO':
                if (attivitaAccordoViewModel.testoTarget == null || attivitaAccordoViewModel.testoTarget == "") {
                    result = false;
                }
                break;
            case 'PERCENTUALE':
                if (attivitaAccordoViewModel.percentualeIndicatoreDenominazioneNumeratore != null &&
                    attivitaAccordoViewModel.percentualeIndicatoreDenominazioneNumeratore != "" &&
                    attivitaAccordoViewModel.percentualeIndicatoreDenominazioneDenominatore != null &&
                    attivitaAccordoViewModel.percentualeIndicatoreDenominazioneDenominatore != "") {

                    if (attivitaAccordoViewModel.operatoreLogicoIndicatorePercentuale == "compreso tra") {
                        if (attivitaAccordoViewModel.percentualeDaTarget == null || attivitaAccordoViewModel.percentualeDaTarget == "" ||
                            attivitaAccordoViewModel.percentualeATarget == null || attivitaAccordoViewModel.percentualeATarget == "") {
                            result = false;
                        }
                    }
                    else {
                        if (attivitaAccordoViewModel.percentualeTarget == null || attivitaAccordoViewModel.percentualeTarget == "") {
                            result = false;
                        }
                    }
                }
                else {
                    result = false;
                }
                break;
            case 'DATA':
                if (attivitaAccordoViewModel.operatoreLogicoIndicatoreData == "compreso tra") {
                    if (attivitaAccordoViewModel.dataDaTarget == null || attivitaAccordoViewModel.dataDaTarget == "" ||
                        attivitaAccordoViewModel.dataATarget == null || attivitaAccordoViewModel.dataATarget == "") {
                            result = false;
                    }
                }
                else {
                    if (attivitaAccordoViewModel.dataTarget == null || attivitaAccordoViewModel.dataTarget == "") {
                        result = false;
                    }
                }
                break;
            case 'NUMEROASSOLUTO':
                if (attivitaAccordoViewModel.operatoreLogicoIndicatoreNumeroAssoluto == "compreso tra") {
                    if (attivitaAccordoViewModel.numeroAssolutoDaTarget == null || attivitaAccordoViewModel.numeroAssolutoDaTarget == "" ||
                        attivitaAccordoViewModel.numeroAssolutoATarget == null || attivitaAccordoViewModel.numeroAssolutoATarget == "") {
                            result = false;
                    }
                }
                else {
                    if (attivitaAccordoViewModel.numeroAssolutoTarget == null || attivitaAccordoViewModel.numeroAssolutoTarget == "") {
                        result = false;
                    }
                }
                break;
        }
    }
    else {
        result = false;
    }

    return result;
}

/** Inizializza i pulsanti di maggiori dettagli */
function initMoreDetailsButtons() {
    $('#moreDetailsPianificazioneDate-input, #moreDetailsReperibilita-input, #moreDetailsValutazione-input').click(function (a) {
        var action = $(a.target).data("action");
        $('#divModalBodyPianificazioneDate').hide();
        $('#divModalBodyFasceContattabilita').hide();
        $('#divModalBodyValutazione').hide();

        switch (action) {
            case 'dettagliPianificazioneDate':
                $('#divModalBodyPianificazioneDate').show();
                break;
            case 'dettagliFasceContattabilita':
                $('#divModalBodyFasceContattabilita').show();
                break;
            case 'dettagliValutazione':
                $('#divModalBodyValutazione').show();
                break;
        }
        $('#moreDetails-modal').modal('show');
    });
}

//Gestione alert deroga accordo
function initAlertDeroga() {
    if ($('#DerogaPianificazioneDate').is(":checked")) {
        $('#alertDeroga').show();
    }
    else {
        $('#alertDeroga').hide();
    }
}