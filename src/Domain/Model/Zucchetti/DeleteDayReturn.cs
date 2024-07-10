namespace Domain.Model.Zucchetti
{
    /// <summary>
    /// Rappresenta la risposta ricevuta dal servizio di cancellazione giornate.
    /// </summary>
    public class DeleteDayReturn
    {
        public string status { get; set; }
        public string errorCode { get; set; }
        public string errorDescription { get; set; }
    }
}
