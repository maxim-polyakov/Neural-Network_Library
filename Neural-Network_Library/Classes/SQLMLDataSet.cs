using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
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
}
