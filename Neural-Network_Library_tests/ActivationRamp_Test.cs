using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationRamp_Test
    {
        IActivationFunction _activationFunction = new ActivationRamp();

        [TestMethod]
        public void ActivationFunction_Test() {

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
        public void DerevativeFunction_Test() {
            double b = 2.0, a = 3.0;
            var res = this._activationFunction.DerivativeFunction(b, a);
            Assert.AreEqual(1, res);
        }

        [TestMethod]
        public void HasDerivative() {
            Assert.AreEqual(true, _activationFunction.HasDerivative());
        }

        [TestMethod]
        public void Clone_Test()
        {

            this._activationFunction.Clone();
        }
    }
}
