using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitBox.Util
{
    // TODO
    public static class KeyGenerator
    {
        private static long m_CurrentID;
        private static readonly long INVALID = 0;

        public static long Alloc()
        {
            long key = Interlocked.Increment(ref m_CurrentID);
            if (key <= 0)
            {
                // TODO 예외? 로그?
            }
            return key;
        }

        public static void Free(long key)
        {
        }

        public static bool IsValid(long key)
        {
            return key != INVALID;
        }
    }
}
