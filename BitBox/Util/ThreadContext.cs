using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitBox.Log;

namespace BitBox.Util
{
    // ADO.NET은 기본적으로 디비 커넥션 풀을 사용하고 있으나 별로(?)라고 한다 이유는 모르겠으나 직접 만들어 쓴다고 하니..
    // http://www.csharpstudy.com/SQL/SQL-connection-pooling.aspx
    // http://msdn.microsoft.com/KO-KR/library/8xx3tyca(v=vs.110).aspx
    // http://www.codeproject.com/Articles/710384/Creating-a-custom-database-connection-pool

    // 스레드 저장소
    // 참고
    // http://msdn.microsoft.com/ko-kr/library/dd642243(v=vs.110).aspx
    // http://jacking.tistory.com/1118

    public abstract class ThreadContextBase : IDisposable
    {
        private static readonly string STRING_FORMAT = "ThreadContext id:{0}";
        public int ThreadID { get; set; }

        public abstract void Create();
        public abstract void Dispose();

        public override string ToString()
        {
            return string.Format(STRING_FORMAT, ThreadID);
        }
    }

    public static class ThreadContext<T> where T : ThreadContextBase, new()
    {
        private static ThreadLocal<T> m_Tls = null;
        public static void Init()
        {
            m_Tls = new ThreadLocal<T>(() => 
            {
                var context = new T();
                context.ThreadID = Thread.CurrentThread.ManagedThreadId;
                context.Create();
                return context;
            }, true);
        }

        public static void Dispose()
        {
            foreach (var context in m_Tls.Values)
            {
                context.Dispose();
            }
        }

        public static T GetContext()
        {
            T context = m_Tls.Value;
            Logger.Debug(context.ToString());
            return context;
        }
    }
}
