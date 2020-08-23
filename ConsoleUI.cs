using System;
using System.Collections.Generic;

namespace MsBuildDebugger
{
    public class ConsoleUI
    {
        private Debugger debugger;
        private readonly Dictionary<ConsoleKey, Func<bool>> commands;

        public ConsoleUI(Debugger debugger)
        {
            this.debugger = debugger;
            commands = new Dictionary<ConsoleKey, Func<bool>>();
            commands.Add(ConsoleKey.H, PrintUsage);
            commands.Add(ConsoleKey.P, PrintProperties);
            commands.Add(ConsoleKey.F5, Continue);
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
            ClearLine();
            while (!ExecuteCommand(key))
            {
                key = Console.ReadKey();
                ClearLine();
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

        private bool ValidInput(ConsoleKeyInfo key)
        {
            return commands.ContainsKey(key.Key);
        }

        private static bool PrintUsage()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  H: Print this help message");
            Console.WriteLine("  P: Print all Properties");
            Console.WriteLine(" F5: Continue");
            return false;
        }

        private static bool Continue()
        {
            return true;
        }

        private bool PrintProperties()
        {
            Console.WriteLine("Current defined properties:");
            var properties = debugger.Analyzer.GetProperties();
            Array.Sort(properties, new PropertyNameComparer());
            foreach(var prop in properties)
            {
                Console.WriteLine("  $({0}) = {1}", prop.Name, prop.EvaluatedValue);
            }
            return false;
        }

        private bool ExecuteCommand(ConsoleKeyInfo key)
        {
            if (commands.TryGetValue(key.Key, out Func<bool> command))
            {
                return command();
            } else
            {
                Console.WriteLine("Invalid input: " + key.KeyChar);
                return false;
            }
        }
    }
}
