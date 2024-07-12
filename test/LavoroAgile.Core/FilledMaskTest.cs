using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LavoroAgile.Core
{
    public class FilledMaskTest
    {
        [Fact]
        public void TestZucchettiFill()
        {
            var expected = "000000000000210";

            int val = 210;

            var actual = val.ToString("000000000000000");

            Assert.Equal(expected, actual);
        }
    }
}
