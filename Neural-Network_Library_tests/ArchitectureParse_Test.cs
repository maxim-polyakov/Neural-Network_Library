using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ArchitectureParse_Test {

        [TestMethod]
        public void ParseLayer_Test_one()
        {

            String line = "";
            int defaultValue = 0;
            ArchitectureLayer al = ArchitectureParse.ParseLayer(line, defaultValue);
            Assert.AreNotEqual(null, al);
        }

        [TestMethod]
        public void ParseLayer_Test_two()
        {
            String line = "";
            IList<String> al = ArchitectureParse.ParseLayers(line);
            Assert.AreNotEqual(null, al);
        }

        [TestMethod]
        public void ParseParams_Test()
        {
            String line = "";
            IDictionary<String, String> al = ArchitectureParse.ParseParams(line);
            Assert.AreNotEqual(null, al);
        }
    }
}
