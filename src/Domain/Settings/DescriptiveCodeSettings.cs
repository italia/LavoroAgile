namespace Domain.Settings
{
    /// <summary>
    /// Impostazioni relative al formato descrittivo del codice accordo.
    /// </summary>
    public class DescriptiveCodeSettings
    {
        /// <summary>
        /// Formato del codice.
        /// </summary>
        /// <remarks>
        /// Fra parentesi graffe deve essere specificato il nome del campo da mostrare. 
        /// Nel caso di campo data, il campo deve essere specificato seguito da un carattere pipe
        /// e quindi dalla formattazione.
        /// Esempio: "LA-{Codice}{DataSottoscrizione|-dd-MM-yyyy}"
        /// </remarks>
        public string DescriptiveCodeFormat { get; set; } = "LA-{Codice}{DataSottoscrizione|-dd-MM-yyyy}";
    }
}
