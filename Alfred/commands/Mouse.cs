using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.commands
{
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// Command that changes mouse position
    /// </summary>
    class MousePosition: PositionCommand, ICommand
    {
        /// <summary>
        /// Set expected mouse position
        /// </summary>
        /// <param name="position">New mouse position</param>
        public MousePosition(Point position)
        {
            Position = position;
        }

        /// <summary>
        /// Move cursor to expected position
        /// </summary>
        public void Execute()
        {
            Cursor.Position = Position;
        }
    }

    /// <summary>
    /// Command that triggers a mouse simple left click
    /// </summary>
    class MouseLeftClick : PositionCommand, ICommand
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        /// <summary>
        /// Set mouse click position
        /// </summary>
        /// <param name="position">Mouse click position</param>
        public MouseLeftClick(Point position)
        {
            Position = position;
        }

        /// <summary>
        /// Triggers a left simple click at expected position
        /// </summary>
        public void Execute()
        {
            Console.WriteLine("click at {0}:{1}", Position.X, Position.Y);
            mouse_event(0x02 | 0x04, Position.X, Position.Y, 0, 0);    
        }
    }
}
