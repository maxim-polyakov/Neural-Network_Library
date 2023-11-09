using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class StringUtil
    {
        /// <summary>
        /// Compare two strings, ignore case.
        /// </summary>
        /// <param name="a">The first string.</param>
        /// <param name="b">The second string.</param>
        /// <returns></returns>
        public static Boolean EqualsIgnoreCase(String a, String b)
        {
            return a.Equals(b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Simple utility to take an array of ASCII bytes and convert to
        /// a String.  Works with Silverlight as well.
        /// </summary>
        /// <param name="b">The byte array.</param>
        /// <returns>The string created from the byte array.</returns>
        public static String FromBytes(byte[] b)
        {
            var b2 = new byte[b.Length * 2];
            for (int i = 0; i < b.Length; i++)
            {
                b2[i * 2] = b[i];
                b2[(i * 2) + 1] = 0;
            }

            return (new UnicodeEncoding()).GetString(b2, 0, b2.Length);
        }
    }
}
