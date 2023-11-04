using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class YInnovationList : BasicInnovationList
    {
        /// <summary>
        /// The next neuron id.
        /// </summary>
        ///
        private long nextNeuronID;

        /// <summary>
        /// The population.
        /// </summary>
        ///
        private IPopulation population;

        /// <summary>
        /// The default constructor, used mainly for persistance.
        /// </summary>
        ///
        public YInnovationList()
        {
            nextNeuronID = 0;
        }

        /// <summary>
        /// Construct an innovation list.
        /// </summary>
        ///
        /// <param name="population_0">The population.</param>
        /// <param name="links">The links.</param>
        /// <param name="neurons">THe neurons.</param>
        public YInnovationList(IPopulation population_0,
                                  Q links, Q neurons)
        {
            //nextNeuronID = 0;
            //population = population_0;

            //foreach (IGene gene in neurons.Genes)
            //{
            //    var neuronGene = (YNeuronGene)gene;

            //    var innovation = new YInnovation(neuronGene,
            //                                        population_0.AssignInnovationID(), AssignNeuronID());
            //    Add(innovation);
            //}


            //foreach (IGene gene_1 in links.Genes)
            //{
            //    var linkGene = (YLinkGene)gene_1;
            //    var innovation_2 = new YInnovation(
            //        linkGene.FromNeuronID, linkGene.ToNeuronID,
            //        YInnovationType.NewLink,
            //        population.AssignInnovationID());
            //    Add(innovation_2);
            //}
        }

        /// <summary>
        /// The population.
        /// </summary>
        public YPopulation Population
        {
            set { population = value; }
        }

        /// <summary>
        /// Assign a neuron ID.
        /// </summary>
        ///
        /// <returns>The neuron id.</returns>
        private long AssignNeuronID()
        {
            return nextNeuronID++;
        }

        /// <summary>
        /// Check to see if we already have an innovation.
        /// </summary>
        ///
        /// <param name="ins0">The input neuron.</param>
        /// <param name="xout">THe output neuron.</param>
        /// <param name="type">The type.</param>
        /// <returns>The innovation, either new or existing if found.</returns>
        public YInnovation CheckInnovation(long ins0, long xout,
                                              YInnovationType type)
        {
            foreach (IInnovation i in Innovations)
            {
                var innovation = (YInnovation)i;
                if ((innovation.FromNeuronID == ins0)
                    && (innovation.ToNeuronID == xout)
                    && (innovation.InnovationType == type))
                {
                    return innovation;
                }
            }

            return null;
        }

        /// <summary>
        /// Create a new neuron gene from an id.
        /// </summary>
        ///
        /// <param name="neuronID">The neuron id.</param>
        /// <returns>The neuron gene.</returns>
        public YNeuronGene CreateNeuronFromID(long neuronID)
        {
            var result = new YNeuronGene(YNeuronType.Hidden,
                                            0, 0, 0);


            foreach (IInnovation i in Innovations)
            {
                var innovation = (YInnovation)i;
                if (innovation.NeuronID == neuronID)
                {
                    result.NeuronType = innovation.NeuronType;
                    result.Id = innovation.NeuronID;
                    result.SplitY = innovation.SplitY;
                    result.SplitX = innovation.SplitX;

                    return result;
                }
            }

            throw new TrainingError("Failed to find innovation for neuron: "
                                    + neuronID);
        }

        /// <summary>
        /// Create a new innovation.
        /// </summary>
        ///
        /// <param name="ins0">The input neuron.</param>
        /// <param name="xout">The output neuron.</param>
        /// <param name="type">The type.</param>
        public void CreateNewInnovation(long ins0, long xout,
                                        YInnovationType type)
        {
            var newInnovation = new YInnovation(ins0, xout, type,
                                                   population.AssignInnovationID());

            if (type == YInnovationType.NewNeuron)
            {
                newInnovation.NeuronID = AssignNeuronID();
            }

            Add(newInnovation);
        }

        /// <summary>
        /// Create a new innovation.
        /// </summary>
        ///
        /// <param name="from">The from neuron.</param>
        /// <param name="to">The to neuron.</param>
        /// <param name="innovationType">THe innovation type.</param>
        /// <param name="neuronType">The neuron type.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The new innovation.</returns>
        public long CreateNewInnovation(long from, long to,
                                        YInnovationType innovationType,
                                        YNeuronType neuronType, double x, double y)
        {
            var newInnovation = new YInnovation(from, to,
                                                   innovationType, population.AssignInnovationID(),
                                                   neuronType, x, y);

            if (innovationType == YInnovationType.NewNeuron)
            {
                newInnovation.NeuronID = AssignNeuronID();
            }

            Add(newInnovation);

            return (nextNeuronID - 1); // ??????? should it be innov?
        }
    }
}
