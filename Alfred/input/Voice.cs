using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.input
{
    using System.Speech.Recognition;

    /// <summary>
    /// Event argument class for gaze position event
    /// </summary>
    class VoiceEventArgs : EventArgs
    {
        /// <summary>
        /// Recognized pattern
        /// </summary>
        public Voice.Pattern Recognized;

        /// <summary>
        /// Creates a new event arguments
        /// </summary>
        /// <param name="position">Recognized text</param>
        public VoiceEventArgs(Voice.Pattern recognized)
        {
            Recognized = recognized;
        }
    }

    /// <summary>
    /// Input method using the Speech recognition engine to get extra commands, for:
    /// <ul>
    ///   <li>EyeX calibration</li>
    ///   <li>click and double click</li>
    ///   <li>drag and drop</li>
    ///   <li>dictation toggling</li>
    ///   <li>pan mode toggling</li>
    ///   <li>zoom toggling</li>
    ///   <li>cut, copy an paste</li>
    ///   <li>current application change</li>
    ///   <li>application closure</li>
    /// </ul>
    /// </summary>
    class Voice : IDisposable
    {

        /// <summary>
        /// Recognized patterns
        /// </summary>
        public enum Pattern { Quit, LeftClick, Calibrate };

        /// <summary>
        /// Speech recognition engine
        /// </summary>
        protected SpeechRecognitionEngine speech;

        /// <summary>
        /// Voice event handler method
        /// </summary>
        /// <param name="sender">The Gaze instance that sent gaze position</param>
        /// <param name="e">Event arguments, including position as a point</param>
        public delegate void VoiceEventHandler(object sender, VoiceEventArgs e);

        /// <summary>
        /// Raised when another text was recognized
        /// </summary>
        public event VoiceEventHandler Next;

        /// <summary>
        /// Loads grammar and init recognition engine
        /// </summary>
        public Voice()
        {
            // creates speech engine and 
            // TODO load commands grammar
            speech = new SpeechRecognitionEngine();
            var choices = new Choices("top", "calibration", "terminer");
            speech.LoadGrammar(new Grammar(choices));

            // starts recognition
            speech.SpeechRecognized += OnSpeech;
            speech.SetInputToDefaultAudioDevice();
            speech.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Invoked when Speech recognize a known command
        /// </summary>
        /// <param name="sender">Speech recognition engine publishing this event</param>
        /// <param name="evt">Recognized speech details</param>
        protected void OnSpeech(object sender, SpeechRecognizedEventArgs evt)
        {
            VoiceEventArgs triggered = null;
            switch (evt.Result.Text)
            {
                case "calibration":
                    triggered = new VoiceEventArgs(Pattern.Calibrate);
                    break;
                case "top":
                    triggered = new VoiceEventArgs(Pattern.LeftClick);
                    break;
                case "terminer":
                    triggered = new VoiceEventArgs(Pattern.Quit);
                    break;
            }
            if (Next != null && triggered != null)
            {
                Next(this, triggered);
            }
        }

        /// <summary>
        /// Dispose speech recognition engine
        /// </summary>
        public void Dispose() 
        {
            speech.Dispose();
        }
    }
}
