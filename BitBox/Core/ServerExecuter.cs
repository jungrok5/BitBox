using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitBox.Log;

namespace BitBox.Core
{
    public static class ServerExecuter<T> where T : Server, new()
    {
        public static void Execute(string[] args, string version, string name = null)
        {
            if (System.Environment.UserInteractive)
                Execute_Console(args, version, name);
            else
                Execute_Service(version, name);
        }

        static void Execute_Console(string[] args, string version, string name = null)
        {
            T server = new T();
            server.Init(ServerExecuteType.Console, version, name);

            Logger.Info("Execute_Console");

            Console.Title = string.Format("{0} v{1}", Server.ModuleName, version);

            if (Parse_n_Process_Argument(server, args))
            {
                // TODO
                // Init에서 생성했던것들이 있다면 여기서 초기화 또는 삭제해주자
                return;
            }

            server.ServerStart();

            server.ServerRun();
        }

        static void Execute_Service(string version, string name = null)
        {
            T server = new T();
            server.Init(ServerExecuteType.Service, version, name);

            Logger.Info("Execute_Service");

            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            ServiceBase.Run(server);
        }

        static bool Parse_n_Process_Argument(T server, string[] args)
        {
            if (args == null || args.Length <= 0)
                return false;

            if (args[0].Equals("-i", StringComparison.OrdinalIgnoreCase)
                || args[0].Equals("-install", StringComparison.OrdinalIgnoreCase)
                || args[0].Equals("/i", StringComparison.OrdinalIgnoreCase)
                || args[0].Equals("/install", StringComparison.OrdinalIgnoreCase))
            {
                server.InstallService();
            }
            else if (args[0].Equals("-u", StringComparison.OrdinalIgnoreCase)
                    || args[0].Equals("-uninstall", StringComparison.OrdinalIgnoreCase)
                    || args[0].Equals("/u", StringComparison.OrdinalIgnoreCase)
                    || args[0].Equals("/uninstall", StringComparison.OrdinalIgnoreCase))
            {
                server.UninstallService();
            }
            else
            {
                Console.WriteLine(Server.ModuleName);
                Console.WriteLine("invalid argument");
                Console.WriteLine("usage:");
                Console.WriteLine("	-i");
                Console.WriteLine("	/i");
                Console.WriteLine("	-install");
                Console.WriteLine("	/install : install service");
                Console.WriteLine("	-u");
                Console.WriteLine("	/u");
                Console.WriteLine("	-uninstall");
                Console.WriteLine("	/uninstall : uninstall service");
            }
            return true;
        }
    }
}
