using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LavoroAgile.Core
{
    public class ConvertDates
    {
        [Fact]
        public void ConvertDatesTest()
        {
            var dates = "24/10/2022, 28/10/2022, 31/10/2022, 04/11/2022, 07/11/2022, 11/11/2022, 14/11/2022, 18/11/2022, 21/11/2022, 25/11/2022, 28/11/2022, 02/12/2022, 05/12/2022, 09/12/2022, 12/12/2022, 16/12/2022, 19/12/2022, 23/12/2022, 26/12/2022, 30/12/2022";
            var converted = string.Join(",", dates.Split(",").Select(d => DateTime.Parse(d.Trim()).ToString("yyyy-MM-dd")));

        }
    }
}
