using Neural_Network_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class NeuralPSOWorker : IEngineTask
    {
        private NeuralPSO m_neuralPSO;
        private int m_particleIndex;
        private bool m_init = false;

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="neuralPSO">the training algorithm</param>
        /// <param name="particleIndex">the index of the particle in the swarm</param>
        /// <param name="init">true for an initialisation iteration </param>
        public NeuralPSOWorker(NeuralPSO neuralPSO, int particleIndex, bool init)
        {
            m_neuralPSO = neuralPSO;
            m_particleIndex = particleIndex;
            m_init = init;
        }

        /// <summary>
        /// Update the particle velocity, position and personal best.
        /// </summary>
        public void Run()
        {
            m_neuralPSO.UpdateParticle(m_particleIndex, m_init);
        }

    }
}
