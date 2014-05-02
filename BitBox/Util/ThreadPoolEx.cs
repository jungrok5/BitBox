using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitBox.Util
{
    // 워커스레드및 IOCP에 사용되는 스레드 최대,최소 숫자 설정
    // 최대숫자를 프로세서 숫자보다 낮게 설정할 수 없음
    // 참고
    // http://msdn.microsoft.com/ko-kr/library/system.threading.threadpool(v=vs.110).aspx

    public static class TheadPoolEx
    {
        public static bool SetMinMaxThreads(int minWorkerThreads, int maxWorkerThreads, int minCompletionPortThreads, int maxCompletionPortThreads)
        {
            if (ThreadPool.SetMinThreads(minWorkerThreads, minCompletionPortThreads) == false)
                return false;
            if (ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionPortThreads) == false)
                return false;
            return true;
        }
    }
}
