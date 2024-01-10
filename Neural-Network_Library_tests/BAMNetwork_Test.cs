using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class BAMNetwork_Test {

        private BAMNetwork bAMNetwork = new BAMNetwork(); 


        [TestMethod]
        public void AddPattern_Test() {
            this.bAMNetwork.Clear();
        }
    }
}
