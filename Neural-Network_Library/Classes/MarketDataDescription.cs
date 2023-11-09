using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class MarketDataDescription : TemporalDataDescription
    {
        /// <summary>
        /// The type of data to be loaded from the specified ticker symbol.
        /// </summary>
        private readonly MarketDataType _dataType;

        /// <summary>
        /// The ticker symbol to be loaded.
        /// </summary>
        private readonly TickerSymbol _ticker;

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="activationFunction"> The activation function to apply to this data, can be null.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, Type type,
                                     IActivationFunction activationFunction, bool input,
                                     bool predict)
            : base(activationFunction, type, input, predict)
        {
            _ticker = ticker;
            _dataType = dataType;
        }


        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="type">The normalization type.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, Type type, bool input,
                                     bool predict)
            : this(ticker, dataType, type, null, input, predict)
        {
        }

        /// <summary>
        /// Construct a MarketDataDescription item.
        /// </summary>
        /// <param name="ticker">The ticker symbol to use.</param>
        /// <param name="dataType">The data type needed.</param>
        /// <param name="input">Is this field used for input?</param>
        /// <param name="predict">Is this field used for prediction?</param>
        public MarketDataDescription(TickerSymbol ticker,
                                     MarketDataType dataType, bool input,
                                     bool predict)
            : this(ticker, dataType, Type.PercentChange, null, input, predict)
        {
        }

        /// <summary>
        /// The ticker symbol.
        /// </summary>
        public TickerSymbol Ticker
        {
            get { return _ticker; }
        }

        /// <summary>
        /// The data type that this is.
        /// </summary>
        public MarketDataType DataType
        {
            get { return _dataType; }
        }
    }
}
