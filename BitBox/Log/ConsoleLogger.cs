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
    // 콘솔로 남기기
    public class ConsoleLogger : LoggerBase
    {
        public override IAppender GetAppender()
        {
            var appender = new ColoredConsoleAppender
            {
                Threshold = Level.All,
                Layout = new PatternLayout(layoutString),
            };
            appender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Debug, ForeColor = ColoredConsoleAppender.Colors.Green | ColoredConsoleAppender.Colors.HighIntensity });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Info, ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Warn, ForeColor = ColoredConsoleAppender.Colors.Yellow | ColoredConsoleAppender.Colors.HighIntensity });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Error, ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors { Level = Level.Fatal, ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity, BackColor = ColoredConsoleAppender.Colors.Red });
            appender.ActivateOptions();
            return appender;
        }

        // 일단 Error는 ASSERT 걸자
        public override void Log(LogType type, string message)
        {
            //if (type == LogType.Error)
            //    System.Diagnostics.Debug.Assert(false, message);
            base.Log(type, message);
        }

        public override void Log(LogType type, string message, Exception e)
        {
            //if (type == LogType.Error)
            //    System.Diagnostics.Debug.Assert(false, message);
            base.Log(type, message, e);
        }

        public override void Log(LogType type, Exception e)
        {
            //if (type == LogType.Error)
            //    System.Diagnostics.Debug.Assert(false, e.Message);
            base.Log(type, e);
        }
    }
}
