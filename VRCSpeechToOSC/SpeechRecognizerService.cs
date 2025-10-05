using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using Buildetech.OscCore;
using System.Linq;

namespace VRCSpeechToOSC
{
    public class SpeechRecognizerService : IDisposable
    {
        private readonly SpeechRecognitionEngine _recognizer;
        private bool _isStarted = false;
        private OscClient oscClient = new OscClient(Config.Instance.TargetHost, Config.Instance.TargetPort);

        public SpeechRecognizerService()
        {
            Console.WriteLine("[init] Speech recognizer initializing...");
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.SetInputToDefaultAudioDevice();
            BuildGrammarFromConfig();
            _recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            Console.WriteLine("[init] Speech recognizer initialize completed!");
        }

        public void BuildGrammarFromConfig()
        {
            var choices = new Choices();
            foreach (var group in Config.Instance.CommandGroups)
            {
                var prompts = group.Commands.SelectMany(cmd => cmd.Prompts);
                foreach (var prompt in prompts)
                {
                    choices.Add($"{group.Prefix} {prompt}");
                }
            }
            var gb = new GrammarBuilder();
            gb.Append(choices);
            var grammar = new Grammar(gb);
            _recognizer.UnloadAllGrammars();
            _recognizer.LoadGrammar(grammar);
        }

        public void Start()
        {
            if (!_isStarted)
            {
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                _isStarted = true;
            }
        }

        public void Stop()
        {
            if (_isStarted)
            {
                _recognizer.RecognizeAsyncStop();
                _isStarted = false;
            }
        }

        private void Recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            string text = e.Result.Text.Trim();
            Console.WriteLine($"[voice] {text}");

            var group = Config.Instance.CommandGroups.FirstOrDefault(g => text.StartsWith(g.Prefix));
            if (group == null) return;

            string commandText = text.Substring(group.Prefix.Length).Trim();
            var cmd = group.Commands.FirstOrDefault(c => c.Prompts.Contains(commandText));
            if (cmd == null) return;

            if (cmd.Value is null)
            {
                oscClient.Send(cmd.OscPath);
            }
            else
            {
                switch (cmd.Type)
                {
                    case Config.OSC_TYPE_INT:
                        oscClient.Send(cmd.OscPath, Convert.ToInt32(cmd.Value));
                        break;
                    case Config.OSC_TYPE_BOOL:
                        oscClient.Send(cmd.OscPath, Convert.ToBoolean(cmd.Value));
                        break;
                    case Config.OSC_TYPE_FLOAT:
                        oscClient.Send(cmd.OscPath, Convert.ToDouble(cmd.Value));
                        break;
                    case Config.OSC_TYPE_STRING:
                        oscClient.Send(cmd.OscPath, cmd.Value?.ToString() ?? string.Empty);
                        break;
                    default:
                        // 型が不正な時は送信しない
                        return;
                }
            }

            Console.WriteLine($"[cmd sent] {cmd.OscPath} {cmd.Value}");
        }

        public void Dispose()
        {
            Stop();
            _recognizer.Dispose();
        }
    }
}
