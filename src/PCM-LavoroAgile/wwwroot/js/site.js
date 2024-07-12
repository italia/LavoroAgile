function DisplaySpinner() {
    $("body").addClass("submit-spinner-bg");

    setTimeout(function () {
    $(".submit-spinner").removeClass("hidden-spinner");
    }, 1);

    return true;
}

/**
 * Inizializza gli autocomplete di ricerca strutture e registra gli 
 * handler per la compilazione del campo responsabile accordo.
 * @param {string} autocompleteUrl Url del servizio di autocomplete da richiamare (è atteso come parametro per gestire in automatico l'allineamento con la definizione delle rotte di ASP.NET)
 */
function initStruttura(autocompleteUrl) {

    $('input[id^="StrutturaLiv"]').each(function () {
        $(this).autocomplete({
            minLength: 2,
            delay: 500,
            source: `${autocompleteUrl}?livello=${this.id}` //, new { livello = "Livello1" })',
        }).data("ui-autocomplete")._renderItem = function (ul, item) {
            let txt = String(item.value).replace(new RegExp(this.term, "gi"), "<b>$&</b>");
            return $("<li></li>")
                .data("ui-autocomplete-item", item)
                .append("<a>" + txt + "</a>")
                .appendTo(ul);
        };

    });

    //// Tutti i controlli che hanno l'attributo data-set-responsabile, devono scatenare
    //// la logica per l'impostazione del responsabile.
    //$('[data-set-responsabile],[data-set-responsabile="true"]').change(function () {

    //    let liv2 = $('#StrutturaLiv2'),
    //        liv3 = $('#StrutturaLiv3'),
    //        respName = $('#ResponsabileAccordo'),
    //        respMail = $('#EmailResponsabileAccordo');

    //    // Il valore da assegnare al campo del responsabile dell'accordo sono nome 
    //    // e mail del responsabile del livello in basso compilato.
    //    if (liv3.val()) {
    //        respName.val($('#DirigenteResponsabile').val());
    //        respMail.val($('#EmailDirigenteResponsabile').val());
    //    } else if (liv2.val()) {
    //        respName.val($('#CapoIntermedio').val());
    //        respMail.val($('#EmailCapoIntermedio').val());
    //    } else {
    //        respName.val($('#CapoStruttura').val());
    //        respMail.val($('#EmailCapoStruttura').val());
    //    }

    //});

};

/**
 * Consente di ricercare e prelevare parametri dalla url
 */
function getQueryParam(param, defaultValue = undefined) {
    location.search.substr(1)
        .split("&")
        .some(function (item) { // returns first occurence and stops
            return item.split("=")[0].toLowerCase() == param?.toLowerCase() && (defaultValue = item.split("=")[1], true)
        })
    return defaultValue
}

// Mostra le card appropriate per l'utente in funzione del ruolo ed aggiorna il link per l'apertura della pagina di ricerca accordi.
function ShowCardForRole(role, endpointUrl) {
    // Nasconde tutte le card attualmente visualizzate
    $('[data-show-if]').hide();

    // Mostra tutte le card relative al ruolo selezionato
    $('[data-show-if='+role+']').show();

    // Aggiorna la url del link goToListaAccordi
    var accordiUrl = endpointUrl + '?role=' + role
    $("#goToListaAccordi_" + role).attr("href", accordiUrl);
}

//Recupera le informazioni sulle cose da fare in funzione del ruolo e visualizza o meno il valore del badge sulla card di ricerca
function GetToDoForRole(role, endpointUrl) {
    $.ajax({
        type: 'GET',
        url: endpointUrl+'?role='+role,
        success: function (data) {
            if (data != null && data != 0) {
                $("#badge_" + role).text(data);
            }
        }
    });
}

//Recupera le informazioni sugli accordi da valutare e visualizza o meno il valore del badge sulla card di valutazione
//Viene fatto solo per il ruolo di responsabile accordo
function GetToDoValutazioni(role, endpointUrl) {
    $.ajax({
        type: 'GET',
        url: endpointUrl + '?role=' + role,
        success: function (data) {
            if (data != null && data != 0) {
                $("#badge_valutazioni_" + role).text(data);
            }
        }
    });
}