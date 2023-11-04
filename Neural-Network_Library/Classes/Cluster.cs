using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Cluster<T>
    {
        /// <summary>
        /// The contents of the cluster.
        /// </summary>
        private readonly IList<T> _contents = new List<T>();

        /// <summary>
        /// The centroid of this cluster.
        /// </summary>
        private ICentroid<T> _centroid;

        /// <summary>
        /// Create an empty cluster.
        /// </summary>
        public Cluster()
        {
        }

        /// <summary>
        /// Create a cluster with one initial data point. 
        /// </summary>
        /// <param name="d">The initial data point.</param>
        public Cluster(T d)
        {
            _contents.Add(d);
            _centroid = ((ICentroidFactory<T>)d).CreateCentroid();
        }

        /// <summary>
        /// The contents of this cluster.
        /// </summary>
        public IList<T> Contents
        {
            get { return _contents as List<T>; }
        }

        /// <summary>
        /// Add a element to the cluster.
        /// </summary>
        /// <param name="e">The element to add.</param>
        public void Add(T e)
        {
            if (_centroid == null)
                _centroid = ((ICentroidFactory<T>)e) as ICentroid<T>;
            else
                _centroid.Add(e);

            _contents.Add(e);
        }

        /// <summary>
        /// Remove the specified index from the cluster. 
        /// </summary>
        /// <param name="i">The index to remove.</param>
        public void Remove(int i)
        {
            _centroid.Remove(_contents[i]);
            _contents.RemoveAt(i);
        }

        /// <summary>
        /// The centroid of this cluster.
        /// </summary>
        /// <returns>The centroid.</returns>
        public ICentroid<T> Centroid()
        {
            return _centroid;
        }
    }
}
