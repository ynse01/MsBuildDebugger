
using System;

namespace MsBuildDebugger
{
    public enum BreakpointPosition
    {
        Start,
        End
    }

    public class BreakpointLocation
    {
        public BreakpointPosition Position { get; }
        public string Target { get; }

        public BreakpointLocation(string target, BreakpointPosition pos)
        {
            Target = target;
            Position = pos;
        }

        public string GetPositionString()
        {
            return Enum.GetName(typeof(BreakpointPosition), Position);
        }

        public override int GetHashCode()
        {
            return Target.GetHashCode() * 0x00010000 + (int)Position;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BreakpointLocation);
        }

        public bool Equals(BreakpointLocation loc)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(loc, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, loc))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != loc.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (Target == loc.Target) && (Position == loc.Position);
        }
    }
}
