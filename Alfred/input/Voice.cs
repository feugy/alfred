namespace Alfred.input
{
    using System;
    using System.Speech.Recognition;
    using System.Threading;

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
        public static Pattern Prefix = new Pattern("al");

        // Supported patterns
        public static Pattern Quit = new Pattern("terminer");
        public static Pattern LeftDoubleClick = new Pattern("tiptop", true);
        public static Pattern LeftClick = new Pattern("top", true);
        public static Pattern Calibrate = new Pattern("calibration");
        public static Pattern Close = new Pattern("fermer");

        /// <summary>
        /// List of supported patterns
        /// </summary>
        public static Pattern[] Values = new Pattern[] { LeftClick, LeftDoubleClick, Close, Calibrate, Quit };
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
        /// Speech recognition engine
        /// </summary>
        protected SpeechRecognitionEngine speech;

        /// <summary>
        /// Completion handle used
        /// </summary>
        public EventWaitHandle Completion;

        /// <summary>
        /// Gaze input to get current mouse position
        /// </summary>
        public Gaze GazeInput;

        /// <summary>
        /// Loads grammar and init recognition engine
        /// </summary>
        /// <param name="completion">Program completion handler</param>
        /// <param name="gazeInput">Gaze input handler</param>
        public Voice(EventWaitHandle completion, Gaze gazeInput)
        {
            Completion = completion;
            GazeInput = gazeInput;

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
            if (evt.Result != null && evt.Result.Semantics != null)
            {
                var command = Pattern.ValueOf(evt.Result.Semantics["command"].Value.ToString());
                if (Pattern.Calibrate.Equals(command))
                {
                    new commands.Calibrate(GazeInput.Engine).Execute();
                }
                else if (Pattern.LeftClick.Equals(command))
                {
                    new commands.MouseLeftClick(GazeInput.Position).Execute();
                }
                else if (Pattern.LeftDoubleClick.Equals(command))
                {
                    new commands.MouseLeftDoubleClick(GazeInput.Position).Execute();
                }
                else if (Pattern.Close.Equals(command))
                {
                    new commands.Close().Execute();
                }
                else if (Pattern.Quit.Equals(command))
                {
                    new commands.Quit(Completion).Execute();
                }

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
