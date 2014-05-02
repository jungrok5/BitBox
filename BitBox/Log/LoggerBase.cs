using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace BitBox.Log
{
    public enum LogType
    {
        Debug, Info, Warning, Error
    };

    // 기본적으로 log4net을 로그모듈로 사용하려고함
    // 콘솔,윈도우시스템이벤트,파일,DB 기타 여러가지 지원하고 빠름(인지는 모르겠지만)
    // 로그를 쌓는 방식은 여러가지가 있음 아래 링크 참고
    // http://logging.apache.org/log4net/release/config-examples.html

    public abstract class LoggerBase
    {
        // 스레드에 안전함
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static readonly string layoutString = "%date [%thread] %level - %message%newline";

        public virtual void Init() { BasicConfigurator.Configure(GetAppender()); }

        public abstract IAppender GetAppender();

        public void Debug(string message) { Log(LogType.Debug, message); }
        public void Info(string message) { Log(LogType.Info, message); }
        public void Warning(string message) { Log(LogType.Warning, message); }
        public void Error(string message) { Log(LogType.Error, message); }

        public void Debug(string message, Exception e) { Log(LogType.Debug, message, e); }
        public void Info(string message, Exception e) { Log(LogType.Info, message, e); }
        public void Warning(string message, Exception e) { Log(LogType.Warning, message, e); }
        public void Error(string message, Exception e) { Log(LogType.Error, message, e); }

        public void Debug(Exception e) { Log(LogType.Debug, e); }
        public void Info(Exception e) { Log(LogType.Info, e); }
        public void Warning(Exception e) { Log(LogType.Warning, e); }
        public void Error(Exception e) { Log(LogType.Error, e); }

        public virtual void Log(LogType type, string message)
        {
            switch (type)
            {
                case LogType.Debug: log.Debug(message); break;
                case LogType.Info: log.Info(message); break;
                case LogType.Warning: log.Warn(message); break;
                case LogType.Error: log.Error(message); break;
            }
        }

        public virtual void Log(LogType type, string message, Exception e)
        {
            switch (type)
            {
                case LogType.Debug: log.Debug(message, e); break;
                case LogType.Info: log.Info(message, e); break;
                case LogType.Warning: log.Warn(message, e); break;
                case LogType.Error: log.Error(message, e); break;
            }
        }

        public virtual void Log(LogType type, Exception e)
        {
            switch (type)
            {
                case LogType.Debug: log.Debug(e); break;
                case LogType.Info: log.Info(e); break;
                case LogType.Warning: log.Warn(e); break;
                case LogType.Error: log.Error(e); break;
            }
        }
    }

    public class NullLogger : LoggerBase
    {
        public override void Init() { }
        public override IAppender GetAppender() { return null; }

        public override void Log(LogType type, string message) { }
        public override void Log(LogType type, string message, Exception e) { }
        public override void Log(LogType type, Exception e) { }
    }
}
