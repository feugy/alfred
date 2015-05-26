using Alfred.Core;
using System;

namespace Alfred
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting inputs...");
            var engine = new Engine();
            engine.Start().WaitOne();
            Console.WriteLine("closing application...");
            engine.Dispose();
        }
    }
}
