using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class YahooSearch
    {
        ///// <summary>
        ///// Perform a Yahoo search.
        ///// </summary>
        ///// <param name="url">The REST URL.</param>
        ///// <returns>The search results.</returns>
        //private ICollection<Uri> DoSearch(Uri url)
        //{
        //    ICollection<Uri> result = new List<Uri>();
        //    // submit the search
        //    //WebRequest http = WebRequest.Create(url);
        //    //var response = (HttpWebResponse)http.GetResponse();

        //    using (Stream istream = response.GetResponseStream())
        //    {
        //        var parse = new ReadHTML(istream);
        //        var buffer = new StringBuilder();
        //        bool capture = false;

        //        // parse the results
        //        int ch;
        //        while ((ch = parse.Read()) != -1)
        //        {
        //            if (ch == 0)
        //            {
        //                Tag tag = parse.LastTag;
        //                if (tag.Name.Equals("Url", StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    buffer.Length = 0;
        //                    capture = true;
        //                }
        //                else if (tag.Name.Equals("/Url", StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    result.Add(new Uri(buffer.ToString()));
        //                    buffer.Length = 0;
        //                    capture = false;
        //                }
        //            }
        //            else
        //            {
        //                if (capture)
        //                {
        //                    buffer.Append((char)ch);
        //                }
        //            }
        //        }
        //    }

        //    //response.Close();

        //    return result;
        //}

        ///// <summary>
        ///// Perform a Yahoo search.
        ///// </summary>
        ///// <param name="searchFor">What are we searching for.</param>
        ///// <returns>The URLs that contain the specified item.</returns>
        //public ICollection<Uri> Search(String searchFor)
        //{
        //    ICollection<Uri> result = null;

        //    // build the Uri
        //    var mstream = new MemoryStream();
        //    var form = new FormUtility(mstream, null);
        //    form.Add("appid", "YahooDemo");
        //    form.Add("results", "100");
        //    form.Add("query", searchFor);
        //    form.Complete();

        //    var enc = new ASCIIEncoding();

        //    String str = enc.GetString(mstream.GetBuffer());
        //    mstream.Dispose();

        //    var uri = new Uri(
        //        "http://search.yahooapis.com/WebSearchService/V1/webSearch?"
        //        + str);

        //    int tries = 0;
        //    bool done = false;
        //    while (!done)
        //    {
        //        try
        //        {
        //            result = DoSearch(uri);
        //            done = true;
        //        }
        //        catch (IOException e)
        //        {
        //            if (tries == 5)
        //            {
        //                throw;
        //            }
        //            Thread.Sleep(5000);
        //        }
        //        tries++;
        //    }

        //    return result;
        //}
    }
}
