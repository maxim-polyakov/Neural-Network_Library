using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IMLDataSet
    {
        /// <summary>
        /// The size of the ideal data, 0 if no ideal data.
        /// </summary>
        int IdealSize { get; }

        /// <summary>
        /// The size of the input data.
        /// </summary>
        int InputSize { get; }

        /// <summary>
        /// The number of records in the data set.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Return true if supervised.
        /// </summary>
        bool Supervised { get; }

        /// <summary>
        /// Add a NeuralData object to the dataset. This is used with unsupervised
        /// training, as no ideal output is provided. Note: not all implemenations
        /// support the add methods. 
        /// </summary>
        /// <param name="data1">The data to add.</param>
        void Add(IMLData data1);

        /// <summary>
        /// Add a set of input and ideal data to the dataset. This is used with
        /// supervised training, as ideal output is provided. Note: not all
        /// implementations support the add methods.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="idealData">Ideal data.</param>
        void Add(IMLData inputData, IMLData idealData);

        /// <summary>
        /// Add a NeuralData object to the dataset. This is used with unsupervised
        /// training, as no ideal output is provided. Note: not all implementations
        /// support the add methods. 
        /// </summary>
        /// <param name="inputData">A NeuralDataPair object that contains both input and ideal
        /// data.</param>
        void Add(IMLDataPair inputData);


        /// <summary>
        /// Close this datasource and release any resources obtained by it, including
        /// any iterators created. 
        /// </summary>
        void Close();

        /// <summary>
        /// Get an enumerator to access the data.
        /// </summary>
        /// <returns></returns>
        IEnumerator<IMLDataPair> GetEnumerator();

        /// <summary>
        /// Get one record from the data set.
        /// </summary>
        /// <param name="index">The index to read.</param>
        /// <param name="pair">The pair to read into.</param>
        void GetRecord(int index, IMLDataPair pair);

        /// <summary>
        /// Open an additional instance of this dataset.
        /// </summary>
        /// <returns>The new instance of this dataset.</returns>
        IMLDataSet OpenAdditional();

        /// <summary>
        /// Get the specified record.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        IMLDataPair this[int x] { get; }
    }
}
