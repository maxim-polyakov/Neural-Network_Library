using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    [Serializable]
    public class BiPolarMLData : IMLData
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private bool[] _data;

        /// <summary>
        /// Construct this object with the specified data. 
        /// </summary>
        /// <param name="d">The data to create this object with.</param>
        public BiPolarMLData(bool[] d)
        {
            _data = new bool[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                _data[i] = d[i];
            }
        }

        /// <summary>
        /// Construct a data object with the specified size.
        /// </summary>
        /// <param name="size">The size of this data object.</param>
        public BiPolarMLData(int size)
        {
            _data = new bool[size];
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        /// <param name="x">The index.</param>
        /// <returns>The value at the specified index.</returns>
        public double this[int x]
        {
            get { return BiPolarUtil.Bipolar2double(_data[x]); }
            set { _data[x] = BiPolarUtil.Double2bipolar(value); }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public double[] Data
        {
            get { return BiPolarUtil.Bipolar2double(_data); }
            set { _data = BiPolarUtil.Double2bipolar(value); }
        }

        /// <summary>
        /// The size of the array.
        /// </summary>
        public int Count
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Get the specified data item as a boolean.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool GetBoolean(int i)
        {
            return _data[i];
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public virtual object Clone()
        {
            return new BiPolarMLData(_data);
        }

        /// <summary>
        /// Set the value as a boolean.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="b">The boolean value.</param>
        public void SetBoolean(int index, bool b)
        {
            _data[index] = b;
        }

        /// <summary>
        /// Clear to false.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = false;
            }
        }

        /// <inheritdoc/>
        public String ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            for (var i = 0; i < Count; i++)
            {
                result.Append(this[i] > 0 ? "T" : "F");
                if (i != Count - 1)
                {
                    result.Append(",");
                }
            }
            result.Append(']');
            return (result.ToString());
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>Nothing.</returns>
        public ICentroid<IMLData> CreateCentroid()
        {
            return null;
        }

    }
}
