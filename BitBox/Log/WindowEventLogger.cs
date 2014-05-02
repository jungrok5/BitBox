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
    // 윈도우이벤트로 남기기
    public class WindowEventLogger : LoggerBase
    {
        public string ApplicationName;

        public override IAppender GetAppender()
        {
            var appender = new EventLogAppender
            {
                Threshold = Level.All,
                Layout = new PatternLayout(layoutString),
                ApplicationName = ApplicationName,
            };
            appender.ActivateOptions();
            return appender;
        }
    }
}
