namespace Alfred.Core
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
        public static Pattern Prefix = new Pattern("al");

        // Supported patterns
        public static Pattern Quit = new Pattern("terminer");
        public static Pattern LeftDoubleClick = new Pattern("tiptop", true);
        public static Pattern LeftClick = new Pattern("top", true);
        public static Pattern Calibrate = new Pattern("calibration");
        public static Pattern Close = new Pattern("fermer");
        public static Pattern ZoomIn = new Pattern("zoom");
        public static Pattern Dictation = new Pattern("texte");
        public static Pattern Cancel = new Pattern("annuler");

        /// <summary>
        /// List of supported patterns
        /// </summary>
        public static Pattern[] Values = new Pattern[] 
        { 
            LeftClick, 
            LeftDoubleClick, 
            Cancel,
            Close, 
            ZoomIn, 
            Dictation, 
            Calibrate, 
            Quit 
        };
    }
}
