using Neural_Network_Library;

namespace Neural_Network_Library_tests
{
    [TestClass]
    public class RBFNetwork_Test
    {
        [TestMethod]
        public void TestMethod_TrainBayesian()
        {
            IMLDataSet data = new MLDataSet();

            RBFNetwork ac = new RBFNetwork();


            ac.CalculateError(data);
        }
    }
}