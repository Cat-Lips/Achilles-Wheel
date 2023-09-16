using Godot;

namespace GodotSharp.BuildingBlocks
{
    [InputMap]
    public partial class MyInput : Node
    {
        public static InputEventMouseMotion MouseMotion { get; private set; }

        public override void _UnhandledInput(InputEvent e)
        {
            if (e is InputEventMouseMotion motion)
            {
                MouseMotion = motion;
                ClearEvents.Run();
            }
        }

        private static readonly AutoAction ClearEvents = new(() => MouseMotion = null);
    }
}
