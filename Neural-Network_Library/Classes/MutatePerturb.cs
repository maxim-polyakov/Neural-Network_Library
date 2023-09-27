using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class MutatePerturb : IMutate
    {
        /// <summary>
        /// The amount to perturb by.
        /// </summary>
        ///
        private readonly double _perturbAmount;

        /// <summary>
        /// Construct a perturb mutation.
        /// </summary>
        ///
        /// <param name="thePerturbAmount">The amount to mutate by(percent).</param>
        public MutatePerturb(double thePerturbAmount)
        {
            _perturbAmount = thePerturbAmount;
        }

        #region IMutate Members

        /// <summary>
        /// Perform a perturb mutation on the specified Q.
        /// </summary>
        ///
        /// <param name="Q">The Q to mutate.</param>
        public void PerformMutation(Q Q)
        {
            foreach (IGene gene in Q.Genes)
            {
                if (gene is DoubleGene)
                {
                    var doubleGene = (DoubleGene)gene;
                    double v = doubleGene.Value;
                    v += (_perturbAmount - (ThreadSafeRandom.NextDouble() * _perturbAmount * 2));
                    doubleGene.Value = v;
                }
            }
        }

        #endregion
    }
}
