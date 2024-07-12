/**
 * Libreria per la gestione della dialog delle azioni massive
 */

/**
 * Rappresenta un accordo selezionato.
 * @param {string} id Identificativo dell'accordo selezionato
 * @param {string} codice Codice dell'accordo
 * @param {string} proponente Nome e cognome del proponente
 * @param {string} dataInizio Data inizio dell'accordo.
 * @param {string} dataFine Data fine dell'accordo.
 * @param {string} codiceStato Codice dello stato dell'elemento.
 * @param {boolean} rinnovo Indica se si tratta di un rinnovo.
 * @param {boolean} valutazionePositivaAccordoPrecedente Indica una valutazione positiva dell'accordo precedente.
 * @param {boolean} isResponsabileAccordo Responsabile accordo.
 */
function SelectedAccordo(id, codice, proponente, dataInizio, dataFine, codiceStato, rinnovo, valutazionePositivaAccordoPrecedente, isResponsabileAccordo) {
    this.id = id;
    this.codice = codice;
    this.proponente = proponente;
    this.dataInizio = dataInizio;
    this.dataFine = dataFine;
    this.codiceStato = codiceStato;
    this.rinnovo = rinnovo;
    this.valutazionePositivaAccordoPrecedente = valutazionePositivaAccordoPrecedente;
    this.isResponsabileAccordo = isResponsabileAccordo;
}

// Oggetto con l'insieme degli elementi selezionati.
// La chiave dell'oggetto corrisponde all'id dell'accordo, il valore ad una istanza di SelectedAccordo.
var selectedItems = {};

// Riferimento al pulsante delle azioni massive ed al suo popover
var azioniMassiveButton, azioniMassivePopover;

// Ruolo con cui sta operando l'utente
var userRole;

/**
 * Inizializza la gestione degli eventi di selezione di elementi nell'elenco dei risultati di ricerca.
 * @param {string} sectionId Identificativo del contenitore in cui sono contenuti i risultati.
 * @param {string} role Ruolo con cui sta operando l'utente
 * @param {string} openDialogButtonId Identificativo del pulsante da cliccare per aprire la dialog.
 * @param {string} openDialogPopoverId Identificativo del popover del bottone di apertua dialog.
 */
function initSelectionManager(sectionId, role, openDialogButtonId, openDialogPopoverId) {

    // Salva il ruolo, necessario per invocare gli endpoint delle azioni massive.
    userRole = role;

    // Inizializza riferimento al popover
    azioniMassivePopover = $(openDialogPopoverId);

    // Ad ogni change su un elemento del contenitore (al momento l'unico in grado di scatenarlo è la
    // checkbox), si accerta che l'elemento che ha scatenato l'evento è la checkbox e in caso positivo
    // aggiorna lo stato degli elementi selezionati.
    $(sectionId).change(function (htmlElem) {

        let converted = $(htmlElem.target);

        if (converted.is(':checkbox')) {

            if (converted.is(':checked')) {
                let nextDiv = converted.parents('.col-1').next();
                selectedItems[converted.attr('id')] = new SelectedAccordo(converted.attr('id'), nextDiv.find('#codice').text(), nextDiv.find('#proponente').text(), nextDiv.find('#data-inizio').text(), nextDiv.find('#data-fine').text(), nextDiv.find('#stato').data('codice-stato'), nextDiv.find('#is-rinnovo-input').val() != '', nextDiv.find('#rinnovabile-input').val() != '', converted.data('isresponsabileaccordo').toLowerCase());

            }
            else {
                delete selectedItems[converted.attr('id')];

            }

            // Verifica se gli elementi selezionati siano tutti nello stesso stato e che,
            // nel caso in cui siano stati selezionati dei rinnovi, questi siano tutti stati valutati
            // positivamente.
            let sameStato = true, statoPrimoElemento = Object.values(selectedItems), existsRinnovoNonRinnovabile = false;
            jQuery.each(selectedItems, function (name, value) {
                sameStato &= statoPrimoElemento.length > 0 && value.codiceStato == statoPrimoElemento[0].codiceStato;
                existsRinnovoNonRinnovabile |= value.rinnovo && !value.valutazionePositivaAccordoPrecedente;
            });

            // Il pulsante azioni, deve poter essere abilitato solo se gli stati ricadono in:
            // - Da approvare responsabile accordo
            let validStato = statoPrimoElemento.length > 0 && statoPrimoElemento[0].codiceStato == 'DaApprovareRA' && JSON.parse(statoPrimoElemento[0].isResponsabileAccordo);

            // Se lo stato del primo elemento è valido, non ci sono elementi selezionati o se quelli selezionati sono in stati diversi, 
            // esiste almeno un accordo con valutazione negativa, disabilita il pulsante delle azioni massive
            azioniMassiveButton.prop('disabled', !validStato || Object.keys(selectedItems).length === 0 || !sameStato || existsRinnovoNonRinnovabile);

            // In base allo stato del pulsante, adegua lo stile del pulsante per consentire il corretto
            // funzionamento del popover.
            if (azioniMassiveButton.prop('disabled')) {
                azioniMassiveButton.css('pointer-events', 'none');
            } else {
                azioniMassiveButton.css('pointer-events', 'all');
            }

            // Aggiornamento del popover del bottone di apertura dialog
            azioniMassivePopover.attr('data-content', "Esegui azioni");
            if (!validStato) {
                azioniMassivePopover.attr('data-content', "Selezionare accordi di cui si è il responsabile in uno stato valido (Da approvare Responsabile accordo)");
            }
            if (!sameStato) {
                azioniMassivePopover.attr('data-content', "Selezionare accordi nello stesso stato");

            }
            if (Object.keys(selectedItems).length === 0) {
                azioniMassivePopover.attr('data-content', "Selezionare almeno un accordo per attivare il pulsante");
            }
            if (existsRinnovoNonRinnovabile) {
                azioniMassivePopover.attr('data-content', "E' stato selezionato almeno un rinnovo di accordo non rinnovabile");

            }

        }

    });

    // Al click sul pulsante di apertura della dialog, inizializza la tabella e le azioni
    // e quindi apre la dialog stessa.
    azioniMassiveButton = $(openDialogButtonId);
    azioniMassiveButton.click(function () {

        let accordiSelezionati = $('#accordi-selezionati');

        // Svuota la tabella ed inserisce le informazioni sugli accordi selezionati.
        accordiSelezionati.empty();
        jQuery.each(selectedItems, function (name, value) {
            accordiSelezionati.append($(`<tr>
                                <td hidden>${value.id}</td>
                                <th scope="row">${value.codice}</th>
                                <td>${value.proponente}</td>
                                <td>${value.dataInizio} - ${value.dataFine}</td>
                            </tr>`));
        });

        // Inizializzazione dei pulsanti delle possibili azioni.
        initAzioni();

        // Mostra la dialog delle azioni massive.
        $('#azioni-massive').modal('show');
    });

    // Gestione click sul pulsante di sottoscrizione accordo
    $('#sottoscrivi-input').click(function () {

        let data = {
            'action': 'SottoscrittoRA'
        }
        executeAction('Sottoscrivi', data);
    });

    // Gestione click sul pulsante di approvazione accordo
    $('#approva-input').click(function () {

        // Preleva le ultime due lettere del codice dello stato del primo accordo
        let codRuolo = Object.values(selectedItems)[0].codiceStato.slice(-2);

        let data = {
            'action': `Approvato${codRuolo}`
        }
        executeAction('ApprovaRifiutaIntegra', data);

    });

    // Gestione click sul pulsante di rifiuto accordo
    $('#rifiuta-input').click(function () {

        // Preleva le ultime due lettere del codice dello stato del primo accordo
        let codRuolo = Object.values(selectedItems)[0].codiceStato.slice(-2);

        let data = {
            'action': `Rifiutato${codRuolo}`
        }

        executeAction('ApprovaRifiutaIntegra', data);

    });

    // Gestione click sul pulsante di richiesta modifica accordo
    $('#modifica-input').click(function () {
        let data = {
            'action': 'RichiestaModifiche'
        }
        executeAction('ApprovaRifiutaIntegra', data);

    });

}

/**
 * Inizializza i pulsanti delle azioni
 */
function initAzioni() {

    // Nasconde tutti i pulsanti
    $('#sottoscrivi-input, #approva-input, #rifiuta-input, #modifica-input').attr('hidden', true);
    
    let primoStato = Object.values(selectedItems)[0].codiceStato;

    if (primoStato == "DaSottoscrivereRA") {
        $('#sottoscrivi-input').removeAttr('hidden');
    }

    if (primoStato.startsWith("DaApprovare")) {
        $('#approva-input').removeAttr('hidden');
        $('#rifiuta-input').removeAttr('hidden');
        $('#modifica-input').removeAttr('hidden');
    }
}

/**
 * Svuota la collezione degli elementi selezionati.
 */
function clearSelectedItems() {
    selectedItems = {};
}

/**
 * Invoca l'endpoint responsabile dell'esecuzione dell'azione.
 * @param {any} action Azione da invocare.
 * @param {any} data Dati da inviare all'endpoint.
 */
function executeAction(action, data) {

    // Aggiunge ruolo, identificativi degli accordi selezionati e note all'oggetto da inviare
    // all'endpoint
    data.ids = Object.keys(selectedItems);
    data.role = userRole;
    data.note = $('#note').val();

    $.ajax({
        type: 'POST',
        url: `Approvals/${action}?role=${userRole}`,
        data: data,
        contentType: 'application/x-www-form-urlencoded',
        success: function () {

            // Chiude la dialog delle azioni massive ed aggiorna i risultati di ricerca (metodo gestito dalla
            // libreria delle ricerche).
            $('#azioni-massive').modal('hide');
            refineSearch();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            
            if (XMLHttpRequest.status == 403) {
                window.location = "/account/accessdenied";
            } else {
                window.location = "/home/error";

            }


        }
    });
}