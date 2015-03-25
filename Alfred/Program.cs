namespace Alfred
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using input;

    class Program
    {
        static void Main(string[] args)
        {
            // object used to await for terminate order
            var completion = new ManualResetEvent(false);

            // initialize input methods
            Console.WriteLine("starting inputs...");
            var gazeInput = new Gaze(50);
            var voiceInput = new Voice(completion, gazeInput);
            // wait for completion before disposal
            completion.WaitOne();
            Console.WriteLine("closing application...");
            gazeInput.Dispose();
            voiceInput.Dispose();
        }
    }
}
