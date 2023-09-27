using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Splice : ICrossover
    {
        /// <summary>
        /// The cut length.
        /// </summary>
        ///
        private readonly int _cutLength;

        /// <summary>
        /// Create a slice crossover with the specified cut length.
        /// </summary>
        ///
        /// <param name="theCutLength">The cut length.</param>
        public Splice(int theCutLength)
        {
            _cutLength = theCutLength;
        }

        #region ICrossover Members

        /// <summary>
        /// Assuming this Q is the "mother" mate with the passed in
        /// "father".
        /// </summary>
        ///
        /// <param name="mother">The mother.</param>
        /// <param name="father">The father.</param>
        /// <param name="offspring1">Returns the first offspring</param>
        /// <param name="offspring2">Returns the second offspring.</param>
        public void Mate(Q mother, Q father,
                         Q offspring1, Q offspring2)
        {
            int geneLength = mother.Genes.Count;

            // the Q must be cut at two positions, determine them
            var cutpoint1 = (int)(ThreadSafeRandom.NextDouble() * (geneLength - _cutLength));
            int cutpoint2 = cutpoint1 + _cutLength;

            // handle cut section
            for (int i = 0; i < geneLength; i++)
            {
                if (!((i < cutpoint1) || (i > cutpoint2)))
                {
                    offspring1.GetGene(i).Copy(father.GetGene(i));
                    offspring2.GetGene(i).Copy(mother.GetGene(i));
                }
            }

            // handle outer sections
            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                    offspring1.GetGene(i).Copy(mother.GetGene(i));
                    offspring2.GetGene(i).Copy(father.GetGene(i));
                }
            }
        }

        #endregion
    }
}
