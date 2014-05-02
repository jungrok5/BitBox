using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitBox.Log;
using BitBox.Util;

namespace BitBox.Test
{
    // 뭐가 어떻게 구성될진 모르겠지만 일단 테스용기능들 넣어둘라고
    // 나중에 별도로 어트리뷰트 만들어서 자동으로 로드해서 돌리게 해보자
    // NUnit같은걸 써볼까?
    public static class TestModule
    {
        public static void Start()
        {
            //Test_Log();
            //Test_MiniDump();
            //Test_ThreadLocal();
            //Test_8GB();
            Test_ThreadDbContext();
        }

        private static byte[] m_Buffer1;
        private static byte[] m_Buffer2;
        private static byte[] m_Buffer3;
        private static byte[] m_Buffer4;
        private static byte[] m_Buffer5;
        private static byte[] m_Buffer6;
        private static byte[] m_Buffer7;
        private static byte[] m_Buffer8;

        // 4G이상을 사용하려면 빌드옵션에서 AnyCPU가 아니라 명시적으로 x64(64비트)로 변경해줘야 가능함
        // http://stackoverflow.com/questions/1153702/system-outofmemoryexception-was-thrown-when-there-is-still-plenty-of-memory-fr
        static void Test_8GB()
        {
            m_Buffer1 = new byte[1024 * 1024 * 1024];
            m_Buffer2 = new byte[1024 * 1024 * 1024];
            m_Buffer3 = new byte[1024 * 1024 * 1024];
            m_Buffer4 = new byte[1024 * 1024 * 1024];
            m_Buffer5 = new byte[1024 * 1024 * 1024];
            m_Buffer6 = new byte[1024 * 1024 * 1024];
            m_Buffer7 = new byte[1024 * 1024 * 1024];
            m_Buffer8 = new byte[1024 * 1024 * 1024];
        }

        static void Test_Log()
        {
            Logger.Debug("나는 디버그");
            Logger.Info("나는 정보");
            Logger.Warning("나는 경고");
            Logger.Error("나는 오류");
        }

        static void Test_MiniDump()
        {
            MiniDump.Write(new Exception("minidump test"));
        }

        static void Test_ThreadLocal()
        {
            ThreadLocal<string> ThreadName = new ThreadLocal<string>(() =>
            {
                return "Thread" + Thread.CurrentThread.ManagedThreadId;
            });

            Action action = () =>
            {
                // true면 한번호출되어서 생성되어 있다는거다
                bool repeat = ThreadName.IsValueCreated;

                Console.WriteLine("ThreadName = {0} {1}", ThreadName.Value, repeat ? "(repeat)" : "");
            };

            Parallel.Invoke(action, action, action, action, action, action, action, action);

            ThreadName.Dispose();
            Console.ReadKey();
        }

        static void Test_ThreadDbContext()
        {
            ThreadContext<TestDbThreadContext>.Init();

            Parallel.For(1, 20, x =>
                {
                    TestDbThreadContext context = ThreadContext<TestDbThreadContext>.GetContext();
                    SqlConnection db = context.GetDB();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM x", db);
                    cmd.ExecuteNonQuery();
                });

            ThreadContext<TestDbThreadContext>.Dispose();
        }
    }
}
