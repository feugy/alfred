namespace Alfred.Core
{
    /// <summary>
    /// Common interface for all commands
    /// Use constructor to initialize, and Execute() to apply
    /// 
    /// Some commands ends right after their execution.
    /// Other may keep running after the execution, and must be terminated. 
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Flag that indicate if command is currently running.
        /// Only for commands that keeps running after their Execute() call
        /// </summary>
        public bool IsRunning = false;

        /// <summary>
        /// Invoked by the core engine to detect if a command can be executed.
        /// If so, the command is effectively apply the command.
        /// </summary>
        /// <returns>True if the command was executed</returns>
        public abstract bool Execute(Context context);

        /// <summary>
        /// Invoked by the core engine on running commands, to check if it can be stopped.
        /// If so, the command is effectively stopped.
        /// </summary>
        /// <returns>True if the command was terminated</returns>
        public virtual bool Terminate(Context context)
        {
            // Default implentation is to terminate once the running status was reset to false
            return !IsRunning;
        }
    }
}
