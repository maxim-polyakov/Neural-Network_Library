using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class ArchitectureLayer_Test {

        private ArchitectureLayer _layer = new ArchitectureLayer();

        [TestMethod]
        public void ParseLayer_Test()
        {
            _layer.Name = "Test";
            _layer.Bias = true;
            _layer.UsedDefault = true;
            _layer.Count = 0;
        }            
    }
}
