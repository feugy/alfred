using Alfred.Core;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/// <summary>
/// Utility class to interoperate with system mouse
/// </summary>
class MouseInterop
{
    public const int LeftDown = 0x02;
    public const int LeftUp = 0x04;
    public const int RightDown = 0x08;
    public const int RightUp = 0x10;

    /// <summary>
    /// Programmatically triggers a mouse system event.
    /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms646260">see reference</a>
    /// </summary>
    [DllImport("user32.dll")]
    public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    /// <summary>
    /// Returns system-configurated time below which double click is detected.
    /// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms646258">see reference</a>
    /// </summary>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern uint GetDoubleClickTime();
}

namespace Alfred.Commands
{
    /// <summary>
    /// Command that changes mouse position.
    /// Always executes.
    /// </summary>
    class MousePosition: Command
    {
        /// <summary>
        /// Move cursor to expected position
        /// </summary>
        override public bool Execute(Context context)
        {
            Cursor.Position = context.Position;
            return true;
        }
    }

    /// <summary>
    /// Command that triggers a mouse simple left click
    /// </summary>
    class MouseLeftClick : Command
    {
        /// <summary>
        /// Triggers a left simple click at expected position
        /// </summary>
        override public bool Execute(Context context)
        {
            if (context.Recognized == Pattern.LeftClick)
            {
                Console.WriteLine("click at {0}:{1}", context.Position.X, context.Position.Y);
                MouseInterop.mouse_event(MouseInterop.LeftDown | MouseInterop.LeftUp, context.Position.X, context.Position.Y, 0, 0);
                return true;
            }
            return false;
        }
    }


    /// <summary>
    /// Command that triggers a mouse simple left click
    /// </summary>
    class MouseLeftDoubleClick : Command
    {
        /// <summary>
        /// Number of milliseconds between simple clicks
        /// </summary>
        public int Elapse = (int)(MouseInterop.GetDoubleClickTime()*0.75);

        /// <summary>
        /// Triggers a left double click at expected position
        /// </summary>
        override public bool Execute(Context context)
        {
            if (context.Recognized == Pattern.LeftDoubleClick)
            {
                Console.WriteLine("double click at {0}:{1}", context.Position.X, context.Position.Y);
                MouseInterop.mouse_event(MouseInterop.LeftDown | MouseInterop.LeftUp, context.Position.X, context.Position.Y, 0, 0);
                Thread.Sleep(Elapse);
                MouseInterop.mouse_event(MouseInterop.LeftDown | MouseInterop.LeftUp, context.Position.X, context.Position.Y, 0, 0);
                return true;
            }
            return false;
        }
    }
}
