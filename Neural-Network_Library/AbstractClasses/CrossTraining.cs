using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public abstract class CrossTraining : BasicTraining
    {
        /// <summary>
        /// The folded dataset.
        /// </summary>
        ///
        private readonly FoldedDataSet _folded;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly IMLMethod _network;

        /// <summary>
        /// Construct a cross trainer.
        /// </summary>
        ///
        /// <param name="network">The network.</param>
        /// <param name="training">The training data.</param>
        protected CrossTraining(IMLMethod network, FoldedDataSet training) : base(TrainingImplementationType.Iterative)
        {
            _network = network;
            Training = training;
            _folded = training;
        }


        /// <value>The folded training data.</value>
        public FoldedDataSet Folded
        {
            get { return _folded; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }
    }
}
