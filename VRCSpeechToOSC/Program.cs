using System.Threading;

namespace VRCSpeechToOSC
{
    internal class Program
    {
        static SpeechRecognizerService? _speechRecognizer;

        static void Main(string[] args)
        {
            Config.LoadFromYaml();
            using SpeechRecognizerService speechRecognizer = new();
            _speechRecognizer = speechRecognizer;
            speechRecognizer.Start();

            using var exitEvent = new ManualResetEventSlim(false);

            StartKeyListener();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                exitEvent.Set();
            };
            exitEvent.Wait();
        }

        static Thread StartKeyListener()
        {
            var keyListener = new Thread(() =>
            {
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.R)
                    {
                        Console.WriteLine("[config] reloading...");
                        Config.LoadFromYaml();
                        _speechRecognizer?.BuildGrammarFromConfig();
                        Console.WriteLine("[config] reloaded!");
                    }
                }
            })
            {
                IsBackground = true
            };
            keyListener.Start();

            return keyListener;
        }
    }
}
