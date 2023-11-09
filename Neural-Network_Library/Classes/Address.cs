using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class Address
    {
        /// <summary>
        /// The original text from the address.
        /// </summary>
        private readonly String _original;

        /// <summary>
        /// The address as a URL.
        /// </summary>
        private readonly Uri _url;

        /// <summary>
        /// Construct the address from a URL.
        /// </summary>
        /// <param name="u">The URL to use.</param>
        public Address(Uri u)
        {
            _url = u;
            _original = u.ToString();
        }

        /// <summary>
        /// Construct a URL using a perhaps relative URL and a base URL.
        /// </summary>
        /// <param name="b">The base URL.</param>
        /// <param name="original">A full URL or a URL relative to the base.</param>
        public Address(Uri b, String original)
        {
            _original = original;
            _url = b == null ? new Uri(new Uri("http://localhost/"), original) : new Uri(b, original);
        }

        /// <summary>
        /// The original text from this URL.
        /// </summary>
        public String Original
        {
            get { return _original; }
        }

        /// <summary>
        /// The URL.
        /// </summary>
        public Uri Url
        {
            get { return _url; }
        }

        /// <summary>
        /// The object as a string.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return _url != null ? _url.ToString() : _original;
        }
    }
}
