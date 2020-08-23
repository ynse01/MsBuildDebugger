
using System;
using System.Net.Http.Headers;

namespace MsBuildDebugger
{
    public interface ITargetTreeItem
    {
        string TargetName { get; }

        ITargetTreeItem Parent { get; }
    }

    public class TargetTree
    {
        private class TargetTreeItem : ITargetTreeItem
        {
            public string TargetName { get; }
            public ITargetTreeItem Parent { get; }

            public TargetTreeItem(string name, TargetTreeItem parent)
            {
                TargetName = name;
                Parent = parent;
            }
        }

        public ITargetTreeItem RootTreeItem;

        private ProjectAnalyzer analyzer;

        public TargetTree(ProjectAnalyzer analyzer)
        {
            this.analyzer = analyzer;
        }

        public void SetTargets(string[] targets)
        {
            if (RootTreeItem == null)
            {
                throw new InvalidOperationException("Can only set targets once.");
            }
            RootTreeItem = new TargetTreeItem("Root", null);
        }
    }
}
