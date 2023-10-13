using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    internal class Cache
    {
        //UPGRADE_NOTE: Final was removed from the declaration of 'l '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'

        //UPGRADE_NOTE: Final was removed from the declaration of 'head '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003_3"'
        private readonly head_t[] head;
        private readonly int l;
        private readonly head_t lru_head;
        private int size;

        internal Cache(int l_, int size_)
        {
            l = l_;
            size = size_;
            head = new head_t[l];
            for (int i = 0; i < l; i++)
                head[i] = new head_t(this);
            size /= 4;
            size -= l * (16 / 4); // sizeof(head_t) == 16
            lru_head = new head_t(this);
            lru_head.next = lru_head.prev = lru_head;
        }

        private void lru_delete(head_t h)
        {
            // delete from current location
            h.prev.next = h.next;
            h.next.prev = h.prev;
        }

        private void lru_insert(head_t h)
        {
            // insert to last position
            h.next = lru_head;
            h.prev = lru_head.prev;
            h.prev.next = h;
            h.next.prev = h;
        }

        // request data [0,len)
        // return some position p where [p,len) need to be filled
        // (p >= len if nothing needs to be filled)
        // java: simulate pointer using single-element array
        internal virtual int get_data(int index, float[][] data, int len)
        {
            head_t h = head[index];
            if (h.len > 0)
                lru_delete(h);
            int more = len - h.len;

            if (more > 0)
            {
                // free old space
                while (size < more)
                {
                    head_t old = lru_head.next;
                    lru_delete(old);
                    size += old.len;
                    old.data = null;
                    old.len = 0;
                }

                // allocate new space
                var new_data = new float[len];
                if (h.data != null)
                    Array.Copy(h.data, 0, new_data, 0, h.len);
                h.data = new_data;
                size -= more;
                do
                {
                    int _ = h.len;
                    h.len = len;
                    len = _;
                } while (false);
            }

            lru_insert(h);
            data[0] = h.data;
            return len;
        }

        internal virtual void swap_index(int i, int j)
        {
            if (i == j)
                return;

            if (head[i].len > 0)
                lru_delete(head[i]);
            if (head[j].len > 0)
                lru_delete(head[j]);
            do
            {
                float[] _ = head[i].data;
                head[i].data = head[j].data;
                head[j].data = _;
            } while (false);
            do
            {
                int _ = head[i].len;
                head[i].len = head[j].len;
                head[j].len = _;
            } while (false);
            if (head[i].len > 0)
                lru_insert(head[i]);
            if (head[j].len > 0)
                lru_insert(head[j]);

            if (i > j)
                do
                {
                    int _ = i;
                    i = j;
                    j = _;
                } while (false);
            for (head_t h = lru_head.next; h != lru_head; h = h.next)
            {
                if (h.len > i)
                {
                    if (h.len > j)
                        do
                        {
                            float _ = h.data[i];
                            h.data[i] = h.data[j];
                            h.data[j] = _;
                        } while (false);
                    else
                    {
                        // give up
                        lru_delete(h);
                        size += h.len;
                        h.data = null;
                        h.len = 0;
                    }
                }
            }
        }

        #region Nested type: head_t

        private sealed class head_t
        {
            internal float[] data;
            private Cache enclosingInstance;
            internal int len; // data[0,len) is cached in this entry
            internal head_t next; // a cicular list
            internal head_t prev; // a cicular list

            public head_t(Cache enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            public Cache Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            private void InitBlock(Cache enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
        }

        #endregion
    }
}
