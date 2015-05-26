using Alfred.Core;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Alfred.Commands
{
    /// <summary>
    /// Alfred terminaison command
    /// </summary>
    class Quit: Command
    {
        public EventWaitHandle Completion;

        public Quit(EventWaitHandle completion)
        {
            Completion = completion;
        }

        override public bool Execute(Context context)
        {
            if (context.Recognized == Pattern.Quit)
            {
                Completion.Set();
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Sends close shortcut (alt + F4) to active window
    /// </summary>
    class Close: Command
    {
        override public bool Execute(Context context)
        {
            if (context.Recognized == Pattern.Close)
            {
                bool runningCommandEnded = false;
                foreach (Command running in context.RunningCommands)
                {
                    if (running.GetType() == typeof(Calibrate))
                    {
                        // calibration in progress: triggers enter
                        runningCommandEnded = true;
                        SendKeys.SendWait("~");
                        break;
                    } else if (running.GetType() == typeof(Dictate))
                    {
                        // dictation in progress: ends it
                        runningCommandEnded = true;
                        running.IsRunning = false;
                        break;
                    }
                }
                if (!runningCommandEnded)
                {
                    Console.WriteLine("send close shortcut");
                    // trigger Alt + F4
                    SendKeys.SendWait("%{F4}");
                }
                return true;
            }
            return false;
        }
    }
}
