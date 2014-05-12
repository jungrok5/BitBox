using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitBox.Log;

namespace BitBox.Util
{
    // http://msdn.microsoft.com/ko-kr/library/2x96zfy7(v=vs.110).aspx

    public class TimerThread
    {
        public delegate void TimerEvent(object context);
        public TimerEvent TimerHandler;

        private int m_PeriodMS;
        private Thread m_Thread;
        private AutoResetEvent m_ExitEvent;
        private AutoResetEvent m_CompletionEvent;

        public TimerThread(TimerEvent timerHandler, object context = null, int periodMS = 1000)
        {
            m_ExitEvent = new AutoResetEvent(false);
            m_CompletionEvent = new AutoResetEvent(false);

            TimerHandler += timerHandler;
            m_PeriodMS = periodMS;

            m_Thread = new Thread(Run);
            m_Thread.Start(context);
        }

        private void Run(object context)
        {
            while (true)
            {
                if (m_CompletionEvent.WaitOne(m_PeriodMS) == false)
                    TimerHandler(context);

                if (m_ExitEvent.WaitOne(0) == true)
                    break;
            }

            m_ExitEvent.Dispose();
            m_ExitEvent = null;
            m_CompletionEvent.Dispose();
            m_CompletionEvent = null;
        }

        public void Stop()
        {
            m_ExitEvent.Set();
        }
    }
}
