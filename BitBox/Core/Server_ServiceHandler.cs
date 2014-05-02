using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BitBox.Log;

// 참고
// http://warmz.tistory.com/933
// http://www.thedavejay.com/2012/04/self-installing-c-windows-service-safe.html

namespace BitBox.Core
{
    [RunInstaller(true)]
    public class CustomServiceInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public CustomServiceInstaller()
        {
            process = new ServiceProcessInstaller();

            process.Account = ServiceAccount.LocalSystem;

            service = new ServiceInstaller();
            service.ServiceName = Server.ModuleName;

            Installers.Add(process);
            Installers.Add(service);
        } 
    }

    public partial class Server : ServiceBase
    {
        bool IsServiceInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == ModuleName);
        }

        public void InstallService()
        {
            if (IsServiceInstalled())
                UninstallService();

            Logger.Info("Start Service install.");
            ManagedInstallerClass.InstallHelper(new string[] { System.Reflection.Assembly.GetExecutingAssembly().Location });
            Logger.Info("Success. End Service install");
        }

        public void UninstallService()
        {
            if (IsServiceInstalled() == false)
            {
                Logger.Warning(string.Format("Not exists Service:{0}", ModuleName));
            }
            else
            {
                Logger.Info("Start Service uninstall.");
                ManagedInstallerClass.InstallHelper(new string[] { "/u", System.Reflection.Assembly.GetExecutingAssembly().Location });
                Logger.Info("Success. End Service uninstall.");
            }
        }

        protected override void OnStart(string[] args)
        {
            ServerStart();

            ServerRun();
        }

        protected override void OnStop()
        {
            Logger.Info("Service stop event Received");
            ServerStop();
        }
    }
}
