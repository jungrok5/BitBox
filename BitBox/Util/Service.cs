using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;
using BitBox.Log;

namespace BitBox.Util
{
    // http://msdn.microsoft.com/en-us/library/System.ServiceProcess(v=vs.110).aspx
    // http://msdn.microsoft.com/ko-kr/library/system.serviceprocess.servicecontroller(v=vs.90).aspx
    // http://msdn.microsoft.com/en-us/library/system.serviceprocess.servicecontrollerstatus.aspx

    public static class Service
    {
        public static bool Start(string name, string ip)
        {
            try
            {
                ServiceController sc = new ServiceController();
                sc.MachineName = ip;
                sc.ServiceName = name;
                sc.Start();
            }
            catch (Exception e)
            {
                Logger.Error("Service.Start", e);
                return false;
            }
            return true;
        }

        public static bool Stop(string name, string ip)
        {
            try
            {
                ServiceController sc = new ServiceController();
                sc.MachineName = ip;
                sc.ServiceName = name;
                sc.Stop();
            }
            catch (Exception e)
            {
                Logger.Error("Service.Stop", e);
                return false;
            }
            return true;
        }
    }
}
