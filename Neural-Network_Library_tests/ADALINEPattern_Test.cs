using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ADALINEPattern_Test {

        private INeuralNetworkPattern _activationFunction = new ADALINEPattern();

        [TestMethod]
        public void ActivationFunction_Test()
        {
        }

        [TestMethod]
        public void AddHiddenLayer_Test() {
            int count = 10;
            _activationFunction.AddHiddenLayer(count);
        }

        [TestMethod]
        public void Clear_Test()
        {
            _activationFunction.Clear();
        }

        [TestMethod]
        public void Generate_Test() {
            _activationFunction.Generate();
        }
    }
}
