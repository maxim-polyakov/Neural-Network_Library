using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationElliottSymmetric_Test
    {

        private IActivationFunction _activationFunction = new ActivationElliottSymmetric();

        [TestMethod]
        public void TestMethod_ActivationFunction() {
            double[] d = new double[10];

            for(int i = 0; i < 10; i++) 
            {
                d[i] = i;
            }

            int start = 0, size = 10;

            this._activationFunction.ActivationFunction(d, start, size);
        }

        [TestMethod]
        public void TestMethod_DerivationFunction() {
            double b = 2.0, a = 1.0;

            double res = this._activationFunction.DerivativeFunction(b , a);

            Assert.AreEqual(res, 0.1111111111111111);
        }

        [TestMethod]
        public void HasDerivative_Test() {
            Assert.AreEqual(this._activationFunction.HasDerivative(), true);
        }

        [TestMethod]
        public void Clone_Test() {

            this._activationFunction.Clone();
        }
    }
}
