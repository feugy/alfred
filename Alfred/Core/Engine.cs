using Alfred.Input;
using Alfred.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Alfred.Core
{
    /// <summary>
    /// Execution context, given to commands
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Recognized pattern
        /// </summary>
        public Pattern Recognized = null;

        /// <summary>
        /// Current position
        /// </summary>
        public Point Position = Cursor.Position;

        /// <summary>
        /// Currently active commands.
        /// </summary>
        public List<Command> RunningCommands = new List<Command>();

        /// <summary>
        /// Indicates whether a command of a given type is currently running
        /// </summary>
        /// <param name="candidate">Type of which a running command is searched</param>
        /// <returns>True if a least one command of this type is running</returns>
        public bool IsRunning(Type candidate) 
        {
            return RunningCommands.Find(x => x.GetType() == candidate) != null;
        }
    }

    /// <summary>
    /// This engines loads several inputs methods.
    /// It maintains an execution context, and when an input changed, tries to execute (or stop) supported commands.
    /// </summary>
    class Engine : IDisposable
    {
        /// <summary>
        /// Current execution context.
        /// </summary>
        public Context Context = new Context();

        /// <summary>
        /// Active inputs
        /// </summary>
        public List<InputMethod> InputMethods = new List<InputMethod>();

        /// <summary>
        /// Supported commands
        /// </summary>
        public List<Command> SupportedCommands = new List<Command>();

        /// <summary>
        /// Starts the engine, initializing input methods and supported commands
        /// </summary>
        /// <returns>An handle to wait on, that must be used to properly close the engine</returns>
        public EventWaitHandle Start() {
            // object used to await for terminate order
            var completion = new ManualResetEvent(false);

            // initialize input methods
            var gaze = new Gaze(50);
            gaze.Change += (InputMethod input, EventArgs evt) => {
                // reset recognized command to avoid triggering on the next position change
                Context.Recognized = null;
                Context.Position = ((GazeChangeEventArgs)evt).Position;
                TriesCommands();
            };
            var voice = new Voice();
            voice.Change += (InputMethod input, EventArgs evt) => {
                Context.Recognized = ((PatternRecognizedEventArgs)evt).Recognized;
                TriesCommands();
            };
            InputMethods.Add(gaze);
            InputMethods.Add(voice);

            // initialize supported commands
            SupportedCommands.Add(new MousePosition());
            SupportedCommands.Add(new Calibrate(gaze.Engine));
            SupportedCommands.Add(new MouseLeftClick());
            SupportedCommands.Add(new MouseLeftDoubleClick());
            SupportedCommands.Add(new Cancel());
            SupportedCommands.Add(new Close());
            SupportedCommands.Add(new Dictate(voice.Speech));
            SupportedCommands.Add(new Quit(completion));

            return completion;
        }

        /// <summary>
        /// On context change, tries to stop running commands, and to execute new ones
        /// </summary>
        protected void TriesCommands()
        {
            // first stops running commands
            foreach(Command running in Context.RunningCommands) 
            {
                if (running.Terminate(Context)) 
                {
                    // effectively removes terminated command
                    Context.RunningCommands.Remove(running);
                }
            }
            foreach(Command candidate in SupportedCommands)
            {
                if (!candidate.IsRunning)
                {
                    // do not re-execute commands that are already running
                    if (candidate.Execute(Context))
                    {
                        if (candidate.IsRunning)
                        {
                            // for long running command, stores them
                            Context.RunningCommands.Add(candidate);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dispose input methods
        /// </summary>
        public void Dispose() 
        {
            foreach (InputMethod input in InputMethods) {
                input.Dispose();
            }
        }
    }
}
