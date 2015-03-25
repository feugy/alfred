namespace Alfred.commands
{
    using System.Drawing;

    /// <summary>
    /// Common interface for all commands
    /// Use constructor to initialize, and Execute() to apply
    /// </summary>
    interface ICommand
    {
        /// <summary>
        /// Invoked to effectively apply the command
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// Common class for commands in need of screen position
    /// </summary>
    public abstract class PositionCommand
    {
        /// <summary>
        /// Screen position where command is applied
        /// </summary>
        public Point Position;
    }
}
