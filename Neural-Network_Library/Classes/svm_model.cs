using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class svm_model
    {
        internal svm_parameter param; // parameter
        internal int nr_class; // number of classes, = 2 in regression/one class svm
        internal int l; // total #SV
        public svm_node[][] SV; // SVs (SV[l])
        internal double[][] sv_coef; // coefficients for SVs in decision functions (sv_coef[n-1][l])
        internal double[] rho; // constants in decision functions (rho[n*(n-1)/2])
        internal double[] probA; // pariwise probability information
        internal double[] probB;

        // for classification only

        internal int[] label; // label of each class (label[n])
        internal int[] nSV; // number of SVs for each class (nSV[n])
        // nSV[0] + nSV[1] + ... + nSV[n-1] = l
    }
}
