using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LavoroAgile.Core
{
    /// <summary>
    /// Test sulle date.
    /// </summary>
    public class DateTimeTest
    {
        [Fact]
        public void TestParse()
        {
            string date = "09/11/1976";
            DateTime parsed;
            DateTime.TryParse(date, out parsed);

            Assert.Equal(new DateTime(1976, 11, 09), parsed);

        }
    }
}
