using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Config;

namespace BitBox.Log
{
    // log4net 자체가 appender(로그 남기는모듈)를 여러개를 셋팅할 수 있는 구조이다. ex) 콘솔에도 남기면서, 파일로도 남겨라
    // 로거를 멀티로 셋팅하게 수정하다 지저분해짐 나중에 바꾸자 ㅋ
    // 그냥 요기서 로거를 루프돌면서 호출해도 되는데 분명그거랑 log4net자체 멀티 appender랑은 다를것 같아서 그렇게 처리 안함
    
    public static class Logger
    {
        private static LoggerBase m_Logger;
        private static bool m_IsDebug;

        public static void Init(LoggerBase mainLogger, LoggerBase logger, bool isDebug = true)
        {
            m_Logger = logger;
            List<LoggerBase> loggers = new List<LoggerBase>();
            loggers.Add(logger);
            Init(mainLogger, loggers, isDebug);
        }

        public static void Init(LoggerBase mainLogger, List<LoggerBase> loggers, bool isDebug = true)
        {
            List<IAppender> appenders = new List<IAppender>();
            foreach (var logger in loggers)
            {
                appenders.Add(logger.GetAppender());
            }

            m_Logger = mainLogger;
            m_IsDebug = isDebug;

            BasicConfigurator.Configure(appenders.ToArray());
        }

        public static void Debug(string message) { if (m_IsDebug == false) return; Log(LogType.Debug, message); }
        public static void Info(string message) { Log(LogType.Info, message); }
        public static void Warning(string message) { Log(LogType.Warning, message); }
        public static void Error(string message) { Log(LogType.Error, message); }

        public static void Debug(string message, Exception e) { if (m_IsDebug == false) return; Log(LogType.Debug, message, e); }
        public static void Info(string message, Exception e) { Log(LogType.Info, message, e); }
        public static void Warning(string message, Exception e) { Log(LogType.Warning, message, e); }
        public static void Error(string message, Exception e) { Log(LogType.Error, message, e); }

        public static void Debug(Exception e) { if (m_IsDebug == false) return; Log(LogType.Debug, e); }
        public static void Info(Exception e) { Log(LogType.Info, e); }
        public static void Warning(Exception e) { Log(LogType.Warning, e); }
        public static void Error(Exception e) { Log(LogType.Error, e); }

        public static void Log(LogType type, string message)
        {
            if (m_Logger == null) return;
            m_Logger.Log(type, message);
        }

        public static void Log(LogType type, string message, Exception e)
        {
            if (m_Logger == null) return;
            m_Logger.Log(type, message, e);
        }

        public static void Log(LogType type, Exception e)
        {
            if (m_Logger == null) return;
            m_Logger.Log(type, e);
        }
    }
}
