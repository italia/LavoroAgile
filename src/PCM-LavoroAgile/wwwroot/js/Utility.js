function calcolaDataInizioAccordo() {
    var dataInizioAccordo = $("#DataInizioUtc").datepicker('getDate', { dateFormat: 'dd,MM,yyyy' });

    return dataInizioAccordo;
}

function calcolaDataInizioModificaAccordo() {
    var dataInizioModificaAccordo = $("#DataInizioModificaAccordo").datepicker('getDate', { dateFormat: 'dd,MM,yyyy' });

    return dataInizioModificaAccordo;
}

/**
 * Converte una stringa valorizzata con un booleano, in testo.
 * @param {any} value Valore da verificare e convertire.
 * @returns Stringa corrispondete al valore booleano.
 */
function getBoolValueAsString(value) {
    if (!value) {
        return value;
    }

    switch (value.toLowerCase()) {
        case 'true':
            return "Selezionato";
        case 'false':
            return "Non selezionato";
        default:
            return value;
    }
}