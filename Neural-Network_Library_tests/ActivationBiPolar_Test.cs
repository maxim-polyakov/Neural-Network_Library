using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationBiPolar_Test
    {

        private IActivationFunction _activationFunction = new ActivationBiPolar();

        [TestMethod]
        public void TestMethod_ActivationFunction()
        {
            int start = 1;
            int size = 10;
            double[] x = new double[10];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = i;
            }

            this._activationFunction.ActivationFunction(x, start, size);
        }

        [TestMethod]
        public void TestMethod_DerivativeFunction()
        {
            double a = 5.0;
            double b = 1.0;

            var res = this._activationFunction.DerivativeFunction(a, b);

            Assert.AreEqual(1, res);
        }

        [TestMethod]
        public void TestMethod_HasDerivative()
        {

            var res = this._activationFunction.HasDerivative();

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void Clone_Test()
        {

            this._activationFunction.Clone();
        }

    }
}

