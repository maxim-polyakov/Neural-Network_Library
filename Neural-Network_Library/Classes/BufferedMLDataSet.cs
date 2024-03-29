﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
