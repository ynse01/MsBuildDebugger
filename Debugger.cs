
using System;
using System.Collections.Generic;

namespace MsBuildDebugger
{
    public enum BreakpointPosition
    {
        Start,
        End
    }

    public class Debugger
    {
        private ConsoleUI ui;
        private Dictionary<string, BreakpointPosition> breakpoints = new Dictionary<string, BreakpointPosition>();

        public Debugger(string projectFile, string[] targets)
        {
            Analyzer = new ProjectAnalyzer(projectFile);
            var breakTargets = targets;
            if (breakTargets.Length > 0)
            {
                breakTargets = Analyzer.GetDefaultTargets();
            }
            foreach (var target in breakTargets)
            {
                SetBreakpoint(target, BreakpointPosition.Start);
            }
        }

        public ProjectAnalyzer Analyzer { get; }

        public void SetUI(ConsoleUI ui)
        {
            this.ui = ui;
        }

        public void SetBreakpoint(string target, BreakpointPosition pos)
        {
            Console.WriteLine("Set breakpoint at {0} of {1}", Enum.GetName(typeof(BreakpointPosition), pos), target);
            breakpoints.Add(target, pos);
        }

        public void OnTargetEnter(string name)
        {
            if (breakpoints.TryGetValue(name, out BreakpointPosition pos))
            {
                if (pos == BreakpointPosition.Start)
                {
                    ui.OnHitBreakpoint(name, pos);
                }
            }
            ui.OnTargetEnter(name);
        }

        public void OnTargetLeave(string name)
        {
            ui.OnTargetLeave(name);
        }
    }
}
