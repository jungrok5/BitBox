using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitBox.Util
{
    // 원자성을 지니고 있다
    // http://msdn.microsoft.com/ko-kr/library/system.threading.interlocked(v=vs.110).aspx

    // A랑 C가 같다면 B로 바꿔라. 다르다면 바뀌지 않으며, 성공실패를 떠나 리턴값은 항상 A의 원래값이다
    // Interlocked.CompareExchange(A, B, C)

    public class AtomicBool
    {
        private int m_Value;

        public AtomicBool() { m_Value = 0; }

        // true로 값을 설정. 리턴값은 성공이면 true, 실패면 false
        public bool SetTrue()
        {
            return Interlocked.CompareExchange(ref m_Value, 1, 0) == 0;
        }

        // false로 값을 설정. 리턴값은 성공이면 true, 실패면 false
        public bool SetFalse()
        {
            return Interlocked.CompareExchange(ref m_Value, 0, 1) == 1;
        }

        // 강제로 값을 true로 변경
        public void ForceTrue()
        {
            Interlocked.Exchange(ref m_Value, 1);
        }

        // 강제로 값을 false로 변경
        public void ForceFalse()
        {
            Interlocked.Exchange(ref m_Value, 0);
        }

        // 값이 true인가?
        // 참고로
        // 32bit 숫자의 경우는 읽기 작업은 항상 원자적이다
        // 64bit 숫자의 경우는 32bit시스템에서는 원자적이지 않기 때문에 Interlocked.Read 함수를 사용해야함
        public bool IsTrue()
        {
            return m_Value == 1;
        }
    }
}
