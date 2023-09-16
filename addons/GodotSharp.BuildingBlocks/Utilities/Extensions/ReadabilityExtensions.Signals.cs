using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static partial class WheelExtensions
    {
        public static bool IsConnected(this GodotObject source, StringName signal, Action action) => source.IsConnected(signal, Callable.From(action));
        public static bool IsConnected<T>(this GodotObject source, StringName signal, Action<T> action) => source.IsConnected(signal, Callable.From(action));
        public static bool IsConnected<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) => source.IsConnected(signal, Callable.From(action));

        public static void Connect(this GodotObject source, StringName signal, Action action) { var callable = Callable.From(action); if (!source.IsConnected(signal, callable)) source.Connect(signal, callable); }
        public static void Connect<T>(this GodotObject source, StringName signal, Action<T> action) { var callable = Callable.From(action); if (!source.IsConnected(signal, callable)) source.Connect(signal, callable); }
        public static void Connect<T1, T2>(this GodotObject source, StringName signal, Action<T1, T2> action) { var callable = Callable.From(action); if (!source.IsConnected(signal, callable)) source.Connect(signal, callable); }
    }
}
