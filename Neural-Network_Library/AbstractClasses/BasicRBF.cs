using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public abstract class BasicRBF : IRadialBasisFunction
    {

        private double[] _center;


        private double _peak;


        private double _width;


        public double[] Centers
        {
            get { return _center; }
            set { _center = value; }
        }


        public int Dimensions
        {
            get { return _center.Length; }
        }


        public double Peak
        {
            get { return _peak; }
            set { _peak = value; }
        }


        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }



        public abstract double Calculate(double[] x);
    }
}
