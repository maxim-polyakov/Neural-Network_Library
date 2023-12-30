using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ArrayDataCODEC_test {

        [TestMethod]
        public void TestArrayDataCODEC_Test() {

            double[][] input = new double[3][];
            double[][] ideal = new double[3][];

            ArrayDataCODEC ADC = new ArrayDataCODEC(input, ideal);
        }
    }
}
