namespace Alfred.input
{
    using System;
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
        public sealed class Pattern
        {
            /// <summary>
            /// Builds a pattern from a given spoken word
            /// </summary>
            /// <param name="value">Spoken word</param>
            /// <param name="direct">False if word must be prefixed</param>
            private Pattern(string value, bool direct = false) 
            { 
                Value = value;
                IsDirect = direct;
            }

            /// <summary>
            /// Spoken word for this patter
            /// </summary>
            public string Value;

            /// <summary>
            /// True when word is spoken without prefix
            /// </summary>
            public bool IsDirect;

            /// <summary>
            /// Returns a string representation of this pattern
            /// </summary>
            /// <returns>The spoken word</returns>
            public override string ToString()
            {
                return Value;
            }

            /// <summary>
            /// Build a known pattern from a string value
            /// </summary>
            /// <param name="value">string to be converted to pattern</param>
            /// <returns>The known pattern or null if value isn't recognized</returns>
            public static Pattern ValueOf(string value)
            {
                foreach (Pattern known in Values)
                {
                    if (known.Value.Equals(value))
                    {
                        return known;
                    }
                }
                return null;
            }

            /// <summary>
            /// Prefix used before commands
            /// </summary>
            public static Pattern Prefix            = new Pattern("al");

            // Supported patterns
            public static Pattern Quit              = new Pattern("terminer");
            public static Pattern LeftDoubleClick   = new Pattern("tiptop", true);
            public static Pattern LeftClick         = new Pattern("top", true);
            public static Pattern Calibrate         = new Pattern("calibration");
            public static Pattern Close             = new Pattern("fermer");

            /// <summary>
            /// List of supported patterns
            /// </summary>
            public static Pattern[] Values = new Pattern[] { LeftClick, LeftDoubleClick, Close, Calibrate, Quit };
        }

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
            // creates speech engine 
            speech = new SpeechRecognitionEngine();
            // load commands grammar
            var directCommands = new Choices();
            var commands = new Choices();
            foreach(Pattern pattern in Pattern.Values)
            {
                if (pattern.IsDirect)
                {
                    directCommands.Add(pattern.Value);
                }
                else
                {
                    commands.Add(pattern.Value);
                }
            }
            // first direct patterns
            var direct = new GrammarBuilder();
            direct.Append(new SemanticResultKey("command", directCommands));
            speech.LoadGrammar(new Grammar(direct));
            // then prefixed ones
            var prefixed = new GrammarBuilder(Pattern.Prefix.Value);
            prefixed.Append(new SemanticResultKey("command", commands));
            speech.LoadGrammar(new Grammar(prefixed));

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
            if (evt.Result != null && evt.Result.Semantics != null)
            {
                triggered = new VoiceEventArgs(Pattern.ValueOf(evt.Result.Semantics["command"].Value.ToString()));
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
