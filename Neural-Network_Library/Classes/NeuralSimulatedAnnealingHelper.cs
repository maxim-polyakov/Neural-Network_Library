using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeuralSimulatedAnnealingHelper : SimulatedAnnealing<Double>
    {
        /// <summary>
        /// The class that this class should report to.
        /// </summary>
        ///
        private readonly NeuralSimulatedAnnealing _owner;

        /// <summary>
        /// Constructs this object.
        /// </summary>
        ///
        /// <param name="owner">The owner of this class, that recieves all messages.</param>
        public NeuralSimulatedAnnealingHelper(NeuralSimulatedAnnealing owner)
        {
            _owner = owner;
            ShouldMinimize = _owner.CalculateScore.ShouldMinimize;
        }

        /// <summary>
        /// Used to pass the getArray call on to the parent object.
        /// </summary>
        public override double[] Array
        {
            get { return _owner.Array; }
        }


        /// <summary>
        /// Used to pass the getArrayCopy call on to the parent object.
        /// </summary>
        ///
        /// <value>The array copy created by the owner.</value>
        public override double[] ArrayCopy
        {
            get { return _owner.ArrayCopy; }
        }

        /// <summary>
        /// Used to pass the determineError call on to the parent object.
        /// </summary>
        ///
        /// <returns>The error returned by the owner.</returns>
        public override sealed double PerformCalculateScore()
        {
            return _owner.CalculateScore.CalculateScore(((BasicNetwork)_owner.Method));
        }


        /// <summary>
        /// Used to pass the putArray call on to the parent object.
        /// </summary>
        ///
        /// <param name="array">The array.</param>
        public override sealed void PutArray(double[] array)
        {
            _owner.PutArray(array);
        }

        /// <summary>
        /// Call the owner's randomize method.
        /// </summary>
        ///
        public override sealed void Randomize()
        {
            _owner.Randomize();
        }
    }
}
