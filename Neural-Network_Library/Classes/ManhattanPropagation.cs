using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ManhattanPropagation : Propagation, ILearningRate
    {
        /// <summary>
        /// The default tolerance to determine of a number is close to zero.
        /// </summary>
        ///
        internal const double DefaultZeroTolerance = 0.001d;

        /// <summary>
        /// The zero tolerance to use.
        /// </summary>
        ///
        private readonly double _zeroTolerance;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double _learningRate;


        /// <summary>
        /// Construct a Manhattan propagation training object.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learnRate">The learning rate.</param>
        public ManhattanPropagation(BasicNetwork network,
                                    IMLDataSet training, double learnRate) : base(network, training)
        {
            _learningRate = learnRate;
            _zeroTolerance = RPROPConst.DefaultZeroTolerance;
        }


        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        #region ILearningRate Members

        /// <summary>
        /// Set the learning rate.
        /// </summary>
        public virtual double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }

        #endregion

        /// <summary>
        /// This training type does not support training continue.
        /// </summary>
        ///
        /// <returns>Always returns null.</returns>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// This training type does not support training continue.
        /// </summary>
        ///
        /// <param name="state">Not used.</param>
        public override sealed void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override sealed double UpdateWeight(double[] gradients,
                                                   double[] lastGradient, int index)
        {
            if (Math.Abs(gradients[index]) < _zeroTolerance)
            {
                return 0;
            }
            else if (gradients[index] > 0)
            {
                return _learningRate;
            }
            else
            {
                return -_learningRate;
            }
        }

        /// <summary>
        /// Not needed for this training type.
        /// </summary>
        public override void InitOthers()
        {
        }

    }
}
