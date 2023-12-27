using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationElliott_Test
    {
        private IActivationFunction _activationFunction = new ActivationElliott();

        [TestMethod]
        public void TestMethod_DerivativeFunction()
        {
            double a = 5.0;
            double b = 1.0;

            Assert.AreEqual(0.013888888888888888, this._activationFunction.DerivativeFunction(a, b));
        }
    }
}
