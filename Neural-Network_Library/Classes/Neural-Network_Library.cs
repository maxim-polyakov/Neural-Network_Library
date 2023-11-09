using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace Neural_Network_Library
{    
    public class PersistBayes : ISyntPersistor
    {
        /// <summary>
        /// The file version.
        /// </summary>
        public int FileVersion
        {
            get
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public Object Read(Stream istream)
        {
            BayesianNetwork result = new BayesianNetwork();
            SyntReadHelper input = new SyntReadHelper(istream);
            SyntFileSection section;
            String queryType = "";
            String queryStr = "";
            String contentsStr = "";

            while ((section = input.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-PARAM"))
                {
                    IDictionary<String, String> p = section.ParseParams();
                    queryType = p["queryType"];
                    queryStr = p["query"];
                    contentsStr = p["contents"];
                }
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-TABLE"))
                {

                    result.Contents = contentsStr;

                    // first, define relationships (1st pass)
                    foreach (String line in section.Lines)
                    {
                        result.DefineRelationship(line);
                    }

                    result.FinalizeStructure();

                    // now define the probabilities (2nd pass)
                    foreach (String line in section.Lines)
                    {
                        result.DefineProbability(line);
                    }
                }
                if (section.SectionName.Equals("BAYES-NETWORK")
                        && section.SubSectionName.Equals("BAYES-PROPERTIES"))
                {
                    IDictionary<String, String> paras = section.ParseParams();
                    EngineArray.PutAll(paras, result.Properties);
                }
            }

            // define query, if it exists
            if (queryType.Length > 0)
            {
                IBayesianQuery query = null;
                if (queryType.Equals("EnumerationQuery"))
                {
                    query = new EnumerationQuery(result);
                }
                else
                {
                    query = new SamplingQuery(result);
                }

                if (query != null && queryStr.Length > 0)
                {
                    result.Query = query;
                    result.DefineClassificationStructure(queryStr);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            SyntWriteHelper o = new SyntWriteHelper(os);
            BayesianNetwork b = (BayesianNetwork)obj;
            o.AddSection("BAYES-NETWORK");
            o.AddSubSection("BAYES-PARAM");
            String queryType = "";
            String queryStr = b.ClassificationStructure;

            if (b.Query != null)
            {
                queryType = b.Query.GetType().Name;
            }

            o.WriteProperty("queryType", queryType);
            o.WriteProperty("query", queryStr);
            o.WriteProperty("contents", b.Contents);
            o.AddSubSection("BAYES-PROPERTIES");
            o.AddProperties(b.Properties);

            o.AddSubSection("BAYES-TABLE");
            foreach (BayesianEvent e in b.Events)
            {
                foreach (TableLine line in e.Table.Lines)
                {
                    if (line == null)
                        continue;
                    StringBuilder str = new StringBuilder();
                    str.Append("P(");

                    str.Append(BayesianEvent.FormatEventName(e, line.Result));

                    if (e.Parents.Count > 0)
                    {
                        str.Append("|");
                    }

                    int index = 0;
                    bool first = true;
                    foreach (BayesianEvent parentEvent in e.Parents)
                    {
                        if (!first)
                        {
                            str.Append(",");
                        }
                        first = false;
                        int arg = line.Arguments[index++];
                        if (parentEvent.IsBoolean)
                        {
                            if (arg == 0)
                            {
                                str.Append("+");
                            }
                            else
                            {
                                str.Append("-");
                            }
                        }
                        str.Append(parentEvent.Label);
                        if (!parentEvent.IsBoolean)
                        {
                            str.Append("=");
                            if (arg >= parentEvent.Choices.Count)
                            {
                                throw new BayesianError("Argument value " + arg + " is out of range for event " + parentEvent.ToString());
                            }
                            str.Append(parentEvent.GetChoice(arg));
                        }
                    }
                    str.Append(")=");
                    str.Append(line.Probability);
                    str.Append("\n");
                    o.Write(str.ToString());
                }
            }

            o.Flush();
        }

        /// <inheritdoc/>
        public String PersistClassString
        {
            get
            {
                return "BayesianNetwork";
            }
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(BayesianNetwork); }
        }

    }

    [Serializable]
    public class BasicMLComplexData : IMLComplexData
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private ComplexNumber[] _data;

        /// <summary>
        /// Construct this object with the specified data.  Use only real numbers. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLComplexData(double[] d)
            : this(d.Length)
        {
        }

        /// <summary>
        /// Construct this object with the specified data. Use complex numbers. 
        /// </summary>
        /// <param name="d">The data to construct this object with.</param>
        public BasicMLComplexData(ComplexNumber[] d)
        {
            _data = d;
        }

        /// <summary>
        /// Construct this object with blank data and a specified size. 
        /// </summary>
        /// <param name="size">The amount of data to store.</param>
        public BasicMLComplexData(int size)
        {
            _data = new ComplexNumber[size];
        }

        /// <summary>
        /// Construct a new BasicMLData object from an existing one. This makes a
        /// copy of an array. If MLData is not complex, then only reals will be 
        /// created. 
        /// </summary>
        /// <param name="d">The object to be copied.</param>
        public BasicMLComplexData(IMLData d)
        {
            if (d is IMLComplexData)
            {
                var c = (IMLComplexData)d;
                for (int i = 0; i < d.Count; i++)
                {
                    _data[i] = new ComplexNumber(c.GetComplexData(i));
                }
            }
            else
            {
                for (int i = 0; i < d.Count; i++)
                {
                    _data[i] = new ComplexNumber(d[i], 0);
                }
            }
        }

        #region IMLComplexData Members

        /// <summary>
        /// Clear all values to zero.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = new ComplexNumber(0, 0);
            }
        }

        /// <inheritdoc/>
        public Object Clone()
        {
            return new BasicMLComplexData(this);
        }


        /// <summary>
        /// The complex numbers.
        /// </summary>
        public ComplexNumber[] ComplexData
        {
            get { return _data; }
            set { _data = value; }
        }


        /// <inheritdoc/>
        public ComplexNumber GetComplexData(int index)
        {
            return _data[index];
        }

        /// <summary>
        /// Set a data element to a complex number. 
        /// </summary>
        /// <param name="index">The index to set.</param>
        /// <param name="d">The complex number.</param>
        public void SetComplexData(int index, ComplexNumber d)
        {
            _data[index] = d;
        }

        /// <summary>
        /// Set the complex data array.
        /// </summary>
        /// <param name="d">A new complex data array.</param>
        public void SetComplexData(ComplexNumber[] d)
        {
            _data = d;
        }

        /// <summary>
        /// Access the data by index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        public virtual double this[int x]
        {
            get { return _data[x].Real; }
            set { _data[x] = new ComplexNumber(value, 0); }
        }

        /// <summary>
        /// Get the data as an array.
        /// </summary>
        public virtual double[] Data
        {
            get
            {
                var d = new double[_data.Length];
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = _data[i].Real;
                }
                return d;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    _data[i] = new ComplexNumber(value[i], 0);
                }
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _data.Count(); }
        }

        #endregion

        /// <inheritdoc/>
        public String ToString()
        {
            var builder = new StringBuilder("[");
            builder.Append(GetType().Name);
            builder.Append(":");
            for (int i = 0; i < _data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(_data[i].ToString());
            }
            builder.Append("]");
            return builder.ToString();
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

    public class BasicMLDataCentroid : ICentroid<IMLData>
    {
        /// <summary>
        /// The value this centroid is based on.
        /// </summary>
        private BasicMLData value;

        /// <summary>
        /// Construct the centroid. 
        /// </summary>
        /// <param name="o">The object to base the centroid on.</param>
        public BasicMLDataCentroid(IMLData o)
        {
            this.value = (BasicMLData)o.Clone();
        }

        /// <inheritdoc/>
        public void Add(IMLData d)
        {
            double[] a = d.Data;

            for (int i = 0; i < value.Count; i++)
                value.Data[i] =
                    ((value.Data[i] * value.Count + a[i]) / (value.Count + 1));
        }

        /// <inheritdoc/>
        public void Remove(IMLData d)
        {
            double[] a = d.Data;

            for (int i = 0; i < value.Count; i++)
                value[i] =
                    ((value[i] * value.Count - a[i]) / (value.Count - 1));
        }

        /// <inheritdoc/>
        public double Distance(IMLData d)
        {
            IMLData diff = value.Minus(d);
            double sum = 0.0;

            for (int i = 0; i < diff.Count; i++)
                sum += diff[i] * diff[i];

            return Math.Sqrt(sum);
        }
    }

    [Serializable]
    public class BasicMLDataPair : IMLDataPair
    {
        /// <summary>
        /// The the expected output from the neural network, or null
        /// for unsupervised training.
        /// </summary>
        private readonly IMLData _ideal;

        /// <summary>
        /// The training input to the neural network.
        /// </summary>
        private readonly IMLData _input;

        /// <summary>
        /// The significance.
        /// </summary>
        private double _significance = 1.0;

        /// <summary>
        /// Construct a BasicMLDataPair class with the specified input
        /// and ideal values.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <param name="ideal">The expected results from the neural network.</param>
        public BasicMLDataPair(IMLData input, IMLData ideal)
        {
            _input = input;
            _ideal = ideal;
        }

        /// <summary>
        /// Construct a data pair that only includes input. (unsupervised)
        /// </summary>
        /// <param name="input">The input data.</param>
        public BasicMLDataPair(IMLData input)
        {
            _input = input;
            _ideal = null;
        }

        /// <summary>
        /// The input data.
        /// </summary>
        public virtual IMLData Input
        {
            get { return _input; }
        }

        /// <summary>
        /// The ideal data.
        /// </summary>
        public virtual IMLData Ideal
        {
            get { return _ideal; }
        }

        /// <summary>
        /// Convert object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append('[');
            result.Append("Input:");
            result.Append(Input);
            result.Append(",Ideal:");
            result.Append(Ideal);
            result.Append(']');
            return result.ToString();
        }

        /// <summary>
        /// Deterimine if this pair is supervised or unsupervised.
        /// </summary>
        /// <returns>True if this is a supervised pair.</returns>
        public bool IsSupervised
        {
            get { return _ideal != null; }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            Object result;

            if (Ideal == null)
                result = new BasicMLDataPair((IMLData)_input.Clone());
            else
                result = new BasicMLDataPair((IMLData)_input.Clone(),
                                             (IMLData)_ideal.Clone());

            return result;
        }

        /// <summary>
        /// Create a new neural data pair object of the correct size for the neural
        /// network that is being trained. This object will be passed to the getPair
        /// method to allow the neural data pair objects to be copied to it.
        /// </summary>
        /// <param name="inputSize">The size of the input data.</param>
        /// <param name="idealSize">The size of the ideal data.</param>
        /// <returns>A new neural data pair object.</returns>
        public static IMLDataPair CreatePair(int inputSize, int idealSize)
        {
            IMLDataPair result;

            if (idealSize > 0)
            {
                result = new BasicMLDataPair(new BasicMLData(inputSize),
                                             new BasicMLData(idealSize));
            }
            else
            {
                result = new BasicMLDataPair(new BasicMLData(inputSize));
            }

            return result;
        }

        /// <summary>
        /// The supervised ideal data.
        /// </summary>
        public double[] IdealArray
        {
            get
            {
                return _ideal == null ? null : _ideal.Data;
            }
            set { _ideal.Data = value; }
        }

        /// <summary>
        /// The input array.
        /// </summary>
        public double[] InputArray
        {
            get { return _input.Data; }
            set { _input.Data = value; }
        }

        /// <summary>
        /// Returns true, if supervised.
        /// </summary>
        public bool Supervised
        {
            get { return _ideal != null; }
        }

        /// <summary>
        /// The significance of this training element.
        /// </summary>
        public double Significance
        {
            get { return _significance; }
            set { _significance = value; }
        }

        /// <inheritdoc/>
        public ICentroid<IMLDataPair> CreateCentroid()
        {
            if (!(Input is BasicMLData))
            {
                throw new SyntError("The input data type of " + Input.GetType().Name + " must be BasicMLData.");
            }
            return new BasicMLDataPairCentroid(this);
        }
    }

    public class BasicMLDataPairCentroid : ICentroid<IMLDataPair>
    {
        /// <summary>
        /// The value the centroid is based on.
        /// </summary>
        private readonly BasicMLData _value;

        /// <summary>
        /// Construct the centroid. 
        /// </summary>
        /// <param name="o"> The pair to base the centroid on.</param>
        public BasicMLDataPairCentroid(BasicMLDataPair o)
        {
            _value = (BasicMLData)o.Input.Clone();
        }

        /// <inheritdoc/>
        public void Remove(IMLDataPair d)
        {
            double[] a = d.InputArray;

            for (int i = 0; i < _value.Count; i++)
                _value[i] =
                    ((_value[i] * _value.Count - a[i]) / (_value.Count - 1));
        }

        /// <inheritdoc/>
        public double Distance(IMLDataPair d)
        {
            IMLData diff = _value.Minus(d.Input);
            double sum = 0.0;

            for (int i = 0; i < diff.Count; i++)
                sum += diff[i] * diff[i];

            return Math.Sqrt(sum);
        }

        /// <inheritdoc/>
        public void Add(IMLDataPair d)
        {
            double[] a = d.InputArray;

            for (int i = 0; i < _value.Count; i++)
                _value[i] =
                    ((_value[i] * _value.Count) + a[i]) / (_value.Count + 1);
        }

    }

    [Serializable]
    public class BasicMLDataSet : IMLDataSet, IEnumerable<IMLDataPair>
    {
        /// <summary>
        /// The enumerator for the basic neural data set.
        /// </summary>
        [Serializable]
        public class BasicNeuralEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            /// The current index.
            /// </summary>
            private int _current;

            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BasicMLDataSet _owner;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicNeuralEnumerator(BasicMLDataSet owner)
            {
                _current = -1;
                _owner = owner;
            }

            /// <summary>
            /// The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get { return _owner._data[_current]; }
            }

            /// <summary>
            /// Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                // nothing needed
            }

            /// <summary>
            /// The current item.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_current < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner._data[_current];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                _current++;
                if (_current >= _owner._data.Count)
                    return false;
                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                _current = -1;
            }
        }

        /// <summary>
        /// Access to the list of data items.
        /// </summary>
        public IList<IMLDataPair> Data
        {
            get { return _data; }
            set { _data = value; }
        }


        /// <summary>
        /// The data held by this object.
        /// </summary>
        private IList<IMLDataPair> _data = new List<IMLDataPair>();

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        /// <param name="data">The data to use.</param>
        public BasicMLDataSet(IList<IMLDataPair> data)
        {
            _data = data;
        }

        /// <summary>
        /// Copy whatever dataset type is specified into a memory dataset.
        /// </summary>
        ///
        /// <param name="set">The dataset to copy.</param>
        public BasicMLDataSet(IMLDataSet set)
        {
            _data = new List<IMLDataPair>();
            int inputCount = set.InputSize;
            int idealCount = set.IdealSize;


            foreach (IMLDataPair pair in set)
            {
                BasicMLData input = null;
                BasicMLData ideal = null;

                if (inputCount > 0)
                {
                    input = new BasicMLData(inputCount);
                    EngineArray.ArrayCopy(pair.InputArray, input.Data);
                }

                if (idealCount > 0)
                {
                    ideal = new BasicMLData(idealCount);
                    EngineArray.ArrayCopy(pair.IdealArray, ideal.Data);
                }

                Add(new BasicMLDataPair(input, ideal));
            }
        }


        /// <summary>
        /// Construct a data set from an input and idea array.
        /// </summary>
        /// <param name="input">The input into the neural network for training.</param>
        /// <param name="ideal">The idea into the neural network for training.</param>
        public BasicMLDataSet(double[][] input, double[][] ideal)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var tempInput = new double[input[0].Length];
                double[] tempIdeal;

                for (int j = 0; j < tempInput.Length; j++)
                {
                    tempInput[j] = input[i][j];
                }

                BasicMLData idealData = null;

                if (ideal != null)
                {
                    tempIdeal = new double[ideal[0].Length];
                    for (int j = 0; j < tempIdeal.Length; j++)
                    {
                        tempIdeal[j] = ideal[i][j];
                    }
                    idealData = new BasicMLData(tempIdeal);
                }

                var inputData = new BasicMLData(tempInput);

                Add(inputData, idealData);
            }
        }

        /// <summary>
        /// Construct a basic neural data set.
        /// </summary>
        public BasicMLDataSet()
        {
        }

        /// <summary>
        /// Get the ideal size, or zero for unsupervised.
        /// </summary>
        public virtual int IdealSize
        {
            get
            {
                if (_data == null || _data.Count == 0)
                {
                    return 0;
                }

                IMLDataPair pair = _data[0];

                if (pair.IdealArray == null)
                {
                    return 0;
                }

                return pair.Ideal.Count;
            }
        }

        /// <summary>
        /// Get the input size.
        /// </summary>
        public virtual int InputSize
        {
            get
            {
                if (_data == null || _data.Count == 0)
                    return 0;
                IMLDataPair pair = _data[0];
                return pair.Input.Count;
            }
        }

        /// <summary>
        /// Add the specified data to the set.  Add unsupervised data.
        /// </summary>
        /// <param name="data1">The data to add to the set.</param>
        public virtual void Add(IMLData data1)
        {
            IMLDataPair pair = new BasicMLDataPair(data1, null);
            _data.Add(pair);
        }

        /// <summary>
        /// Add supervised data to the set.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public virtual void Add(IMLData inputData, IMLData idealData)
        {
            IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
            _data.Add(pair);
        }

        /// <summary>
        /// Add a pair to the set.
        /// </summary>
        /// <param name="inputData">The pair to add to the set.</param>
        public virtual void Add(IMLDataPair inputData)
        {
            _data.Add(inputData);
        }

        /// <summary>
        /// Close the neural data set.
        /// </summary>
        public void Close()
        {
            // not needed
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new BasicNeuralEnumerator(this);
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BasicNeuralEnumerator(this);
        }

        /// <summary>
        /// Determine if the dataset is supervised.  It is assumed that all pairs
        /// are either supervised or not.  So we can determine the entire set by
        /// looking at the first item.  If the set is empty then return false, or
        /// unsupervised.
        /// </summary>
        public bool IsSupervised
        {
            get
            {
                if (_data.Count == 0)
                    return false;
                return (_data[0].Supervised);
            }
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public object Clone()
        {
            var result = new BasicMLDataSet();
            foreach (IMLDataPair pair in Data)
            {
                result.Add((IMLDataPair)pair.Clone());
            }
            return result;
        }

        /// <summary>
        /// The number of records in this data set.
        /// </summary>
        public int Count
        {
            get { return _data.Count; }
        }

        /// <summary>
        /// Get one record from the data set.
        /// </summary>
        /// <param name="index">The index to read.</param>
        /// <param name="pair">The pair to read into.</param>
        public void GetRecord(int index, IMLDataPair pair)
        {
            IMLDataPair source = _data[index];
            pair.InputArray = source.Input.Data;
            if (pair.IdealArray != null)
            {
                pair.IdealArray = source.Ideal.Data;
            }
        }

        /// <summary>
        /// Open an additional instance of this dataset.
        /// </summary>
        /// <returns>The new instance of this dataset.</returns>
        public IMLDataSet OpenAdditional()
        {
            return new BasicMLDataSet(Data);
        }


        /// <summary>
        /// Return true if supervised.
        /// </summary>
        public bool Supervised
        {
            get
            {
                if (_data.Count == 0)
                {
                    return false;
                }
                return _data[0].Supervised;
            }
        }

        public IMLDataPair this[int x]
        {
            get { return _data[x]; }
        }
    }
    //__________________
    [Serializable]
    public class BasicMLSequenceSet : IMLSequenceSet
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private readonly IList<IMLDataSet> _sequences = new List<IMLDataSet>();

        private IMLDataSet _currentSequence;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BasicMLSequenceSet()
        {
            _currentSequence = new BasicMLDataSet();
            _sequences.Add(_currentSequence);
        }

        public BasicMLSequenceSet(BasicMLSequenceSet other)
        {
            _sequences = other._sequences;
            _currentSequence = other._currentSequence;
        }

        /// <summary>
        /// Construct a data set from an input and ideal array.
        /// </summary>
        /// <param name="input">The input into the machine learning method for training.</param>
        /// <param name="ideal">The ideal output for training.</param>
        public BasicMLSequenceSet(double[][] input, double[][] ideal)
        {
            _currentSequence = new BasicMLDataSet(input, ideal);
            _sequences.Add(_currentSequence);
        }

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        /// <param name="theData">The data to use.</param>
        public BasicMLSequenceSet(IList<IMLDataPair> theData)
        {
            _currentSequence = new BasicMLDataSet(theData);
            _sequences.Add(_currentSequence);
        }

        /// <summary>
        /// Copy whatever dataset type is specified into a memory dataset. 
        /// </summary>
        /// <param name="set">The dataset to copy.</param>
        public BasicMLSequenceSet(IMLDataSet set)
        {
            _currentSequence = new BasicMLDataSet();
            _sequences.Add(_currentSequence);

            int inputCount = set.InputSize;
            int idealCount = set.IdealSize;

            foreach (IMLDataPair pair in set)
            {
                BasicMLData input = null;
                BasicMLData ideal = null;

                if (inputCount > 0)
                {
                    input = new BasicMLData(inputCount);
                    EngineArray.ArrayCopy(pair.InputArray, input.Data);
                }

                if (idealCount > 0)
                {
                    ideal = new BasicMLData(idealCount);
                    EngineArray.ArrayCopy(pair.IdealArray, ideal.Data);
                }

                _currentSequence.Add(new BasicMLDataPair(input, ideal));
            }
        }

        #region IMLSequenceSet Members

        /// <inheritdoc/>
        public void Add(IMLData theData)
        {
            _currentSequence.Add(theData);
        }

        /// <inheritdoc/>
        public void Add(IMLData inputData, IMLData idealData)
        {
            IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
            _currentSequence.Add(pair);
        }

        /// <inheritdoc/>
        public void Add(IMLDataPair inputData)
        {
            _currentSequence.Add(inputData);
        }

        /// <inheritdoc/>
        public void Close()
        {
            // nothing to close
        }


        /// <inheritdoc/>
        public int IdealSize
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return 0;
                }
                return _sequences[0].IdealSize;
            }
        }

        /// <inheritdoc/>
        public int InputSize
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return 0;
                }
                return _sequences[0].IdealSize;
            }
        }

        /// <inheritdoc/>
        public void GetRecord(int index, IMLDataPair pair)
        {
            int recordIndex = index;
            int sequenceIndex = 0;

            while (_sequences[sequenceIndex].Count < recordIndex)
            {
                recordIndex -= _sequences[sequenceIndex].Count;
                sequenceIndex++;
                if (sequenceIndex > _sequences.Count)
                {
                    throw new MLDataError("Record out of range: " + index);
                }
            }

            _sequences[sequenceIndex].GetRecord(recordIndex, pair);
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return _sequences.Sum(ds => ds.Count);
            }
        }

        /// <inheritdoc/>
        public bool Supervised
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return false;
                }
                return _sequences[0].Supervised;
            }
        }

        /// <inheritdoc/>
        public IMLDataSet OpenAdditional()
        {
            return new BasicMLSequenceSet(this);
        }

        public void StartNewSequence()
        {
            if (_currentSequence.Count > 0)
            {
                _currentSequence = new BasicMLDataSet();
                _sequences.Add(_currentSequence);
            }
        }

        /// <inheritdoc/>
        public int SequenceCount
        {
            get { return _sequences.Count; }
        }

        /// <inheritdoc/>
        public IMLDataSet GetSequence(int i)
        {
            return _sequences[i];
        }

        /// <inheritdoc/>
        public ICollection<IMLDataSet> Sequences
        {
            get { return _sequences; }
        }

        /// <inheritdoc/>
        public void Add(IMLDataSet sequence)
        {
            foreach (IMLDataPair pair in sequence)
            {
                Add(pair);
            }
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new BasicMLSequenceSetEnumerator(this);
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
                IMLDataPair result = BasicMLDataPair.CreatePair(InputSize, IdealSize);
                GetRecord(x, result);
                return result;
            }
        }

        #endregion

        /// <inheritdoc/>
        public Object Clone()
        {
            return ObjectCloner.DeepCopy(this);
        }

        #region Nested type: BasicMLSequenceSetEnumerator

        /// <summary>
        /// Enumerate.
        /// </summary>
        public class BasicMLSequenceSetEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BasicMLSequenceSet _owner;

            /// <summary>
            /// The index that the iterator is currently at.
            /// </summary>
            private int _currentIndex;

            /// <summary>
            /// The sequence index.
            /// </summary>
            private int _currentSequenceIndex;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicMLSequenceSetEnumerator(BasicMLSequenceSet owner)
            {
                Reset();
                _owner = owner;
            }

            #region IEnumerator<IMLDataPair> Members

            /// <summary>
            /// The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get
                {
                    if (_currentSequenceIndex >= _owner.SequenceCount)
                    {
                        throw new InvalidOperationException("Trying to read past the end of the dataset.");
                    }

                    if (_currentIndex < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner.GetSequence(_currentSequenceIndex)[_currentIndex];
                }
            }

            /// <summary>
            /// Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                // nothing needed
            }

            /// <summary>
            /// The current item.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_currentSequenceIndex >= _owner.SequenceCount)
                    {
                        throw new InvalidOperationException("Trying to read past the end of the dataset.");
                    }

                    if (_currentIndex < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner.GetSequence(_currentSequenceIndex)[_currentIndex];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                if (_currentSequenceIndex >= _owner.SequenceCount)
                {
                    return false;
                }

                IMLDataSet current = _owner.GetSequence(_currentSequenceIndex);
                _currentIndex++;

                if (_currentIndex >= current.Count)
                {
                    _currentIndex = 0;
                    _currentSequenceIndex++;
                }

                if (_currentSequenceIndex >= _owner.SequenceCount)
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                _currentIndex = -1;
                _currentSequenceIndex = 0;
            }

            #endregion
        }

        #endregion
    }

    public class ArrayDataCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The ideal array.
        /// </summary>
        private double[][] _ideal;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int _idealSize;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _index;

        /// <summary>
        /// The input array.
        /// </summary>
        private double[][] _input;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int _inputSize;

        /// <summary>
        /// Construct an array CODEC. 
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="ideal">The ideal array.</param>
        public ArrayDataCODEC(double[][] input, double[][] ideal)
        {
            _input = input;
            _ideal = ideal;
            _inputSize = input[0].Length;
            _idealSize = ideal[0].Length;
            _index = 0;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArrayDataCODEC()
        {
        }

        /// <inheritdoc/>
        public double[][] Input
        {
            get { return _input; }
        }

        /// <inheritdoc/>
        public double[][] Ideal
        {
            get { return _ideal; }
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (_index >= _input.Length)
            {
                return false;
            }
            EngineArray.ArrayCopy(_input[_index], input);
            EngineArray.ArrayCopy(_ideal[_index], ideal);
            _index++;
            significance = 1.0;
            return true;
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            EngineArray.ArrayCopy(input, _input[_index]);
            EngineArray.ArrayCopy(ideal, _ideal[_index]);
            _index++;
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            _input = EngineArray.AllocateDouble2D(recordCount, inputSize);
            _ideal = EngineArray.AllocateDouble2D(recordCount, idealSize);
            _inputSize = inputSize;
            _idealSize = idealSize;
            _index = 0;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }

    public class CSVDataCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The external CSV file.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The CSV format to use.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// True, if headers are present in the CSV file.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        /// The size of the ideal data.
        /// </summary>
        private int _idealCount;

        /// <summary>
        /// The size of the input data.
        /// </summary>
        private int _inputCount;

        /// <summary>
        /// A file used to output the CSV file.
        /// </summary>
        private TextWriter _output;

        /// <summary>
        /// The utility to assist in reading the CSV file.
        /// </summary>
        private ReadCSV _readCSV;

        /// <summary>
        /// Should a significance column be expected.
        /// </summary>
        private bool _significance;

        /// <summary>
        /// Create a CODEC to load data from CSV to binary. 
        /// </summary>
        /// <param name="file">The CSV file to load.</param>
        /// <param name="format">The format that the CSV file is in.</param>
        /// <param name="headers">True, if there are headers.</param>
        /// <param name="inputCount">The number of input columns.</param>
        /// <param name="idealCount">The number of ideal columns.</param>
        /// <param name="significance">Is there a signficance column.</param>
        public CSVDataCODEC(
            String file,
            CSVFormat format,
            bool headers,
            int inputCount, int idealCount, bool significance)
        {
            if (_inputCount != 0)
            {
                throw new BufferedDataError(
                    "To export CSV, you must use the CSVDataCODEC constructor that does not specify input or ideal sizes.");
            }
            _file = file;
            _format = format;
            _inputCount = inputCount;
            _idealCount = idealCount;
            _headers = headers;
            _significance = significance;
        }

        /// <summary>
        /// Constructor to create CSV from binary.
        /// </summary>
        /// <param name="file">The CSV file to create.</param>
        /// <param name="format">The format for that CSV file.</param>
        public CSVDataCODEC(String file, CSVFormat format, bool significance)
        {
            _file = file;
            _format = format;
            _significance = significance;
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (_readCSV.Next())
            {
                int index = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = _readCSV.GetDouble(index++);
                }

                for (int i = 0; i < ideal.Length; i++)
                {
                    ideal[i] = _readCSV.GetDouble(index++);
                }

                if (_significance)
                {
                    significance = _readCSV.GetDouble(index++);
                }
                else
                {
                    significance = 1;
                }
                return true;
            }
            return false;
        }


        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            if (_significance)
            {
                var record = new double[input.Length + ideal.Length + 1];
                EngineArray.ArrayCopy(input, record);
                EngineArray.ArrayCopy(ideal, 0, record, input.Length, ideal.Length);
                record[record.Length - 1] = significance;
                var result = new StringBuilder();
                NumberList.ToList(_format, result, record);
                _output.WriteLine(result.ToString());
            }
            else
            {
                var record = new double[input.Length + ideal.Length];
                EngineArray.ArrayCopy(input, record);
                EngineArray.ArrayCopy(ideal, 0, record, input.Length, ideal.Length);
                var result = new StringBuilder();
                NumberList.ToList(_format, result, record);
                _output.WriteLine(result.ToString());
            }
        }

        /// <summary>
        /// Prepare to write to a CSV file. 
        /// </summary>
        /// <param name="recordCount">The total record count, that will be written.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void PrepareWrite(
            int recordCount,
            int inputSize,
            int idealSize)
        {
            try
            {
                _inputCount = inputSize;
                _idealCount = idealSize;
                _output = new StreamWriter(new FileStream(_file, FileMode.Create));
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Prepare to read from the CSV file.
        /// </summary>
        public void PrepareRead()
        {
            if (_inputCount == 0)
            {
                throw new BufferedDataError(
                    "To import CSV, you must use the CSVDataCODEC constructor that specifies input and ideal sizes.");
            }
            _readCSV = new ReadCSV(_file, _headers,
                                  _format);
        }

        /// <inheritDoc/>
        public int InputSize
        {
            get { return _inputCount; }
        }

        /// <inheritDoc/>
        public int IdealSize
        {
            get { return _idealCount; }
        }

        /// <inheritDoc/>
        public void Close()
        {
            if (_readCSV != null)
            {
                _readCSV.Close();
                _readCSV = null;
            }

            if (_output != null)
            {
                _output.Close();
                _output = null;
            }
        }

        #endregion
    }

    public class NeuralDataSetCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The dataset.
        /// </summary>
        private readonly IMLDataSet _dataset;

        /// <summary>
        /// The iterator used to read through the dataset.
        /// </summary>
        private IEnumerator<IMLDataPair> _enumerator;

        /// <summary>
        /// The number of ideal elements.
        /// </summary>
        private int _idealSize;

        /// <summary>
        /// The number of input elements.
        /// </summary>
        private int _inputSize;

        /// <summary>
        /// Construct a CODEC. 
        /// </summary>
        /// <param name="dataset">The dataset to use.</param>
        public NeuralDataSetCODEC(IMLDataSet dataset)
        {
            _dataset = dataset;
            _inputSize = dataset.InputSize;
            _idealSize = dataset.IdealSize;
        }

        #region IDataSetCODEC Members

        /// <inheritdoc/>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <inheritdoc/>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <inheritdoc/>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (!_enumerator.MoveNext())
            {
                return false;
            }
            else
            {
                IMLDataPair pair = _enumerator.Current;
                EngineArray.ArrayCopy(pair.Input.Data, input);
                EngineArray.ArrayCopy(pair.Ideal.Data, ideal);
                significance = pair.Significance;
                return true;
            }
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            IMLDataPair pair = BasicMLDataPair.CreatePair(_inputSize,
                                                         _idealSize);
            EngineArray.ArrayCopy(input, pair.Input.Data);
            EngineArray.ArrayCopy(ideal, pair.Ideal.Data);
            pair.Significance = significance;
        }

        /// <inheritdoc/>
        public void PrepareWrite(int recordCount,
                                 int inputSize, int idealSize)
        {
            _inputSize = inputSize;
            _idealSize = idealSize;
        }

        /// <inheritdoc/>
        public void PrepareRead()
        {
            _enumerator = _dataset.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Close()
        {
        }

        #endregion
    }

    public class SQLCODEC : IDataSetCODEC
    {
        /// <summary>
        /// The database connection.
        /// </summary>
        private readonly DbConnection _connection;

        /// <summary>
        /// What is the size of the ideal data?
        /// </summary>
        private readonly int _idealSize;

        /// <summary>
        /// What is the size of the input data?
        /// </summary>
        private readonly int _inputSize;

        /// <summary>
        /// The SQL statement being used.
        /// </summary>
        private readonly DbCommand _statement;

        /// <summary>
        /// Holds results from the SQL query.
        /// </summary>
        private DbDataReader _results;

        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLCODEC(String sql, int inputSize,
                        int idealSize, String connectString)
        {
            _inputSize = inputSize;
            _idealSize = idealSize;
            _connection = new OleDbConnection(connectString);
            _connection.Open();
            _statement = _connection.CreateCommand();
            _statement.CommandText = sql;
            _statement.Prepare();
            _statement.Connection = _connection;
        }

        #region IDataSetCODEC Members

        /// <summary>
        /// Read a record.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="ideal">The ideal data.</param>
        /// <returns></returns>
        public bool Read(double[] input, double[] ideal, ref double significance)
        {
            if (!_results.NextResult())
                return false;

            for (int i = 1; i <= _inputSize; i++)
            {
                input[i - 1] = _results.GetDouble(i);
            }

            if (_idealSize > 0)
            {
                for (int i = 1; i <= _idealSize; i++)
                {
                    ideal[i - 1] =
                        _results.GetDouble(i + _inputSize);
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public void Write(double[] input, double[] ideal, double significance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepare to write.
        /// </summary>
        /// <param name="recordCount">The record count.</param>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void PrepareWrite(int recordCount, int inputSize, int idealSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prepare to read.
        /// </summary>
        public void PrepareRead()
        {
            _results = _statement.ExecuteReader();
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return _inputSize; }
        }

        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return _idealSize; }
        }

        /// <summary>
        /// Close the codec.
        /// </summary>
        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            if (_results != null)
            {
                _results.Close();
            }
        }

        #endregion
    }

    public class BinaryDataLoader
    {
        /// <summary>
        /// The CODEC to use.
        /// </summary>
        private readonly IDataSetCODEC _codec;

        /// <summary>
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public BinaryDataLoader(IDataSetCODEC codec)
        {
            _codec = codec;
            Status = new NullStatusReportable();
        }

        /// <summary>
        /// Used to report the status.
        /// </summary>
        public IStatusReportable Status { get; set; }

        /// <summary>
        /// The CODEC that is being used.
        /// </summary>
        public IDataSetCODEC CODEC
        {
            get { return _codec; }
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to the Synt binary
        /// training format. 
        /// </summary>
        /// <param name="binaryFile">The binary file to create.</param>
        public void External2Binary(String binaryFile)
        {
            Status.Report(0, 0, "Importing to binary file: "
                                + binaryFile);

            var egb = new SyntEGBFile(binaryFile);

            egb.Create(_codec.InputSize, _codec.IdealSize);

            var input = new double[_codec.InputSize];
            var ideal = new double[_codec.IdealSize];

            _codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;
            double significance = 0;

            while (_codec.Read(input, ideal, ref significance))
            {
                egb.Write(input);
                egb.Write(ideal);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(0, currentRecord, "Importing...");
                }
                egb.Write(significance);
            }

            egb.Close();
            _codec.Close();
            Status.Report(0, 0, "Done importing to binary file: "
                                + binaryFile);
        }

        /// <summary>
        /// Convert an Synt binary file to an external form, such as CSV. 
        /// </summary>
        /// <param name="binaryFile">THe binary file to use.</param>
        public void Binary2External(String binaryFile)
        {
            Status.Report(0, 0, "Exporting binary file: " + binaryFile);

            var egb = new SyntEGBFile(binaryFile);
            egb.Open();

            _codec.PrepareWrite(egb.NumberOfRecords, egb.InputCount,
                               egb.IdealCount);

            int inputCount = egb.InputCount;
            int idealCount = egb.IdealCount;

            var input = new double[inputCount];
            var ideal = new double[idealCount];

            int currentRecord = 0;
            int lastUpdate = 0;

            // now load the data
            for (int i = 0; i < egb.NumberOfRecords; i++)
            {
                for (int j = 0; j < inputCount; j++)
                {
                    input[j] = egb.Read();
                }

                for (int j = 0; j < idealCount; j++)
                {
                    ideal[j] = egb.Read();
                }

                double significance = egb.Read();

                _codec.Write(input, ideal, significance);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(egb.NumberOfRecords, currentRecord,
                                  "Exporting...");
                }
            }

            egb.Close();
            _codec.Close();
            Status.Report(0, 0, "Done exporting binary file: "
                                + binaryFile);
        }
    }

    public class BufferedDataError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public BufferedDataError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public BufferedDataError(Exception e)
            : base(e)
        {
        }
    }

    public class BufferedMLDataSet : IMLDataSet
    {
        /// <summary>
        /// Error message for ADD.
        /// </summary>
        public const String ErrorAdd = "Add can only be used after calling beginLoad.";

        /// <summary>
        /// True, if we are in the process of loading.
        /// </summary>
        [NonSerialized]
        private bool _loading;

        /// <summary>
        /// The file being used.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The EGB file we are working wtih.
        /// </summary>
        [NonSerialized]
        private SyntEGBFile _egb;

        /// <summary>
        /// Additional sets that were opened.
        /// </summary>
        [NonSerialized]
        private readonly IList<BufferedMLDataSet> _additional = new List<BufferedMLDataSet>();

        /// <summary>
        /// The owner.
        /// </summary>
        [NonSerialized]
        private BufferedMLDataSet _owner;


        /// <summary>
        /// Construct a buffered dataset using the specified file. 
        /// </summary>
        /// <param name="binaryFile">The file to read/write binary data to/from.</param>
        public BufferedMLDataSet(String binaryFile)
        {
            _file = binaryFile;
            _egb = new SyntEGBFile(binaryFile);
            if (File.Exists(_file))
            {
                _egb.Open();
            }
        }


        /// <summary>
        /// Create an enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            if (_loading)
            {
                throw new IMLDataError(
                    "Can't create enumerator while loading, call EndLoad first.");
            }
            var result = new BufferedNeuralDataSetEnumerator(this);
            return result;
        }


        /// <summary>
        /// Open the binary file for reading.
        /// </summary>
        public void Open()
        {
            _egb.Open();
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int Count
        {
            get
            {
                return _egb == null ? 0 : _egb.NumberOfRecords;
            }
        }

        /// <summary>
        /// Read an individual record. 
        /// </summary>
        /// <param name="index">The zero-based index. Specify 0 for the first record, 1 for
        /// the second, and so on.</param>
        /// <param name="pair">The data to read.</param>
        public void GetRecord(int index, IMLDataPair pair)
        {
            double[] inputTarget = pair.InputArray;
            double[] idealTarget = pair.IdealArray;

            _egb.SetLocation(index);
            _egb.Read(inputTarget);
            if (idealTarget != null)
            {
                _egb.Read(idealTarget);
            }
            pair.Significance = _egb.Read();
        }

        /// <summary>
        /// Open an additional training set.
        /// </summary>
        /// <returns>An additional training set.</returns>
        public IMLDataSet OpenAdditional()
        {
            var result = new BufferedMLDataSet(_file) { _owner = this };
            _additional.Add(result);
            return result;
        }

        /// <summary>
        /// Add only input data, for an unsupervised dataset. 
        /// </summary>
        /// <param name="data1">The data to be added.</param>
        public void Add(IMLData data1)
        {
            if (!_loading)
            {
                throw new IMLDataError(ErrorAdd);
            }

            _egb.Write(data1.Data);
            _egb.Write(1.0);
        }


        /// <summary>
        /// Add both the input and ideal data. 
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="idealData">The ideal data.</param>
        public void Add(IMLData inputData, IMLData idealData)
        {
            if (!_loading)
            {
                throw new IMLDataError(ErrorAdd);
            }

            _egb.Write(inputData.Data);
            _egb.Write(idealData.Data);
            _egb.Write(1.0);
        }

        /// <summary>
        /// Add a data pair of both input and ideal data. 
        /// </summary>
        /// <param name="pair">The pair to add.</param>
        public void Add(IMLDataPair pair)
        {
            if (!_loading)
            {
                throw new IMLDataError(ErrorAdd);
            }

            _egb.Write(pair.Input.Data);
            _egb.Write(pair.Ideal.Data);
            _egb.Write(pair.Significance);
        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            Object[] obj = _additional.ToArray();

            foreach (var set in obj.Cast<BufferedMLDataSet>())
            {
                set.Close();
            }

            _additional.Clear();

            if (_owner != null)
            {
                _owner.RemoveAdditional(this);
            }

            _egb.Close();
            _egb = null;
        }

        /// <summary>
        /// The ideal data size.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return _egb == null ? 0 : _egb.IdealCount;
            }
        }

        /// <summary>
        /// The input data size.
        /// </summary>
        public int InputSize
        {
            get
            {
                return _egb == null ? 0 : _egb.InputCount;
            }
        }

        /// <summary>
        /// True if this dataset is supervised.
        /// </summary>
        public bool Supervised
        {
            get
            {
                if (_egb == null)
                {
                    return false;
                }
                return _egb.IdealCount > 0;
            }
        }


        /// <summary>
        /// Remove an additional dataset that was created. 
        /// </summary>
        /// <param name="child">The additional dataset to remove.</param>
        public void RemoveAdditional(BufferedMLDataSet child)
        {
            lock (this)
            {
                _additional.Remove(child);
            }
        }

        /// <summary>
        /// Begin loading to the binary file. After calling this method the add
        /// methods may be called. 
        /// </summary>
        /// <param name="inputSize">The input size.</param>
        /// <param name="idealSize">The ideal size.</param>
        public void BeginLoad(int inputSize, int idealSize)
        {
            _egb.Create(inputSize, idealSize);
            _loading = true;
        }

        /// <summary>
        /// This method should be called once all the data has been loaded. The
        /// underlying file will be closed. The binary fill will then be opened for
        /// reading.
        /// </summary>
        public void EndLoad()
        {
            if (!_loading)
            {
                throw new BufferedDataError("Must call beginLoad, before endLoad.");
            }

            _egb.Close();
            _loading = false;
        }

        /// <summary>
        /// The binary file used.
        /// </summary>
        public String BinaryFile
        {
            get { return _file; }
        }

        /// <summary>
        /// The EGB file to use.
        /// </summary>
        public SyntEGBFile EGB
        {
            get { return _egb; }
        }

        /// <summary>
        /// Load the binary dataset to memory.  Memory access is faster. 
        /// </summary>
        /// <returns>A memory dataset.</returns>
        public IMLDataSet LoadToMemory()
        {
            var result = new BasicMLDataSet();

            foreach (IMLDataPair pair in this)
            {
                result.Add(pair);
            }

            return result;
        }

        /// <summary>
        /// Load the specified training set. 
        /// </summary>
        /// <param name="training">The training set to load.</param>
        public void Load(IMLDataSet training)
        {
            BeginLoad(training.InputSize, training.IdealSize);
            foreach (IMLDataPair pair in training)
            {
                Add(pair);
            }
            EndLoad();
        }

        /// <summary>
        /// The owner.  Set when create additional is used.
        /// </summary>
        public BufferedMLDataSet Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
                IMLDataPair result = BasicMLDataPair.CreatePair(InputSize, IdealSize);
                GetRecord(x, result);
                return result;
            }
        }
    }

    public class LinearErrorFunction : IErrorFunction
    {
        /// <inheritdoc/>
        public void CalculateError(double[] ideal, double[] actual, double[] error)
        {
            for (int i = 0; i < actual.Length; i++)
            {
                error[i] = ideal[i] - actual[i];
            }

        }
    }

    public class BufferedNeuralDataSetEnumerator : IEnumerator<IMLDataPair>
    {
        /// <summary>
        /// The dataset being iterated over.
        /// </summary>
        private readonly BufferedMLDataSet _data;

        /// <summary>
        /// The current record.
        /// </summary>
        private int _current;

        /// <summary>
        /// The current record.
        /// </summary>
        private IMLDataPair _currentRecord;

        /// <summary>
        /// Construct the buffered enumerator. This is where the file is actually
        /// opened.
        /// </summary>
        /// <param name="owner">The object that created this enumeration.</param>
        public BufferedNeuralDataSetEnumerator(BufferedMLDataSet owner)
        {
            _data = owner;
            _current = 0;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// Get the current record
        /// </summary>
        public IMLDataPair Current
        {
            get { return _currentRecord; }
        }

        /// <summary>
        /// Dispose of the enumerator.
        /// </summary>
        public void Dispose()
        {
        }


        object IEnumerator.Current
        {
            get
            {
                if (_currentRecord == null)
                {
                    throw new IMLDataError("Can't read current record until MoveNext is called once.");
                }
                return _currentRecord;
            }
        }

        /// <summary>
        /// Move to the next element.
        /// </summary>
        /// <returns>True if there are more elements to read.</returns>
        public bool MoveNext()
        {
            try
            {
                if (_current >= _data.Count)
                    return false;

                _currentRecord = BasicMLDataPair.CreatePair(_data
                                                               .InputSize, _data.IdealSize);
                _data.GetRecord(_current++, _currentRecord);
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Close the enumerator, and the underlying file.
        /// </summary>
        public void Close()
        {
        }
    }

    public class SyntEGBFile
    {
        /// <summary>
        /// The size of a double.
        /// </summary>
        public const int DoubleSize = sizeof(double);

        /// <summary>
        /// The size of the file header.
        /// </summary>
        public const int HeaderSize = DoubleSize * 3;

        /// <summary>
        /// The file that we are working with.
        /// </summary>
        private readonly String _file;

        /// <summary>
        /// The binary reader.
        /// </summary>
        private BinaryReader _binaryReader;

        /// <summary>
        /// The binary writer.
        /// </summary>
        private BinaryWriter _binaryWriter;

        /// <summary>
        /// The number of ideal values per record.
        /// </summary>
        private int _idealCount;

        /// <summary>
        /// The number of input values per record.
        /// </summary>
        private int _inputCount;

        /// <summary>
        /// The number of records int he file.
        /// </summary>
        private int _numberOfRecords;

        /// <summary>
        /// The number of values in a record, this is the input and ideal combined.
        /// </summary>
        private int _recordCount;

        /// <summary>
        /// The size of a record.
        /// </summary>
        private int _recordSize;

        /// <summary>
        /// The underlying file.
        /// </summary>
        private FileStream _stream;

        /// <summary>
        /// Construct an EGB file. 
        /// </summary>
        /// <param name="file">The file.</param>
        public SyntEGBFile(String file)
        {
            _file = file;
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return _inputCount; }
        }

        /// <summary>
        /// The ideal count.
        /// </summary>
        public int IdealCount
        {
            get { return _idealCount; }
        }

        /// <summary>
        /// The stream.
        /// </summary>
        public FileStream Stream
        {
            get { return _stream; }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int RecordCount
        {
            get { return _recordCount; }
        }

        /// <summary>
        /// The record size.
        /// </summary>
        public int RecordSize
        {
            get { return _recordSize; }
        }

        /// <summary>
        /// The number of records.
        /// </summary>
        public int NumberOfRecords
        {
            get { return _numberOfRecords; }
        }

        /// <summary>
        /// Create a new RGB file. 
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The ideal count.</param>
        public void Create(int inputCount, int idealCount)
        {
            try
            {
                _inputCount = inputCount;
                _idealCount = idealCount;

                var input = new double[inputCount];
                var ideal = new double[idealCount];

                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }

                _stream = new FileStream(_file, FileMode.Create, FileAccess.ReadWrite);
                _binaryWriter = new BinaryWriter(_stream);
                _binaryReader = null;

                _binaryWriter.Write((byte)'E');
                _binaryWriter.Write((byte)'N');
                _binaryWriter.Write((byte)'C');
                _binaryWriter.Write((byte)'O');
                _binaryWriter.Write((byte)'G');
                _binaryWriter.Write((byte)'-');
                _binaryWriter.Write((byte)'0');
                _binaryWriter.Write((byte)'0');

                _binaryWriter.Write((double)input.Length);
                _binaryWriter.Write((double)ideal.Length);

                _numberOfRecords = 0;
                _recordCount = _inputCount + _idealCount + 1;
                _recordSize = _recordCount * DoubleSize;
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Open an existing EGB file.
        /// </summary>
        public void Open()
        {
            try
            {
                _stream = new FileStream(_file, FileMode.Open, FileAccess.Read);
                _binaryReader = new BinaryReader(_stream);
                _binaryWriter = null;

                bool isSyntFile = true;

                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'E' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'N' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'C' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'O' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == 'G' : false;
                isSyntFile = isSyntFile ? _binaryReader.ReadByte() == '-' : false;

                if (!isSyntFile)
                {
                    throw new BufferedDataError(
                        "File is not a valid Synt binary file:"
                        + _file);
                }

                var v1 = (char)_binaryReader.ReadByte();
                var v2 = (char)_binaryReader.ReadByte();
                String versionStr = "" + v1 + v2;

                try
                {
                    int version = int.Parse(versionStr);
                    if (version > 0)
                    {
                        throw new BufferedDataError(
                            "File is from a newer version of Synt than is currently in use.");
                    }
                }
                catch (Exception)
                {
                    throw new BufferedDataError("File has invalid version number.");
                }

                _inputCount = (int)_binaryReader.ReadDouble();
                _idealCount = (int)_binaryReader.ReadDouble();

                _recordCount = _inputCount + _idealCount + 1;
                _recordSize = _recordCount * DoubleSize;
                _numberOfRecords = (int)((_stream.Length - HeaderSize) / _recordSize);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            try
            {
                if (_binaryWriter != null)
                {
                    _binaryWriter.Close();
                    _binaryWriter = null;
                }

                if (_binaryReader != null)
                {
                    _binaryReader.Close();
                    _binaryReader = null;
                }

                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Calculate the index for the specified row. 
        /// </summary>
        /// <param name="row">The row to calculate for.</param>
        /// <returns>The index.</returns>
        private long CalculateIndex(long row)
        {
            return (long)HeaderSize + (row * (long)_recordSize);
        }

        /// <summary>
        /// Read a row and column. 
        /// </summary>
        /// <param name="row">The row, or record, to read.</param>
        /// <param name="col">The column to read.</param>
        /// <returns>THe value read.</returns>
        private int CalculateIndex(int row, int col)
        {
            return HeaderSize + (row * _recordSize)
                   + (col * DoubleSize);
        }

        /// <summary>
        /// Set the current location to the specified row. 
        /// </summary>
        /// <param name="row">The row.</param>
        public void SetLocation(int row)
        {
            try
            {
                _stream.Position = CalculateIndex(row);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write the specified row and column. 
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The column.</param>
        /// <param name="v">The value.</param>
        public void Write(int row, int col, double v)
        {
            try
            {
                _stream.Position = CalculateIndex(row, col);
                _binaryWriter.Write(v);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write an array at the specified record.
        /// </summary>
        /// <param name="row">The record to write.</param>
        /// <param name="v">The array to write.</param>
        public void Write(int row, double[] v)
        {
            try
            {
                _stream.Position = CalculateIndex(row, 0);
                foreach (double t in v)
                {
                    _binaryWriter.Write(t);
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write an array. 
        /// </summary>
        /// <param name="v">The array to write.</param>
        public void Write(double[] v)
        {
            try
            {
                foreach (double t in v)
                {
                    _binaryWriter.Write(t);
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write a byte. 
        /// </summary>
        /// <param name="b">The byte to write.</param>
        public void Write(byte b)
        {
            try
            {
                _binaryWriter.Write(b);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Write a double. 
        /// </summary>
        /// <param name="d">The double to write.</param>
        public void Write(double d)
        {
            try
            {
                _binaryWriter.Write(d);
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a row and column. 
        /// </summary>
        /// <param name="row">The row to read.</param>
        /// <param name="col">The column to read.</param>
        /// <returns>The value read.</returns>
        public double Read(int row, int col)
        {
            try
            {
                _stream.Position = CalculateIndex(row, col);
                return _binaryReader.ReadDouble();
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a double array at the specified record. 
        /// </summary>
        /// <param name="row">The record to read.</param>
        /// <param name="d">The array to read into.</param>
        public void Read(int row, double[] d)
        {
            try
            {
                _stream.Position = CalculateIndex(row, 0);

                for (int i = 0; i < _recordCount; i++)
                {
                    d[i] = _binaryReader.ReadDouble();
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read an array of doubles. 
        /// </summary>
        /// <param name="d">The array to read into.</param>
        public void Read(double[] d)
        {
            try
            {
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = _binaryReader.ReadDouble();
                }
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }

        /// <summary>
        /// Read a single double. 
        /// </summary>
        /// <returns>The double read.</returns>
        public double Read()
        {
            try
            {
                return _binaryReader.ReadDouble();
            }
            catch (IOException ex)
            {
                throw new BufferedDataError(ex);
            }
        }
    }

    public class MemoryDataLoader
    {
        /// <summary>
        /// The CODEC to use.
        /// </summary>
        private readonly IDataSetCODEC _codec;

        /// <summary>
        /// Construct a loader with the specified CODEC. 
        /// </summary>
        /// <param name="codec">The codec to use.</param>
        public MemoryDataLoader(IDataSetCODEC codec)
        {
            _codec = codec;
            Status = new NullStatusReportable();
        }

        /// <summary>
        /// Used to report the status.
        /// </summary>
        private IStatusReportable Status { get; set; }

        /// <summary>
        /// The dataset to load to.
        /// </summary>
        public BasicMLDataSet Result { get; set; }

        /// <summary>
        /// The CODEC that is being used.
        /// </summary>
        public IDataSetCODEC CODEC
        {
            get { return _codec; }
        }

        /// <summary>
        /// Convert an external file format, such as CSV, to an Synt memory training set. 
        /// </summary>
        public IMLDataSet External2Memory()
        {
            Status.Report(0, 0, "Importing to memory");

            if (Result == null)
            {
                Result = new BasicMLDataSet();
            }

            var input = new double[_codec.InputSize];
            var ideal = new double[_codec.IdealSize];

            _codec.PrepareRead();

            int currentRecord = 0;
            int lastUpdate = 0;
            double significance = 1.0;

            while (_codec.Read(input, ideal, ref significance))
            {
                IMLData b = null;

                IMLData a = new BasicMLData(input);

                if (_codec.IdealSize > 0)
                    b = new BasicMLData(ideal);

                IMLDataPair pair = new BasicMLDataPair(a, b);
                pair.Significance = significance;
                Result.Add(pair);

                currentRecord++;
                lastUpdate++;
                if (lastUpdate >= 10000)
                {
                    lastUpdate = 0;
                    Status.Report(0, currentRecord, "Importing...");
                }
            }

            _codec.Close();
            Status.Report(0, 0, "Done importing to memory");
            return Result;
        }
    }
    public class FoldedDataSet : IMLDataSet
    {
        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String AddNotSupported = "Direct adds to the folded dataset are not supported.";

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        private readonly IMLDataSet _underlying;

        /// <summary>
        /// The fold that we are currently on.
        /// </summary>
        private int _currentFold;

        /// <summary>
        /// The offset to the current fold.
        /// </summary>
        private int _currentFoldOffset;

        /// <summary>
        /// The size of the current fold.
        /// </summary>
        private int _currentFoldSize;

        /// <summary>
        /// The size of all folds, except the last fold, the last fold may have a
        /// different number.
        /// </summary>
        private int _foldSize;

        /// <summary>
        /// The size of the last fold.
        /// </summary>
        private int _lastFoldSize;

        /// <summary>
        /// The total number of folds. Or 0 if the data has not been folded yet.
        /// </summary>
        private int _numFolds;

        /// <summary>
        /// Create a folded dataset. 
        /// </summary>
        /// <param name="underlying">The underlying folded dataset.</param>
        public FoldedDataSet(IMLDataSet underlying)
        {
            _underlying = underlying;
            Fold(1);
        }

        /// <summary>
        /// The owner object(from openAdditional)
        /// </summary>
        public FoldedDataSet Owner { get; set; }

        /// <summary>
        /// The current fold.
        /// </summary>
        public int CurrentFold
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFold;
                }
                return _currentFold;
            }
            set
            {
                if (Owner != null)
                {
                    throw new TrainingError("Can't set the fold on a non-top-level set.");
                }

                if (value >= _numFolds)
                {
                    throw new TrainingError(
                        "Can't set the current fold to be greater than the number of folds.");
                }
                _currentFold = value;
                _currentFoldOffset = _foldSize * _currentFold;

                _currentFoldSize = _currentFold == (_numFolds - 1) ? _lastFoldSize : _foldSize;
            }
        }

        /// <summary>
        /// The current fold offset.
        /// </summary>
        public int CurrentFoldOffset
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFoldOffset;
                }
                return _currentFoldOffset;
            }
        }

        /// <summary>
        /// The current fold size.
        /// </summary>
        public int CurrentFoldSize
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFoldSize;
                }
                return _currentFoldSize;
            }
        }

        /// <summary>
        /// The number of folds.
        /// </summary>
        public int NumFolds
        {
            get { return _numFolds; }
        }

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        public IMLDataSet Underlying
        {
            get { return _underlying; }
        }

        #region MLDataSet Members

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="data1">Not used.</param>
        public void Add(IMLData data1)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public void Add(IMLData inputData, IMLData idealData)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public void Add(IMLDataPair inputData)
        {
            throw new TrainingError(AddNotSupported);
        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            _underlying.Close();
        }


        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return _underlying.IdealSize; }
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return _underlying.InputSize; }
        }

        /// <summary>
        /// Get a record.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="pair">The record.</param>
        public void GetRecord(int index, IMLDataPair pair)
        {
            _underlying.GetRecord(CurrentFoldOffset + index, pair);
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int Count
        {
            get { return CurrentFoldSize; }
        }

        /// <summary>
        /// True if this is a supervised set.
        /// </summary>
        public bool Supervised
        {
            get { return _underlying.Supervised; }
        }


        /// <summary>
        /// Open an additional dataset.
        /// </summary>
        /// <returns>The dataset.</returns>
        public IMLDataSet OpenAdditional()
        {
            var folded = new FoldedDataSet(_underlying.OpenAdditional()) { Owner = this };
            return folded;
        }


        /// <summary>
        /// Get an enumberator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new FoldedEnumerator(this);
        }

        #endregion

        /// <summary>
        /// Fold the dataset. Must be done before the dataset is used. 
        /// </summary>
        /// <param name="numFolds">The number of folds.</param>
        public void Fold(int numFolds)
        {
            _numFolds = Math.Min(numFolds, _underlying
                                               .Count);
            _foldSize = _underlying.Count / _numFolds;
            _lastFoldSize = _underlying.Count - (_foldSize * _numFolds);
            CurrentFold = 0;
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
                IMLDataPair result = BasicMLDataPair.CreatePair(InputSize, IdealSize);
                this.GetRecord(x, result);
                return result;
            }
        }
    }

    public class FoldedEnumerator : IEnumerator<IMLDataPair>
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private readonly FoldedDataSet _owner;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// The current data item.
        /// </summary>
        private IMLDataPair _currentPair;

        /// <summary>
        /// Construct an enumerator.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public FoldedEnumerator(FoldedDataSet owner)
        {
            _owner = owner;
            _currentIndex = -1;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// The current object.
        /// </summary>
        public IMLDataPair Current
        {
            get
            {
                if (_currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return _currentPair;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move to the next record.
        /// </summary>
        /// <returns>True, if we were able to move to the next record.</returns>
        public bool MoveNext()
        {
            if (HasNext())
            {
                IMLDataPair pair = BasicMLDataPair.CreatePair(
                    _owner.InputSize, _owner.IdealSize);
                _owner.GetRecord(_currentIndex++, pair);
                _currentPair = pair;
                return true;
            }
            _currentPair = null;
            return false;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The current object.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                if (_currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return _currentPair;
            }
        }

        #endregion

        /// <summary>
        /// Determine if there is a next record.
        /// </summary>
        /// <returns>True, if there is a next record.</returns>
        public bool HasNext()
        {
            return _currentIndex < _owner.CurrentFoldSize;
        }
    }

    public class ImageMLData : BasicMLData
    {
        /// <summary>
        /// Construct an object based on an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        public ImageMLData(Bitmap image)
            : base(1)
        {
            Image = image;
        }

        /// <summary>
        /// The image associated with this class.
        /// </summary>
        public Bitmap Image { get; set; }


        /// <summary>
        /// Downsample, and copy, the image contents into the data of this object.
        /// Calling this method has no effect on the image, as the same image can be
        /// downsampled multiple times to different resolutions.
        /// </summary>
        /// <param name="downsampler">The downsampler object to use.</param>
        /// <param name="findBounds">Should the bounds be located and cropped.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public void Downsample(IDownSample downsampler,
                               bool findBounds, int height, int width,
                               double hi, double lo)
        {
     
        }

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        /// <returns>The string form of this object.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder("[ImageNeuralData:");
            for (int i = 0; i < Data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(Data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }

    public class ImageMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Error message to inform the caller that only ImageNeuralData objects can
        /// be used with this collection.
        /// </summary>
        public const String MUST_USE_IMAGE =
            "This data set only supports ImageNeuralData or Image objects.";

        /// <summary>
        /// The downsampler to use.
        /// </summary>
        private readonly IDownSample downsampler;

        /// <summary>
        /// Should the bounds be found and cropped.
        /// </summary>
        private readonly bool findBounds;

        /// <summary>
        /// The high value to normalize to.
        /// </summary>
        private readonly double hi;

        /// <summary>
        /// The low value to normalize to.
        /// </summary>
        private readonly double lo;

        /// <summary>
        /// The height to downsample to.
        /// </summary>
        private int height;

        /// <summary>
        /// The width to downsample to.
        /// </summary>
        private int width;


        /// <summary>
        /// Construct this class with the specified downsampler.
        /// </summary>
        /// <param name="downsampler">The downsampler to use.</param>
        /// <param name="findBounds">Should the bounds be found and clipped.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public ImageMLDataSet(IDownSample downsampler,
                              bool findBounds, double hi, double lo)
        {
            this.downsampler = downsampler;
            this.findBounds = findBounds;
            height = -1;
            width = -1;
            this.hi = hi;
            this.lo = lo;
        }

        /// <summary>
        /// The height.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// The width.
        /// </summary>
        public int Width
        {
            get { return width; }
        }


        /// <summary>
        /// Add the specified data, must be an ImageNeuralData class.
        /// </summary>
        /// <param name="data">The data The object to add.</param>
        public override void Add(IMLData data)
        {
            if (!(data is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(data);
        }

        /// <summary>
        /// Add the specified input and ideal object to the collection.
        /// </summary>
        /// <param name="inputData">The image to train with.</param>
        /// <param name="idealData">The expected otuput form this image.</param>
        public override void Add(IMLData inputData, IMLData idealData)
        {
            if (!(inputData is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(inputData, idealData);
        }

        /// <summary>
        /// Add input and expected output. This is used for supervised training.
        /// </summary>
        /// <param name="inputData">The input data to train on.</param>
        public override void Add(IMLDataPair inputData)
        {
            if (!(inputData.Input is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(inputData);
        }


        /// <summary>
        /// Downsample all images and generate training data.
        /// </summary>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        public void Downsample(int height, int width)
        {
            this.height = height;
            this.width = width;

            foreach (IMLDataPair pair in this)
            {
                if (!(pair.Input is ImageMLData))
                {
                    throw new NeuralNetworkError(
                        "Invalid class type found in ImageNeuralDataSet, only "
                        + "ImageNeuralData items are allowed.");
                }

                var input = (ImageMLData)pair.Input;
                input.Downsample(downsampler, findBounds, height, width,
                                 hi, lo);
            }
        }
    }

    public class CSVFinal : IMarketLoader
    {

        #region IMarketLoader Members


        string Precision { get; set; }
        public static string LoadedFile { get; set; }


        /// <summary>
        /// Reads the CSV and call loader.
        /// Used internally to load the csv and place data in the marketdataset.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="neededTypes">The needed types.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="File">The file.</param>
        /// <returns></returns>
        ICollection<LoadedMarketData> ReadAndCallLoader(
            TickerSymbol symbol,
            IEnumerable<MarketDataType> neededTypes,
            DateTime from,
            DateTime to,
            string File)
        {
            //We got a file, lets load it.

            ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
            ReadCSV csv = new ReadCSV(File, true, CSVFormat.English);
            //In case we want to use a different date format...and have used the SetDateFormat method, our DateFormat must then not be null..
            //We will use the ?? operator to check for nullables.
            csv.DateFormat = DateFormat ?? "yyyy-MM-dd HH:mm:ss";
            csv.TimeFormat = "HH:mm:ss";

            DateTime ParsedDate = from;
            bool writeonce = true;

            while (csv.Next())
            {
                DateTime date = csv.GetDate(0);
                ParsedDate = date;

                if (writeonce)
                {
                    Console.WriteLine(@"First parsed date in csv:" + ParsedDate.ToShortDateString());
                    Console.WriteLine(@"Stopping at date:" + to.ToShortDateString());
                    Console.WriteLine(@"Current DateTime:" + ParsedDate.ToShortDateString() + @" Time:" +
                                      ParsedDate.ToShortTimeString() + @"  Asked Start date was " +
                                      from.ToShortDateString());
                    writeonce = false;
                }
                if (ParsedDate >= from && ParsedDate <= to)
                {
                    DateTime datex = csv.GetDate(0);
                    double open = csv.GetDouble(1);
                    double close = csv.GetDouble(2);
                    double high = csv.GetDouble(3);
                    double low = csv.GetDouble(4);
                    double volume = csv.GetDouble(5);
                    double range = Math.Abs(open - close);
                    double HighLowRange = Math.Abs(high - low);
                    double DirectionalRange = close - open;
                    LoadedMarketData data = new LoadedMarketData(datex, symbol);
                    data.SetData(MarketDataType.Open, open);
                    data.SetData(MarketDataType.High, high);
                    data.SetData(MarketDataType.Low, low);
                    data.SetData(MarketDataType.Close, close);
                    data.SetData(MarketDataType.Volume, volume);
                    data.SetData(MarketDataType.RangeHighLow, Math.Round(HighLowRange, 6));
                    data.SetData(MarketDataType.RangeOpenClose, Math.Round(range, 6));
                    data.SetData(MarketDataType.RangeOpenCloseNonAbsolute, Math.Round(DirectionalRange, 6));
                    result.Add(data);


                }

            }

            csv.Close();
            return result;
        }

        /// <summary>
        /// Gets or sets the date format for the whole csv file.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string DateFormat { get; set; }
        /// <summary>
        /// Sets the date format for the csv file.
        /// </summary>
        /// <param name="stringFormat">The string format.</param>
        public void SetDateFormat(string stringFormat)
        {
            DateFormat = stringFormat;
            return;
        }

        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {


            return File.Exists(LoadedFile) ? (ReadAndCallLoader(ticker, dataNeeded, from, to, LoadedFile)) : null;
        }



        #endregion

        #region IMarketLoader Members




        #endregion

        #region IMarketLoader Members


        /// <summary>
        /// Gets the file we want to parse.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public string GetFile(string file)
        {
            if (File.Exists(file))
                LoadedFile = file;
            return LoadedFile;
        }

        #endregion

    }

    public class CSVLoader : IMarketLoader
    {

        #region IMarketLoader Members  


        string Precision { get; set; }
        string LoadedFile { get; set; }
        public List<MarketDataType> TypesLoaded = new List<MarketDataType>();
        CSVFormat LoadedFormat { get; set; }
        string DateTimeFormat { get; set; }

        public ICollection<LoadedMarketData> ReadAndCallLoader(TickerSymbol symbol, IList<MarketDataType> neededTypes, DateTime from, DateTime to, string File)
        {
            try
            {


                //We got a file, lets load it.



                ICollection<LoadedMarketData> result = new List<LoadedMarketData>();
                ReadCSV csv = new ReadCSV(File, true, LoadedFormat);


                csv.DateFormat = DateTimeFormat.Normalize();
                //  Time,Open,High,Low,Close,Volume
                while (csv.Next())
                {
                    DateTime date = csv.GetDate("Time");
                    double open = csv.GetDouble("Open");
                    double close = csv.GetDouble("High");
                    double high = csv.GetDouble("Low");
                    double low = csv.GetDouble("Close");
                    double volume = csv.GetDouble("Volume");
                    LoadedMarketData data = new LoadedMarketData(date, symbol);
                    data.SetData(MarketDataType.Open, open);
                    data.SetData(MarketDataType.High, high);
                    data.SetData(MarketDataType.Low, low);
                    data.SetData(MarketDataType.Close, close);
                    data.SetData(MarketDataType.Volume, volume);
                    result.Add(data);
                }

                csv.Close();
                return result;
            }

            catch (Exception ex)
            {

                Console.WriteLine("Something went wrong reading the csv");
                Console.WriteLine("Something went wrong reading the csv:" + ex.Message);
            }

            Console.WriteLine("Something went wrong reading the csv");
            return null;
        }

        public CSVFormat fromStringCSVFormattoCSVFormat(string csvformat)
        {
            switch (csvformat)
            {
                case "Decimal Point":
                    LoadedFormat = CSVFormat.DecimalPoint;
                    break;
                case "Decimal Comma":
                    LoadedFormat = CSVFormat.DecimalComma;
                    break;
                case "English Format":
                    LoadedFormat = CSVFormat.English;
                    break;
                case "EG Format":
                    LoadedFormat = CSVFormat.EgFormat;
                    break;

                default:
                    break;
            }

            return LoadedFormat;
        }

        public ICollection<LoadedMarketData> Load(TickerSymbol ticker, IList<MarketDataType> dataNeeded, DateTime from, DateTime to)
        {
            //ICollection<LoadedMarketData> result = new List<LoadedMarketData>();


            //CSVFormLoader formLoader = new CSVFormLoader();
            //if (File.Exists(formLoader.Chosenfile))
            //{
            //    LoadedFormat = formLoader.format;

            //    //Lets add all the marketdatatypes we selected in the form.
            //    foreach (MarketDataType item in formLoader.TypesLoaded)
            //    {
            //        TypesLoaded.Add(item);

            //    }
            //    DateTimeFormat = formLoader.DateTimeFormatTextBox.Text;
            //    result = ReadAndCallLoader(ticker, dataNeeded, from, to, formLoader.Chosenfile);
            //    return result;
            //}


            return null;
        }



        #endregion

        #region IMarketLoader Members


        public string GetFile(string file)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class LoadedMarketData : IComparable<LoadedMarketData>
    {
        /// <summary>
        /// The data that was collection for the sample date.
        /// </summary>
        private readonly IDictionary<MarketDataType, Double> _data;

        /// <summary>
        /// What is the ticker symbol for this data sample.
        /// </summary>
        private readonly TickerSymbol _ticker;

        /// <summary>
        /// Construct one sample of market data.
        /// </summary>
        /// <param name="when">When was this sample taken.</param>
        /// <param name="ticker">What is the ticker symbol for this data.</param>
        public LoadedMarketData(DateTime when, TickerSymbol ticker)
        {
            When = when;
            _ticker = ticker;
            _data = new Dictionary<MarketDataType, Double>();
        }

        /// <summary>
        /// When is this data from.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// The ticker symbol that this data was from.
        /// </summary>
        public TickerSymbol Ticker
        {
            get { return _ticker; }
        }

        /// <summary>
        /// The data that was downloaded.
        /// </summary>
        public IDictionary<MarketDataType, Double> Data
        {
            get { return _data; }
        }

        #region IComparable<LoadedMarketData> Members

        /// <summary>
        /// Compare this object with another of the same type.
        /// </summary>
        /// <param name="other">The other object to compare.</param>
        /// <returns>Zero if equal, greater or less than zero to indicate order.</returns>
        public int CompareTo(LoadedMarketData other)
        {
            return When.CompareTo(other.When);
        }

        #endregion

        /// <summary>
        /// Set the specified type of data.
        /// </summary>
        /// <param name="t">The type of data to set.</param>
        /// <param name="d">The value to set.</param>
        public void SetData(MarketDataType t, double d)
        {
            _data[t] = d;
        }

        /// <summary>
        /// Get the specified data type.
        /// </summary>
        /// <param name="t">The type of data to get.</param>
        /// <returns>The value.</returns>
        public double GetData(MarketDataType t)
        {
            return _data[t];
        }
    }

    public class LoaderError : MarketError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public LoaderError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public LoaderError(Exception e)
            : base(e)
        {
        }
    }

    public class YahooFinanceLoader : IMarketLoader
    {
        #region IMarketLoader Members

        /// <summary>
        /// Load the specified financial data. 
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="dataNeeded">The financial data needed.</param>
        /// <param name="from">The beginning date to load data from.</param>
        /// <param name="to">The ending date to load data to.</param>
        /// <returns>A collection of LoadedMarketData objects that represent the data
        /// loaded.</returns>
        public ICollection<LoadedMarketData> Load(TickerSymbol ticker,
                                                  IList<MarketDataType> dataNeeded, DateTime from,
                                                  DateTime to)
        {
            ICollection<LoadedMarketData> result =
                new List<LoadedMarketData>();
            //Uri url = BuildURL(ticker, from, to);
            //WebRequest http = WebRequest.Create(url);
            //var response = (HttpWebResponse)http.GetResponse();

            //using (Stream istream = response.GetResponseStream())
            //{
            //    var csv = new ReadCSV(istream, true, CSVFormat.DecimalPoint);

            //    while (csv.Next())
            //    {
            //        DateTime date = csv.GetDate("date");
            //        double adjClose = csv.GetDouble("adj close");
            //        double open = csv.GetDouble("open");
            //        double close = csv.GetDouble("close");
            //        double high = csv.GetDouble("high");
            //        double low = csv.GetDouble("low");
            //        double volume = csv.GetDouble("volume");

            //        var data =
            //            new LoadedMarketData(date, ticker);
            //        data.SetData(MarketDataType.AdjustedClose, adjClose);
            //        data.SetData(MarketDataType.Open, open);
            //        data.SetData(MarketDataType.Close, close);
            //        data.SetData(MarketDataType.High, high);
            //        data.SetData(MarketDataType.Low, low);
            //        data.SetData(MarketDataType.Open, open);
            //        data.SetData(MarketDataType.Volume, volume);
            //        result.Add(data);
            //    }

            //    csv.Close();
            //    istream.Close();
            //}
            return result;
        }

        #endregion

        /// <summary>
        /// This method builds a URL to load data from Yahoo Finance for a neural
        /// network to train with.
        /// </summary>
        /// <param name="ticker">The ticker symbol to access.</param>
        /// <param name="from">The beginning date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns>The URL to read from</returns>
        private static Uri BuildURL(TickerSymbol ticker, DateTime from,
                             DateTime to)
        {
            // construct the URL
            var mstream = new MemoryStream();
            var form = new FormUtility(mstream, null);

            form.Add("s", ticker.Symbol.ToUpper());
            form.Add("a", "" + (from.Month - 1));
            form.Add("b", "" + from.Day);
            form.Add("c", "" + from.Year);
            form.Add("d", "" + (to.Month - 1));
            form.Add("e", "" + to.Day);
            form.Add("f", "" + to.Year);
            form.Add("g", "d");
            form.Add("ignore", ".csv");
            mstream.Close();
            byte[] b = mstream.GetBuffer();

            String str = "http://ichart.finance.yahoo.com/table.csv?"
                         + StringUtil.FromBytes(b);
            return new Uri(str);
        }

        #region IMarketLoader Members


        public string GetFile(string file)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class MarketDataDescription : TemporalDataDescription
    {
        /// <summary>
        /// The type of data to be loaded from the specified ticker symbol.
        /// </summary>
        private readonly MarketDataType _dataType;

        /// <summary>
        /// The ticker symbol to be loaded.
        /// </summary>
        private readonly TickerSymbol _ticker;

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="activationFunction"> The activation function to apply to this data, can be null.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, Type type,
                                     IActivationFunction activationFunction, bool input,
                                     bool predict)
            : base(activationFunction, type, input, predict)
        {
            _ticker = ticker;
            _dataType = dataType;
        }


        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, Type type, bool input,
                                     bool predict)
            : this(ticker, dataType, type, null, input, predict)
        {
        }

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, bool input,
                                     bool predict)
            : this(ticker, dataType, Type.PercentChange, null, input, predict)
        {
        }

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        public TickerSymbol Ticker
        {
            get { return _ticker; }
        }

        /// <summary>
        /// The data type that this is.
        /// </summary>
        public MarketDataType DataType
        {
            get { return _dataType; }
        }
    }

    public class MarketError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public MarketError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public MarketError(Exception e)
            : base(e)
        {
        }
    }

    public sealed class MarketMLDataSet : TemporalMLDataSet
    {
        /// <summary>
        /// The loader to use to obtain the data.
        /// </summary>
        private readonly IMarketLoader _loader;

        /// <summary>
        /// A map between the data points and actual data.
        /// </summary>
        private readonly IDictionary<Int64, TemporalPoint> _pointIndex =
            new Dictionary<Int64, TemporalPoint>();

        /// <summary>
        /// Construct a market data set object.
        /// </summary>
        /// <param name="loader">The loader to use to get the financial data.</param>
        /// <param name="inputWindowSize">The input window size, that is how many datapoints do we use to predict.</param>
        /// <param name="predictWindowSize">How many datapoints do we want to predict.</param>
        public MarketMLDataSet(IMarketLoader loader, Int64 inputWindowSize, Int64 predictWindowSize)
            : base((int)inputWindowSize, (int)predictWindowSize)
        {
            _loader = loader;
            SequenceGrandularity = TimeUnit.Days;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketMLDataSet"/> class.
        /// </summary>
        /// <param name="loader">The loader.</param>
        /// <param name="inputWindowSize">Size of the input window.</param>
        /// <param name="predictWindowSize">Size of the predict window.</param>
        /// <param name="unit">The time unit to use.</param>
        public MarketMLDataSet(IMarketLoader loader, Int64 inputWindowSize, Int64 predictWindowSize, TimeUnit unit)
            : base((int)inputWindowSize, (int)predictWindowSize)
        {

            _loader = loader;
            SequenceGrandularity = unit;
        }

        /// <summary>
        /// The loader that is being used for this set.
        /// </summary>
        public IMarketLoader Loader
        {
            get { return _loader; }
        }

        /// <summary>
        /// Add one description of the type of market data that we are seeking at
        /// each datapoint.
        /// </summary>
        /// <param name="desc"></param>
        public override void AddDescription(TemporalDataDescription desc)
        {
            if (!(desc is MarketDataDescription))
            {
                throw new MarketError(
                    "Only MarketDataDescription objects may be used "
                    + "with the MarketMLDataSet container.");
            }
            base.AddDescription(desc);
        }


        /// <summary>
        /// Create a datapoint at the specified date.
        /// </summary>
        /// <param name="when">The date to create the point at.</param>
        /// <returns>Returns the TemporalPoint created for the specified date.</returns>
        public override TemporalPoint CreatePoint(DateTime when)
        {
            Int64 sequence = (Int64)GetSequenceFromDate(when);
            TemporalPoint result;

            if (_pointIndex.ContainsKey(sequence))
            {
                result = _pointIndex[sequence];
            }
            else
            {
                result = base.CreatePoint(when);
                _pointIndex[(int)result.Sequence] = result;
            }

            return result;
        }


        /// <summary>
        /// Load data from the loader.
        /// </summary>
        /// <param name="begin">The beginning date.</param>
        /// <param name="end">The ending date.</param>
        public void Load(DateTime begin, DateTime end)
        {
            // define the starting point if it is not already defined
            if (StartingPoint == DateTime.MinValue)
            {
                StartingPoint = begin;
            }

            // clear out any loaded points
            Points.Clear();

            // first obtain a collection of symbols that need to be looked up
            IDictionary<TickerSymbol, object> symbolSet = new Dictionary<TickerSymbol, object>();
            foreach (MarketDataDescription desc in Descriptions)
            {
                if (symbolSet.Count == 0)
                {
                    symbolSet[desc.Ticker] = null;
                }
                foreach (TickerSymbol ts in symbolSet.Keys)
                {
                    if (!ts.Equals(desc.Ticker))
                    {
                        symbolSet[desc.Ticker] = null;
                        break;
                    }
                }
            }

            // now loop over each symbol and load the data
            foreach (TickerSymbol symbol in symbolSet.Keys)
            {
                LoadSymbol(symbol, begin, end);
            }

            // resort the points
            SortPoints();
        }


        /// <summary>
        /// Load one point of market data.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="point">The point to load at.</param>
        /// <param name="item">The item being loaded.</param>
        private void LoadPointFromMarketData(TickerSymbol ticker,
                                             TemporalPoint point, LoadedMarketData item)
        {
            foreach (TemporalDataDescription desc in Descriptions)
            {
                var mdesc = (MarketDataDescription)desc;

                if (mdesc.Ticker.Equals(ticker))
                {
                    point.Data[mdesc.Index] = item.Data[mdesc.DataType];
                }
            }
        }

        /// <summary>
        /// Load one ticker symbol.
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="from">Load data from this date.</param>
        /// <param name="to">Load data to this date.</param>
        private void LoadSymbol(TickerSymbol ticker, DateTime from,
                                DateTime to)
        {
            IList<MarketDataType> types = new List<MarketDataType>();
            foreach (MarketDataDescription desc in Descriptions)
            {
                if (desc.Ticker.Equals(ticker))
                {
                    types.Add(desc.DataType);
                }
            }
            ICollection<LoadedMarketData> data = Loader.Load(ticker, types, from, to);
            foreach (LoadedMarketData item in data)
            {
                TemporalPoint point = CreatePoint(item.When);

                LoadPointFromMarketData(ticker, point, item);
            }
        }
    }

    public class MarketPoint : TemporalPoint
    {
        /// <summary>
        /// When to hold the data from.
        /// </summary>
        private readonly DateTime _when;


        /// <summary>
        /// Construct a MarketPoint with the specified date and size.
        /// </summary>
        /// <param name="when">When is this data from.</param>
        /// <param name="size">What is the size of the data.</param>
        public MarketPoint(DateTime when, int size)
            : base(size)
        {
            _when = when;
        }

        /// <summary>
        /// When is this point from.
        /// </summary>
        public DateTime When
        {
            get { return _when; }
        }
    }

    public class TickerSymbol
    {
        /// <summary>
        /// The exchange.
        /// </summary>
        private readonly String _exchange;

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        private readonly String _symbol;


        /// <summary>
        /// Construct a ticker symbol with no exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol</param>
        public TickerSymbol(String symbol)
        {
            _symbol = symbol;
            _exchange = null;
        }

        /// <summary>
        /// Construct a ticker symbol with exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol.</param>
        /// <param name="exchange">The exchange.</param>
        public TickerSymbol(String symbol, String exchange)
        {
            _symbol = symbol;
            _exchange = exchange;
        }

        /// <summary>
        /// The stock symbol.
        /// </summary>
        public String Symbol
        {
            get { return _symbol; }
        }

        /// <summary>
        /// The exchange that this stock is on.
        /// </summary>
        public String Exchange
        {
            get { return _exchange; }
        }


        /// <summary>
        /// Determine if two ticker symbols equal each other.
        /// </summary>
        /// <param name="other">The other ticker symbol.</param>
        /// <returns>True if the two symbols equal.</returns>
        public bool Equals(TickerSymbol other)
        {
            // if the symbols do not even match then they are not equal
            if (!other.Symbol.Equals(this.Symbol))
            {
                return false;
            }

            // if the symbols match then we need to compare the exchanges
            if (other.Exchange == null && other.Exchange == null)
            {
                return true;
            }

            if (other.Exchange == null || this.Exchange == null)
            {
                return false;
            }

            return other.Exchange.Equals(this.Exchange);
        }
    }

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

    public class CSVMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// The CSV filename to read from.
        /// </summary>
        private readonly String _filename;

        /// <summary>
        /// The format that separates the columns, defaults to a comma.
        /// </summary>
        private readonly CSVFormat _format;

        /// <summary>
        /// Specifies if headers are present on the first row.
        /// </summary>
        private readonly bool _headers;

        /// <summary>
        /// The number of columns of ideal data.
        /// </summary>
        private readonly int _idealSize;

        /// <summary>
        /// The number of columns of input data.
        /// </summary>
        private readonly int _inputSize;

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        public CSVMLDataSet(String filename, int inputSize,
                            int idealSize, bool headers)
            : this(filename, inputSize, idealSize, headers, CSVFormat.English, false)
        {
        }

        /// <summary>
        /// Construct this data set using a comma as a delimiter.
        /// </summary>
        /// <param name="filename">The CSV filename to read.</param>
        /// <param name="inputSize">The number of columns that make up the input set.</param>
        /// <param name="idealSize">The number of columns that make up the ideal set.</param>
        /// <param name="headers">True if headers are present on the first line.</param>
        /// <param name="format">The format to use.</param>
        public CSVMLDataSet(String filename, int inputSize,
                            int idealSize, bool headers, CSVFormat format, bool expectSignificance)
        {
            _filename = filename;
            _inputSize = inputSize;
            _idealSize = idealSize;
            _format = format;
            _headers = headers;

            IDataSetCODEC codec = new CSVDataCODEC(filename, format, headers, inputSize, idealSize, expectSignificance);
            var load = new MemoryDataLoader(codec) { Result = this };
            load.External2Memory();
        }

        /// <summary>
        /// Get the filename for the CSV file.
        /// </summary>
        public String Filename
        {
            get { return _filename; }
        }

        /// <summary>
        /// The delimiter.
        /// </summary>
        public CSVFormat Format
        {
            get { return _format; }
        }

        /// <summary>
        /// True if the first row specifies field names.
        /// </summary>
        public bool Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// The amount of ideal data.
        /// </summary>
        public override int IdealSize
        {
            get { return _idealSize; }
        }

        /// <summary>
        /// The amount of input data.
        /// </summary>
        public override int InputSize
        {
            get { return _inputSize; }
        }
    }

    public class SQLMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLMLDataSet(String sql, int inputSize,
                            int idealSize, String connectString)
        {
            IDataSetCODEC codec = new SQLCODEC(sql, inputSize, idealSize, connectString);
            var load = new MemoryDataLoader(codec) { Result = this };
            load.External2Memory();
        }
    }

    public class TemporalDataDescription
    {
        #region Type enum

        /// <summary>
        /// The type of data requested.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Data in its raw, unmodified form.
            /// </summary>
            Raw,
            /// <summary>
            /// The percent change.
            /// </summary>
            PercentChange,
            /// <summary>
            /// The difference change.
            /// </summary>
            DeltaChange,
        }

        #endregion

        /// <summary>
        /// Should an activation function be used?
        /// </summary>
        private readonly IActivationFunction _activationFunction;

        /// <summary>
        /// What type of data is requested?
        /// </summary>
        private readonly Type _type;

        /// <summary>
        /// Construct a data description item. Set both low and high to zero for
        /// unbounded.
        /// </summary>
        /// <param name="activationFunction">What activation function should be used?</param>
        /// <param name="low">What is the lowest allowed value.</param>
        /// <param name="high">What is the highest allowed value.</param>
        /// <param name="type">What type of data is this.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                                       double low, double high, Type type,
                                       bool input, bool predict)
        {
            Low = low;
            _type = type;
            High = high;
            IsInput = input;
            IsPredict = predict;
            _activationFunction = activationFunction;
        }

        /// <summary>
        /// Construct a data description with an activation function, but no range.
        /// </summary>
        /// <param name="activationFunction">The activation function.</param>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(IActivationFunction activationFunction,
                                       Type type, bool input, bool predict)
            : this(activationFunction, 0, 0, type, input, predict)
        {
        }

        /// <summary>
        /// Construct a data description with no activation function or range.
        /// </summary>
        /// <param name="type">The type of data.</param>
        /// <param name="input">Used for input?</param>
        /// <param name="predict">Used for prediction?</param>
        public TemporalDataDescription(Type type, bool input,
                                       bool predict)
            : this(null, 0, 0, type, input, predict)
        {
        }

        /// <summary>
        /// The lowest allowed data.
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// The highest allowed value.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Is this data input?  Or is it to be predicted.
        /// </summary>
        public bool IsInput { get; set; }

        /// <summary>
        /// Determine if this is a predicted value.
        /// </summary>
        public bool IsPredict { get; set; }

        /// <summary>
        /// Get the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The type of data this is.
        /// </summary>
        public Type DescriptionType
        {
            get { return _type; }
        }

        /// <summary>
        /// The activation function for this layer.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get { return _activationFunction; }
        }
    }

    public class TemporalError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public TemporalError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public TemporalError(Exception e)
            : base(e)
        {
        }
    }

    public class TemporalMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String AddNotSupported = "Direct adds to the temporal dataset are not supported.  "
                                                + "Add TemporalPoint objects and call generate.";

        /// <summary>
        /// Descriptions of the data needed.
        /// </summary>
        private readonly IList<TemporalDataDescription> _descriptions =
            new List<TemporalDataDescription>();

        /// <summary>
        /// The temporal points at which we have data.
        /// </summary>
        private readonly List<TemporalPoint> _points = new List<TemporalPoint>();

        /// <summary>
        /// How big would we like the input size to be.
        /// </summary>
        private int _desiredSetSize;

        /// <summary>
        /// The highest sequence.
        /// </summary>
        private int _highSequence;

        /// <summary>
        /// How many input neurons will be used.
        /// </summary>
        private int _inputNeuronCount;

        /// <summary>
        /// The size of the input window, this is the data being used to predict.
        /// </summary>
        private int _inputWindowSize;

        /// <summary>
        /// The lowest sequence.
        /// </summary>
        private int _lowSequence;

        /// <summary>
        /// How many output neurons will there be.
        /// </summary>
        private int _outputNeuronCount;

        /// <summary>
        /// The size of the prediction window.
        /// </summary>
        private int _predictWindowSize;

        /// <summary>
        /// What is the granularity of the temporal points? Days, months, years,
        /// etc?
        /// </summary>
        private TimeUnit _sequenceGrandularity;

        /// <summary>
        /// What is the date for the first temporal point.
        /// </summary>
        private DateTime _startingPoint = DateTime.MinValue;

        /// <summary>
        /// Construct a dataset.
        /// </summary>
        /// <param name="inputWindowSize">What is the input window size.</param>
        /// <param name="predictWindowSize">What is the prediction window size.</param>
        public TemporalMLDataSet(int inputWindowSize,
                                     int predictWindowSize)
        {
            _inputWindowSize = inputWindowSize;
            _predictWindowSize = predictWindowSize;
            _lowSequence = int.MinValue;
            _highSequence = int.MaxValue;
            _desiredSetSize = int.MaxValue;
            _startingPoint = DateTime.MinValue;
            _sequenceGrandularity = TimeUnit.Days;
        }


        /// <summary>
        /// The data descriptions.
        /// </summary>
        public virtual IList<TemporalDataDescription> Descriptions
        {
            get { return _descriptions; }
        }

        /// <summary>
        /// The points, or time slices to take data from.
        /// </summary>
        public virtual IList<TemporalPoint> Points
        {
            get { return _points; }
        }

        /// <summary>
        /// Get the size of the input window.
        /// </summary>
        public virtual int InputWindowSize
        {
            get { return _inputWindowSize; }
            set { _inputWindowSize = value; }
        }

        /// <summary>
        /// The prediction window size.
        /// </summary>
        public virtual int PredictWindowSize
        {
            get { return _predictWindowSize; }
            set { _predictWindowSize = value; }
        }

        /// <summary>
        /// The low value for the sequence.
        /// </summary>
        public virtual int LowSequence
        {
            get { return _lowSequence; }
            set { _lowSequence = value; }
        }

        /// <summary>
        /// The high value for the sequence.
        /// </summary>
        public virtual int HighSequence
        {
            get { return _highSequence; }
            set { _highSequence = value; }
        }

        /// <summary>
        /// The desired dataset size.
        /// </summary>
        public virtual int DesiredSetSize
        {
            get { return _desiredSetSize; }
            set { _desiredSetSize = value; }
        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public virtual int InputNeuronCount
        {
            get { return _inputNeuronCount; }
            set { _inputNeuronCount = value; }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public virtual int OutputNeuronCount
        {
            get { return _outputNeuronCount; }
            set { _outputNeuronCount = value; }
        }

        /// <summary>
        /// The starting point.
        /// </summary>
        public virtual DateTime StartingPoint
        {
            get { return _startingPoint; }
            set { _startingPoint = value; }
        }

        /// <summary>
        /// The size of the timeslices.
        /// </summary>
        public virtual TimeUnit SequenceGrandularity
        {
            get { return _sequenceGrandularity; }
            set { _sequenceGrandularity = value; }
        }


        /// <summary>
        /// Add a data description.
        /// </summary>
        /// <param name="desc">The data description to add.</param>
        public virtual void AddDescription(TemporalDataDescription desc)
        {
            if (_points.Count > 0)
            {
                throw new TemporalError(
                    "Can't add anymore descriptions, there are "
                    + "already temporal points defined.");
            }

            int index = _descriptions.Count;
            desc.Index = index;

            _descriptions.Add(desc);
            CalculateNeuronCounts();
        }

        /// <summary>
        /// Clear the entire dataset.
        /// </summary>
        public virtual void Clear()
        {
            _descriptions.Clear();
            _points.Clear();
            Data.Clear();
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="inputData">Not used</param>
        /// <param name="idealData">Not used</param>
        public override void Add(IMLData inputData, IMLData idealData)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public override void Add(IMLDataPair inputData)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Adding directly is not supported. Rather, add temporal points and
        /// generate the training data.
        /// </summary>
        /// <param name="data">Not used.</param>
        public override void Add(IMLData data)
        {
            throw new TemporalError(AddNotSupported);
        }

        /// <summary>
        /// Create a temporal data point using a sequence number. They can also be
        /// created using time. No two points should have the same sequence number.
        /// </summary>
        /// <param name="sequence">The sequence number.</param>
        /// <returns>A new TemporalPoint object.</returns>
        public virtual TemporalPoint CreatePoint(int sequence)
        {
            var point = new TemporalPoint(_descriptions.Count) { Sequence = sequence };
            _points.Add(point);
            return point;
        }

        /// <summary>
        /// Create a sequence number from a time. The first date will be zero, and
        /// subsequent dates will be increased according to the grandularity
        /// specified. 
        /// </summary>
        /// <param name="when">The date to generate the sequence number for.</param>
        /// <returns>A sequence number.</returns>
        public virtual int GetSequenceFromDate(DateTime when)
        {
            int sequence;

            if (_startingPoint != DateTime.MinValue)
            {
                var span = new TimeSpanUtil(_startingPoint, when);
                sequence = (int)span.GetSpan(_sequenceGrandularity);
            }
            else
            {
                _startingPoint = when;
                sequence = 0;
            }

            return sequence;
        }


        /// <summary>
        /// Create a temporal point from a time. Using the grandularity each date is
        /// given a unique sequence number. No two dates that fall in the same
        /// grandularity should be specified.
        /// </summary>
        /// <param name="when">The time that this point should be created at.</param>
        /// <returns>The point TemporalPoint created.</returns>
        public virtual TemporalPoint CreatePoint(DateTime when)
        {
            int sequence = GetSequenceFromDate(when);
            var point = new TemporalPoint(_descriptions.Count) { Sequence = sequence };
            _points.Add(point);
            return point;
        }

        /// <summary>
        /// Calculate how many points are in the high and low range. These are the
        /// points that the training set will be generated on.
        /// </summary>
        /// <returns>The number of points.</returns>
        public virtual int CalculatePointsInRange()
        {
            return _points.Count(IsPointInRange);
        }

        /// <summary>
        /// Calculate the actual set size, this is the number of training set entries
        /// that will be generated.
        /// </summary>
        /// <returns>The size of the training set.</returns>
        public virtual int CalculateActualSetSize()
        {
            int result = CalculatePointsInRange();
            result = Math.Min(_desiredSetSize, result);
            return result;
        }

        /// <summary>
        /// Calculate how many input and output neurons will be needed for the
        /// current data.
        /// </summary>
        public virtual void CalculateNeuronCounts()
        {
            _inputNeuronCount = 0;
            _outputNeuronCount = 0;

            foreach (TemporalDataDescription desc in _descriptions)
            {
                if (desc.IsInput)
                {
                    _inputNeuronCount += _inputWindowSize;
                }
                if (desc.IsPredict)
                {
                    _outputNeuronCount += _predictWindowSize;
                }
            }
        }

        /// <summary>
        /// Is the specified point within the range. If a point is in the selection
        /// range, then the point will be used to generate the training sets.
        /// </summary>
        /// <param name="point">The point to consider.</param>
        /// <returns>True if the point is within the range.</returns>
        public virtual bool IsPointInRange(TemporalPoint point)
        {
            return ((point.Sequence >= LowSequence) && (point.Sequence <= HighSequence));
        }

        /// <summary>
        /// Generate input neural data for the specified index.
        /// </summary>
        /// <param name="index">The index to generate neural data for.</param>
        /// <returns>The input neural data generated.</returns>
        public virtual BasicNeuralData GenerateInputNeuralData(int index)
        {
            if (index + _inputWindowSize > _points.Count)
            {
                throw new TemporalError("Can't generate input temporal data "
                                        + "beyond the end of provided data.");
            }

            var result = new BasicNeuralData(_inputNeuronCount);
            int resultIndex = 0;

            for (int i = 0; i < _inputWindowSize; i++)
            {
                foreach (TemporalDataDescription desc in _descriptions)
                {
                    if (desc.IsInput)
                    {
                        result[resultIndex++] = FormatData(desc, index
                                                                 + i);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get data between two points in raw form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataRaw(TemporalDataDescription desc,
                                  int index)
        {
            TemporalPoint point = _points[index - 1];
            return point[desc.Index];
        }

        /// <summary>
        /// Get data between two points in delta form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataDeltaChange(TemporalDataDescription desc,
                                          int index)
        {
            if (index == 0)
            {
                return 0.0;
            }
            TemporalPoint point = _points[index];
            TemporalPoint previousPoint = _points[index - 1];
            return point[desc.Index]
                   - previousPoint[desc.Index];
        }


        /// <summary>
        /// Get data between two points in percent form.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to get data from.</param>
        /// <returns>The requested data.</returns>
        private double GetDataPercentChange(TemporalDataDescription desc,
                                            int index)
        {
            if (index == 0)
            {
                return 0.0;
            }
            TemporalPoint point = _points[index];
            TemporalPoint previousPoint = _points[index - 1];
            double currentValue = point[desc.Index];
            double previousValue = previousPoint[desc.Index];
            return (currentValue - previousValue) / previousValue;
        }

        /// <summary>
        /// Format data according to the type specified in the description.
        /// </summary>
        /// <param name="desc">The data description.</param>
        /// <param name="index">The index to format the data at.</param>
        /// <returns>The formatted data.</returns>
        private double FormatData(TemporalDataDescription desc,
                                  int index)
        {
            var result = new double[1];

            switch (desc.DescriptionType)
            {
                case TemporalDataDescription.Type.DeltaChange:
                    result[0] = GetDataDeltaChange(desc, index);
                    break;
                case TemporalDataDescription.Type.PercentChange:
                    result[0] = GetDataPercentChange(desc, index);
                    break;
                case TemporalDataDescription.Type.Raw:
                    result[0] = GetDataRaw(desc, index);
                    break;
                default:
                    throw new TemporalError("Unsupported data type.");
            }

            if (desc.ActivationFunction != null)
            {
                desc.ActivationFunction.ActivationFunction(result, 0, 1);
            }

            return result[0];
        }

        /// <summary>
        /// Generate neural ideal data for the specified index.
        /// </summary>
        /// <param name="index">The index to generate for.</param>
        /// <returns>The neural data generated.</returns>
        public virtual BasicNeuralData GenerateOutputNeuralData(int index)
        {
            if (index + _predictWindowSize > _points.Count)
            {
                throw new TemporalError("Can't generate prediction temporal data "
                                        + "beyond the end of provided data.");
            }

            var result = new BasicNeuralData(_outputNeuronCount);
            int resultIndex = 0;

            for (int i = 0; i < _predictWindowSize; i++)
            {
                foreach (TemporalDataDescription desc in _descriptions)
                {
                    if (desc.IsPredict)
                    {
                        result[resultIndex++] = FormatData(desc, index
                                                                 + i);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Calculate the index to start at.
        /// </summary>
        /// <returns>The starting point.</returns>
        public virtual int CalculateStartIndex()
        {
            for (int i = 0; i < _points.Count; i++)
            {
                TemporalPoint point = _points[i];
                if (IsPointInRange(point))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sort the points.
        /// </summary>
        public virtual void SortPoints()
        {
            _points.Sort();
        }

        /// <summary>
        /// Generate the training sets.
        /// </summary>
        public virtual void Generate()
        {
            //SortPoints();
            //int start = CalculateStartIndex() + 1;
            //int setSize = CalculateActualSetSize();
            //int range = start
            //            + (setSize - _predictWindowSize - _inputWindowSize);

            //for (int i = start; i < range; i++)
            //{
            //    BasicNeuralData input = GenerateInputNeuralData(i);
            //    BasicNeuralData ideal = GenerateOutputNeuralData(i
            //                                                     + _inputWindowSize);
            //    var pair = new BasicNeuralDataPair(input, ideal);
            //    base.Add(pair);
            //}
        }
    }

    public class TemporalPoint : IComparable<TemporalPoint>
    {
        /// <summary>
        /// The data for this point.
        /// </summary>
        private double[] _data;

        /// <summary>
        /// The sequence number for this point.
        /// </summary>
        private int _sequence;

        /// <summary>
        /// Construct a temporal point of the specified size.
        /// </summary>
        /// <param name="size">The size to create the temporal point for.</param>
        public TemporalPoint(int size)
        {
            _data = new double[size];
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        public double[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// The sequence number, used to sort.
        /// </summary>
        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        /// <param name="x">The index.</param>
        /// <returns>The data at the specified index.</returns>
        public double this[int x]
        {
            get { return _data[x]; }
            set { _data[x] = value; }
        }

        #region IComparable<TemporalPoint> Members

        /// <summary>
        /// Compare two temporal points.
        /// </summary>
        /// <param name="that">The other temporal point to compare.</param>
        /// <returns>Returns 0 if they are equal, less than 0 if this point is less,
        /// greater than zero if this point is greater.</returns>
        public int CompareTo(TemporalPoint that)
        {
            if (Sequence == that.Sequence)
            {
                return 0;
            }
            if (Sequence < that.Sequence)
            {
                return -1;
            }
            return 1;
        }

        #endregion

        /**
         * Convert this point to string form.
         * @return This point as a string.
         */

        public override String ToString()
        {
            var builder = new StringBuilder("[TemporalPoint:");
            builder.Append("Seq:");
            builder.Append(_sequence);
            builder.Append(",Data:");
            for (int i = 0; i < _data.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(_data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }

    public class IMLDataError : SyntError
    {
       
        public IMLDataError(String str)
            : base(str)
        {
        }

       
        public IMLDataError(Exception e)
            : base(e)
        {
        }
    }

    public class MLDataError : SyntError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public MLDataError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public MLDataError(Exception e)
            : base(e)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="e">The exception.</param>
        public MLDataError(String msg, Exception e)
            : base(msg, e)
        {
        }
    }

    public class BayesianFactory
    {
        /// <summary>
        /// Create a bayesian network.
        /// </summary>
        /// <param name="architecture">The architecture to use.</param>
        /// <param name="input">The input neuron count.</param>
        /// <param name="output">The output neuron count.</param>
        /// <returns>The new bayesian network.</returns>
        public IMLMethod Create(String architecture, int input,
                                int output)
        {
            var method = new BayesianNetwork { Contents = architecture };
            return method;
        }
    }

    public class FeedforwardFactory
    {
        /// <summary>
        /// Error.
        /// </summary>
        ///
        public const String CantDefineAct = "Can't define activation function before first layer.";

        /// <summary>
        /// The activation function factory to use.
        /// </summary>
        private MLActivationFactory _factory = new MLActivationFactory();

        /// <summary>
        /// Create a feed forward network.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The feedforward network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            var result = new BasicNetwork();
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            IActivationFunction af = new ActivationLinear();

            int questionPhase = 0;

            foreach (String layerStr in layers)
            {
                // determine default
                int defaultCount = questionPhase == 0 ? input : output;

                ArchitectureLayer layer = ArchitectureParse.ParseLayer(
                    layerStr, defaultCount);
                bool bias = layer.Bias;

                String part = layer.Name;
                part = part != null ? part.Trim() : "";

                IActivationFunction lookup = _factory.Create(part);

                if (lookup != null)
                {
                    af = lookup;
                }
                else
                {
                    if (layer.UsedDefault)
                    {
                        questionPhase++;
                        if (questionPhase > 2)
                        {
                            throw new SyntError("Only two ?'s may be used.");
                        }
                    }

                    if (layer.Count == 0)
                    {
                        throw new SyntError("Unknown architecture element: "
                                             + architecture + ", can't parse: " + part);
                    }

                    result.AddLayer(new BasicLayer(af, bias, layer.Count));
                }
            }

            result.Structure.FinalizeStructure();
            result.Reset();

            return result;
        }
    }

    public class PNNFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create a PNN network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "PNN Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer pnnLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            PNNKernelType kernel;
            PNNOutputMode outmodel;

            if (pnnLayer.Name.Equals("c", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Classification;
            }
            else if (pnnLayer.Name.Equals("r", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Regression;
            }
            else if (pnnLayer.Name.Equals("u", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Unsupervised;
            }
            else
            {
                throw new NeuralNetworkError("Unknown model: " + pnnLayer.Name);
            }

            var holder = new ParamsHolder(pnnLayer.Params);

            String kernelStr = holder.GetString("KERNEL", false, "gaussian");

            if (kernelStr.Equals("gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Gaussian;
            }
            else if (kernelStr.Equals("reciprocal", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Reciprocal;
            }
            else
            {
                throw new NeuralNetworkError("Unknown kernel: " + kernelStr);
            }

            var result = new BasicPNN(kernel, outmodel, inputCount,
                                      outputCount);

            return result;
        }
    }

    public class RBFNetworkFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create a RBF network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "RBF Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer rbfLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            RBFEnum t;

            if (rbfLayer.Name.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfLayer.Name.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfLayer.Name.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfLayer.Name.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                throw new NeuralNetworkError("Unknown RBF: " + rbfLayer.Name);
            }

            var holder = new ParamsHolder(rbfLayer.Params);

            int rbfCount = holder.GetInt("C", true, 0);

            var result = new RBFNetwork(inputCount, rbfCount,
                                        outputCount, t);

            return result;
        }
    }

    public class SOMFactory
    {
        /// <summary>
        /// Create a SOM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SOM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != 2)
            {
                throw new SyntError(
                    "SOM's must have exactly two elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[1], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            var pattern = new SOMPattern { InputNeurons = inputCount, OutputNeurons = outputCount };
            return pattern.Generate();
        }
    }

    public class SRNFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create the SRN.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SRN.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new SyntError(
                    "SRN Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer rbfLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            RBFEnum t;

            if (rbfLayer.Name.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfLayer.Name.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfLayer.Name.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfLayer.Name.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                t = RBFEnum.Gaussian;
            }

            var result = new RBFNetwork(inputCount,
                                        rbfLayer.Count, outputCount, t);

            return result;
        }
    }

    public class SVMFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MAX_LAYERS = 3;

        /// <summary>
        /// Create the SVM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SVM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MAX_LAYERS)
            {
                throw new SyntError(
                    "SVM's must have exactly three elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer paramsLayer = ArchitectureParse.ParseLayer(
                layers[1], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            String name = paramsLayer.Name;
            String kernelStr = paramsLayer.Params.ContainsKey("KERNEL") ? paramsLayer.Params["KERNEL"] : null;
            String svmTypeStr = paramsLayer.Params.ContainsKey("TYPE") ? paramsLayer.Params["TYPE"] : null;

            SVMType svmType = SVMType.NewSupportVectorClassification;
            KernelType kernelType = KernelType.RadialBasisFunction;

            bool useNew = true;

            if (svmTypeStr == null)
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("NEW", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("OLD", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = false;
            }
            else
            {
                throw new SyntError("Unsupported type: " + svmTypeStr
                                     + ", must be NEW or OLD.");
            }

            if (name.Equals("C", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorClassification;
                }
                else
                {
                    svmType = SVMType.SupportVectorClassification;
                }
            }
            else if (name.Equals("R", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorRegression;
                }
                else
                {
                    svmType = SVMType.EpsilonSupportVectorRegression;
                }
            }
            else
            {
                throw new SyntError("Unsupported mode: " + name
                                     + ", must be C for classify or R for regression.");
            }

            if (kernelStr == null)
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("linear".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Linear;
            }
            else if ("poly".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Poly;
            }
            else if ("precomputed".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Precomputed;
            }
            else if ("rbf".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("sigmoid".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Sigmoid;
            }
            else
            {
                throw new SyntError("Unsupported kernel: " + kernelStr
                                     + ", must be linear,poly,precomputed,rbf or sigmoid.");
            }

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            if (outputCount != 1)
            {
                throw new SyntError("SVM can only have an output size of 1.");
            }

            var result = new SupportVectorMachine(inputCount, svmType, kernelType);

            return result;
        }
    }

    public class ArchitectureLayer
    {
        /// <summary>
        /// Holds any paramaters that were specified for the layer.
        /// </summary>
        ///
        private readonly IDictionary<String, String> _paras;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public ArchitectureLayer()
        {
            _paras = new Dictionary<String, String>();
        }


        /// <value>the count to set</value>
        public int Count { get; set; }


        /// <value>the name to set</value>
        public String Name { get; set; }


        /// <value>the params</value>
        public IDictionary<String, String> Params
        {
            get { return _paras; }
        }


        /// <value>the bias to set</value>
        public bool Bias { get; set; }


        /// <value>the usedDefault to set</value>
        public bool UsedDefault { get; set; }
    }

    public static class ArchitectureParse
    {
        /// <summary>
        /// parse a layer.
        /// </summary>
        ///
        /// <param name="line">The line to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The parsed ArchitectureLayer.</returns>
        public static ArchitectureLayer ParseLayer(String line,
                                                   int defaultValue)
        {
            var layer = new ArchitectureLayer();

            String check = line.Trim().ToUpper();

            // first check for bias
            if (check.EndsWith(":B"))
            {
                check = check.Substring(0, (check.Length - 2) - (0));
                layer.Bias = true;
            }

            // see if simple number
            try
            {
                layer.Count = Int32.Parse(check);
                if (layer.Count < 0)
                {
                    throw new SyntError("Count cannot be less than zero.");
                }
            }
            catch (FormatException f)
            {
                SyntLogging.Log(f);
            }

            // see if it is a default
            if ("?".Equals(check))
            {
                if (defaultValue < 0)
                {
                    throw new SyntError("Default (?) in an invalid location.");
                }
                layer.Count = defaultValue;
                layer.UsedDefault = true;
                return layer;
            }

            // single item, no function
            int startIndex = check.IndexOf('(');
            int endIndex = check.LastIndexOf(')');
            if (startIndex == -1)
            {
                layer.Name = check;
                return layer;
            }

            // function
            if (endIndex == -1)
            {
                throw new SyntError("Illegal parentheses.");
            }

            layer.Name = check.Substring(0, (startIndex) - (0)).Trim();

            String paramStr = check.Substring(startIndex + 1, (endIndex) - (startIndex + 1));
            IDictionary<String, String> paras = ParseParams(paramStr);
            EngineArray.PutAll(paras, layer.Params);
            return layer;
        }

        /// <summary>
        /// Parse all layers from a line of text.
        /// </summary>
        ///
        /// <param name="line">The line of text.</param>
        /// <returns>A list of the parsed layers.</returns>
        public static IList<String> ParseLayers(String line)
        {
            IList<String> result = new List<String>();

            int bs = 0;
            bool done = false;

            do
            {
                String part;
                int index = line.IndexOf("->", bs);
                if (index != -1)
                {
                    part = line.Substring(bs, (index) - (bs)).Trim();
                    bs = index + 2;
                }
                else
                {
                    part = line.Substring(bs).Trim();
                    done = true;
                }

                bool bias = part.EndsWith("b");
                if (bias)
                {
                    part = part.Substring(0, (part.Length - 1) - (0));
                }

                result.Add(part);
            } while (!done);

            return result;
        }

        /// <summary>
        /// Parse a name.
        /// </summary>
        ///
        /// <param name="parser">The parser to use.</param>
        /// <returns>The name.</returns>
        private static String ParseName(SimpleParser parser)
        {
            var result = new StringBuilder();
            parser.EatWhiteSpace();
            while (parser.IsIdentifier())
            {
                result.Append(parser.ReadChar());
            }
            return result.ToString();
        }

        /// <summary>
        /// Parse parameters.
        /// </summary>
        ///
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed values.</returns>
        public static IDictionary<String, String> ParseParams(String line)
        {
            IDictionary<String, String> result = new Dictionary<String, String>();

            var parser = new SimpleParser(line);

            while (!parser.EOL())
            {
                String name = ParseName(parser)
                    .ToUpper();

                parser.EatWhiteSpace();
                if (!parser.LookAhead("=", false))
                {
                    throw new SyntError("Missing equals(=) operator.");
                }
                parser.Advance();

                String v = ParseValue(parser);

                result[name.ToUpper()] = v;

                if (!parser.ParseThroughComma())
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Parse a value.
        /// </summary>
        ///
        /// <param name="parser">The parser to use.</param>
        /// <returns>The newly parsed value.</returns>
        private static String ParseValue(SimpleParser parser)
        {
            bool quoted = false;
            var str = new StringBuilder();

            parser.EatWhiteSpace();

            if (parser.Peek() == '\"')
            {
                quoted = true;
                parser.Advance();
            }

            while (!parser.EOL())
            {
                if (parser.Peek() == '\"')
                {
                    if (quoted)
                    {
                        parser.Advance();
                        if (parser.Peek() == '\"')
                        {
                            str.Append(parser.ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        str.Append(parser.ReadChar());
                    }
                }
                else if (!quoted
                         && (parser.IsWhiteSpace() || (parser.Peek() == ',')))
                {
                    break;
                }
                else
                {
                    str.Append(parser.ReadChar());
                }
            }
            return str.ToString();
        }
    }

    public class AnnealFactory
    {
        /// <summary>
        /// Create an annealing trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new TrainingError(
                    "Invalid method type, requires BasicNetwork");
            }

            ICalculateScore score = new TrainingSetScore(training);

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            double startTemp = holder.GetDouble(
                MLTrainFactory.PropertyTemperatureStart, false, 10);
            double stopTemp = holder.GetDouble(
                MLTrainFactory.PropertyTemperatureStop, false, 2);

            int cycles = holder.GetInt(MLTrainFactory.Cycles, false, 100);

            IMLTrain train = new NeuralSimulatedAnnealing(
                (BasicNetwork)method, score, startTemp, stopTemp, cycles);

            return train;
        }
    }

    public class BackPropFactory
    {
        /// <summary>
        /// Create a backpropagation trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.7d);
            double momentum = holder.GetDouble(
                MLTrainFactory.PropertyLearningMomentum, false, 0.3d);

            return new Backpropagation((BasicNetwork)method, training,
                                       learningRate, momentum);
        }
    }

    public class ClusterSOMFactory
    {
        /// <summary>
        /// Create a cluster SOM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new SyntError(
                    "Cluster SOM training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SOMClusterCopyTraining((SOMNetwork)method, training);
        }
    }

    public class GFactory
    {
        /// <summary>
        /// Create an annealing trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new TrainingError(
                    "Invalid method type, requires BasicNetwork");
            }

            ICalculateScore score = new TrainingSetScore(training);

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            int populationSize = holder.GetInt(
                MLTrainFactory.PropertyPopulationSize, false, 5000);
            double mutation = holder.GetDouble(
                MLTrainFactory.PropertyMutation, false, 0.1d);
            double mate = holder.GetDouble(MLTrainFactory.PropertyMate,
                                           false, 0.25d);

            IMLTrain train = new NeuralGAlgorithm((BasicNetwork)method,
                                                       (IRandomizer)(new RangeRandomizer(-1, 1)), score, populationSize, mutation,
                                                       mate);

            return train;
        }
    }

    public class LMAFactory
    {
        /// <summary>
        /// Create a LMA trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is BasicNetwork))
            {
                throw new SyntError(
                    "LMA training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            var result = new LevenbergMarquardtTraining(
                (BasicNetwork)method, training);
            return result;
        }
    }

    public class ManhattanFactory
    {
        /// <summary>
        /// Create a Manhattan trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.1d);

            return new ManhattanPropagation((BasicNetwork)method, training,
                                            learningRate);
        }
    }

    public class NeighborhoodSOMFactory
    {
        /// <summary>
        /// Create a LMA trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SOMNetwork))
            {
                throw new SyntError(
                    "Neighborhood training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 0.7d);
            String neighborhoodStr = holder.GetString(
                MLTrainFactory.PropertyNeighborhood, false, "rbf");
            String rbfTypeStr = holder.GetString(
                MLTrainFactory.PropertyRBFType, false, "gaussian");

            RBFEnum t;

            if (rbfTypeStr.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfTypeStr.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfTypeStr.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfTypeStr.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                t = RBFEnum.Gaussian;
            }

            INeighborhoodFunction nf = null;

            if (neighborhoodStr.Equals("bubble", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodBubble(1);
            }
            else if (neighborhoodStr.Equals("rbf", StringComparison.InvariantCultureIgnoreCase))
            {
                String str = holder.GetString(
                    MLTrainFactory.PropertyDimensions, true, null);
                int[] size = NumberList.FromListInt(CSVFormat.EgFormat, str);
                nf = new NeighborhoodRBF(size, t);
            }
            else if (neighborhoodStr.Equals("rbf1d", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodRBF1D(t);
            }
            if (neighborhoodStr.Equals("single", StringComparison.InvariantCultureIgnoreCase))
            {
                nf = new NeighborhoodSingle();
            }

            var result = new BasicTrainSOM((SOMNetwork)method,
                                           learningRate, training, nf);

            if (args.ContainsKey(MLTrainFactory.PropertyIterations))
            {
                int plannedIterations = holder.GetInt(
                    MLTrainFactory.PropertyIterations, false, 1000);
                double startRate = holder.GetDouble(
                    MLTrainFactory.PropertyStartLearningRate, false, 0.05d);
                double endRate = holder.GetDouble(
                    MLTrainFactory.PropertyEndLearningRate, false, 0.05d);
                double startRadius = holder.GetDouble(
                    MLTrainFactory.PropertyStartRadius, false, 10);
                double endRadius = holder.GetDouble(
                    MLTrainFactory.PropertyEndRadius, false, 1);
                result.SetAutoDecay(plannedIterations, startRate, endRate,
                                    startRadius, endRadius);
            }

            return result;
        }
    }

    public class NelderMeadFactory
    {
        ///// <summary>
        ///// Create a Nelder Mead trainer.
        ///// </summary>
        ///// <param name="method">The method to use.</param>
        ///// <param name="training">The training data to use.</param>
        ///// <param name="argsStr">The arguments to use.</param>
        ///// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            //final double learningRate = holder.getDouble(
            //		MLTrainFactory.PROPERTY_LEARNING_RATE, false, 0.1);

            return new NelderMeadTraining((BasicNetwork)method, training);
        }
    }

    public class PNNTrainFactory
    {
        /// <summary>
        /// Create a PNN trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is BasicPNN))
            {
                throw new SyntError(
                    "PNN training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new TrainBasicPNN((BasicPNN)method, training);
        }
    }

    public class YTraining : GAlgorithm, IMLTrain
    {
        /// <summary>
        /// The number of inputs.
        /// </summary>
        private readonly int inputCount;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private readonly int outputCount;

        /// <summary>
        /// The average fit adjustment.
        /// </summary>
        private double averageFitAdjustment;

        /// <summary>
        /// The best ever network.
        /// </summary>
        private YNetwork bestEverNetwork;

        /// <summary>
        /// The best ever score.
        /// </summary>
        private double bestEverScore;

        /// <summary>
        /// The iteration number.
        /// </summary>
        private int iteration;

        /// <summary>
        /// The activation mutation rate.
        /// </summary>
        private double paramActivationMutationRate = 0.1;

        /// <summary>
        /// The likelyhood of adding a link.
        /// </summary>
        private double paramChanceAddLink = 0.07;

        /// <summary>
        /// The likelyhood of adding a node.
        /// </summary>
        private double paramChanceAddNode = 0.04;

        /// <summary>
        /// The likelyhood of adding a recurrent link.
        /// </summary>
        private double paramChanceAddRecurrentLink = 0.05;

        /// <summary>
        /// The compatibility threshold for a species.
        /// </summary>
        private double paramCompatibilityThreshold = 0.26;

        /// <summary>
        /// The crossover rate.
        /// </summary>
        private double paramCrossoverRate = 0.7;

        /// <summary>
        /// The max activation perturbation.
        /// </summary>
        private double paramMaxActivationPerturbation = 0.1;

        /// <summary>
        /// The maximum number of species.
        /// </summary>
        private int paramMaxNumberOfSpecies;

        /// <summary>
        /// The maximum number of neurons.
        /// </summary>
        private double paramMaxPermittedNeurons = 100;

        /// <summary>
        /// The maximum weight perturbation.
        /// </summary>
        private double paramMaxWeightPerturbation = 0.5;

        /// <summary>
        /// The mutation rate.
        /// </summary>
        private double paramMutationRate = 0.2;

        /// <summary>
        /// The number of link add attempts.
        /// </summary>
        private int paramNumAddLinkAttempts = 5;

        /// <summary>
        /// The number of generations allowed with no improvement.
        /// </summary>
        private int paramNumGensAllowedNoImprovement = 15;

        /// <summary>
        /// The number of tries to find a looped link.
        /// </summary>
        private int paramNumTrysToFindLoopedLink = 5;

        /// <summary>
        /// The number of tries to find an old link.
        /// </summary>
        private int paramNumTrysToFindOldLink = 5;

        /// <summary>
        /// The probability that the weight will be totally replaced.
        /// </summary>
        private double paramProbabilityWeightReplaced = 0.1;

        /// <summary>
        /// Determines if we are using snapshot mode.
        /// </summary>
        private bool snapshot;

        /// <summary>
        /// The total fit adjustment.
        /// </summary>
        private double totalFitAdjustment;

        /// <summary>
        /// Construct a Y trainer with a new population. The new population is
        /// created from the specified parameters.
        /// </summary>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="inputCount">The input neuron count.</param>
        /// <param name="outputCount">The output neuron count.</param>
        /// <param name="populationSize">The population size.</param>
        public YTraining(ICalculateScore calculateScore,
                            int inputCount, int outputCount,
                            int populationSize)
        {
            this.inputCount = inputCount;
            this.outputCount = outputCount;

            CalculateScore = new GScoreAdapter(calculateScore);
            Comparator = new TComparator(CalculateScore);
            Population = new YPopulation(inputCount, outputCount,
                                            populationSize);

            Init();
        }

        /// <summary>
        /// Construct Y training with an existing population.
        /// </summary>
        /// <param name="calculateScore">The score object to use.</param>
        /// <param name="population">The population to use.</param>
        public YTraining(ICalculateScore calculateScore,
                            IPopulation population)
        {
            if (population.Size() < 1)
            {
                throw new TrainingError("Population can not be empty.");
            }

            var T = (YT)population.Ts[0];
            CalculateScore = new GScoreAdapter(calculateScore);
            Comparator = new TComparator(CalculateScore);
            Population = (population);
            inputCount = T.InputCount;
            outputCount = T.OutputCount;

            Init();
        }

        /// <summary>
        /// The innovations.
        /// </summary>
        public YInnovationList Innovations
        {
            get { return (YInnovationList)Population.Innovations; }
        }

        /// <summary>
        /// The input count.
        /// </summary>
        public int InputCount
        {
            get { return inputCount; }
        }

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        public int OutputCount
        {
            get { return outputCount; }
        }

        /// <summary>
        /// Set the activation mutation rate.
        /// </summary>
        public double ParamActivationMutationRate
        {
            get { return paramActivationMutationRate; }
            set { paramActivationMutationRate = value; }
        }


        /// <summary>
        /// Set the chance to add a link.
        /// </summary>
        public double ParamChanceAddLink
        {
            get { return paramChanceAddLink; }
            set { paramChanceAddLink = value; }
        }


        /// <summary>
        /// Set the chance to add a node.
        /// </summary>
        public double ParamChanceAddNode
        {
            get { return paramChanceAddNode; }
            set { paramChanceAddNode = value; }
        }

        /// <summary>
        /// Set the chance to add a recurrent link.
        /// </summary>
        public double ParamChanceAddRecurrentLink
        {
            get { return paramChanceAddRecurrentLink; }
            set { paramChanceAddRecurrentLink = value; }
        }


        /// <summary>
        /// Set the compatibility threshold for species.
        /// </summary>
        public double ParamCompatibilityThreshold
        {
            get { return paramCompatibilityThreshold; }
            set { paramCompatibilityThreshold = value; }
        }


        /// <summary>
        /// Set the cross over rate.
        /// </summary>
        public double ParamCrossoverRate
        {
            get { return paramCrossoverRate; }
            set { paramCrossoverRate = value; }
        }


        /// <summary>
        /// Set the max activation perturbation.
        /// </summary>
        public double ParamMaxActivationPerturbation
        {
            get { return paramMaxActivationPerturbation; }
            set { paramMaxActivationPerturbation = value; }
        }

        /// <summary>
        /// Set the maximum number of species.
        /// </summary>
        public int ParamMaxNumberOfSpecies
        {
            get { return paramMaxNumberOfSpecies; }
            set { paramMaxNumberOfSpecies = value; }
        }

        /// <summary>
        /// Set the max permitted neurons.
        /// </summary>
        public double ParamMaxPermittedNeurons
        {
            get { return paramMaxPermittedNeurons; }
            set { paramMaxPermittedNeurons = value; }
        }

        /// <summary>
        /// Set the max weight perturbation.
        /// </summary>
        public double ParamMaxWeightPerturbation
        {
            get { return paramMaxWeightPerturbation; }
            set { paramMaxWeightPerturbation = value; }
        }

        /// <summary>
        /// Set the mutation rate.
        /// </summary>
        public double ParamMutationRate
        {
            get { return paramMutationRate; }
            set { paramMutationRate = value; }
        }

        /// <summary>
        /// Set the number of attempts to add a link.
        /// </summary>
        public int ParamNumAddLinkAttempts
        {
            get { return paramNumAddLinkAttempts; }
            set { paramNumAddLinkAttempts = value; }
        }

        /// <summary>
        /// Set the number of no-improvement generations allowed.
        /// </summary>
        public int ParamNumGensAllowedNoImprovement
        {
            get { return paramNumGensAllowedNoImprovement; }
            set { paramNumGensAllowedNoImprovement = value; }
        }

        /// <summary>
        /// Set the number of tries to create a looped link.
        /// </summary>
        public int ParamNumTrysToFindLoopedLink
        {
            get { return paramNumTrysToFindLoopedLink; }
            set { paramNumTrysToFindLoopedLink = value; }
        }


        /// <summary>
        /// Set the number of tries to try an old link.
        /// </summary>
        public int ParamNumTrysToFindOldLink
        {
            get { return paramNumTrysToFindOldLink; }
            set { paramNumTrysToFindOldLink = value; }
        }


        /// <summary>
        /// Set the probability to replace a weight.
        /// </summary>
        public double ParamProbabilityWeightReplaced
        {
            get { return paramProbabilityWeightReplaced; }
            set { paramProbabilityWeightReplaced = value; }
        }

        /// <summary>
        /// Set if we are using snapshot mode.
        /// </summary>
        public bool Snapshot
        {
            get { return snapshot; }
            set { snapshot = value; }
        }

        #region MLTrain Members

        /// <inheritdoc/>
        public void AddStrategy(IStrategy strategy)
        {
            throw new TrainingError(
                "Strategies are not supported by this training method.");
        }

        /// <inheritdoc/>
        public bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public void FinishTraining()
        {
        }

        /// <summary>
        /// The error for the best T.
        /// </summary>
        public double Error
        {
            get { return bestEverScore; }
            set { bestEverScore = value; }
        }

        /// <inheritdoc/>
        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }

        /// <inheritdoc/>
        public int IterationNumber
        {
            get { return iteration; }
            set { iteration = value; }
        }

        /// <summary>
        /// A network created for the best T.
        /// </summary>
        public IMLMethod Method
        {
            get { return bestEverNetwork; }
        }

        /// <inheritdoc/>
        public IList<IStrategy> Strategies
        {
            get { return new List<IStrategy>(); }
        }

        /// <summary>
        /// Returns null, does not use a training set, rather uses a score function.
        /// </summary>
        public IMLDataSet Training
        {
            get { return null; }
        }

        /// <inheritdoc/>
        public bool TrainingDone
        {
            get { return false; }
        }

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override void Iteration()
        {
            iteration++;
            IList<YT> newPop = new List<YT>();

            int numSpawnedSoFar = 0;

            foreach (ISpecies s in Population.Species)
            {
                if (numSpawnedSoFar < Population.Size())
                {
                    var numToSpawn = (int)Math.Round(s.NumToSpawn);

                    bool bChosenBestYet = false;

                    while ((numToSpawn--) > 0)
                    {
                        YT baby = null;

                        if (!bChosenBestYet)
                        {
                            baby = (YT)s.Leader;

                            bChosenBestYet = true;
                        }

                        else
                        {
                            // if the number of individuals in this species is only
                            // one
                            // then we can only perform mutation
                            if (s.Members.Count == 1)
                            {
                                // spawn a child
                                baby = new YT((YT)s.ChooseParent());
                            }
                            else
                            {
                                var g1 = (YT)s.ChooseParent();

                                if (ThreadSafeRandom.NextDouble() < paramCrossoverRate)
                                {
                                    var g2 = (YT)s.ChooseParent();

                                    int numAttempts = 5;

                                    while ((g1.TID == g2.TID)
                                           && ((numAttempts--) > 0))
                                    {
                                        g2 = (YT)s.ChooseParent();
                                    }

                                    if (g1.TID != g2.TID)
                                    {
                                        baby = Crossover(g1, g2);
                                    }
                                }

                                else
                                {
                                    baby = new YT(g1);
                                }
                            }

                            if (baby != null)
                            {
                                baby.TID = Population.AssignTID();

                                if (baby.Neurons.Size() < paramMaxPermittedNeurons)
                                {
                                    baby.AddNeuron(paramChanceAddNode,
                                                   paramNumTrysToFindOldLink);
                                }

                                // now there's the chance a link may be added
                                baby.AddLink(paramChanceAddLink,
                                             paramChanceAddRecurrentLink,
                                             paramNumTrysToFindLoopedLink,
                                             paramNumAddLinkAttempts);

                                // mutate the weights
                                baby.MutateWeights(paramMutationRate,
                                                   paramProbabilityWeightReplaced,
                                                   paramMaxWeightPerturbation);

                                baby.MutateActivationResponse(
                                    paramActivationMutationRate,
                                    paramMaxActivationPerturbation);
                            }
                        }

                        if (baby != null)
                        {
                            // sort the baby's genes by their innovation numbers
                            baby.SortGenes();

                            // add to new pop
                            // if (newPop.contains(baby)) {
                            // throw new SyntError("readd");
                            // }
                            newPop.Add(baby);

                            ++numSpawnedSoFar;

                            if (numSpawnedSoFar == Population.Size())
                            {
                                numToSpawn = 0;
                            }
                        }
                    }
                }
            }

            while (newPop.Count < Population.Size())
            {
                newPop.Add(TournamentSelection(Population.Size() / 5));
            }

            Population.Clear();
            foreach (YT T in newPop)
            {
                Population.Add(T);
            }

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <inheritdoc/>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        /// <inheritdoc/>
        public TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public void Resume(TrainingContinuation state)
        {
        }

        #endregion

        /// <summary>
        /// Add the specified neuron id.
        /// </summary>
        /// <param name="nodeID">The neuron to add.</param>
        /// <param name="vec">The list to add to.</param>
        public void AddNeuronID(long nodeID, IList<long> vec)
        {
            for (int i = 0; i < vec.Count; i++)
            {
                if (vec[i] == nodeID)
                {
                    return;
                }
            }

            vec.Add(nodeID);

            return;
        }

        /// <summary>
        /// Adjust the compatibility threshold.
        /// </summary>
        public void AdjustCompatibilityThreshold()
        {
            // has this been disabled (unlimited species)
            if (paramMaxNumberOfSpecies < 1)
            {
                return;
            }

            double thresholdIncrement = 0.01;

            if (Population.Species.Count > paramMaxNumberOfSpecies)
            {
                paramCompatibilityThreshold += thresholdIncrement;
            }

            else if (Population.Species.Count < 2)
            {
                paramCompatibilityThreshold -= thresholdIncrement;
            }
        }

        /// <summary>
        /// Adjust each species score.
        /// </summary>
        public void AdjustSpeciesScore()
        {
            foreach (ISpecies s in Population.Species)
            {
                // loop over all Ts and adjust scores as needed
                foreach (IT member in s.Members)
                {
                    double score = member.Score;

                    // apply a youth bonus
                    if (s.Age < Population.YoungBonusAgeThreshold)
                    {
                        score = Comparator.ApplyBonus(score,
                                                      Population.YoungScoreBonus);
                    }

                    // apply an old age penalty
                    if (s.Age > Population.OldAgeThreshold)
                    {
                        score = Comparator.ApplyPenalty(score,
                                                        Population.OldAgePenalty);
                    }

                    double adjustedScore = score / s.Members.Count;

                    member.AdjustedScore = adjustedScore;
                }
            }
        }

        /// <summary>
        /// Perform a cross over.  
        /// </summary>
        /// <param name="mom">The mother T.</param>
        /// <param name="dad">The father T.</param>
        /// <returns></returns>
        public new YT Crossover(YT mom, YT dad)
        {
            YParent best;

            // first determine who is more fit, the mother or the father?
            if (mom.Score == dad.Score)
            {
                if (mom.NumGenes == dad.NumGenes)
                {
                    if (ThreadSafeRandom.NextDouble() > 0)
                    {
                        best = YParent.Mom;
                    }
                    else
                    {
                        best = YParent.Dad;
                    }
                }

                else
                {
                    if (mom.NumGenes < dad.NumGenes)
                    {
                        best = YParent.Mom;
                    }
                    else
                    {
                        best = YParent.Dad;
                    }
                }
            }
            else
            {
                if (Comparator.IsBetterThan(mom.Score, dad.Score))
                {
                    best = YParent.Mom;
                }

                else
                {
                    best = YParent.Dad;
                }
            }

            var babyNeurons = new Q();
            var babyGenes = new Q();

            var vecNeurons = new List<long>();

            int curMom = 0;
            int curDad = 0;

            YLinkGene momGene;
            YLinkGene dadGene;

            YLinkGene selectedGene = null;

            while ((curMom < mom.NumGenes) || (curDad < dad.NumGenes))
            {
                if (curMom < mom.NumGenes)
                {
                    momGene = (YLinkGene)mom.Links.Get(curMom);
                }
                else
                {
                    momGene = null;
                }

                if (curDad < dad.NumGenes)
                {
                    dadGene = (YLinkGene)dad.Links.Get(curDad);
                }
                else
                {
                    dadGene = null;
                }

                if ((momGene == null) && (dadGene != null))
                {
                    if (best == YParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if ((dadGene == null) && (momGene != null))
                {
                    if (best == YParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (momGene.InnovationId < dadGene.InnovationId)
                {
                    if (best == YParent.Mom)
                    {
                        selectedGene = momGene;
                    }
                    curMom++;
                }
                else if (dadGene.InnovationId < momGene.InnovationId)
                {
                    if (best == YParent.Dad)
                    {
                        selectedGene = dadGene;
                    }
                    curDad++;
                }
                else if (dadGene.InnovationId == momGene.InnovationId)
                {
                    if (ThreadSafeRandom.NextDouble() < 0.5f)
                    {
                        selectedGene = momGene;
                    }

                    else
                    {
                        selectedGene = dadGene;
                    }
                    curMom++;
                    curDad++;
                }

                if (babyGenes.Size() == 0)
                {
                    babyGenes.Add(selectedGene);
                }

                else
                {
                    if (((YLinkGene)babyGenes.Get(babyGenes.Size() - 1))
                            .InnovationId != selectedGene.InnovationId)
                    {
                        babyGenes.Add(selectedGene);
                    }
                }

                // Check if we already have the nodes referred to in SelectedGene.
                // If not, they need to be added.
                AddNeuronID(selectedGene.FromNeuronID, vecNeurons);
                AddNeuronID(selectedGene.ToNeuronID, vecNeurons);
            } // end while

            // now create the required nodes. First sort them into order
            vecNeurons.Sort();

            for (int i = 0; i < vecNeurons.Count; i++)
            {
                babyNeurons.Add(Innovations.CreateNeuronFromID(
                    vecNeurons[i]));
            }

            // finally, create the T
            var babyT = new YT(Population
                                                .AssignTID(), babyNeurons, babyGenes, mom.InputCount,
                                            mom.OutputCount);
            babyT.GA = this;
            babyT.Population = Population;

            return babyT;
        }

        /// <summary>
        /// Init the training.
        /// </summary>
        private void Init()
        {
            if (CalculateScore.ShouldMinimize)
            {
                bestEverScore = Double.MaxValue;
            }
            else
            {
                bestEverScore = Double.MinValue;
            }

            // check the population
            foreach (IT obj in Population.Ts)
            {
                if (!(obj is YT))
                {
                    throw new TrainingError(
                        "Population can only contain objects of YT.");
                }

                var Y = (YT)obj;

                if ((Y.InputCount != inputCount)
                    || (Y.OutputCount != outputCount))
                {
                    throw new TrainingError(
                        "All YT's must have the same input and output sizes as the base network.");
                }
                Y.GA = this;
            }

            Population.Claim(this);

            ResetAndKill();
            SortAndRecord();
            SpeciateAndCalculateSpawnLevels();
        }

        /// <summary>
        /// Reset counts and kill Ts with worse scores.
        /// </summary>
        public void ResetAndKill()
        {
            totalFitAdjustment = 0;
            averageFitAdjustment = 0;

            var speciesArray = new ISpecies[Population.Species.Count];

            for (int i = 0; i < Population.Species.Count; i++)
            {
                speciesArray[i] = Population.Species[i];
            }

            foreach (Object element in speciesArray)
            {
                var s = (ISpecies)element;
                s.Purge();

                if ((s.GensNoImprovement > paramNumGensAllowedNoImprovement)
                    && Comparator.IsBetterThan(bestEverScore,
                                               s.BestScore))
                {
                    Population.Species.Remove(s);
                }
            }
        }

        /// <summary>
        /// Sort the Ts.
        /// </summary>
        public void SortAndRecord()
        {
            foreach (IT g in Population.Ts)
            {
                g.Decode();
                PerformCalculateScore(g);
            }

            Population.Sort();

            IT T = Population.Best;
            double currentBest = T.Score;

            if (Comparator.IsBetterThan(currentBest, bestEverScore))
            {
                bestEverScore = currentBest;
                bestEverNetwork = ((YNetwork)T.Organism);
            }

            bestEverScore = Comparator.BestScore(Error,
                                                 bestEverScore);
        }

        /// <summary>
        /// Determine the species.
        /// </summary>
        public void SpeciateAndCalculateSpawnLevels()
        {
            // calculate compatibility between Ts and species
            AdjustCompatibilityThreshold();

            // assign Ts to species (if any exist)
            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                bool added = false;

                foreach (ISpecies s in Population.Species)
                {
                    double compatibility = T.GetCompatibilityScore((YT)s.Leader);

                    if (compatibility <= paramCompatibilityThreshold)
                    {
                        AddSpeciesMember(s, T);
                        T.SpeciesID = s.SpeciesID;
                        added = true;
                        break;
                    }
                }

                // if this T did not fall into any existing species, create a
                // new species
                if (!added)
                {
                    Population.Species.Add(
                        new BasicSpecies(Population, T,
                                         Population.AssignSpeciesID()));
                }
            }

            AdjustSpeciesScore();

            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                totalFitAdjustment += T.AdjustedScore;
            }

            averageFitAdjustment = totalFitAdjustment
                                   / Population.Size();

            foreach (IT g in Population.Ts)
            {
                var T = (YT)g;
                double toSpawn = T.AdjustedScore
                                 / averageFitAdjustment;
                T.AmountToSpawn = toSpawn;
            }

            foreach (ISpecies species in Population.Species)
            {
                species.CalculateSpawnAmount();
            }
        }

        /// <summary>
        /// Select a gene using a tournament.
        /// </summary>
        /// <param name="numComparisons">The number of compares to do.</param>
        /// <returns>The chosen T.</returns>
        public YT TournamentSelection(int numComparisons)
        {
            double bestScoreSoFar = 0;

            int chosenOne = 0;

            for (int i = 0; i < numComparisons; ++i)
            {
                var thisTry = (int)RangeRandomizer.Randomize(0, Population.Size() - 1);

                if (Population.Get(thisTry).Score > bestScoreSoFar)
                {
                    chosenOne = thisTry;

                    bestScoreSoFar = Population.Get(thisTry).Score;
                }
            }

            return (YT)Population.Get(chosenOne);
        }
    }

    public class PSOFactory
    {
        /// <summary>
        /// Create a PSO trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            ParamsHolder holder = new ParamsHolder(args);

            int particles = holder.GetInt(
                    MLTrainFactory.PropertyParticles, false, 20);

            ICalculateScore score = new TrainingSetScore(training);
            IRandomizer randomizer =(IRandomizer)( new NguyenWidrowRandomizer());

            IMLTrain train = new NeuralPSO((BasicNetwork)method, randomizer, score, particles);

            return train;
        }
    }

    public class QuickPropFactory
    {
        /// <summary>
        /// Create a quick propagation trainer.
        /// </summary>
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                               IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);

            double learningRate = holder.GetDouble(
                MLTrainFactory.PropertyLearningRate, false, 2.0);

            return new QuickPropagation((BasicNetwork)method, training, learningRate);
        }
    }

    public class RBFSVDFactory
    {
        /// <summary>
        /// Create a RBF-SVD trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is RBFNetwork))
            {
                throw new SyntError(
                    "RBF-SVD training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new SVDTraining((RBFNetwork)method, training);
        }
    }

    public class RPROPFactory
    {
        /// <summary>
        /// Create a RPROP trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is IContainsFlat))
            {
                throw new SyntError(
                    "RPROP training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            var holder = new ParamsHolder(args);
            double initialUpdate = holder.GetDouble(
                MLTrainFactory.PropertyInitialUpdate, false,
                RPROPConst.DefaultInitialUpdate);
            double maxStep = holder.GetDouble(
                MLTrainFactory.PropertyMaxStep, false,
                RPROPConst.DefaultMaxStep);

            return new ResilientPropagation((IContainsFlat)method, training,
                                            initialUpdate, maxStep);
        }
    }

    public class SVMSearchFactory
    {
        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma1 = "GAMMA1";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC1 = "C1";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGamma2 = "GAMMA2";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyC2 = "C2";

        /// <summary>
        /// Property for gamma.
        /// </summary>
        ///
        public const String PropertyGammaStep = "GAMMASTEP";

        /// <summary>
        /// Property for constant.
        /// </summary>
        ///
        public const String PropertyCStep = "CSTEP";

        /// <summary>
        /// Create a SVM trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="argsStr">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String argsStr)
        {
            if (!(method is SupportVectorMachine))
            {
                throw new SyntError(
                    "SVM Train training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            new ParamsHolder(args);

            var holder = new ParamsHolder(args);
            double gammaStart = holder.GetDouble(
                PropertyGamma1, false,
                SVMSearchTrain.DefaultGammaBegin);
            double cStart = holder.GetDouble(PropertyC1,
                                             false, SVMSearchTrain.DefaultConstBegin);
            double gammaStop = holder.GetDouble(
                PropertyGamma2, false,
                SVMSearchTrain.DefaultGammaEnd);
            double cStop = holder.GetDouble(PropertyC2,
                                            false, SVMSearchTrain.DefaultConstEnd);
            double gammaStep = holder.GetDouble(
                PropertyGammaStep, false,
                SVMSearchTrain.DefaultGammaStep);
            double cStep = holder.GetDouble(PropertyCStep,
                                            false, SVMSearchTrain.DefaultConstStep);

            var result = new SVMSearchTrain((SupportVectorMachine)method, training)
            {
                GammaBegin = gammaStart,
                GammaEnd = gammaStop,
                GammaStep = gammaStep,
                ConstBegin = cStart,
                ConstEnd = cStop,
                ConstStep = cStep
            };

            return result;
        }
    }

    public class SVMTrain : BasicTraining
    {
        /// <summary>
        /// The default starting number for C.
        /// </summary>
        ///
        public const double DefaultConstBegin = -5;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        ///
        public const double DefaultConstEnd = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        ///
        public const double DefaultConstStep = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        ///
        public const double DefaultGammaBegin = -10;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        ///
        public const double DefaultGammaEnd = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        ///
        public const double DefaultGammaStep = 1;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine _network;

        /// <summary>
        /// The problem to train for.
        /// </summary>
        ///
        private readonly svm_problem _problem;

        /// <summary>
        /// The const c value.
        /// </summary>
        ///
        private double _c;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int _fold;

        /// <summary>
        /// The gamma value.
        /// </summary>
        ///
        private double _gamma;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool _trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The network to train.</param>
        /// <param name="dataSet">The training data for this network.</param>
        public SVMTrain(SupportVectorMachine method, IMLDataSet dataSet) : base(TrainingImplementationType.OnePass)
        {
            _fold = 0;
            _network = method;
            Training = dataSet;
            _trainingDone = false;

            _problem = SyntesisSVMProblem.Syntesis(dataSet, 0);
            _gamma = 1.0d / _network.InputCount;
            _c = 1.0d;
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Set the constant C.
        /// </summary>
        public double C
        {
            get { return _c; }
            set
            {
                if (value <= 0 || value < SyntFramework.DefaultDoubleEqual)
                {
                    throw new SyntError("SVM training cannot use a c value less than zero.");
                }

                _c = value;
            }
        }


        /// <summary>
        /// Set the number of folds.
        /// </summary>
        public int Fold
        {
            get { return _fold; }
            set { _fold = value; }
        }


        /// <summary>
        /// Set the gamma.
        /// </summary>
        public double Gamma
        {
            get { return _gamma; }
            set
            {
                if (value <= 0 || value < SyntFramework.DefaultDoubleEqual)
                {
                    throw new SyntError("SVM training cannot use a gamma value less than zero.");
                }
                _gamma = value;
            }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <value>The problem being trained.</value>
        public svm_problem Problem
        {
            get { return _problem; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return _trainingDone; }
        }

        /// <summary>
        /// Evaluate the error for the specified model.
        /// </summary>
        ///
        /// <param name="param">The params for the SVN.</param>
        /// <param name="prob">The problem to evaluate.</param>
        /// <param name="target">The output values from the SVN.</param>
        /// <returns>The calculated error.</returns>
        private static double Evaluate(svm_parameter param, svm_problem prob,
                                double[] target)
        {
            int totalCorrect = 0;

            var error = new ErrorCalculation();

            if ((param.svm_type == svm_parameter.EPSILON_SVR)
                || (param.svm_type == svm_parameter.NU_SVR))
            {
                for (int i = 0; i < prob.l; i++)
                {
                    double ideal = prob.y[i];
                    double actual = target[i];
                    error.UpdateError(actual, ideal);
                }
                return error.Calculate();
            }
            for (int i = 0; i < prob.l; i++)
            {
                if (target[i] == prob.y[i])
                {
                    ++totalCorrect;
                }
            }

            return Format.HundredPercent * totalCorrect / prob.l;
        }


        /// <summary>
        /// Perform either a train or a cross validation.  If the folds property is 
        /// greater than 1 then cross validation will be done.  Cross validation does 
        /// not produce a usable model, but it does set the error. 
        /// If you are cross validating try C and Gamma values until you have a good 
        /// error rate.  Then use those values to train, producing the final model.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            _network.Params.C = _c;
            _network.Params.gamma = _gamma;

            SyntLogging.Log(SyntLogging.LevelInfo, "Training with parameters C = " + _c + ", gamma = " + _gamma);

            if (_fold > 1)
            {
                // cross validate
                var target = new double[_problem.l];

                svm.svm_cross_validation(_problem, _network.Params,
                                         _fold, target);
                _network.Model = null;

                Error = Evaluate(_network.Params, _problem, target);
            }
            else
            {
                // train
                _network.Model = svm.svm_train(_problem,
                                              _network.Params);

                Error = _network.CalculateError(Training);
            }

            _trainingDone = true;
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }
    }

    public class SVMSearchTrain : BasicTraining
    {
        /// <summary>
        /// The default starting number for C.
        /// </summary>
        ///
        public const double DefaultConstBegin = 1;

        /// <summary>
        /// The default ending number for C.
        /// </summary>
        ///
        public const double DefaultConstEnd = 15;

        /// <summary>
        /// The default step for C.
        /// </summary>
        ///
        public const double DefaultConstStep = 2;

        /// <summary>
        /// The default gamma begin.
        /// </summary>
        ///
        public const double DefaultGammaBegin = 1;

        /// <summary>
        /// The default gamma end.
        /// </summary>
        ///
        public const double DefaultGammaEnd = 10;

        /// <summary>
        /// The default gamma step.
        /// </summary>
        ///
        public const double DefaultGammaStep = 1;

        /// <summary>
        /// The internal training object, used for the search.
        /// </summary>
        ///
        private readonly SVMTrain _internalTrain;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly SupportVectorMachine _network;

        /// <summary>
        /// The best values found for C.
        /// </summary>
        ///
        private double _bestConst;

        /// <summary>
        /// The best error.
        /// </summary>
        ///
        private double _bestError;

        /// <summary>
        /// The best values found for gamma.
        /// </summary>
        ///
        private double _bestGamma;

        /// <summary>
        /// The beginning value for C.
        /// </summary>
        ///
        private double _constBegin;

        /// <summary>
        /// The ending value for C.
        /// </summary>
        ///
        private double _constEnd;

        /// <summary>
        /// The step value for C.
        /// </summary>
        ///
        private double _constStep;

        /// <summary>
        /// The current C.
        /// </summary>
        ///
        private double _currentConst;

        /// <summary>
        /// The current gamma.
        /// </summary>
        ///
        private double _currentGamma;

        /// <summary>
        /// The number of folds.
        /// </summary>
        ///
        private int _fold;

        /// <summary>
        /// The beginning value for gamma.
        /// </summary>
        ///
        private double _gammaBegin;

        /// <summary>
        /// The ending value for gamma.
        /// </summary>
        ///
        private double _gammaEnd;

        /// <summary>
        /// The step value for gamma.
        /// </summary>
        ///
        private double _gammaStep;

        /// <summary>
        /// Is the network setup.
        /// </summary>
        ///
        private bool _isSetup;

        /// <summary>
        /// Is the training done.
        /// </summary>
        ///
        private bool _trainingDone;

        /// <summary>
        /// Construct a trainer for an SVM network.
        /// </summary>
        ///
        /// <param name="method">The method to train.</param>
        /// <param name="training">The training data for this network.</param>
        public SVMSearchTrain(SupportVectorMachine method, IMLDataSet training)
            : base(TrainingImplementationType.Iterative)
        {
            _fold = 0;
            _constBegin = DefaultConstBegin;
            _constStep = DefaultConstStep;
            _constEnd = DefaultConstEnd;
            _gammaBegin = DefaultGammaBegin;
            _gammaEnd = DefaultGammaEnd;
            _gammaStep = DefaultGammaStep;
            _network = method;
            Training = training;
            _isSetup = false;
            _trainingDone = false;

            _internalTrain = new SVMTrain(_network, training);
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <value>the constBegin to set</value>
        public double ConstBegin
        {
            get { return _constBegin; }
            set { _constBegin = value; }
        }


        /// <value>the constEnd to set</value>
        public double ConstEnd
        {
            get { return _constEnd; }
            set { _constEnd = value; }
        }


        /// <value>the constStep to set</value>
        public double ConstStep
        {
            get { return _constStep; }
            set { _constStep = value; }
        }


        /// <value>the fold to set</value>
        public int Fold
        {
            get { return _fold; }
            set { _fold = value; }
        }


        /// <value>the gammaBegin to set</value>
        public double GammaBegin
        {
            get { return _gammaBegin; }
            set { _gammaBegin = value; }
        }


        /// <value>the gammaEnd to set.</value>
        public double GammaEnd
        {
            get { return _gammaEnd; }
            set { _gammaEnd = value; }
        }


        /// <value>the gammaStep to set</value>
        public double GammaStep
        {
            get { return _gammaStep; }
            set { _gammaStep = value; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <value>True if the training is done.</value>
        public override bool TrainingDone
        {
            get { return _trainingDone; }
        }

        /// <inheritdoc/>
        public override sealed void FinishTraining()
        {
            _internalTrain.Gamma = _bestGamma;
            _internalTrain.C = _bestConst;
            _internalTrain.Iteration();
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        public override sealed void Iteration()
        {
            if (!_trainingDone)
            {
                if (!_isSetup)
                {
                    Setup();
                }

                PreIteration();

                _internalTrain.Fold = _fold;

                if (_network.KernelType == KernelType.RadialBasisFunction)
                {
                    _internalTrain.Gamma = _currentGamma;
                    _internalTrain.C = _currentConst;
                    _internalTrain.Iteration();
                    double e = _internalTrain.Error;

                    //System.out.println(this.currentGamma + "," + this.currentConst
                    //		+ "," + e);

                    // new best error?
                    if (!Double.IsNaN(e))
                    {
                        if (e < _bestError)
                        {
                            _bestConst = _currentConst;
                            _bestGamma = _currentGamma;
                            _bestError = e;
                        }
                    }

                    // advance
                    _currentConst += _constStep;
                    if (_currentConst > _constEnd)
                    {
                        _currentConst = _constBegin;
                        _currentGamma += _gammaStep;
                        if (_currentGamma > _gammaEnd)
                        {
                            _trainingDone = true;
                        }
                    }

                    Error = _bestError;
                }
                else
                {
                    _internalTrain.Gamma = _currentGamma;
                    _internalTrain.C = _currentConst;
                    _internalTrain.Iteration();
                }

                PostIteration();
            }
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        /// Setup to train the SVM.
        /// </summary>
        ///
        private void Setup()
        {
            _currentConst = _constBegin;
            _currentGamma = _gammaBegin;
            _bestError = Double.PositiveInfinity;
            _isSetup = true;

            if (_currentGamma <= 0 || _currentGamma < SyntFramework.DefaultDoubleEqual)
            {
                throw new SyntError("SVM search training cannot use a gamma value less than zero.");
            }

            if (_currentConst <= 0 || _currentConst < SyntFramework.DefaultDoubleEqual)
            {
                throw new SyntError("SVM search training cannot use a const value less than zero.");
            }

            if (_gammaStep < 0)
            {
                throw new SyntError("SVM search gamma step cannot use a const value less than zero.");
            }

            if (_constStep < 0)
            {
                throw new SyntError("SVM search const step cannot use a const value less than zero.");
            }
        }
    }
    
    public class TrainBayesianFactory
    {
        /**
 * Create a K2 trainer.
 * 
 * @param method
 *            The method to use.
 * @param training
 *            The training data to use.
 * @param argsStr
 *            The arguments to use.
 * @return The newly created trainer.
 */
        public IMLTrain Create(IMLMethod method,
                IMLDataSet training, String argsStr)
        {
            IDictionary<String, String> args = ArchitectureParse.ParseParams(argsStr);
            ParamsHolder holder = new ParamsHolder(args);

            int maxParents = holder.GetInt(
                    MLTrainFactory.PropertyMaxParents, false, 1);
            String searchStr = holder.GetString("SEARCH", false, "k2");
            String estimatorStr = holder.GetString("ESTIMATOR", false, "simple");
            String initStr = holder.GetString("INIT", false, "naive");

            IBayesSearch search;
            IBayesEstimator estimator;
            BayesianInit init;

            if (string.Compare(searchStr, "k2", true) == 0)
            {
                search = new SearchK2();
            }
            else if (string.Compare(searchStr, "none", true) == 0)
            {
                search = new SearchNone();
            }
            else
            {
                throw new BayesianError("Invalid search type: " + searchStr);
            }

            if (string.Compare(estimatorStr, "simple", true) == 0)
            {
                estimator = new SimpleEstimator();
            }
            else if (string.Compare(estimatorStr, "none", true) == 0)
            {
                estimator = new EstimatorNone();
            }
            else
            {
                throw new BayesianError("Invalid estimator type: " + estimatorStr);
            }

            if (string.Compare(initStr, "simple") == 0)
            {
                init = BayesianInit.InitEmpty;
            }
            else if (string.Compare(initStr, "naive") == 0)
            {
                init = BayesianInit.InitNaiveBayes;
            }
            else if (string.Compare(initStr, "none") == 0)
            {
                init = BayesianInit.InitNoChange;
            }
            else
            {
                throw new BayesianError("Invalid init type: " + initStr);
            }

            return new TrainBayesian((BayesianNetwork)method, training, maxParents, init, search, estimator);
        }
    }

    public class MLActivationFactory
    {
        public const String AF_BIPOLAR = "bipolar";
        public const String AF_COMPETITIVE = "comp";
        public const String AF_GAUSSIAN = "gauss";
        public const String AF_LINEAR = "linear";
        public const String AF_LOG = "log";
        public const String AF_RAMP = "ramp";
        public const String AF_SIGMOID = "sigmoid";
        public const String AF_SIN = "sin";
        public const String AF_SOFTMAX = "softmax";
        public const String AF_STEP = "step";
        public const String AF_TANH = "tanh";

        public IActivationFunction Create(String fn)
        {

            foreach (SyntPluginBase plugin in SyntFramework.Instance.Plugins)
            {
                if (plugin is ISyntPluginService1)
                {
                    IActivationFunction result = ((ISyntPluginService1)plugin).CreateActivationFunction(fn);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }

    public class MLMethodFactory
    {
        /// <summary>
        /// String constant for a bayesian neural network.
        /// </summary>
	    public const String TypeBayesian = "bayesian";

        /// <summary>
        /// String constant for feedforward neural networks.
        /// </summary>
        ///
        public const String TypeFeedforward = "feedforward";

        /// <summary>
        /// String constant for RBF neural networks.
        /// </summary>
        ///
        public const String TypeRbfnetwork = "rbfnetwork";

        /// <summary>
        /// String constant for support vector machines.
        /// </summary>
        ///
        public const String TypeSVM = "svm";

        /// <summary>
        /// String constant for SOMs.
        /// </summary>
        ///
        public const String TypeSOM = "som";

        /// <summary>
        /// A probabilistic neural network. Supports both PNN and GRNN.
        /// </summary>
        ///
        public const String TypePNN = "pnn";

        /// <summary>
        /// Create a new machine learning method.
        /// </summary>
        ///
        /// <param name="methodType">The method to create.</param>
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created machine learning method.</returns>
        public IMLMethod Create(String methodType,
                               String architecture, int input, int output)
        {
            foreach (SyntPluginBase plugin in SyntFramework.Instance.Plugins)
            {
                if (plugin is ISyntPluginService1)
                {
                    IMLMethod result = ((ISyntPluginService1)plugin).CreateMethod(
                            methodType, architecture, input, output);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            throw new SyntError("Unknown method type: " + methodType);
        }
    }

    [Serializable]
    public class Q
    {
        /// <summary>
        /// The individual elements of this Q.
        /// </summary>
        ///
        private readonly List<IGene> _genes;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public Q()
        {
            _genes = new List<IGene>();
        }

        /// <summary>
        /// Used the get the entire gene list.
        /// </summary>
        ///
        /// <value>the genes</value>
        public List<IGene> Genes
        {
            get { return _genes; }
        }

        /// <summary>
        /// Add a gene.
        /// </summary>
        ///
        /// <param name="gene">The gene to add.</param>
        public void Add(IGene gene)
        {
            _genes.Add(gene);
        }

        /// <summary>
        /// Get an individual gene.
        /// </summary>
        ///
        /// <param name="i">The index of the gene.</param>
        /// <returns>The gene.</returns>
        public IGene Get(int i)
        {
            return _genes[i];
        }

        /// <summary>
        /// Get the specified gene.
        /// </summary>
        ///
        /// <param name="gene">The specified gene.</param>
        /// <returns>The gene specified.</returns>
        public IGene GetGene(int gene)
        {
            return _genes[gene];
        }


        /// <returns>The number of genes in this Q.</returns>
        public int Size()
        {
            return _genes.Count;
        }
    }

    public class FlatLayer
    {
        /// <summary>
        /// The neuron count.
        /// </summary>
        ///
        private readonly int _count;

        /// <summary>
        /// The bias activation, usually 1 for bias or 0 for no bias.
        /// </summary>
        ///
        private double _biasActivation;

        /// <summary>
        /// The layer that feeds this layer's context.
        /// </summary>
        ///
        private FlatLayer _contextFedBy;

        /// <summary>
        /// Construct a flat layer.
        /// </summary>
        ///
        /// <param name="activation">The activation function.</param>
        /// <param name="count">The neuron count.</param>
        /// <param name="biasActivation">The bias activation.</param>
        public FlatLayer(IActivationFunction activation, int count,
                         double biasActivation)
        {
            Activation = activation;
            _count = count;
            _biasActivation = biasActivation;
            _contextFedBy = null;
        }


        /// <value>the activation to set</value>
        public IActivationFunction Activation { get; set; }


        /// <summary>
        /// Set the bias activation.
        /// </summary>
        public double BiasActivation
        {
            get
            {
                if (HasBias())
                {
                    return _biasActivation;
                }
                return 0;
            }
            set { _biasActivation = value; }
        }


        /// <value>The number of neurons our context is fed by.</value>
        public int ContextCount
        {
            get
            {
                if (_contextFedBy == null)
                {
                    return 0;
                }
                return _contextFedBy.Count;
            }
        }


        /// <summary>
        /// Set the layer that this layer's context is fed by.
        /// </summary>
        public FlatLayer ContextFedBy
        {
            get { return _contextFedBy; }
            set { _contextFedBy = value; }
        }


        /// <value>the count</value>
        public int Count
        {
            get { return _count; }
        }


        /// <value>The total number of neurons on this layer, includes context, bias
        /// and regular.</value>
        public int TotalCount
        {
            get
            {
                if (_contextFedBy == null)
                {
                    return Count + ((HasBias()) ? 1 : 0);
                }
                return Count + ((HasBias()) ? 1 : 0)
                       + _contextFedBy.Count;
            }
        }


        /// <returns>the bias</returns>
        public bool HasBias()
        {
            return Math.Abs(_biasActivation) > SyntFramework.DefaultDoubleEqual;
        }

        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder();
            result.Append("[");
            result.Append(GetType().Name);
            result.Append(": count=");
            result.Append(_count);
            result.Append(",bias=");

            if (HasBias())
            {
                result.Append(_biasActivation);
            }
            else
            {
                result.Append("false");
            }
            if (_contextFedBy != null)
            {
                result.Append(",contextFed=");
                if (_contextFedBy == this)
                {
                    result.Append("itself");
                }
                else
                {
                    result.Append(_contextFedBy);
                }
            }
            result.Append("]");
            return result.ToString();
        }
    }

    public class TComparator : IComparer<IT>
    {
        /// <summary>
        /// The method to calculate the score.
        /// </summary>
        ///
        private readonly ICalculateTScore _calculateScore;

        /// <summary>
        /// Construct the T comparator.
        /// </summary>
        ///
        /// <param name="theCalculateScore">The score calculation object to use.</param>
        public TComparator(ICalculateTScore theCalculateScore)
        {
            _calculateScore = theCalculateScore;
        }

        /// <value>The score calculation object.</value>
        public ICalculateTScore CalculateScore
        {
            get { return _calculateScore; }
        }

        #region IComparer<IT> Members

        /// <summary>
        /// Compare two Ts.
        /// </summary>
        ///
        /// <param name="T1">The first T.</param>
        /// <param name="T2">The second T.</param>
        /// <returns>Zero if equal, or less than or greater than zero to indicate
        /// order.</returns>
        public int Compare(IT T1, IT T2)
        {
            return T1.Score.CompareTo(T2.Score);
        }

        #endregion

        /// <summary>
        /// Apply a bonus, this is a simple percent that is applied in the direction
        /// specified by the "should minimize" property of the score function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyBonus(double v, double bonus)
        {
            double amount = v * bonus;
            if (_calculateScore.ShouldMinimize)
            {
                return v - amount;
            }
            return v + amount;
        }

        /// <summary>
        /// Apply a penalty, this is a simple percent that is applied in the
        /// direction specified by the "should minimize" property of the score
        /// function.
        /// </summary>
        ///
        /// <param name="v">The current value.</param>
        /// <param name="bonus">The penalty.</param>
        /// <returns>The resulting value.</returns>
        public double ApplyPenalty(double v, double bonus)
        {
            double amount = v * bonus;
            return _calculateScore.ShouldMinimize ? v - amount : v + amount;
        }

        /// <summary>
        /// Determine the best score from two scores, uses the "should minimize"
        /// property of the score function.
        /// </summary>
        ///
        /// <param name="d1">The first score.</param>
        /// <param name="d2">The second score.</param>
        /// <returns>The best score.</returns>
        public double BestScore(double d1, double d2)
        {
            return _calculateScore.ShouldMinimize ? Math.Min(d1, d2) : Math.Max(d1, d2);
        }


        /// <summary>
        /// Determine if one score is better than the other.
        /// </summary>
        ///
        /// <param name="d1">The first score to compare.</param>
        /// <param name="d2">The second score to compare.</param>
        /// <returns>True if d1 is better than d2.</returns>
        public bool IsBetterThan(double d1, double d2)
        {
            return _calculateScore.ShouldMinimize ? d1 < d2 : d1 > d2;
        }
    }

    public class SCGFactory
    {
        /// <summary>
        /// Create a SCG trainer.
        /// </summary>
        ///
        /// <param name="method">The method to use.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="args">The arguments to use.</param>
        /// <returns>The newly created trainer.</returns>
        public IMLTrain Create(IMLMethod method,
                              IMLDataSet training, String args)
        {
            if (!(method is BasicNetwork))
            {
                throw new SyntError(
                    "SCG training cannot be used on a method of type: "
                    + method.GetType().FullName);
            }

            return new ScaledConjugateGradient((BasicNetwork)method, training);
        }
    }

    public class BasicNeuralData : BasicMLData, INeuralData
    {
        /// <summary>
        /// Construct the object from an array.
        /// </summary>
        /// <param name="d">The array to base on.</param>
        public BasicNeuralData(double[] d) : base(d)
        {
        }

        /// <summary>
        /// Construct an empty array of the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        public BasicNeuralData(int size) : base(size)
        {
        }
    }

    public class CrossValidationKFold : CrossTraining
    {
        /// <summary>
        /// The flat network to train.
        /// </summary>
        ///
        private readonly FlatNetwork _flatNetwork;

        /// <summary>
        /// The network folds.
        /// </summary>
        ///
        private readonly NetworkFold[] _networks;

        /// <summary>
        /// The underlying trainer to use. This trainer does the actual training.
        /// </summary>
        ///
        private readonly IMLTrain _train;

        /// <summary>
        /// Construct a cross validation trainer.
        /// </summary>
        ///
        /// <param name="train">The training</param>
        /// <param name="k">The number of folds.</param>
        public CrossValidationKFold(IMLTrain train, int k) : base(train.Method, (FoldedDataSet)train.Training)
        {
            _train = train;
            Folded.Fold(k);

            _flatNetwork = ((BasicNetwork)train.Method).Structure.Flat;

            _networks = new NetworkFold[k];
            for (int i = 0; i < _networks.Length; i++)
            {
                _networks[i] = new NetworkFold(_flatNetwork);
            }
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        ///
        public override void Iteration()
        {
            double error = 0;

            for (int valFold = 0; valFold < Folded.NumFolds; valFold++)
            {
                //// restore the correct network
                //_networks[valFold].CopyToNetwork(_flatNetwork);

                //// train with non-validation folds
                //for (int curFold = 0; curFold < Folded.NumFolds; curFold++)
                //{
                //    if (curFold != valFold)
                //    {
                //        Folded.CurrentFold = curFold;
                //        _train.Iteration();
                //    }
                //}

                //// evaluate with the validation fold			
                //Folded.CurrentFold = valFold;
                //double e = _flatNetwork.CalculateError(Folded);
                ////System.out.println("Fold " + valFold + ", " + e);
                //error += e;
                //_networks[valFold].CopyFromNetwork(_flatNetwork);
            }

            Error = error / Folded.NumFolds;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Resume(TrainingContinuation state)
        {
        }
    }

    public class StopTrainingStrategy : IEndTrainingStrategy
    {
        /// <summary>
        /// The default minimum improvement before training stops.
        /// </summary>
        ///
        public const double DefaultMinImprovement = 0.0000001d;

        /// <summary>
        /// The default number of cycles to tolerate.
        /// </summary>
        ///
        public const int DefaultTolerateCycles = 100;

        /// <summary>
        /// The minimum improvement before training stops.
        /// </summary>
        ///
        private readonly double _minImprovement;

        /// <summary>
        /// The number of cycles to tolerate the minimum improvement.
        /// </summary>
        ///
        private readonly int _toleratedCycles;

        /// <summary>
        /// The number of bad training cycles.
        /// </summary>
        ///
        private int _badCycles;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _bestError;

        /// <summary>
        /// The error rate from the previous iteration.
        /// </summary>
        ///
        private double _lastError;

        /// <summary>
        /// Has one iteration passed, and we are now ready to start evaluation.
        /// </summary>
        ///
        private bool _ready;

        /// <summary>
        /// Flag to indicate if training should stop.
        /// </summary>
        ///
        private bool _shouldStop;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private IMLTrain _train;

        /// <summary>
        /// Construct the strategy with default options.
        /// </summary>
        ///
        public StopTrainingStrategy() : this(DefaultMinImprovement, DefaultTolerateCycles)
        {
        }

        /// <summary>
        /// Construct the strategy with the specified parameters.
        /// </summary>
        ///
        /// <param name="minImprovement">The minimum accepted improvement.</param>
        /// <param name="toleratedCycles">The number of cycles to tolerate before stopping.</param>
        public StopTrainingStrategy(double minImprovement,
                                    int toleratedCycles)
        {
            _minImprovement = minImprovement;
            _toleratedCycles = toleratedCycles;
            _badCycles = 0;
            _bestError = Double.MaxValue;
        }

        #region EndTrainingStrategy Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void Init(IMLTrain train)
        {
            _train = train;
            _shouldStop = false;
            _ready = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PostIteration()
        {
            if (_ready)
            {
                if (Math.Abs(_bestError - _train.Error) < _minImprovement)
                {
                    _badCycles++;
                    if (_badCycles > _toleratedCycles)
                    {
                        _shouldStop = true;
                    }
                }
                else
                {
                    _badCycles = 0;
                }
            }
            else
            {
                _ready = true;
            }

            _lastError = _train.Error;
            _bestError = Math.Min(_lastError, _bestError);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual void PreIteration()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual bool ShouldStop()
        {
            return _shouldStop;
        }

        #endregion
    }
    [Serializable]
    public class PersistYPopulation : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <summary>
        /// The persistence class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return typeof(YPopulation).Name; }
        }


        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read the object from.</param>
        /// <returns>The object that was loaded.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new YPopulation();
            var innovationList = new YInnovationList { Population = result };
            result.Innovations = innovationList;
            var ins0 = new SyntReadHelper(mask0);
            IDictionary<Int32, ISpecies> speciesMap = new Dictionary<Int32, ISpecies>();
            IDictionary<ISpecies, Int32> leaderMap = new Dictionary<ISpecies, Int32>();
            IDictionary<Int32, IT> TMap = new Dictionary<Int32, IT>();
            SyntFileSection section;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("Y-POPULATION")
                    && section.SubSectionName.Equals("INNOVATIONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        var innovation = new YInnovation
                        {
                            InnovationID = Int32.Parse(cols[0]),
                            InnovationType = StringToInnovationType(cols[1]),
                            NeuronType = StringToNeuronType(cols[2]),
                            SplitX = CSVFormat.EgFormat.Parse(cols[3]),
                            SplitY = CSVFormat.EgFormat.Parse(cols[4]),
                            NeuronID = Int32.Parse(cols[5]),
                            FromNeuronID = Int32.Parse(cols[6]),
                            ToNeuronID = Int32.Parse(cols[7])
                        };
                        result.Innovations.Add(innovation);
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("SPECIES"))
                {
                    foreach (String line in section.Lines)
                    {
                        String[] cols = line.Split(',');
                        var species = new BasicSpecies
                        {
                            SpeciesID = Int32.Parse(cols[0]),
                            Age = Int32.Parse(cols[1]),
                            BestScore = CSVFormat.EgFormat.Parse(cols[2]),
                            GensNoImprovement = Int32.Parse(cols[3]),
                            SpawnsRequired = CSVFormat.EgFormat
                                                  .Parse(cols[4])
                        };

                        species.SpawnsRequired = CSVFormat.EgFormat
                            .Parse(cols[5]);
                        leaderMap[(species)] = (Int32.Parse(cols[6]));
                        result.Species.Add(species);
                        speciesMap[((int)species.SpeciesID)] = (species);
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("TS"))
                {
                    YT lastT = null;

                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        if (cols[0].Equals("g", StringComparison.InvariantCultureIgnoreCase))
                        {
                            lastT = new YT
                            {
                                NeuronsQ = new Q(),
                                LinksQ = new Q()
                            };
                            lastT.Qs.Add(lastT.NeuronsQ);
                            lastT.Qs.Add(lastT.LinksQ);
                            lastT.TID = Int32.Parse(cols[1]);
                            lastT.SpeciesID = Int32.Parse(cols[2]);
                            lastT.AdjustedScore = CSVFormat.EgFormat
                                .Parse(cols[3]);
                            lastT.AmountToSpawn = CSVFormat.EgFormat
                                .Parse(cols[4]);
                            lastT.NetworkDepth = Int32.Parse(cols[5]);
                            lastT.Score = CSVFormat.EgFormat.Parse(cols[6]);
                            result.Add(lastT);
                            TMap[(int)lastT.TID] = lastT;
                        }
                        else if (cols[0].Equals("n", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var neuronGene = new YNeuronGene
                            {
                                Id = Int32.Parse(cols[1]),
                                NeuronType = StringToNeuronType(cols[2]),
                                Enabled = Int32.Parse(cols[3]) > 0,
                                InnovationId = Int32.Parse(cols[4]),
                                ActivationResponse = CSVFormat.EgFormat
                                                         .Parse(cols[5]),
                                SplitX = CSVFormat.EgFormat.Parse(cols[6]),
                                SplitY = CSVFormat.EgFormat.Parse(cols[7])
                            };
                            lastT.Neurons.Add(neuronGene);
                        }
                        else if (cols[0].Equals("l", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var linkGene = new YLinkGene();
                            linkGene.Id = Int32.Parse(cols[1]);
                            linkGene.Enabled = Int32.Parse(cols[2]) > 0;
                            linkGene.Recurrent = Int32.Parse(cols[3]) > 0;
                            linkGene.FromNeuronID = Int32.Parse(cols[4]);
                            linkGene.ToNeuronID = Int32.Parse(cols[5]);
                            linkGene.Weight = CSVFormat.EgFormat.Parse(cols[6]);
                            linkGene.InnovationId = Int32.Parse(cols[7]);
                            lastT.Links.Add(linkGene);
                        }
                    }
                }
                else if (section.SectionName.Equals("Y-POPULATION")
                         && section.SubSectionName.Equals("CONFIG"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    result.YActivationFunction = SyntFileSection
                        .ParseActivationFunction(paras,
                                                 YPopulation.PropertyYActivation);
                    result.OutputActivationFunction = SyntFileSection
                        .ParseActivationFunction(paras,
                                                 YPopulation.PropertyOutputActivation);
                    result.Snapshot = SyntFileSection.ParseBoolean(paras,
                                                                    PersistConst.Snapshot);
                    result.InputCount = SyntFileSection.ParseInt(paras,
                                                                  PersistConst.InputCount);
                    result.OutputCount = SyntFileSection.ParseInt(paras,
                                                                   PersistConst.OutputCount);
                    result.OldAgePenalty = SyntFileSection.ParseDouble(paras,
                                                                        PopulationConst.PropertyOldAgePenalty);
                    result.OldAgeThreshold = SyntFileSection.ParseInt(paras,
                                                                       PopulationConst.PropertyOldAgeThreshold);
                    result.PopulationSize = SyntFileSection.ParseInt(paras,
                                                                      PopulationConst.PropertyPopulationSize);
                    result.SurvivalRate = SyntFileSection.ParseDouble(paras,
                                                                       PopulationConst.PropertySurvivalRate);
                    result.YoungBonusAgeThreshhold = SyntFileSection.ParseInt(
                        paras, PopulationConst.PropertyYoungAgeThreshold);
                    result.YoungScoreBonus = SyntFileSection.ParseDouble(paras,
                                                                          PopulationConst.PropertyYoungAgeBonus);
                    result.TIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                  PopulationConst.
                                                                                      PropertyNextTID);
                    result.InnovationIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                      PopulationConst.
                                                                                          PropertyNextInnovationID);
                    result.GeneIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                PopulationConst.
                                                                                    PropertyNextGeneID);
                    result.SpeciesIDGenerate.CurrentID = SyntFileSection.ParseInt(paras,
                                                                                   PopulationConst.
                                                                                       PropertyNextSpeciesID);
                }
            }

            // now link everything up


            // first put all the Ts into correct species
            foreach (IT T in result.Ts)
            {
                var YT = (YT)T;
                var speciesId = (int)YT.SpeciesID;
                if (speciesMap.ContainsKey(speciesId))
                {
                    ISpecies s = speciesMap[speciesId];
                    s.Members.Add(YT);
                }

                YT.InputCount = result.InputCount;
                YT.OutputCount = result.OutputCount;
            }


            // set the species leader links
            foreach (ISpecies species in leaderMap.Keys)
            {
                int leaderID = leaderMap[species];
                IT leader = TMap[leaderID];
                species.Leader = leader;
                ((BasicSpecies)species).Population = result;
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The stream to write to.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var pop = (YPopulation)obj;
            xout.AddSection("Y-POPULATION");
            xout.AddSubSection("CONFIG");
            xout.WriteProperty(PersistConst.Snapshot, pop.Snapshot);
            xout.WriteProperty(YPopulation.PropertyOutputActivation,
                               pop.OutputActivationFunction);
            xout.WriteProperty(YPopulation.PropertyYActivation,
                               pop.YActivationFunction);
            xout.WriteProperty(PersistConst.InputCount, pop.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, pop.OutputCount);
            xout.WriteProperty(PopulationConst.PropertyOldAgePenalty,
                               pop.OldAgePenalty);
            xout.WriteProperty(PopulationConst.PropertyOldAgeThreshold,
                               pop.OldAgeThreshold);
            xout.WriteProperty(PopulationConst.PropertyPopulationSize,
                               pop.PopulationSize);
            xout.WriteProperty(PopulationConst.PropertySurvivalRate,
                               pop.SurvivalRate);
            xout.WriteProperty(PopulationConst.PropertyYoungAgeThreshold,
                               pop.YoungBonusAgeThreshold);
            xout.WriteProperty(PopulationConst.PropertyYoungAgeBonus,
                               pop.YoungScoreBonus);
            xout.WriteProperty(PopulationConst.PropertyNextTID, pop.TIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextInnovationID, pop.InnovationIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextGeneID, pop.GeneIDGenerate.CurrentID);
            xout.WriteProperty(PopulationConst.PropertyNextSpeciesID, pop.SpeciesIDGenerate.CurrentID);
            xout.AddSubSection("INNOVATIONS");
            if (pop.Innovations != null)
            {
                foreach (IInnovation innovation in pop.Innovations.Innovations)
                {
                    var YInnovation = (YInnovation)innovation;
                    xout.AddColumn(YInnovation.InnovationID);
                    xout.AddColumn(InnovationTypeToString(YInnovation.InnovationType));
                    xout.AddColumn(NeuronTypeToString(YInnovation.NeuronType));
                    xout.AddColumn(YInnovation.SplitX);
                    xout.AddColumn(YInnovation.SplitY);
                    xout.AddColumn(YInnovation.NeuronID);
                    xout.AddColumn(YInnovation.FromNeuronID);
                    xout.AddColumn(YInnovation.ToNeuronID);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("TS");

            foreach (IT T in pop.Ts)
            {
                var YT = (YT)T;
                xout.AddColumn("g");
                xout.AddColumn(YT.TID);
                xout.AddColumn(YT.SpeciesID);
                xout.AddColumn(YT.AdjustedScore);
                xout.AddColumn(YT.AmountToSpawn);
                xout.AddColumn(YT.NetworkDepth);
                xout.AddColumn(YT.Score);
                xout.WriteLine();


                foreach (IGene neuronGene in YT.Neurons.Genes)
                {
                    var YNeuronGene = (YNeuronGene)neuronGene;
                    xout.AddColumn("n");
                    xout.AddColumn(YNeuronGene.Id);
                    xout.AddColumn(NeuronTypeToString(YNeuronGene.NeuronType));
                    xout.AddColumn(YNeuronGene.Enabled);
                    xout.AddColumn(YNeuronGene.InnovationId);
                    xout.AddColumn(YNeuronGene.ActivationResponse);
                    xout.AddColumn(YNeuronGene.SplitX);
                    xout.AddColumn(YNeuronGene.SplitY);
                    xout.WriteLine();
                }

                foreach (IGene linkGene in YT.Links.Genes)
                {
                    var YLinkGene = (YLinkGene)linkGene;
                    xout.AddColumn("l");
                    xout.AddColumn(YLinkGene.Id);
                    xout.AddColumn(YLinkGene.Enabled);
                    xout.AddColumn(YLinkGene.Recurrent);
                    xout.AddColumn(YLinkGene.FromNeuronID);
                    xout.AddColumn(YLinkGene.ToNeuronID);
                    xout.AddColumn(YLinkGene.Weight);
                    xout.AddColumn(YLinkGene.InnovationId);
                    xout.WriteLine();
                }
            }
            xout.AddSubSection("SPECIES");

            foreach (ISpecies species in pop.Species)
            {
                xout.AddColumn(species.SpeciesID);
                xout.AddColumn(species.Age);
                xout.AddColumn(species.BestScore);
                xout.AddColumn(species.GensNoImprovement);
                xout.AddColumn(species.NumToSpawn);
                xout.AddColumn(species.SpawnsRequired);
                xout.AddColumn(species.Leader.TID);
                xout.WriteLine();
            }
            xout.Flush();
        }

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        #endregion

        /// <summary>
        /// Convert the neuron type to a string.
        /// </summary>
        /// <param name="t">The neuron type.</param>
        /// <returns>The string.</returns>
        public static String NeuronTypeToString(YNeuronType t)
        {
            switch (t)
            {
                case YNeuronType.Bias:
                    return ("b");
                case YNeuronType.Hidden:
                    return ("h");
                case YNeuronType.Input:
                    return ("i");
                case YNeuronType.None:
                    return ("n");
                case YNeuronType.Output:
                    return ("o");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert the innovation type to a string.
        /// </summary>
        /// <param name="t">The innovation type.</param>
        /// <returns>The string.</returns>
        public static String InnovationTypeToString(YInnovationType t)
        {
            switch (t)
            {
                case YInnovationType.NewLink:
                    return "l";
                case YInnovationType.NewNeuron:
                    return "n";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Convert a string to an innovation type.
        /// </summary>
        /// <param name="t">The string to convert.</param>
        /// <returns>The innovation type.</returns>
        public static YInnovationType StringToInnovationType(String t)
        {
            if (t.Equals("l", StringComparison.InvariantCultureIgnoreCase))
            {
                return YInnovationType.NewLink;
            }
            if (t.Equals("n", StringComparison.InvariantCultureIgnoreCase))
            {
                return YInnovationType.NewNeuron;
            }
            return default(YInnovationType) /* was: null */;
        }

        /// <summary>
        /// Convert a string to a neuron type.
        /// </summary>
        /// <param name="t">The string.</param>
        /// <returns>The resulting neuron type.</returns>
        public static YNeuronType StringToNeuronType(String t)
        {
            if (t.Equals("b"))
            {
                return YNeuronType.Bias;
            }
            if (t.Equals("h"))
            {
                return YNeuronType.Hidden;
            }
            if (t.Equals("i"))
            {
                return YNeuronType.Input;
            }
            if (t.Equals("n"))
            {
                return YNeuronType.None;
            }
            if (t.Equals("o"))
            {
                return YNeuronType.Output;
            }
            throw new SyntError("Unknonw neuron type: " + t);
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(YPopulation); }
        }
    }

    public class PersistYNetwork : ISyntPersistor
    {
        #region SyntPersistor Members

        /// <summary>
        /// The file version.
        /// </summary>
        public virtual int FileVersion
        {
            get { return 1; }
        }

        /// <summary>
        /// The persist class string.
        /// </summary>
        public virtual String PersistClassString
        {
            get { return "YNetwork"; }
        }

        /// <summary>
        /// Read the object.
        /// </summary>
        /// <param name="mask0">The stream to read from.</param>
        /// <returns>The loaded object.</returns>
        public virtual Object Read(Stream mask0)
        {
            var result = new YNetwork();
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;
            IDictionary<Int32, YNeuron> neuronMap = new Dictionary<Int32, YNeuron>();

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("Y")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    foreach (String key in paras.Keys)
                    {
                        result.Properties.Add(key, paras[key]);
                    }
                }
                if (section.SectionName.Equals("Y")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> p = section.ParseParams();

                    result.InputCount = SyntFileSection.ParseInt(p,
                                                                  PersistConst.InputCount);
                    result.OutputCount = SyntFileSection.ParseInt(p,
                                                                   PersistConst.OutputCount);
                    result.ActivationFunction = SyntFileSection
                        .ParseActivationFunction(p,
                                                 PersistConst.ActivationFunction);
                    result.OutputActivationFunction = SyntFileSection
                        .ParseActivationFunction(p,
                                                 YPopulation.PropertyOutputActivation);
                    result.NetworkDepth = SyntFileSection.ParseInt(p,
                                                                    PersistConst.Depth);
                    result.Snapshot = SyntFileSection.ParseBoolean(p,
                                                                    PersistConst.Snapshot);
                }
                else if (section.SectionName.Equals("Y")
                         && section.SubSectionName.Equals("NEURONS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);

                        long neuronID = Int32.Parse(cols[0]);
                        YNeuronType neuronType = PersistYPopulation
                            .StringToNeuronType(cols[1]);
                        double activationResponse = CSVFormat.EgFormat
                            .Parse(cols[2]);
                        double splitY = CSVFormat.EgFormat
                            .Parse(cols[3]);
                        double splitX = CSVFormat.EgFormat
                            .Parse(cols[4]);

                        var YNeuron = new YNeuron(neuronType,
                                                        neuronID, splitY, splitX, activationResponse);
                        result.Neurons.Add(YNeuron);
                        neuronMap[((int)neuronID)] = (YNeuron);
                    }
                }
                else if (section.SectionName.Equals("Y")
                         && section.SubSectionName.Equals("LINKS"))
                {
                    foreach (String line in section.Lines)
                    {
                        IList<String> cols = SyntFileSection.SplitColumns(line);
                        int fromID = Int32.Parse(cols[0]);
                        int toID = Int32.Parse(cols[1]);
                        bool recurrent = Int32.Parse(cols[2]) > 0;
                        double weight = CSVFormat.EgFormat.Parse(cols[3]);
                        YNeuron fromNeuron = (neuronMap[fromID]);
                        YNeuron toNeuron = (neuronMap[toID]);
                        var YLink = new YLink(weight, fromNeuron,
                                                    toNeuron, recurrent);
                        fromNeuron.OutputboundLinks.Add(YLink);
                        toNeuron.InboundLinks.Add(YLink);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Save the object.
        /// </summary>
        /// <param name="os">The output stream.</param>
        /// <param name="obj">The object to save.</param>
        public virtual void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var Y = (YNetwork)obj;
            xout.AddSection("Y");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(Y.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.InputCount, Y.InputCount);
            xout.WriteProperty(PersistConst.OutputCount, Y.OutputCount);
            xout.WriteProperty(PersistConst.ActivationFunction,
                               Y.ActivationFunction);
            xout.WriteProperty(YPopulation.PropertyOutputActivation,
                               Y.OutputActivationFunction);
            xout.WriteProperty(PersistConst.Depth, Y.NetworkDepth);
            xout.WriteProperty(PersistConst.Snapshot, Y.Snapshot);

            xout.AddSubSection("NEURONS");

            foreach (YNeuron YNeuron in Y.Neurons)
            {
                xout.AddColumn((int)YNeuron.NeuronID);
                xout.AddColumn(PersistYPopulation.NeuronTypeToString(YNeuron.NeuronType));
                xout.AddColumn(YNeuron.ActivationResponse);
                xout.AddColumn(YNeuron.SplitX);
                xout.AddColumn(YNeuron.SplitY);
                xout.WriteLine();
            }

            xout.AddSubSection("LINKS");

            foreach (YNeuron YNeuron in Y.Neurons)
            {
                foreach (YLink link in YNeuron.OutputboundLinks)
                {
                    WriteLink(xout, link);
                }
            }

            xout.Flush();
        }

        #endregion

        /// <summary>
        /// Write a link.
        /// </summary>
        /// <param name="xout">The output file.</param>
        /// <param name="link">The link.</param>
        private static void WriteLink(SyntWriteHelper xout, YLink link)
        {
            xout.AddColumn((int)link.FromNeuron.NeuronID);
            xout.AddColumn((int)link.ToNeuron.NeuronID);
            xout.AddColumn(link.Recurrent);
            xout.AddColumn(link.Weight);
            xout.WriteLine();
        }

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(YNetwork); }
        }
    }


    public class PersistCPN : ISyntPersistor
    {
        /// <summary>
        /// The input to instar property.
        /// </summary>
        ///
        internal const String PropertyInputToInstar = "inputToInstar";

        /// <summary>
        /// The instar to input property.
        /// </summary>
        ///
        internal const String PropertyInstarToInput = "instarToInput";

        /// <summary>
        /// The winner count property.
        /// </summary>
        ///
        internal const String PropertyWinnerCount = "winnerCount";

        /// <inheritdoc/>
        public Type NativeType
        {
            get { return typeof(CPNNetwork); }
        }

        #region SyntPersistor Members

        /// <inheritdoc/>
        public int FileVersion
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public String PersistClassString
        {
            get { return "CPN"; }
        }


        /// <inheritdoc/>
        public Object Read(Stream mask0)
        {
            IDictionary<String, String> networkParams = null;
            var ins0 = new SyntReadHelper(mask0);
            SyntFileSection section;
            int inputCount = 0;
            int instarCount = 0;
            int outputCount = 0;
            int winnerCount = 0;
            Matrix m1 = null;
            Matrix m2 = null;

            while ((section = ins0.ReadNextSection()) != null)
            {
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("PARAMS"))
                {
                    networkParams = section.ParseParams();
                }
                if (section.SectionName.Equals("CPN")
                    && section.SubSectionName.Equals("NETWORK"))
                {
                    IDictionary<String, String> paras = section.ParseParams();

                    inputCount = SyntFileSection.ParseInt(paras,
                                                           PersistConst.InputCount);
                    instarCount = SyntFileSection.ParseInt(paras,
                                                            PersistConst.Instar);
                    outputCount = SyntFileSection.ParseInt(paras,
                                                            PersistConst.OutputCount);
                    winnerCount = SyntFileSection.ParseInt(paras,
                                                            PropertyWinnerCount);
                    m1 = SyntFileSection.ParseMatrix(paras,
                                                      PropertyInputToInstar);
                    m2 = SyntFileSection.ParseMatrix(paras,
                                                      PropertyInstarToInput);
                }
            }

            var result = new CPNNetwork(inputCount, instarCount, outputCount,
                                        winnerCount);
            EngineArray.PutAll(networkParams, result.Properties);
            result.WeightsInputToInstar.Set(m1);
            result.WeightsInstarToOutstar.Set(m2);
            return result;
        }

        /// <inheritdoc/>
        public void Save(Stream os, Object obj)
        {
            var xout = new SyntWriteHelper(os);
            var cpn = (CPNNetwork)obj;
            xout.AddSection("CPN");
            xout.AddSubSection("PARAMS");
            xout.AddProperties(cpn.Properties);
            xout.AddSubSection("NETWORK");

            xout.WriteProperty(PersistConst.InputCount, cpn.InputCount);
            xout.WriteProperty(PersistConst.Instar, cpn.InstarCount);
            xout.WriteProperty(PersistConst.OutputCount, cpn.OutputCount);
            xout.WriteProperty(PropertyInputToInstar,
                               cpn.WeightsInputToInstar);
            xout.WriteProperty(PropertyInstarToInput,
                               cpn.WeightsInstarToOutstar);
            xout.WriteProperty(PropertyWinnerCount, cpn.WinnerCount);

            xout.Flush();
        }

        #endregion
    }

    public class PopulationConst
    {
        /// <summary>
        /// Property tag for the next gene id.
        /// </summary>
        ///
        public const String PropertyNextGeneID = "nextGeneID";

        /// <summary>
        /// Property tag for the next T id.
        /// </summary>
        ///
        public const String PropertyNextTID = "nextTID";

        /// <summary>
        /// Property tag for the next innovation id.
        /// </summary>
        ///
        public const String PropertyNextInnovationID = "nextInnovationID";

        /// <summary>
        /// Property tag for the next species id.
        /// </summary>
        ///
        public const String PropertyNextSpeciesID = "nextSpeciesID";

        /// <summary>
        /// Property tag for the old age penalty.
        /// </summary>
        ///
        public const String PropertyOldAgePenalty = "oldAgePenalty";

        /// <summary>
        /// Property tag for the old age threshold.
        /// </summary>
        ///
        public const String PropertyOldAgeThreshold = "oldAgeThreshold";

        /// <summary>
        /// Property tag for the population size.
        /// </summary>
        ///
        public const String PropertyPopulationSize = "populationSize";

        /// <summary>
        /// Property tag for the survival rate.
        /// </summary>
        ///
        public const String PropertySurvivalRate = "survivalRate";

        /// <summary>
        /// Property tag for the young age bonus.
        /// </summary>
        ///
        public const String PropertyYoungAgeBonus = "youngAgeBonus";

        /// <summary>
        /// Property tag for the young age threshold.
        /// </summary>
        ///
        public const String PropertyYoungAgeThreshold = "youngAgeThreshold";

        /// <summary>
        /// Property tag for the Ts collection.
        /// </summary>
        ///
        public const String PropertyTs = "Ts";

        /// <summary>
        /// Property tag for the innovations collection.
        /// </summary>
        ///
        public const String PropertyInnovations = "innovations";

        /// <summary>
        /// Property tag for the species collection.
        /// </summary>
        ///
        public const String PropertySpecies = "species";
    }

    [Serializable]
    public class BasicSpecies : ISpecies
    {
        /// <summary>
        /// The list of Ts.
        /// </summary>
        ///
        private readonly IList<IT> _members;

        /// <summary>
        /// The age of this species.
        /// </summary>
        ///
        private int _age;

        /// <summary>
        /// The best score.
        /// </summary>
        ///
        private double _bestScore;

        /// <summary>
        /// The number of generations with no improvement.
        /// </summary>
        ///
        private int _gensNoImprovement;

        /// <summary>
        /// The leader.
        /// </summary>
        ///
        private IT _leader;

        /// <summary>
        /// The id of the leader.
        /// </summary>
        [NonSerialized]
        private long _leaderID;

        /// <summary>
        /// The owner class.
        /// </summary>
        ///
        private IPopulation _population;

        /// <summary>
        /// The number of spawns required.
        /// </summary>
        ///
        private double _spawnsRequired;

        /// <summary>
        /// The species id.
        /// </summary>
        ///
        private long _speciesID;

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        ///
        public BasicSpecies()
        {
            _members = new List<IT>();
        }

        /// <summary>
        /// Construct a species.
        /// </summary>
        ///
        /// <param name="thePopulation">The population the species belongs to.</param>
        /// <param name="theFirst">The first T in the species.</param>
        /// <param name="theSpeciesID">The species id.</param>
        public BasicSpecies(IPopulation thePopulation, IT theFirst,
                            long theSpeciesID)
        {
            _members = new List<IT>();
            _population = thePopulation;
            _speciesID = theSpeciesID;
            _bestScore = theFirst.Score;
            _gensNoImprovement = 0;
            _age = 0;
            _leader = theFirst;
            _spawnsRequired = 0;
            _members.Add(theFirst);
        }

        /// <value>the population to set</value>
        public IPopulation Population
        {
            get { return _population; }
            set { _population = value; }
        }

        /// <summary>
        /// Set the leader id. This value is not persisted, it is used only for
        /// loading.
        /// </summary>
        ///
        /// <value>the leaderID to set</value>
        public long TempLeaderID
        {
            get { return _leaderID; }
            set { _leaderID = value; }
        }

        #region ISpecies Members

        /// <summary>
        /// Calculate the amount to spawn.
        /// </summary>
        ///
        public void CalculateSpawnAmount()
        {
            _spawnsRequired = 0;

            foreach (IT T in _members)
            {
                _spawnsRequired += T.AmountToSpawn;
            }
        }

        /// <summary>
        /// Choose a parent to mate. Choose from the population, determined by the
        /// survival rate. From this pool, a random parent is chosen.
        /// </summary>
        ///
        /// <returns>The parent.</returns>
        public IT ChooseParent()
        {
            IT baby;

            // If there is a single member, then choose that one.
            if (_members.Count == 1)
            {
                baby = _members[0];
            }
            else
            {
                // If there are many, then choose the population based on survival
                // rate
                // and select a random T.
                int maxIndexSize = (int)(_population.SurvivalRate * _members.Count) + 1;
                var theOne = (int)RangeRandomizer.Randomize(0, maxIndexSize);
                baby = _members[theOne];
            }

            return baby;
        }

        /// <summary>
        /// Set the age of this species.
        /// </summary>
        ///
        /// <value>The age of this species.</value>
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }


        /// <summary>
        /// Set the best score.
        /// </summary>
        ///
        /// <value>The best score.</value>
        public double BestScore
        {
            get { return _bestScore; }
            set { _bestScore = value; }
        }


        /// <summary>
        /// Set the number of generations with no improvement.
        /// </summary>
        ///
        /// <value>The number of generations.</value>
        public int GensNoImprovement
        {
            get { return _gensNoImprovement; }
            set { _gensNoImprovement = value; }
        }


        /// <summary>
        /// Set the leader.
        /// </summary>
        ///
        /// <value>The new leader.</value>
        public IT Leader
        {
            get { return _leader; }
            set { _leader = value; }
        }


        /// <value>The members of this species.</value>
        public IList<IT> Members
        {
            get { return _members; }
        }


        /// <value>The number to spawn.</value>
        public double NumToSpawn
        {
            get { return _spawnsRequired; }
        }


        /// <summary>
        /// Set the number of spawns required.
        /// </summary>
        public double SpawnsRequired
        {
            get { return _spawnsRequired; }
            set { _spawnsRequired = value; }
        }


        /// <summary>
        /// Purge all members, increase age by one and count the number of
        /// generations with no improvement.
        /// </summary>
        ///
        public void Purge()
        {
            _members.Clear();
            _age++;
            _gensNoImprovement++;
            _spawnsRequired = 0;
        }

        /// <summary>
        /// Set the species id.
        /// </summary>
        public long SpeciesID
        {
            get { return _speciesID; }
            set { _speciesID = value; }
        }

        #endregion
    }

}