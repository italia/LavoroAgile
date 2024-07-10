using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Zucchetti
{
    public class CreateActivitiesReturn
    {
        public int td { get; set; }
        public int read { get; set; }
        public int deleted { get; set; }
        public string message { get; set; }
        public int write { get; set; }
        public List<Error> errors { get; set; }
        public string status { get; set; }
    }

    public class Error
    {
        public string msg { get; set; }
        public string valerr { get; set; }
        public string trcode { get; set; }
        public string keyrec { get; set; }
    }
}
