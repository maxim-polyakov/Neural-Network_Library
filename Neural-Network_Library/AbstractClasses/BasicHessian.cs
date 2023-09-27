using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class BasicHessian : IComputeHessian
    {
        /// <summary>
        /// The derivatives.
        /// </summary>
        protected double[] derivative;

        /// <summary>
        /// The flat network.
        /// </summary>
        protected FlatNetwork flat;

        /// <summary>
        /// The gradients of the Hessian.
        /// </summary>
        protected double[] gradients;

        /// <summary>
        /// The Hessian 2d array.
        /// </summary>
        protected double[][] hessian;

        /// <summary>
        /// The Hessian matrix.
        /// </summary>
        protected Matrix hessianMatrix;

        /// <summary>
        /// The neural network that we would like to train.
        /// </summary>
        protected BasicNetwork network;


        /// <summary>
        /// The sum of square error.
        /// </summary>
        protected double sse;

        /// <summary>
        /// The training data that provides the ideal values.
        /// </summary>
        protected IMLDataSet training;

        #region IComputeHessian Members

        /// <inheritdoc/>
        public virtual void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {
            int weightCount = theNetwork.Structure.Flat.Weights.Length;
            flat = theNetwork.Flat;
            training = theTraining;
            network = theNetwork;
            gradients = new double[weightCount];
            hessianMatrix = new Matrix(weightCount, weightCount);
            hessian = hessianMatrix.Data;
            derivative = new double[weightCount];
        }

        /// <inheritdoc/>
        public double[] Gradients
        {
            get { return gradients; }
        }

        /// <inheritdoc/>
        public Matrix HessianMatrix
        {
            get { return hessianMatrix; }
        }

        /// <inheritdoc/>
        public double[][] Hessian
        {
            get { return hessian; }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            EngineArray.Fill(gradients, 0);
            hessianMatrix.Clear();
        }

        /// <inheritdoc/>
        public double SSE
        {
            get { return sse; }
        }


        /// <inheritdoc/>
        public abstract void Compute();

        #endregion

        /// <summary>
        /// Update the Hessian, sum's with what is in the Hessian already.  Call clear to clear out old Hessian.
        /// </summary>
        /// <param name="d">The first derivatives to update with.</param>
        public void UpdateHessian(double[] d)
        {
            // update the hessian
            int weightCount = network.Flat.Weights.Length;
            for (int i = 0; i < weightCount; i++)
            {
                for (int j = 0; j < weightCount; j++)
                {
                    hessian[i][j] += 2 * d[i] * d[j];
                }
            }
        }
    }
}
