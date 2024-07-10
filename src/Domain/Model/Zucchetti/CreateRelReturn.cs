using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Zucchetti
{
    /// <summary>
    /// Rappresenta l'errore ricevuto dal servizio di creazione dei legami
    /// </summary>
    public class CreateRelReturn
    {
        public string td { get; set; }
        public int read { get; set; }
        public int deleted { get; set; }
        public List<Rowserror> rowserrors { get; set; }
        public int written { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }

    public class Rowserror
    {
        public string err_msg { get; set; }
        public string err_val { get; set; }
        public string row_id { get; set; }
    }
}
