using System;

namespace MsBuildDebugger
{
    public class ConsoleUI
    {

        public void OnTargetEnter(string name)
        {
            Console.WriteLine("Entered target: " + name);
            return;
            var key = Console.ReadKey();
            while (KeepBlocked(key))
            {
                ClearLine();
                Console.WriteLine("Invalid input: " + key.KeyChar);
                key = Console.ReadKey();
            }
            Console.CursorLeft = 0;
        }

        public void OnTargetLeave(string name)
        {
            Console.WriteLine("Left target: " + name);
        }

        private static void ClearLine()
        {
            var oversizedWindow = Console.WindowWidth >= Console.BufferWidth;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - (oversizedWindow ? 1 : 0));
        }

        private static bool KeepBlocked(ConsoleKeyInfo key)
        {
            return key.Key != ConsoleKey.F5;
        }
    }
}
