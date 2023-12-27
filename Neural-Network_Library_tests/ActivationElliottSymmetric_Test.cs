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

        private IActivationFunction aes = new ActivationElliottSymmetric();

        [TestMethod]
        public void TestMethod_DerivativeFunction() {
            double[] d = new double[10];

            for(int i = 0; i < 10; i++) 
            {
                d[i] = i;
            }

            int start = 0, size = 10;

            this.aes.ActivationFunction(d, start, size);
        }

        [TestMethod]
        public void TestMethod_DerivationFunction() {
            double b = 2.0, a = 1.0;

            double res = this.aes.DerivativeFunction(b , a);

            Assert.AreEqual(res, 0.1111111111111111);
        }

        [TestMethod]
        public void HasDerivative_Test() {

            bool res = this.aes.HasDerivative();

            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void SetParam_Test() {

            this.aes.Clone();
        }
    }
}
