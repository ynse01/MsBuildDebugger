using Microsoft.Build.Framework;
using System;

namespace MsBuildDebugger
{
    public class DebugLogger : ILogger
    {
        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }

        private IEventSource source;

        public void Initialize(IEventSource eventSource)
        {
            source = eventSource;
            source.BuildStarted += OnBuildStarted;
            source.BuildFinished += OnBuildFinished;
        }

        public void Shutdown()
        {
            if (source != null)
            {
                source.BuildStarted -= OnBuildStarted;
                source.BuildFinished -= OnBuildFinished;
            }
        }

        private void OnBuildStarted(object sender, BuildStartedEventArgs args)
        {
            Console.WriteLine("Build started on thread: " + args.ThreadId);
            var key = Console.ReadKey();
            while(KeepBlocked(key)) {
                ClearLine();
                Console.WriteLine("Invalid input: " + key.KeyChar);
                key = Console.ReadKey();
            }
            Console.CursorLeft = 0;
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs args)
        {

        }

        public static void ClearLine()
        {
            var oversizedWindow = Console.WindowWidth >= Console.BufferWidth;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - (oversizedWindow ? 1 : 0));
        }

        private bool KeepBlocked(ConsoleKeyInfo key)
        {
            return key.Key != ConsoleKey.F5;
        }
    }
}
