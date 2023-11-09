using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BasicMLData : IMLData
    {
        private double[] _data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLData(double[] d)
            : this(d.Length)
        {
            for (int i = 0; i < d.Length; i++)
            {
                _data[i] = d[i];
            }
        }


        /// <summary>
        /// Construct this object with blank data and a specified size.
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public BasicMLData(int size)
        {
            _data = new double[size];
        }

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        public virtual double this[int x]
        {
            get { return _data[x]; }
            set { _data[x] = value; }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Get the count of data items.
        /// </summary>
        public virtual int Count
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Convert the object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                    result.Append(',');
                result.Append(Data[i]);
            }
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            var result = new BasicMLData(_data);
            return result;
        }

        /// <summary>
        /// Clear to zero.
        /// </summary>
        public void Clear()
        {
            EngineArray.Fill(_data, 0);
        }

        /// <inheritdoc/>
        public ICentroid<IMLData> CreateCentroid()
        {
            return new BasicMLDataCentroid(this);
        }

        /// <summary>
        /// Add one data element to another.  This does not modify the object.
        /// </summary>
        /// <param name="o">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Plus(IMLData o)
        {
            if (Count != o.Count)
                throw new SyntError("Lengths must match.");

            var result = new BasicMLData(Count);
            for (int i = 0; i < Count; i++)
                result[i] = this[i] + o[i];

            return result;
        }

        /// <summary>
        /// Multiply one data element with another.  This does not modify the object.
        /// </summary>
        /// <param name="d">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Times(double d)
        {
            IMLData result = new BasicMLData(Count);

            for (int i = 0; i < Count; i++)
                result[i] = this[i] * d;

            return result;
        }

        /// <summary>
        /// Subtract one data element from another.  This does not modify the object.
        /// </summary>
        /// <param name="o">The other data element</param>
        /// <returns>The result.</returns>
        public IMLData Minus(IMLData o)
        {
            if (Count != o.Count)
                throw new SyntError("Counts must match.");

            IMLData result = new BasicMLData(Count);
            for (int i = 0; i < Count; i++)
                result[i] = this[i] - o[i];

            return result;
        }

    }
}
