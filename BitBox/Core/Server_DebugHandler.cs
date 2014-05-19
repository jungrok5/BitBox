using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BitBox.Log;

namespace BitBox.Core
{
    public partial class Server : ServiceBase
    {
        protected void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 일단 로그로 남기자
            Logger.Error("The process crashed for an unhandled exception!", (Exception)e.ExceptionObject);

            if (ExecuteType == ServerExecuteType.Console)
            {
                Console.ReadKey();
            }

            // 나중에 다시 보자 잘안되넹
            //MiniDump.Write((Exception)e.ExceptionObject);
        }
    }
}
