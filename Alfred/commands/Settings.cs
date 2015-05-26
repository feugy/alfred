using Alfred.Core;
using System;
using EyeXFramework;
using Tobii.EyeX.Framework;

namespace Alfred.Commands
{
    /// <summary>
    /// EyeX engine callibration command
    /// </summary>
    class Calibrate: Command
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
        override public bool Execute(Context context)
        {
            if (context.Recognized == Pattern.Calibrate) 
            {
                Engine.EyeTrackingDeviceStatusChanged += OnStatusChanged;
                Console.WriteLine("starts calibration");
                Engine.LaunchRecalibration();
                IsRunning = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Right after calibration has started, awaits for engine to start tracking, meaning the calibration has ended.
        /// Unregisters itself, and allows command to be terminated
        /// </summary>
        /// <param name="source">Engine that sends the event</param>
        /// <param name="evt">New engine state</param>
        protected void OnStatusChanged(Object source, EngineStateValue<EyeTrackingDeviceStatus> evt)
        {
            if (evt.Value == EyeTrackingDeviceStatus.Tracking)
            {
                IsRunning = false;
                Engine.EyeTrackingDeviceStatusChanged -= OnStatusChanged;
                Console.WriteLine("calibration finished");
            }
        }
    }
}
