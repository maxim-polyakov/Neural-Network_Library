using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BasicGAlgorithm : GAlgorithm
    {
        /// <summary>
        /// Is this the first iteration.
        /// </summary>
        ///
        private bool _first;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public BasicGAlgorithm()
        {
            _first = true;
        }

        /// <summary>
        /// Modify the weight matrix and bias values based on the last call to
        /// calcError.
        /// </summary>
        public override sealed void Iteration()
        {
            if (_first)
            {
                Population.Claim(this);
                _first = false;
            }

            var countToMate = (int)(Population.PopulationSize * PercentToMate);
            int offspringCount = countToMate * 2;
            int offspringIndex = Population.PopulationSize
                                 - offspringCount;
            var matingPopulationSize = (int)(Population.PopulationSize * MatingPopulation);

            // mate and form the next generation
            Parallel.For(0, countToMate, i =>
            {
                //IT mother = Population.Ts[i];
                //var fatherInt = (int)(ThreadSafeRandom.NextDouble() * matingPopulationSize);
                //IT father = Population.Ts[fatherInt];
                //IT child1 = Population.Ts[offspringIndex];
                //IT child2 = Population.Ts[offspringIndex + 1];

                //var worker = new MateWorker(mother, father, child1,
                //                            child2);

                //worker.Run();

                //offspringIndex += 2;
            });

            // sort the next generation
            Population.Sort();
        }
    }
}
