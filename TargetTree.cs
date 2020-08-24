
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;

namespace MsBuildDebugger
{
    public interface ITargetTreeItem
    {
        string TargetName { get; }

        ITargetTreeItem Parent { get; }

        IEnumerable<ITargetTreeItem> BeforeTargets { get; }
        IEnumerable<ITargetTreeItem> AfterTargets { get; }
        IEnumerable<ITargetTreeItem> DependsOnTargets { get; }

    }

    public class TargetTree
    {
        private class TargetTreeItem : ITargetTreeItem
        {
            public string TargetName { get; }
            public ITargetTreeItem Parent { get; }

            private List<TargetTreeItem> beforeTargets = new List<TargetTreeItem>();
            private List<TargetTreeItem> afterTargets = new List<TargetTreeItem>();
            private List<TargetTreeItem> dependsOnTargets = new List<TargetTreeItem>();

            public static TargetTreeItem Root => new TargetTreeItem();

            private TargetTreeItem()
            {
                TargetName = "Root";
                Parent = null;
            }

            public TargetTreeItem(string targetName, TargetTreeItem parent)
            {
                TargetName = targetName;
                Parent = parent;
            }

            public IEnumerable<ITargetTreeItem> BeforeTargets => beforeTargets;
            public IEnumerable<ITargetTreeItem> AfterTargets => afterTargets;
            public IEnumerable<ITargetTreeItem> DependsOnTargets => dependsOnTargets;

            public void AddBeforeTarget(TargetTreeItem before)
            {
                beforeTargets.Add(before);
            }

            public void AddAfterTarget(TargetTreeItem after)
            {
                afterTargets.Add(after);
            }

            public void AddDependsOnTarget(TargetTreeItem dep)
            {
                dependsOnTargets.Add(dep);
            }
        }

        private TargetTreeItem rootTreeItem;

        private ProjectAnalyzer analyzer;

        public TargetTree(ProjectAnalyzer analyzer)
        {
            this.analyzer = analyzer;
        }

        public ITargetTreeItem Root => rootTreeItem;

        public void SetDefaultTargets(string[] targets)
        {
            if (rootTreeItem != null)
            {
                throw new InvalidOperationException("Can only set targets once.");
            }
            rootTreeItem = TargetTreeItem.Root;
            foreach(var name in targets)
            {
                ConnectDependant(name, rootTreeItem);
            }
            // TODO: Search for Before and After targets.
        }

        public ITargetTreeItem GetItem(string name)
        {

            return null;
        }

        private void ConnectDependant(string name, TargetTreeItem parent)
        {
            var item = GetItem(name) as TargetTreeItem;
            if (item == null)
            {
                item = new TargetTreeItem(name, rootTreeItem);
                parent.AddDependsOnTarget(item);
                var target = analyzer.GetTarget(name);
                if (target != null)
                {
                    foreach (var dep in Helper.SplitValue(target.DependsOnTargets))
                    {
                        if (dep.StartsWith("$("))
                        {
                            var propName = dep.Trim('$', '(', ')');
                            var deps = Helper.SplitValue(analyzer.GetPropertyValue(propName));
                            foreach(var val in deps)
                            {
                                ConnectDependant(val, item);
                            }
                        }
                        else
                        {
                            ConnectDependant(dep, item);
                        }
                    }
                }
            }
        }
    }
}
