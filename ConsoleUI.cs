
using System;
using System.Collections.Generic;
using System.IO;

namespace MsBuildDebugger
{
    public class ConsoleUI
    {
        private readonly Debugger debugger;
        private readonly Dictionary<ConsoleKey, Func<bool>> commands;
        private string currentTarget;

        public ConsoleUI(Debugger debugger)
        {
            this.debugger = debugger;
            commands = new Dictionary<ConsoleKey, Func<bool>>
            {
                { ConsoleKey.B, SetBreakpoint },
                { ConsoleKey.C, InspectCode },
                { ConsoleKey.F, Finish },
                { ConsoleKey.H, PrintUsage },
                { ConsoleKey.I, PrintItemsInclude },
                { ConsoleKey.M, PrintItemsMetadata },
                { ConsoleKey.P, PrintProperties },
                { ConsoleKey.S, PrintStackTrace },
                { ConsoleKey.T, PrintTargetTree },
                { ConsoleKey.Q, Quit },
                { ConsoleKey.F5, Continue },
                { ConsoleKey.F10, StepOver }
            };
            Console.Title = "Debugging " + debugger.Analyzer.ProjectFileName();
            Console.WriteLine("(Type h for help)");
        }

        public void OnTargetEnter(string name)
        {
            currentTarget = name;
            return;
        }

        public void OnTargetLeave(string name)
        {
            currentTarget = "";
        }

        public void OnHitBreakpoint(BreakpointLocation loc)
        {
            Console.WriteLine("Hit breakpoint at {0} of target {1}", loc.GetPositionString(), loc.Target);
            var projectName = debugger.Analyzer.ProjectFileName();
            Console.Write("{0}> ", projectName);
            var key = Console.ReadKey();
            ClearLine();
            while (!ExecuteCommand(key))
            {
                Console.Write("{0}> ", projectName);
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

        private static bool PrintUsage()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  B : Set a new Breakpoint");
            Console.WriteLine("  C : Inspect executing code");
            Console.WriteLine("  F : Finish executing, without stopping at breakpoints");
            Console.WriteLine("  H : Print this help message");
            Console.WriteLine("  I : Print Item Include");
            Console.WriteLine("  M : Print Item Metadata");
            Console.WriteLine("  P : Print Property");
            Console.WriteLine("  S : Print current StackTrace of Targets");
            Console.WriteLine("  T : Print the overall tree of Targets");
            Console.WriteLine("  Q : Quit immediately");
            Console.WriteLine(" F5 : Continue");
            Console.WriteLine(" F10: Step over");
            return false;
        }

        private bool Finish()
        {
            debugger.RemoveAllBreakpoints();
            return true;
        }

        private bool Quit()
        {
            throw new Exception("User requested to Quit");
        }

        private static bool Continue()
        {
            return true;
        }

        private bool StepOver()
        {
            debugger.SetBreakpoint(currentTarget, BreakpointPosition.End);
            return true;
        }

        private bool SetBreakpoint()
        {
            Console.Write("Set breakpoint at start of Target: ");
            var query = Console.ReadLine();
            var targets = debugger.Analyzer.GetTargets(query);
            ClearLine();
            foreach(var target in targets)
            {
                debugger.SetBreakpoint(target.Name, BreakpointPosition.Start);
                Console.WriteLine("Added breakpoint at start of " + target.Name);
            }
            return false;
        }

        private bool InspectCode()
        {
            var target = debugger.Analyzer.GetTarget(currentTarget);
            var loc = target.Location;
            if (File.Exists(loc.File))
            {
                var lines = File.ReadAllLines(loc.File);
                var startLine = Math.Max(loc.Line - 1, 0);
                var endLine = Math.Min(loc.Line + 20, lines.Length);
                var header = Helper.AlignStringInSpace(loc.File, Console.WindowWidth - 8, HorizontalAlignment.Center);
                Console.WriteLine(" -- {0} --", header);
                Console.WriteLine(new string('=', Console.WindowWidth - 1));
                for(var i = startLine; i < endLine; i++)
                {
                    Console.Write("{0:####}:", i);
                    Console.WriteLine(lines[i]);
                }
            }
            return false;
        }

        private bool PrintProperties()
        {
            Console.Write("Property: ");
            var query = Console.ReadLine();
            var properties = debugger.Analyzer.GetProperties(query);
            Array.Sort(properties, new PropertyNameComparer());
            ClearLine();
            foreach (var prop in properties)
            {
                Console.WriteLine("  $({0}) = {1}", prop.Name, prop.EvaluatedValue);
            }
            return false;
        }

        private bool PrintItemsInclude()
        {
            Console.Write("Items: ");
            var query = Console.ReadLine();
            var items = debugger.Analyzer.GetItems(query);
            Array.Sort(items, new ItemTypeComparer());
            ClearLine();
            foreach (var item in items)
            {
                Console.WriteLine("  @({0}) = {1}", item.ItemType, item.EvaluatedInclude);
            }
            return false;
        }

        private bool PrintItemsMetadata()
        {
            Console.Write("Items: ");
            var query = Console.ReadLine();
            var items = debugger.Analyzer.GetItems(query);
            Array.Sort(items, new ItemTypeComparer());
            ClearLine();
            foreach (var item in items)
            {
                Console.WriteLine("  @({0})", item.ItemType);
                Console.WriteLine("    %(Identity) = {0}", item.EvaluatedInclude); 
                foreach (var meta in item.Metadata)
                {
                    Console.WriteLine("    %({0}) = {1}", meta.Name, meta.EvaluatedValue);
                }
            }
            return false;
        }

        private bool PrintTargetTree()
        {
            int indent = 0;
            var rootItem = debugger.Analyzer.TargetTree.Root;
            PrintTargetTree(rootItem, indent);
            return false;
        }

        private void PrintTargetTree(ITargetTreeItem item, int indent)
        {
            var ind = new string(' ', indent * 2);
            Console.WriteLine("{0}{1}()", ind, item.TargetName);
            var children = item.DependsOnTargets;
            indent++;
            foreach(var child in children)
            {
                if (child != null)
                {
                    PrintTargetTree(child, indent);
                }
            }
        }

        private void PrintTargetTrace(ITargetTreeItem item, int indent)
        {
            var ind = new string(' ', indent * 2);
            Console.WriteLine("{0}{1}()", ind, item.TargetName);
            if (item.Parent != null)
            {
                indent++;
                PrintTargetTrace(item.Parent, indent);
            }
        }

        private bool PrintStackTrace()
        {
            Console.WriteLine("StackTrace:");
            var currentItem = debugger.Analyzer.TargetTree.GetItem(currentTarget);
            if (currentItem != null)
            {
                PrintTargetTrace(currentItem, 0);
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
