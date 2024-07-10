namespace Domain.Model
{
    /// <summary>
    /// Modella un referente
    /// </summary>
    public sealed class Referente : PeopleCommon
    {
        public Referente()
            : base()
        {

        }
        public Referente(string name, string lastName, string email) : base(name, lastName, email)
        {
        }
    }
}
