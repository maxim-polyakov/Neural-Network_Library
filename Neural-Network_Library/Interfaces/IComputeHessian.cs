using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IComputeHessian
    {
        /// <summary>
        /// The gradeints. 
        /// </summary>
        double[] Gradients { get; }

        /// <summary>
        /// The sum of squares error over all of the training elements.
        /// </summary>
        double SSE { get; }

        /// <summary>
        /// The Hessian matrix.
        /// </summary>
        Matrix HessianMatrix { get; }

        /// <summary>
        /// Get the Hessian as a 2d array.
        /// </summary>
        double[][] Hessian { get; }

        /// <summary>
        /// Init the class.  
        /// </summary>
        /// <param name="theNetwork">The neural network to train.</param>
        /// <param name="theTraining">The training set to train with.</param>
        void Init(BasicNetwork theNetwork, IMLDataSet theTraining);

        /// <summary>
        /// Compute the Hessian.
        /// </summary>
        void Compute();

        /// <summary>
        /// Clear the Hessian and gradients.
        /// </summary>
        void Clear();
    }
}
