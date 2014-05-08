using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBox.Core;
using BitBoxAppServerExample.Scripts;

namespace BitBoxAppServerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerExecuter<ExampleAppServer>.Execute(args, "0.0.1");
        }
    }
}
