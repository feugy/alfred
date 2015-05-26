using Alfred.Core;
using System.Diagnostics;

namespace Alfred.Commands
{
    /// <summary>
    /// Zoom all screen with Window's Magnification API
    /// </summary>
    class ZoomIn: Command
    {

        override public bool Execute(Context context)
        {
            if (context.Recognized == Pattern.ZoomIn)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "Magnify.exe";
                startInfo.Arguments = "/fullscreen";
                Process.Start(startInfo);
                IsRunning = true;
                return true;
            }
            return false;
        }
    }
}
