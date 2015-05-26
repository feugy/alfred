using System;

namespace Alfred.Core
{
    /// <summary>
    /// Delegate used for input method changes
    /// </summary>
    /// <param name="sender">Input method responsible for the change</param>
    /// <param name="evt">Event details, depends on the input method</param>
    public delegate void ChangedEventHandler(InputMethod sender, EventArgs evt);

    /// <summary>
    /// Input method used by core engine.
    /// </summary>
    public abstract class InputMethod : IDisposable
    {
        /// <summary>
        /// Event triggered when input got something to send to engine
        /// </summary>
        public event ChangedEventHandler Change;

        /// <summary>
        /// Trigger a change event
        /// </summary>
        /// <param name="evt">Arguments passed to event listeners</param>
        protected void TriggerChange(EventArgs evt)
        {
            if (Change != null)
            {
                Change(this, evt);
            }
        }

        public abstract void Dispose();
    }
}
