using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class PersistorRegistry
    {
        /// <summary>
        /// The instance.
        /// </summary>
        ///
        private static PersistorRegistry _instance;


        /// <summary>
        /// The mapping between name and persistor.
        /// </summary>
        ///
        private readonly IDictionary<String, ISyntPersistor> _map;

        /// <summary>
        /// The class map, used to lookup native classes to their persistor.
        /// </summary>
        private readonly IDictionary<Type, ISyntPersistor> _classMap;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        private PersistorRegistry()
        {
            _map = new Dictionary<String, ISyntPersistor>();
            _classMap = new Dictionary<Type, ISyntPersistor>();
            Add(new PersistSVM());
            Add(new PersistHopfield());
            Add(new PersistBoltzmann());
            Add(new PersistART1());
            Add(new PersistBAM());
            Add(new PersistBasicNetwork());
            Add(new PersistRBFNetwork());
            Add(new PersistSOM());
            Add(new PersistYPopulation());
            Add(new PersistYNetwork());
            Add(new PersistBasicPNN());
            Add(new PersistCPN());
            Add(new PersistTrainingContinuation());
            Add(new PersistBayes());

        }

        /// <value>The singleton instance.</value>
        public static PersistorRegistry Instance
        {
            get { return _instance ?? (_instance = new PersistorRegistry()); }
        }

        /// <summary>
        /// Add a persistor.
        /// </summary>
        ///
        /// <param name="persistor">The persistor to add.</param>
        public void Add(ISyntPersistor persistor)
        {
            _map[persistor.PersistClassString] = persistor;
            _classMap[persistor.NativeType] = persistor;
        }

        /// <summary>
        /// Get a persistor.
        /// </summary>
        ///
        /// <param name="clazz">The class to get the persistor for.</param>
        /// <returns>Return the persistor.</returns>
        public ISyntPersistor GetPersistor(Type clazz)
        {
            return _classMap[clazz];
        }

        /// <summary>
        /// Get the persistor by name.
        /// </summary>
        ///
        /// <param name="name">The name of the persistor.</param>
        /// <returns>The persistor.</returns>
        public ISyntPersistor GetPersistor(String name)
        {
            return _map.ContainsKey(name) ? _map[name] : null;
        }
    }
}
