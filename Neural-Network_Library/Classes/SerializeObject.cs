﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class SerializeObject
    {
        /// <summary>
        /// Private constructor, call everything statically.
        /// </summary>
        private SerializeObject()
        {
        }

        /// <summary>
        /// Load the specified filename.
        /// </summary>
        /// <param name="filename">The filename to load from.</param>
        /// <returns>The object loaded from that file.</returns>
        public static object Load(string filename)
        {
            Stream s = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            var b = new BinaryFormatter();
            object obj = b.Deserialize(s);
            s.Close();
            return obj;
        }

        /// <summary>
        /// Save the specified object.
        /// </summary>
        /// <param name="filename">The filename to save to.</param>
        /// <param name="obj">The object to save.</param>
        public static void Save(string filename, object obj)
        {
            Stream s = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            var b = new BinaryFormatter();
            b.Serialize(s, obj);
            s.Close();
        }
    }
}
