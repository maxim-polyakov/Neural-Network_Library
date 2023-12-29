using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationCompetitive_Test
    {
        private IActivationFunction _activationFunction = new ActivationCompetitive();

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

            
            var res = _activationFunction.DerivativeFunction(a, b);
        }

        [TestMethod]
        public void TestMethod_HasDerivative()
        {
            Assert.AreEqual(true, this._activationFunction.HasDerivative(););
        }

        [TestMethod]
        public void Clone_Test()
        {

            this._activationFunction.Clone();
        }
    }
}
