using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IRadialBasisFunction
    {
        double Peak { get; set; }


        double Width
        {
            get;
            set;
        }



        int Dimensions
        {
            get;
        }



        double[] Centers
        {
            get;
            set;
        }


        double Calculate(double[] x);
    }
}
