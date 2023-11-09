using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ObjectCloner
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private ObjectCloner()
        {
        }

        /// <summary>
        /// Perform a deep copy.
        /// </summary>
        /// <param name="oldObj">The old object.</param>
        /// <returns>The new object.</returns>
        public static Object DeepCopy(Object oldObj)
        {
            var formatter = new BinaryFormatter();

            using (var memory = new MemoryStream())
            {
                try
                {
                    // serialize and pass the object
                    formatter.Serialize(memory, oldObj);
                    memory.Flush();
                    memory.Position = 0;

                    // return the new object
                    return formatter.Deserialize(memory);
                }
                catch (Exception e)
                {
                    throw new SyntError(e);
                }
            }
        }
    }
}
