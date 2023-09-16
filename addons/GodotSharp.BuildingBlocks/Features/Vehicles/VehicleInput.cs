using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class VehicleInput : CustomResource
    {
        protected internal virtual bool Help()
            => Input.IsActionJustPressed(MyInput.Help);

        protected internal virtual bool Drift()
            => Input.IsActionPressed(MyInput.Action);

        protected internal virtual bool Turbo()
            => Input.IsActionPressed(MyInput.Boost);

        protected internal virtual bool Bounce()
            => Input.IsActionJustPressed(MyInput.Bounce);

        protected internal virtual float GetDrive()
            => Input.GetAxis(MyInput.Back, MyInput.Forward);

        protected internal virtual float GetSteer()
            => Input.GetAxis(MyInput.Left, MyInput.Right);
    }
}
