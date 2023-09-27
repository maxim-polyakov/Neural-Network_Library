using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public interface IPersistor
    {
        /// <summary>
        /// The native type.
        /// </summary>
        Type NativeType { get; }

        /// <value>Get the class string for the object.</value>
        String PersistClassString { get; }

        /// <value>Get the file version used by this persistor.</value>
        int FileVersion { get; }


        /// <summary>
        /// Read the object from an input stream.
        /// </summary>
        ///
        /// <param name="mask0">The input stream.</param>
        /// <returns>The object.</returns>
        Object Read(Stream mask0);

        /// <summary>
        /// Save the object.
        /// </summary>
        ///
        /// <param name="os">The output stream to save to.</param>
        /// <param name="obj">The object to save.</param>
        void Save(Stream os, Object obj);
    }
}
