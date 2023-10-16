using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    internal class SVC_Q : Kernel
    {
        //UPGRADE_NOTE: Final was removed from the declaration of 'y '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        //UPGRADE_NOTE: Final was removed from the declaration of 'cache '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        private readonly Cache cache;
        private readonly sbyte[] y;

        internal SVC_Q(svm_problem prob, svm_parameter param, sbyte[] y_) : base(prob.l, prob.x, param)
        {
            y = new sbyte[y_.Length];
            y_.CopyTo(y, 0);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
            cache = new Cache(prob.l, (int)(param.cache_size * (1 << 20)));
        }

        internal override float[] get_Q(int i, int len)
        {
            var data = new float[1][];
            int start;
            if ((start = cache.get_data(i, data, len)) < len)
            {
                for (int j = start; j < len; j++)
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
                    data[0][j] = (float)(y[i] * y[j] * kernel_function(i, j));
                }
            }
            return data[0];
        }

        internal override void swap_index(int i, int j)
        {
            cache.swap_index(i, j);
            base.swap_index(i, j);
            do
            {
                sbyte _ = y[i];
                y[i] = y[j];
                y[j] = _;
            } while (false);
        }
    }
}
