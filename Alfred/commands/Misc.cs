namespace Alfred.commands
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Alfred terminaison command
    /// </summary>
    class Quit: ICommand
    {
        /// <summary>
        /// Completion handle used
        /// </summary>
        public EventWaitHandle Completion;

        /// <summary>
        /// Set completion handle that will be terminated
        /// </summary>
        /// <param name="completion">Completion handle</param>
        public Quit(EventWaitHandle completion) 
        {
            Completion = completion;
        }

        /// <summary>
        /// Set the completion handle that will stop tha wait
        /// </summary>
        public void Execute()
        {
            Completion.Set();
        }
    }

    /// <summary>
    /// Sends close shortcut (alt + F4) to active window
    /// </summary>
    class Close: ICommand
    {
        public void Execute()
        {
            Console.WriteLine("send close shortcut");
            SendKeys.SendWait("%{F4}");
        }
    }
}
