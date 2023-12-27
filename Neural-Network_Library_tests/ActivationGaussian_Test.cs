﻿using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ActivationGaussian_Test
    {
        private IActivationFunction ag = new ActivationGaussian();

        [TestMethod]
        public void TestMethod_ActivationFunction()
        {
            double[] d = new double[10];

            for(int i = 0; i < 10; i++)
            {
                d[i] = i;
            }
            int start = 10;
            int size = 10;
            
            this.ag.ActivationFunction(d, start, size);
        }

        [TestMethod]
        public void TestMethod_DerivativeFunction() {

            int b = 10;
            int a = 10;

            this.ag.DerivativeFunction(b, a);
        }

        [TestMethod]
        public void HasDerivative_Test() {

            bool res = this.ag.HasDerivative();

            Assert.AreEqual(res, true);
        }
    }
}
