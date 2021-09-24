using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShimmyMySherbet.ExecPlugin
{
    public static class RepeatManager
    {
        public static ConcurrentDictionary<int, Tuple<ulong, CancellationTokenSource>> m_handles = new ConcurrentDictionary<int, Tuple<ulong, CancellationTokenSource>>();
        private static int m_CurrentValue = -1;

        public static int AssignValue()
        {
            m_CurrentValue++;
            return m_CurrentValue;
        }
    }
}
