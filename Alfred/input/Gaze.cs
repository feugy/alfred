using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.input
{
    using EyeXFramework;
    using System.Drawing;
    using System.Timers;
    using Tobii.EyeX.Client;
    using Tobii.EyeX.Framework;

    /// <summary>
    /// Event argument class for gaze position event
    /// </summary>
    class GazeEventArgs : EventArgs
    {
        /// <summary>
        /// Gaze current position
        /// </summary>
        public Point Position;

        /// <summary>
        /// Creates a new event arguments
        /// </summary>
        /// <param name="position">Current position</param>
        public GazeEventArgs(Point position)
        {
            Position = position;
        }
    }
    
    /// <summary>
    /// Input method using the EyeX controller to get gaze (eye focus point on screen) position.<br/>
    /// Because EyeX is giving a gaze position approximatively 60 times per seconds (every 16ms), we only pick a
    /// bunch of them (one every 30) and interpolate straight path between them.
    /// </summary>
    class Gaze : IDisposable
    {
        /// <summary>
        /// Engine used
        /// </summary>
        public EyeXHost Engine;

        /// <summary>
        /// Generate gaze position on screen at approximatively 60 frame per second
        /// </summary>
        protected FixationDataStream gazeStream;

        /// <summary>
        /// Number of intepolated positions within the time duration
        /// </summary>
        protected int interpolated;

        /// <summary>
        /// Sampling duration, during which interpolation is computed
        /// </summary>
        protected int duration = 500;

        /// <summary>
        /// Current gaze position
        /// </summary>
        protected Point position;

        /// <summary>
        /// Last event timestamp, for cursor interpollation
        /// </summary>
        protected double last = 0;

        /// <summary>
        /// Interpolated gaze positions stored between two sampling
        /// </summary>
        protected Queue<Point> positions;

        /// <summary>
        /// Used to dequeue intepolated position at fixed-rate
        /// </summary>
        protected System.Timers.Timer timer;

        /// <summary>
        /// Gaze event handler method
        /// </summary>
        /// <param name="sender">The Gaze instance that sent gaze position</param>
        /// <param name="e">Event arguments, including position as a point</param>
        public delegate void GazeEventHandler(object sender, GazeEventArgs e);

        /// <summary>
        /// Raised when another gaze position is ready
        /// </summary>
        public event GazeEventHandler Next;
        
        /// <summary>
        /// Detect EyeX engine presence, and starts it to get gaze positions
        /// </summary>
        /// <param name="fps">Number of gaze positions expected per second</param>
        /// <param name="position">Initial position for first interpolation</param>
        /// <exception cref="Exception">If EyeX engine is not installed or running</exception>
        public Gaze(int fps, Point position)
        {
            this.interpolated = duration*fps/1000;
            this.position = position;
            positions = new Queue<Point>(interpolated);

            // simple check of EyeX presence
            switch (EyeXHost.EyeXAvailability)
            {
                case EyeXAvailability.NotAvailable:
                   throw new Exception("Please install the EyeX Engine");
                case EyeXAvailability.NotRunning:
                   throw new Exception("Please make sure that the EyeX Engine is started");
            }

            Engine = new EyeXHost();
            // track gaze position to set cursor on it
            gazeStream = Engine.CreateFixationDataStream(FixationDataMode.Sensitive);
            gazeStream.Next += OnGazeChange;
            // start the EyeX engine
            Engine.Start();

            // start timer
            timer = new System.Timers.Timer(duration/interpolated);
            timer.Enabled = true;
            timer.Elapsed += OnTick;
        }

        /// <summary>
        /// Clean timer and EyeX engine
        /// </summary>
        public void Dispose()
        {
            timer.Dispose();
            Engine.Dispose();
        }

        /// <summary>
        /// Invoked when EyeX engine detect a gaze change
        /// </summary>
        /// <param name="source">EyeX original stream publishing this event</param>
        /// <param name="evt">Gaze event details, including current time and gaze position</param>
        protected void OnGazeChange(Object source, FixationEventArgs evt)
        {
            if (evt.Timestamp - last > duration)
            {
                last = evt.Timestamp;
                positions.Clear();

                var end = new Point((int)evt.X, (int)evt.Y);
    
                // compute straight formula for interpolation
                var a = (double)(end.Y - position.Y) / (end.X - position.X);
                var b = position.Y - a * position.X;
                var step = (double)(end.X - position.X) / interpolated;

                // intepolate enought points for the next interval
                foreach (var i in Enumerable.Range(1, interpolated))
                {
                    double x = position.X + step * i;
                    var added = new Point((int)x, (int)(a * x + b));
                    positions.Enqueue(added);
                }
            }
        }

        /// <summary>
        /// Invoked at fixed-rate by timer to to dequeue available interpolated positions
        /// </summary>
        /// <param name="source">Timer publishing this event</param>
        /// <param name="evt">Tick event details</param>
        protected void OnTick(Object source, ElapsedEventArgs evt)
        {
            if (positions.Count != 0)
            {
                // send new position
                position = positions.Dequeue();
                if (Next != null)
                {
                    Next(this, new GazeEventArgs(position));
                }
            }
        }
    }
}
