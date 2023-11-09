using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class TickerSymbol
    {
        /// <summary>
        /// The exchange.
        /// </summary>
        private readonly String _exchange;

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        private readonly String _symbol;


        /// <summary>
        /// Construct a ticker symbol with no exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol</param>
        public TickerSymbol(String symbol)
        {
            _symbol = symbol;
            _exchange = null;
        }

        /// <summary>
        /// Construct a ticker symbol with exchange.
        /// </summary>
        /// <param name="symbol">The ticker symbol.</param>
        /// <param name="exchange">The exchange.</param>
        public TickerSymbol(String symbol, String exchange)
        {
            _symbol = symbol;
            _exchange = exchange;
        }

        /// <summary>
        /// The stock symbol.
        /// </summary>
        public String Symbol
        {
            get { return _symbol; }
        }

        /// <summary>
        /// The exchange that this stock is on.
        /// </summary>
        public String Exchange
        {
            get { return _exchange; }
        }


        /// <summary>
        /// Determine if two ticker symbols equal each other.
        /// </summary>
        /// <param name="other">The other ticker symbol.</param>
        /// <returns>True if the two symbols equal.</returns>
        public bool Equals(TickerSymbol other)
        {
            // if the symbols do not even match then they are not equal
            if (!other.Symbol.Equals(this.Symbol))
            {
                return false;
            }

            // if the symbols match then we need to compare the exchanges
            if (other.Exchange == null && other.Exchange == null)
            {
                return true;
            }

            if (other.Exchange == null || this.Exchange == null)
            {
                return false;
            }

            return other.Exchange.Equals(this.Exchange);
        }
    }
}
