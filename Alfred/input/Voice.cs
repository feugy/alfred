using Alfred.Core;
using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Threading;

namespace Alfred.Input
{
    /// <summary>
    /// Specific event arguments used when speech command was recognized
    /// </summary>
    public class PatternRecognizedEventArgs : EventArgs
    {
        /// <summary>
        /// Recognized pattern
        /// </summary>
        public Pattern Recognized;

        /// <summary>
        /// Builds argument from a given pattern
        /// </summary>
        /// <param name="recognized">Recognized pattern</param>
        public PatternRecognizedEventArgs(Pattern recognized)
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
    class Voice : InputMethod
    {

        /// <summary>
        /// Speech recognition engine
        /// </summary>
        public SpeechRecognitionEngine Speech;

        /// <summary>
        /// Loads grammar and init recognition engine
        /// </summary>
        public Voice()
        {
            // creates speech engine 
            Speech = new SpeechRecognitionEngine();
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
            var grammar = new Grammar(direct);
            grammar.Name = "direct";
            Speech.LoadGrammar(grammar);
            // then prefixed ones
            var prefixed = new GrammarBuilder(Pattern.Prefix.Value);
            prefixed.Append(new SemanticResultKey("command", commands));
            grammar = new Grammar(prefixed);
            grammar.Name = "prefixed";
            Speech.LoadGrammar(grammar);

            // starts recognition
            Speech.SpeechRecognized += OnSpeech;
            Speech.SetInputToDefaultAudioDevice();
            Speech.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Invoked when Speech recognize a known command
        /// </summary>
        /// <param name="sender">Speech recognition engine publishing this event</param>
        /// <param name="evt">Recognized speech details</param>
        protected void OnSpeech(object sender, SpeechRecognizedEventArgs evt)
        {
            if (evt.Result != null && (evt.Result.Grammar.Name == "prefixed" || evt.Result.Grammar.Name == "direct") &&
                evt.Result.Semantics != null)
            {
                var pattern = Pattern.ValueOf(evt.Result.Semantics["command"].Value.ToString());
                TriggerChange(new PatternRecognizedEventArgs(pattern));
            }
        }

        /// <summary>
        /// Dispose speech recognition engine
        /// </summary>
        override public void Dispose() 
        {
            Speech.Dispose();
        }
    }
}
