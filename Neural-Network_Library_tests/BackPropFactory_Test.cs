using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class BackPropFactory_Test {

        public BackPropFactory BPF = new BackPropFactory();

        [TestMethod]
        public void Create_Test() {
            IMLMethod method = new MLMethod();
            IMLDataSet training = new MLDataSet();
            String argsStr = "";

            IMLTrain imt = this.BPF.Create(method, training, argsStr);
        }
    }
}
