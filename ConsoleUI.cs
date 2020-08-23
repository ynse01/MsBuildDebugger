using System;

namespace MsBuildDebugger
{
    public class ConsoleUI
    {
        private Debugger debugger;

        public ConsoleUI(Debugger debugger)
        {
            this.debugger = debugger;
        }

        public void OnTargetEnter(string name)
        {
            //Console.WriteLine("Entered target: " + name);
            return;
        }

        public void OnTargetLeave(string name)
        {
            //Console.WriteLine("Left target: " + name);
        }

        public void OnHitBreakpoint(string target, BreakpointPosition pos)
        {
            Console.WriteLine("Hit breakpoint at {0} of target {1}", Enum.GetName(typeof(BreakpointPosition), pos), target);
            var key = Console.ReadKey();
            while (!ValidInput(key))
            {
                ClearLine();
                Console.WriteLine("Invalid input: " + key.KeyChar);
                key = Console.ReadKey();
            }
            Console.CursorLeft = 0;
        }

        private static void ClearLine()
        {
            var oversizedWindow = Console.WindowWidth >= Console.BufferWidth;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - (oversizedWindow ? 1 : 0));
        }

        private static bool ValidInput(ConsoleKeyInfo key)
        {
            return key.Key == ConsoleKey.F5;
        }
    }
}
