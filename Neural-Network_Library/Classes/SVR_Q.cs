using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    internal class SVR_Q : Kernel
    {
        //UPGRADE_NOTE: Final was removed from the declaration of 'l '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        //UPGRADE_NOTE: Final was removed from the declaration of 'cache '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        private readonly float[][] buffer;
        private readonly Cache cache;
        //UPGRADE_NOTE: Final was removed from the declaration of 'sign '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        //UPGRADE_NOTE: Final was removed from the declaration of 'index '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        private readonly int[] index;
        private readonly int l;
        private readonly sbyte[] sign;
        private int next_buffer;

        internal SVR_Q(svm_problem prob, svm_parameter param) : base(prob.l, prob.x, param)
        {
            l = prob.l;
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
            cache = new Cache(l, (int)(param.cache_size * (1 << 20)));
            sign = new sbyte[2 * l];
            index = new int[2 * l];
            for (int k = 0; k < l; k++)
            {
                sign[k] = 1;
                sign[k + l] = -1;
                index[k] = k;
                index[k + l] = k;
            }
            buffer = new float[2][];
            for (int i = 0; i < 2; i++)
            {
                buffer[i] = new float[2 * l];
            }
            next_buffer = 0;
        }

        internal override void swap_index(int i, int j)
        {
            do
            {
                sbyte _ = sign[i];
                sign[i] = sign[j];
                sign[j] = _;
            } while (false);
            do
            {
                int _ = index[i];
                index[i] = index[j];
                index[j] = _;
            } while (false);
        }

        internal override float[] get_Q(int i, int len)
        {
            var data = new float[1][];
            int real_i = index[i];
            if (cache.get_data(real_i, data, l) < l)
            {
                for (int j = 0; j < l; j++)
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042_3"'
                    data[0][j] = (float)kernel_function(real_i, j);
                }
            }

            // reorder and copy
            float[] buf = buffer[next_buffer];
            next_buffer = 1 - next_buffer;
            sbyte si = sign[i];
            for (int j = 0; j < len; j++)
                buf[j] = si * sign[j] * data[0][index[j]];
            return buf;
        }
    }
}
