﻿
using System;
using System.Collections.Generic;
using System.Linq;

namespace MsBuildDebugger
{
    public class Debugger
    {
        private ConsoleUI ui;
        private readonly HashSet<BreakpointLocation> breakpoints = new HashSet<BreakpointLocation>();

        public Debugger(string projectFile, IEnumerable<string> targets)
        {
            Analyzer = new ProjectAnalyzer(projectFile);
            var breakTargets = targets;
            if (!breakTargets.Any())
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
            breakpoints.Add(new BreakpointLocation(target, pos));
        }

        public void RemoveAllBreakpoints()
        {
            breakpoints.Clear();
        }

        public void OnTargetEnter(string name)
        {
            ui.OnTargetEnter(name);
            var loc = new BreakpointLocation(name, BreakpointPosition.Start);
            if (breakpoints.Contains(loc))
            {
                ui.OnHitBreakpoint(loc);
            }
        }

        public void OnTargetLeave(string name)
        {
            var loc = new BreakpointLocation(name, BreakpointPosition.End);
            if (breakpoints.Contains(loc))
            {
                ui.OnHitBreakpoint(loc);
            }
            ui.OnTargetLeave(name);
        }
    }
}
