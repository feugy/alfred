using Alfred.Core;
using System;
using System.Windows.Forms;
using System.Speech.Recognition;

namespace Alfred.Commands
{

    /// <summary>
    /// Enable dictation mode
    /// </summary>
    class Dictate : Command
    {
        /// <summary>
        /// Completion handle used
        /// </summary>
        public SpeechRecognitionEngine Speech;

        /// <summary>
        /// Dictation Grammar, exposed for disabling command
        /// 
        /// </summary>
        public DictationGrammar Grammar;

        /// <summary>
        /// Enable dictation mode in the given speech engine
        /// </summary>
        /// <param name="completion">Speech recognition engine used</param>
        public Dictate(SpeechRecognitionEngine speech)
        {
            Speech = speech;
            Grammar = new DictationGrammar();
            Grammar.Name = "dictation";
            Grammar.Weight = 0.1f;
        }

        // TODOC
        override public bool Execute(Context context)
        {
            if (context.Recognized == Pattern.Dictation)
            {
                Console.WriteLine("dictation started...");
                Speech.LoadGrammar(Grammar);
                Speech.SpeechHypothesized += onSpeechHypothesis;
                Speech.SpeechRecognized += onSpeechRecognized;
                // long running command
                IsRunning = true;
                return true;
            }
            return false;
        }

        // TODOC
        protected void onSpeechRecognized (object sender, SpeechRecognizedEventArgs evt) 
        {
            if (evt.Result != null && evt.Result.Grammar == Grammar)
            {
                Console.WriteLine("recognized: " + evt.Result.Text);
                SendKeys.SendWait(evt.Result.Text);
            }
        }

        protected void onSpeechHypothesis (object sender, SpeechHypothesizedEventArgs evt)
        {
            if (evt.Result != null && evt.Result.Grammar == Grammar)
            {
                Console.WriteLine("hypothesis: " + evt.Result.Text);
            }
        }

        // TODOC
        public override bool Terminate(Context context)
        {
            bool finished = base.Terminate(context);
            if (finished)
            {
                Speech.SpeechRecognized += onSpeechRecognized;
                Speech.UnloadGrammar(Grammar);
                Console.WriteLine("dictation ended");
            }
            return finished;
        }
    }

    /// <summary>
    /// Sends cancel sortcut
    /// </summary>
    class Cancel : Command
    {
        // TODOC
        override public bool Execute(Context context)
        {
            if (context.Recognized == Pattern.Cancel)
            {
                Console.WriteLine("send cancel shortcut");
                SendKeys.SendWait("^z");
                return true;
            }
            return false;
        }
    }
}
