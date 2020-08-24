
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MsBuildDebugger
{
    public class ProjectAnalyzer
    {
        private ProjectCollection collection;
        private ProjectInstance instance;
        private TargetTree tree;

        public ProjectAnalyzer(string projectFile)
        {
            collection = ProjectCollection.GlobalProjectCollection;
            var project = collection.LoadProject(projectFile);
            instance = BuildManager.DefaultBuildManager.GetProjectInstanceForBuild(project);
            tree = new TargetTree(this);
            tree.SetDefaultTargets(GetDefaultTargets());
        }

        public TargetTree TargetTree => tree;

        public ProjectTargetInstance GetTarget(string name)
        {
            return instance.Targets[name];
        }

        public string[] GetDefaultTargets()
        {
            return instance.DefaultTargets.ToArray();
        }

        public ProjectPropertyInstance[] GetProperties(string query)
        {
            var props = instance.Properties;
            var result = props.Where(prop =>
            {
                return Regex.IsMatch(prop.Name, query);
            }).ToArray();
            return result;
        }

        public string GetPropertyValue(string name)
        {
            return instance.GetPropertyValue(name);
        }

        public ProjectItemInstance[] GetItems(string query)
        {
            var items = instance.Items;
            var result = items.Where(item =>
            {
                return Regex.IsMatch(item.ItemType, query);
            }).ToArray();
            return result;
        }

        public ProjectItemInstance GetItem(string itemType)
        {
            foreach(var item in instance.Items) {
                if (itemType.Equals(item.ItemType, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        public ProjectTargetInstance[] GetStackTrace(string startTarget)
        {
            var trace = new List<ProjectTargetInstance>();
            trace.Add(GetTarget(startTarget));
            return trace.ToArray();
        }
    }
}
