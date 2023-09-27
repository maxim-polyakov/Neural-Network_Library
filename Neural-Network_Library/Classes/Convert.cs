using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public static class Convert
    {

        public static double String2Double(String str)
        {
            double result = 0;
            try
            {
                if (str != null)
                {
                    result = double.Parse(str);
                }
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }


        public static int String2Int(String str)
        {
            int result = 0;
            try
            {
                if (str != null)
                {
                    result = int.Parse(str);
                }
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }
    }
}
