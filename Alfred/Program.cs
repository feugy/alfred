using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var gazeInput = new Gaze(50, Cursor.Position);
            gazeInput.Next += (object sender, input.GazeEventArgs e) =>
                new commands.MousePosition(e.Position).Execute();

            var voiceInput = new Voice();
            voiceInput.Next += (object sender, input.VoiceEventArgs e) =>
            {
                switch (e.Recognized)
                {
                    case Voice.Pattern.Calibrate:
                        new commands.Calibrate(gazeInput.Engine).Execute();
                        return;
                    case Voice.Pattern.LeftClick:
                        new commands.MouseLeftClick(Cursor.Position).Execute();
                        return;
                    case Voice.Pattern.Quit:
                        new commands.Quit(completion).Execute();
                        return;
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
