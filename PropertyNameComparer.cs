using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;

namespace MsBuildDebugger
{
    public class PropertyNameComparer : IComparer<ProjectPropertyInstance>
    {
        private static readonly IComparer<string> comparer = StringComparer.Ordinal;
        public int Compare(ProjectPropertyInstance x, ProjectPropertyInstance y)
        {
            return comparer.Compare(x.Name, y.Name);
        }
    }
}
