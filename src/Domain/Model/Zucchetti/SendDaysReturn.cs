namespace Domain.Model.Zucchetti
{
    /// <summary>
    /// Rappresenta la risposta ricevuta dal servizio di invio giornate.
    /// </summary>
    public class SendDaysReturn
    {
        public string status { get; set; }
        public string errorCode { get; set; }
        public string errorDescription { get; set; }
    }
}
