using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class InputExtensions
    {
        public static bool IsMouseOver(this Control source)
            => source.GetRect().HasPoint(source.GetLocalMousePosition());

        public static bool IsMouseOver(this Popup source)
            => source.Visible && new Rect2(default, source.Size).HasPoint(source.GetMousePosition());

        public static bool IsKeyPressed(this InputEventKey e, Key key)
            => e.PhysicalKeycode == key && e.Pressed;

        public static bool IsKeyJustPressed(this InputEventKey e, Key key)
            => e.IsKeyPressed(key) && !e.Echo;

        public static bool Handle(this Node source, InputEvent e, StringName action, Action _action)
                => source.On(e.IsActionPressed(action), source.SetInputAsHandled, _action);

        public static bool Handle(this Node source, InputEventKey e, Key key, Action _action)
            => source.On(e.IsKeyJustPressed(key), source.SetInputAsHandled, _action);

        private static bool On(this Node _, bool condition, params Action[] actions)
        {
            if (condition)
            {
                actions.ForEach(x => x());
                return true;
            }

            return false;
        }

        private static void SetInputAsHandled(this Node source)
            => source.GetViewport().SetInputAsHandled();
    }
}
