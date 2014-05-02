using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitBox.Log;

namespace BitBox.Core
{
    // 서비스는 별도의 핸들러가 없고 ServiceBase의 OnStop 함수가 호출됨

    public partial class Server : ServiceBase
    {
        private ManualResetEvent m_MainThreadStopEvent;
        private Thread m_MainThread;

        public virtual void ServerRun()
        {
            if (ExecuteType == ServerExecuteType.Console)
            {
                Console.CancelKeyPress += ConsoleHandler;
                Logger.Info("ServerRun:Console");
            }
            else
            {
                Logger.Info(string.Format("ServerRun:Service - {0}", ModuleName));
            }

            m_MainThreadStopEvent = new ManualResetEvent(false);
            m_MainThread = new Thread(MainThread);
            m_MainThread.Start();
        }

        void MainThread()
        {
            while (true)
            {
                if (m_MainThreadStopEvent.WaitOne() == true)
                    break;
            }
        }

        void ConsoleHandler(object sender, ConsoleCancelEventArgs e)
        {
            Logger.Info("Console stop event Received");
            ServerStop();
        } 
    }
}
