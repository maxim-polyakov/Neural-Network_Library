using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class BotUtil
    {
        /// <summary>
        /// How much data to read at once.
        /// </summary>
        public static int BufferSize = 8192;

        /// <summary>
        /// This method is very useful for grabbing information from a HTML page.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="token1">The text, or tag, that comes before the desired text</param>
        /// <param name="token2">The text, or tag, that comes after the desired text</param>
        /// <param name="index">Index in the string to start searching from.</param>
        /// <param name="occurence">What occurence.</param>
        /// <returns>The contents of the URL that was downloaded.</returns>
        public static String ExtractFromIndex(String str, String token1,
                                              String token2, int index, int occurence)
        {
            // convert everything to lower case
            String searchStr = str.ToLower();
            String token1Lower = token1.ToLower();
            String token2Lower = token2.ToLower();

            int count = occurence;

            // now search
            int location1 = index - 1;
            do
            {
                location1 = searchStr.IndexOf(token1Lower, location1 + 1);

                if (location1 == -1)
                {
                    return null;
                }

                count--;
            } while (count > 0);


            // return the result from the original string that has mixed
            // case
            int location2 = searchStr.IndexOf(token2Lower, location1 + 1);
            if (location2 == -1)
            {
                return null;
            }

            return str.Substring(location1 + token1Lower.Length, location2 - (location1 + token1.Length));
        }

        /// <summary>
        /// This method is very useful for grabbing information from a HTML page.
        /// </summary>
        /// <param name="str">The string to search.</param>
        /// <param name="token1">The text, or tag, that comes before the desired text.</param>
        /// <param name="token2">The text, or tag, that comes after the desired text.</param>
        /// <param name="index">Which occurrence of token1 to use, 1 for the first.</param>
        /// <returns>The contents of the URL that was downloaded.</returns>
        public static String Extract(String str, String token1,
                                     String token2, int index)
        {
            // convert everything to lower case
            String searchStr = str.ToLower();
            String token1Lower = token1.ToLower();
            String token2Lower = token2.ToLower();

            int count = index;

            // now search
            int location1 = -1;
            do
            {
                location1 = searchStr.IndexOf(token1Lower, location1 + 1);

                if (location1 == -1)
                {
                    return null;
                }

                count--;
            } while (count > 0);

            // return the result from the original string that has mixed
            // case
            int location2 = searchStr.IndexOf(token2Lower, location1 + 1);
            if (location2 == -1)
            {
                return null;
            }

            return str.Substring(location1 + token1Lower.Length, location2 - (location1 + token1.Length));
        }

        /// <summary>
        /// Post to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="param">The post params.</param>
        /// <returns>The HTTP response.</returns>


        /// <summary>
        /// Post bytes to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="bytes">The bytes to post.</param>
        /// <param name="length">The length of the posted data.</param>
        /// <returns>The HTTP response.</returns>


        /// <summary>
        /// Load the specified web page into a string.
        /// </summary>
        /// <param name="url">The url to load.</param>
        /// <returns>The web page as a string.</returns>


        /// <summary>
        /// Private constructor.
        /// </summary>
        private BotUtil()
        {
        }

        /// <summary>
        /// Post to a page.
        /// </summary>
        /// <param name="uri">The URI to post to.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The page returned.</returns>

    }
}
