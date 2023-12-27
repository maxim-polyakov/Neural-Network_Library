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
            this._activationFunction.Clone();
        }
    }
}
