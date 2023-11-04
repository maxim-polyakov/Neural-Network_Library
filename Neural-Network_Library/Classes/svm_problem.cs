using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class svm_problem
    {
        public int l;
        public double[] y;
        public svm_node[][] x;
    }
}
