using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class AnnealFactory_Test {

        private AnnealFactory af = new AnnealFactory();

        [TestMethod]
        public void Create_Test() {

            IMLMethod method = null;
            IMLDataSet training = null;
            String argsStr = null;

            IMLTrain mlt = this.af.Create(method, training, argsStr);
        }
    }
}
