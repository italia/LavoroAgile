/**
 * Libreria per il reinvio dei dati a sistemi esterni
 */

/**
 * Url del servizio di base da invocare per sottomettere le richieste di reinvio.
 * @param {string} daysUrl Url del servizio di invio giornate.
 * @param {string} activitiesUrl Url del servizio di invio attività.
 */
function initResendButtons(daysUrl, activitiesUrl) {

    // Al click sul pulsante di reinvio attività, invoca l'endpoint di reinvio
    // attività, passando l'identificativo dell'accordo
    $('#resend-activities-button').click(function (elemRef) {
        $.post(`${activitiesUrl}\\${$(elemRef.target).data('accordo-id')}`);
        
    });

    // Al click sul pulsante di reinvio giornate, invoca l'endpoint di reinvio
    // giornate, passando l'identificativo dell'accordo
    $('#resend-days-button').click(function (elemRef) {
        $.post(`${daysUrl}\\${$(elemRef.target).data('accordo-id')}`);

    });


}