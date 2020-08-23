using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;

namespace MsBuildDebugger
{
    public class ItemTypeComparer : IComparer<ProjectItemInstance>
    {
        private static readonly IComparer<string> comparer = StringComparer.Ordinal;
        public int Compare(ProjectItemInstance x, ProjectItemInstance y)
        {
            return comparer.Compare(x.ItemType, y.ItemType);
        }
    }
}
