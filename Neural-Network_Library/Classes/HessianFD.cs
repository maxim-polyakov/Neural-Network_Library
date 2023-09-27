using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class HessianFD : BasicHessian
    {
        /// <summary>
        /// The initial step size for dStep.
        /// </summary>
        public const double InitialStep = 0.001;


        /// <summary>
        /// The center of the point array.
        /// </summary>
        private int _center;

        /// <summary>
        /// The derivative coefficient, used for the finite difference method.
        /// </summary>
        private double[] _dCoeff;

        /// <summary>
        /// The derivative step size, used for the finite difference method.
        /// </summary>
        private double[] _dStep;

        /// <summary>
        /// The number of points actually used, which is (pointsPerSide*2)+1. 
        /// </summary>
        private int _pointCount;

        /// <summary>
        /// The number of points requested per side.  This determines the accuracy of the calculation.
        /// </summary>
        private int _pointsPerSide = 5;

        /// <summary>
        /// The number of points per side.
        /// </summary>
        public int PointsPerSide
        {
            get { return _pointsPerSide; }
            set { _pointsPerSide = value; }
        }

        /// <inheritdoc/>
        public new void Init(BasicNetwork theNetwork, IMLDataSet theTraining)
        {
            base.Init(theNetwork, theTraining);
            int weightCount = theNetwork.Structure.Flat.Weights.Length;

            _center = _pointsPerSide + 1;
            _pointCount = (_pointsPerSide * 2) + 1;
            _dCoeff = CreateCoefficients();
            _dStep = new double[weightCount];

            for (int i = 0; i < weightCount; i++)
            {
                _dStep[i] = InitialStep;
            }
        }

        /// <inheritdoc/>
        public override void Compute()
        {
            sse = 0;

            for (int i = 0; i < network.OutputCount; i++)
            {
                InternalCompute(i);
            }
        }

        /// <summary>
        /// Called internally to compute each output neuron.
        /// </summary>
        /// <param name="outputNeuron">The output neuron to compute.</param>
        private void InternalCompute(int outputNeuron)
        {
            int row = 0;
            var error = new ErrorCalculation();
            EngineArray.Fill(derivative, 0);

            // Loop over every training element
            foreach (var pair in training)
            {
                var networkOutput = network.Compute(pair.Input);

                double e = pair.Ideal.Data[outputNeuron] - networkOutput[outputNeuron];
                error.UpdateError(networkOutput[outputNeuron], pair.Ideal[outputNeuron]);

                int currentWeight = 0;

                // loop over the output weights
                int outputFeedCount = network.GetLayerTotalNeuronCount(network.LayerCount - 2);
                for (int i = 0; i < network.OutputCount; i++)
                {
                    for (int j = 0; j < outputFeedCount; j++)
                    {
                        double jc;

                        if (i == outputNeuron)
                        {
                            jc = ComputeDerivative(pair.Input, outputNeuron,
                                                   currentWeight, _dStep,
                                                   networkOutput[outputNeuron], row);
                        }
                        else
                        {
                            jc = 0;
                        }

                        gradients[currentWeight] += jc * e;
                        derivative[currentWeight] += jc;
                        currentWeight++;
                    }
                }

                // Loop over every weight in the neural network
                while (currentWeight < network.Flat.Weights.Length)
                {
                    double jc = ComputeDerivative(
                        pair.Input, outputNeuron, currentWeight,
                        _dStep,
                        networkOutput[outputNeuron], row);
                    derivative[currentWeight] += jc;
                    gradients[currentWeight] += jc * e;
                    currentWeight++;
                }

                row++;
            }

            UpdateHessian(derivative);

            sse += error.CalculateSSE();
        }


        private double ComputeDerivative(IMLData inputData, int outputNeuron, int weight, double[] stepSize,
                                         double networkOutput, int row)
        {
            double temp = network.Flat.Weights[weight];

            var points = new double[_dCoeff.Length];

            stepSize[row] = Math.Max(InitialStep * Math.Abs(temp), InitialStep);

            points[_center] = networkOutput;

            for (int i = 0; i < _dCoeff.Length; i++)
            {
                if (i == _center)
                    continue;

                double newWeight = temp + ((i - _center))
                                   * stepSize[row];

                network.Flat.Weights[weight] = newWeight;

                IMLData output = network.Compute(inputData);
                points[i] = output.Data[outputNeuron];
            }

            double result = _dCoeff.Select((t, i) => t * points[i]).Sum();

            result /= Math.Pow(stepSize[row], 1);

            network.Flat.Weights[weight] = temp;

            return result;
        }

        /// <summary>
        /// Compute finite difference coefficients according to the method provided here:
        /// 
        /// http://en.wikipedia.org/wiki/Finite_difference_coefficients
        ///
        /// </summary>
        /// <returns>An array of the coefficients for FD.</returns>
        public double[] CreateCoefficients()
        {
            var result = new double[_pointCount];

            var delts = new Matrix(_pointCount, _pointCount);
            double[][] t = delts.Data;

            for (int j = 0; j < _pointCount; j++)
            {
                double delt = (j - _center);
                double x = 1.0;

                for (int k = 0; k < _pointCount; k++)
                {
                    t[j][k] = x / SyntMath.Factorial(k);
                    x *= delt;
                }
            }

            Matrix invMatrix = delts.Inverse();
            double f = SyntMath.Factorial(_pointCount);


            for (int k = 0; k < _pointCount; k++)
            {
                result[k] = (Math.Round(invMatrix.Data[1][k] * f)) / f;
            }


            return result;
        }
    }
}
