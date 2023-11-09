using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
