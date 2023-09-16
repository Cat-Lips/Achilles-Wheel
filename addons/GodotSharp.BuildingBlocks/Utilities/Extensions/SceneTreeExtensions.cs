using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class SceneTreeExtensions
    {
        public static IEnumerable<T> GetChildren<T>(this Node source) where T : Node
            => source.GetChildren().OfType<T>();

        public static IEnumerable<T> RecurseChildren<T>(this Node source) where T : Node
            => source.SelectRecursive(x => x.GetChildren()).OfType<T>();

        public static void ForEachChild<T>(this Node source, Action<T> action) where T : Node
            => source.GetChildren<T>().ForEach(action);

        public static void ForAllChildren<T>(this Node source, Action<T> action) where T : Node
            => source.RecurseChildren<T>().ForEach(action);

        public static void OwnChild<T>(this Node source, T node) where T : Node
        {
            source.AddChild(node, true);
            node.Owner = source.Owner ?? source;
        }

        public static void RemoveChild(this Node source, Node node, bool free)
        {
            source.RemoveChild(node);
            if (free) node.QueueFree();
        }

        public static void RemoveChildren(this Node source, bool free)
            => source.GetChildren().ForEach(x => source.RemoveChild(x, free));

        public static void RemoveChildren<T>(this Node source, bool free) where T : Node
            => source.GetChildren<T>().ForEach(x => source.RemoveChild(x, free));
    }
}
