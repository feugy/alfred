using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.commands
{
    using System.Threading;

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
}
