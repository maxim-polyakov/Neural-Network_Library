﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class YahooFinanceLoader : IMarketLoader
    {
        #region IMarketLoader Members

        /// <summary>
        /// Load the specified financial data. 
        /// </summary>
        /// <param name="ticker">The ticker symbol to load.</param>
        /// <param name="dataNeeded">The financial data needed.</param>
        /// <param name="from">The beginning date to load data from.</param>
        /// <param name="to">The ending date to load data to.</param>
        /// <returns>A collection of LoadedMarketData objects that represent the data
        /// loaded.</returns>
        public ICollection<LoadedMarketData> Load(TickerSymbol ticker,
                                                  IList<MarketDataType> dataNeeded, DateTime from,
                                                  DateTime to)
        {
            ICollection<LoadedMarketData> result =
                new List<LoadedMarketData>();
            //Uri url = BuildURL(ticker, from, to);
            //WebRequest http = WebRequest.Create(url);
            //var response = (HttpWebResponse)http.GetResponse();

            //using (Stream istream = response.GetResponseStream())
            //{
            //    var csv = new ReadCSV(istream, true, CSVFormat.DecimalPoint);

            //    while (csv.Next())
            //    {
            //        DateTime date = csv.GetDate("date");
            //        double adjClose = csv.GetDouble("adj close");
            //        double open = csv.GetDouble("open");
            //        double close = csv.GetDouble("close");
            //        double high = csv.GetDouble("high");
            //        double low = csv.GetDouble("low");
            //        double volume = csv.GetDouble("volume");

            //        var data =
            //            new LoadedMarketData(date, ticker);
            //        data.SetData(MarketDataType.AdjustedClose, adjClose);
            //        data.SetData(MarketDataType.Open, open);
            //        data.SetData(MarketDataType.Close, close);
            //        data.SetData(MarketDataType.High, high);
            //        data.SetData(MarketDataType.Low, low);
            //        data.SetData(MarketDataType.Open, open);
            //        data.SetData(MarketDataType.Volume, volume);
            //        result.Add(data);
            //    }

            //    csv.Close();
            //    istream.Close();
            //}
            return result;
        }

        #endregion

        /// <summary>
        /// This method builds a URL to load data from Yahoo Finance for a neural
        /// network to train with.
        /// </summary>
        /// <param name="ticker">The ticker symbol to access.</param>
        /// <param name="from">The beginning date.</param>
        /// <param name="to">The ending date.</param>
        /// <returns>The URL to read from</returns>
        private static Uri BuildURL(TickerSymbol ticker, DateTime from,
                             DateTime to)
        {
            // construct the URL
            var mstream = new MemoryStream();
            var form = new FormUtility(mstream, null);

            form.Add("s", ticker.Symbol.ToUpper());
            form.Add("a", "" + (from.Month - 1));
            form.Add("b", "" + from.Day);
            form.Add("c", "" + from.Year);
            form.Add("d", "" + (to.Month - 1));
            form.Add("e", "" + to.Day);
            form.Add("f", "" + to.Year);
            form.Add("g", "d");
            form.Add("ignore", ".csv");
            mstream.Close();
            byte[] b = mstream.GetBuffer();

            String str = "http://ichart.finance.yahoo.com/table.csv?"
                         + StringUtil.FromBytes(b);
            return new Uri(str);
        }

        #region IMarketLoader Members


        public string GetFile(string file)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
