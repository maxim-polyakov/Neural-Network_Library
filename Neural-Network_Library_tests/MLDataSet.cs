using Neural_Network_Library;

namespace Neural_Network_Library_tests
{
    internal class MLDataSet : IMLDataSet
    {
        public IMLDataPair this[int x] => throw new NotImplementedException();

        public int IdealSize => throw new NotImplementedException();

        public int InputSize => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool Supervised => throw new NotImplementedException();

        public void Add(IMLData data1)
        {
            throw new NotImplementedException();
        }

        public void Add(IMLData inputData, IMLData idealData)
        {
            throw new NotImplementedException();
        }

        public void Add(IMLDataPair inputData)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void GetRecord(int index, IMLDataPair pair)
        {
            throw new NotImplementedException();
        }

        public IMLDataSet OpenAdditional()
        {
            throw new NotImplementedException();
        }
    }
}