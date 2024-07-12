/**
 * Codice javascript per la gestione dell'infinite scroll nella pagina dei risultati di ricerca.
 * Dipende dalla libreria jquery.inview (https://github.com/protonet/jquery.inview)
 */

/**
 * Rappresenta l'input da inviare al metodo di ricerca
 * @param {any} role Ruolo con cui vengono richieste le informazioni.
 * @param {any} page Numero di pagina da recuperare (in base 1).
 * @param {any} pageSize Dimensione della pagina.
 */
function SearchViewModel(role, page, pageSize) {
    this.page = page;
    this.pageSize = pageSize;
    this.role = role;
    this.codice = '';
    this.dataDa = null;
    this.dataA = null;
    this.stati = [];
    this.singolaIntervallo;
}

// Numero totale di elementi.
var total = 0;

// Indirizzo da invocare per ottenere una nuova pagina.
var getResultPageUrl;

// Indica che è già in corso un caricamento di elementi
var loading = false;

// Riferimenti agli elementi di: indicazione caricamento in corso; torale degli elementi restituiti
// dalla ricerca e indicazione di nessun risultato restituito dalla ricerca.
var loadingEl, totalResultsEl, noResultsEl;

// Selettore per l'individuazione del totale di elementi restituiti dalla ricerca
// (è l'elemento hidden resituito dall'endpoint di paginazione).
var totalSearchSelector;

// Istanza dell'oggetto da inviare all'endpoint di ricerca
var searchRequest;

/**
 * Registra l'handler per la gestione del caricamento delle pagine di risultati.
 * @param {any} role ruolo con cui richiedere i risultati.
 * @param {any} pageSize dimensione della pagina.
 * @param {any} loadingSelector selettore dell'elemento HTLM da utilizzare per indicare il caricamento in corso.
 * @param {any} totalResultsSelector selettore dell'elemento HTLM in cui inserire la pagina di risultati.
 * @param {any} noResultsSelector selettore dell'elemento HTML da utilizzare per indicare che la ricerca non ha restituito nessun risultato.
 * @param {any} loadResultPageUrl URL del servizio da interrogare per ottenere una pagina di risultati.
 * @param {any} totalSearchElementsSelector selettore dell'elemento HTML restituito dal servizio di ricerca, in cui sarà contenuto il numero totale di elementi trovati.
 */
function registerInViewHandler(role, pageSize, loadingSelector, totalResultsSelector, noResultsSelector, loadResultPageUrl, totalSearchElementsSelector, searchViewModel) {

    // Inizializza l'oggetto con i parametri di ricerca, recupera i riferimenti ai controlli HTLM, salva la url 
    // dell'endpoint di ricerca e salva il selettore per l'elemento con il numero totale di risultati.
    searchRequest = new SearchViewModel(role, 1, pageSize);
    loadingEl = $(loadingSelector);
    totalResultsEl = $(totalResultsSelector);
    noResultsEl = $(noResultsSelector);
    getResultPageUrl = loadResultPageUrl;
    totalSearchSelector = totalSearchElementsSelector;

    //Verifica se ci sono dei filtri da riproporre
    setSavedFilters(searchViewModel);
    
    // Registra handler per intercettazione visualizzazione elemento di ricerca in corso.
    loadingEl.on('inview', function (event, isInView) {

        // Se l'elemento è attualmente visualizzato e non è già in corso il
        // caricamento di una pagina di risultati, richiede la nuova pagina.
        if (isInView && !loading) {
            loadNewPage();

        }
    });
}

/**
 * Carica una nuova pagina di risultati
 */
function loadNewPage() {
    // Segnala che è stata avviata.
    loading = true;

    // Nasconde "nessun risultato"
    noResultsEl.hide();

    $.ajax({
        type: 'POST',
        url: getResultPageUrl,
        data: searchRequest,
        success: function (data) {

            // Inserisce quanto restituito dall'endpoint
            // prima dell'elemento di ricerca.
            loadingEl.before(data);

            // Recupera il numero totale di elementi trovati (è un campo hidden in data);
            // nasconde il "caricamento in corso" se quella restituita è l'ultima pagina;
            // elimina l'elemento dalla partial ed aggiorna i campi con il numero di elementi totali
            // ed eventualmente mostra il testo "nessun risultato".
            var totalElementsEl = $(totalSearchSelector);

            if (searchRequest.page * searchRequest.pageSize >= totalElementsEl.val()) {
                loadingEl.hide();
            }

            totalResultsEl.text('Trovati ' + totalElementsEl.val() + ' risultati');
            if (totalElementsEl.val() === "0") {
                noResultsEl.show();
            }

            totalElementsEl.remove();

            // Indica il completamento dell'operazione di caricamento.
            loading = false;

            // Alla prossima invocazione, bisogna caricare la prossima pagina.
            searchRequest.page++;

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // Gestione errore caricamento
            loading = false;

            if (XMLHttpRequest.status == 403) {
                window.location = "/account/accessdenied";
            } else {
                window.location = "/home/error";

            }
        }
    });
}

/** Inizializza i filtri */
function initFilters() {
    // Ordinamento degli stati
    let sorted = $('#filtro-stati').children().sort(function (a, b) {
        let aValue = parseInt(a.children[0].value);
        let bValue = parseInt(b.children[0].value);

        return (aValue < bValue) ? -1 : (aValue > bValue) ? 1 : 0;

    });
    $('#filtro-stati').html(sorted);

    // Inizializzazione dei controlli sulle date
    $('.it-date-datepicker').datepicker({
        inputFormat: ["dd/MM/yyyy"],
        outputFormat: 'dd/MM/yyyy',
        changeYear: true,
        showButtonPanel: true,
        onSelect: function () {
            let dataDa = $('#dataDa').datepicker('getDate');
            let dataA = $('#dataA').datepicker('getDate');
            let invalidDate = $('#invalidDates');
            invalidDate.hide();
            // Se sono definite entambe le date e dataDa è successiva a dataA
            // mostra l'errore.
            if (dataA != undefined && dataDa != undefined && dataDa > dataA) {
                invalidDate.show();
            }
        }
    });

    // Controllo visibilità campi date in base alla selezione nel menu a tendina
    $("#singola-intervallo").change(function () {

        let daRef = $('#dataDa');
        let aRef = $('#dataA');

        // Mostra i due date picker cancella le date selezionate e nasconde il messaggio di errore.
        $('.it-date-datepicker').show();
        daRef.val('');
        aRef.val('');
        $('#invalidDates').hide();

        // Se è selezionato il filtro DA, nasconde A, discorso inverso se
        // è selezionato A.
        if (this.value == "da") {
            aRef.hide();

        }
        if (this.value == "a") {
            daRef.hide();
        }
    });
}

/**
 * Aggiorna il valore del filtro di ricerca sugli stati.
 * */
function updateStatiSelection() {
    searchRequest.stati = $('input[name="filtroStato"]:checked').map(function () {
        return $(this).val();
    }).get();
}

/**
 * Invoca l'operazione per il raffinamento della ricerca.
 */
function refineSearch() {
    // Aggiorna i valori dei filtri
    searchRequest.codice = $('#codice').val();
    searchRequest.dataDa = $('#dataDa').val();
    searchRequest.dataA = $('#dataA').val();
    searchRequest.vistoSegreteriaTecnica = $('#vistoSegreteriaTecnica').is(":checked");
    searchRequest.proponente = $('#proponente').val();
    searchRequest.dipartimento = $('#dipartimento').val();
    searchRequest.singolaIntervallo = $('#singola-intervallo').val()

    // Aggiorno gli stati selezionati
    updateStatiSelection();

    // Azzera il numero di pagine.
    searchRequest.page = 1;

    // Reimposta l'indicazione del numero di risultati
    totalResultsEl.text('Ricerca in corso...');

    // Svuota la lista degli elementi.
    var resultRows = loadingEl.parent().children(".row");
    resultRows.remove();

    // Mostra l'indicazione di caricamento in corso.
    loadingEl.show();

    // Svuota la lista degli elementi selezionati (definita nella libreria azioni-massive
    clearSelectedItems();

    //// Elimino dal query string l'informazione sulla provenienza
    //getResultPageUrl = getResultPageUrl.split("?")[0];

    // Avvia la ricerca.
    loadNewPage();    
}

/**
 * Verifica se ci sono dei filtri sa riproporre in caco affermativo compila il
 */
function setSavedFilters(searchViewModel) {
    var searchViewModelObject = null;
    if (searchViewModel != "" && searchViewModel != null) {
        searchViewModelObject = JSON.parse(searchViewModel)
        searchRequest.codice = searchViewModelObject.Codice;
        searchRequest.stati = searchViewModelObject.Stati;
        searchRequest.proponente = searchViewModelObject.Proponente;

        searchRequest.singolaIntervallo = searchViewModelObject.SingolaIntervallo;
        searchRequest.dataDa = searchViewModelObject.DataDa;
        if (searchRequest.dataDa != null) {
            var dataDa = new Date(searchRequest.dataDa);
            $('#dataDa').val(String(dataDa.getDate()).padStart(2, '0') + "/" + String(dataDa.getMonth() + 1).padStart(2, '0') + "/" + dataDa.getFullYear());
        }
        searchRequest.dataA = searchViewModelObject.DataA;
        if (searchRequest.dataA != null) {
            var dataA = new Date(searchRequest.dataA);
            $('#dataA').val(String(dataA.getDate()).padStart(2, '0') + "/" + String(dataA.getMonth() + 1).padStart(2, '0') + "/" + dataA.getFullYear());
        }

        searchRequest.dipartimento = searchViewModelObject.Dipartimento;
        searchRequest.vistoSegreteriaTecnica = searchViewModelObject.VistoSegreteriaTecnica;

        if (searchRequest.singolaIntervallo == "da") {
            $('#dataA').hide();
        }
        if (searchRequest.singolaIntervallo == "a") {
            $('#dataDa').hide();
        }
    }
}