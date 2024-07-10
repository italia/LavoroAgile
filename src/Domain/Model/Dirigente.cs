namespace Domain.Model
{
    /// <summary>
    /// Modella un dirigente
    /// </summary>
    public sealed class Dirigente : PeopleCommon
    {
        public Dirigente()
            : base()
        {
            
        }
        public Dirigente(string name, string lastName, string email) : base(name, lastName, email)
        {
        }
    }
}
