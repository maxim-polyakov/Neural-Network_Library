using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationLOG_Test
    {
        private IActivationFunction al = new ActivationLOG();

        [TestMethod]
        public void Clone_Test() {
            this.al.Clone();
        }

        [TestMethod]
        public void DerivativeFunction_Test() {
            double b = 2.0;
            double a = 3.0;

            al.DerivativeFunction(b, a);
        }

        [TestMethod]
        public void ActivationFunction_Test() {
            this.al.Clone();
        }

        [TestMethod]
        public void HasDerivative_Test() {

        }
    }
}
