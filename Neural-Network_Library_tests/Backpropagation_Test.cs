using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neural_Network_Library;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class Backpropagation_Test {
        
        private static BasicNetwork network = new BasicNetwork();
        private static IMLDataSet training = new MLDataSet();

        Backpropagation backpropagation = new Backpropagation(network, training);

        [TestMethod]
        public void IsValidResume_Test() {
            string key = "";
            double[] list = new double[5];

            TrainingContinuation state = new TrainingContinuation();
            state.Put(key, list);    
            this.backpropagation.IsValidResume(state);
        }

        [TestMethod]
        public void Pause() {
            this.backpropagation.Pause();
        }

        [TestMethod]
        public void UpdateWeight() {
            double[] gradients = new double[5];
            double[] lastGradient = new double[6];
            int index = 0;

            this.backpropagation.UpdateWeight(gradients, lastGradient, index);
        }

    }
}
