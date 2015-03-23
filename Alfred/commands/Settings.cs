using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.commands
{
    using EyeXFramework;

    /// <summary>
    /// EyeX engine callibration command
    /// </summary>
    class Calibrate: ICommand
    {
        /// <summary>
        /// EyeX engine to be callibrated
        /// </summary>
        public EyeXHost Engine;

        /// <summary>
        /// Creates a calibration command on a given EyeX engine
        /// </summary>
        /// <param name="engine">Callibrated engine</param>
        public Calibrate(EyeXHost engine)
        {
            Engine = engine;
        }

        /// <summary>
        /// Starts EyeX engine callibration
        /// </summary>
        public void Execute()
        {
            Console.WriteLine("starts calibration");
            Engine.LaunchRecalibration();
        }
    }
}
