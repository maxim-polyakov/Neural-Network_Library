using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class Address_Test
    {
        [TestMethod]
        public void AddressConstr_Test_one()
        {
            Uri _uri = new Uri("http://www.contoso.com/");
            Address _adress = new Address(_uri);
        }

        [TestMethod]
        public void AddressConstr_Test_two()
        {
            Uri _uri = new Uri("http://www.contoso.com/");
            Address _adress = new Address(_uri);           
        }

    }
}
