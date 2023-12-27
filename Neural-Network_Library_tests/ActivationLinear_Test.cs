using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationLinear_Test {
        private IActivationFunction _activationFunction = new ActivationLinear();

        [TestMethod]
        public void HasDerivative_Test() {
            Assert.AreEqual(this._activationFunction.HasDerivative(), true);
        }

        [TestMethod]
        public void TestMethod_DerivationFunction() {
            double b = 2.0, a = 1.0;

            Assert.AreEqual(this._activationFunction.DerivativeFunction(b, a), 1);
        }

        [TestMethod]
        public void TestMethod_ActivationFunction() {
            double[] d = new double[10];

            for (int i = 0; i < 10; i++)
            {
                d[i] = i;
            }
            int start = 10;
            int size = 10;

            this._activationFunction.ActivationFunction(d, start, size);
        }

        [TestMethod]
        public void Clone_Test() {
            _activationFunction.Clone();
        }
    }
}
