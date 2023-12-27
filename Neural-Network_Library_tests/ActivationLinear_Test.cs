using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationLinear_Test
    {
        private IActivationFunction al = new ActivationLinear();

        [TestMethod]
        public void HasDerivative_Test()
        {

            bool res = this.al.HasDerivative();

            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void TestMethod_DerivationFunction()
        {
            double b = 2.0, a = 1.0;

            double res = this.al.DerivativeFunction(b, a);

            Assert.AreEqual(res, 1);
        }

        [TestMethod]
        public void TestMethod_ActivationFunction()
        {
            double[] d = new double[10];

            for (int i = 0; i < 10; i++)
            {
                d[i] = i;
            }
            int start = 10;
            int size = 10;

            this.al.ActivationFunction(d, start, size);
        }

        //[TestMethod]
        //public object Clone()
        //{
            //return new ActivationLinear();
        //}
    }
}
