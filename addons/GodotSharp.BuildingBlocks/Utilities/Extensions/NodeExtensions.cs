using System.Diagnostics;
using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class NodeExtensions
    {
        public static Aabb GetAabb(this Node3D node)
        {
            Debug.Assert(node.IsInsideTree());
            return node.RecurseChildren<VisualInstance3D>()
                .Select(x => x.GlobalTransform * x.GetAabb())
                .Aggregate((a, b) => a.Merge(b));
        }
    }
}
