
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MsBuildDebugger
{
    public class ProjectAnalyzer
    {
        private ProjectCollection collection;
        private ProjectInstance instance;
        
        public ProjectAnalyzer(string projectFile)
        {
            collection = ProjectCollection.GlobalProjectCollection;
            var project = collection.LoadProject(projectFile);
            instance = BuildManager.DefaultBuildManager.GetProjectInstanceForBuild(project);
            TargetTree = new TargetTree(this);
            TargetTree.SetDefaultTargets(GetDefaultTargets());
        }

        public TargetTree TargetTree { get; }

        public string ProjectFileName()
        {
            return Path.GetFileName(instance.FullPath);
        }

        public ProjectTargetInstance GetTarget(string name)
        {
            return instance.Targets[name];
        }

        public ProjectTargetInstance[] GetTargets(string query)
        {
            var targets = instance.Targets;
            var result = targets.Where(pair =>
            {
                return Regex.IsMatch(pair.Key, query);
            }).Select(item => item.Value).ToArray();
            return result;
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
