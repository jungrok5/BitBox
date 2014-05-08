using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace BitBox.Log
{
    // 파일로 남기기
    public class FileLogger : LoggerBase
    {
        public override IAppender GetAppender()
        {
            var appender = new RollingFileAppender
            {
                Threshold = Level.All,
                Layout = new PatternLayout(layoutString),
                // System.IO.Directory.GetCurrentDirectory() 이 값이 ASP.NET에서나 Winform등에서는 다를 수 있음. 다른곳에 가져다 쓰는경우 주의바람
                File = System.IO.Directory.GetCurrentDirectory() + @"\Log\Server.log",
                AppendToFile = true,
                RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date,
                LockingModel = new log4net.Appender.FileAppender.MinimalLock(),
                DatePattern = "_yyyyMMdd_HH\".log\"", // 시간이 지나간 경우 이전 로그에 붙을 이름 구성  
            };
            appender.ActivateOptions();
            return appender;
        }
    }
}
