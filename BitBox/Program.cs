using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitBox.Core;

namespace BitBox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Assembly.GetExecutingAssembly().FullName);
            ServerExecuter<TestServer>.Execute(args);
        }
    }
}
