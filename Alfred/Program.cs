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
            gazeInput.Next += (object sender, input.GazeEventArgs e) =>
                new commands.MousePosition(e.Position).Execute();

            var voiceInput = new Voice();
            voiceInput.Next += (object sender, input.VoiceEventArgs e) =>
            {
                if (e.Recognized.Equals(Voice.Pattern.Calibrate))
                {
                    new commands.Calibrate(gazeInput.Engine).Execute();
                }
                else if (e.Recognized.Equals(Voice.Pattern.LeftClick))
                { 
                    new commands.MouseLeftClick(Cursor.Position).Execute();
                }
                else if (e.Recognized.Equals(Voice.Pattern.LeftDoubleClick))
                {
                    new commands.MouseLeftDoubleClick(Cursor.Position).Execute();
                }
                else if (e.Recognized.Equals(Voice.Pattern.Close))
                {
                    new commands.Close().Execute();
                }
                else if (e.Recognized.Equals(Voice.Pattern.Quit))
                { 
                    new commands.Quit(completion).Execute();
                }
            };

            // wait for completion before disposal
            completion.WaitOne();
            Console.WriteLine("closing application...");
            gazeInput.Dispose();
            voiceInput.Dispose();
        }
    }
}
