
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;

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
        }

        public ProjectTargetInstance GetTarget(string name)
        {
            return instance.Targets[name];
        }

        public string GetPropertyValue(string name)
        {
            return instance.GetPropertyValue(name);
        }

        public ProjectItemInstance GetItems(string itemType)
        {
            foreach(var item in instance.Items) {
                if (itemType.Equals(item.ItemType, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }
    }
}
