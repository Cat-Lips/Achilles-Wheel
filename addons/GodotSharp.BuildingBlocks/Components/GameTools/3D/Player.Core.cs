using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class Player
    {
        private PlayerDataGrounded _groundedMotion = new();
        private PlayerDataFloating _floatingMotion = new();

        [Export] private PlayerDataGrounded GroundedMotion { get => _groundedMotion; set => this.Set(ref _groundedMotion, value ?? new()); }
        [Export] private PlayerDataFloating FloatingMotion { get => _floatingMotion; set => this.Set(ref _floatingMotion, value ?? new()); }

        protected internal virtual bool IsRunning()
            => Input.IsActionPressed(MyInput.Boost);

        protected internal virtual bool IsJumping()
            => Input.IsActionJustPressed(MyInput.Bounce);

        protected internal virtual Vector3 GetMovement()
        {
            var (x, z) = Input.GetVector(MyInput.Left, MyInput.Right, MyInput.Forward, MyInput.Back);
            var y = Input.GetAxis(MyInput.Down, MyInput.Up);
            return new(x, y, z);
        }

        protected internal virtual float GetRotation()
            => Input.GetAxis(MyInput.RotateLeft, MyInput.RotateRight);

        protected internal virtual Vector2 GetOrientation()
            => -MyInput.MouseMotion?.Relative ?? default;

        [GodotOverride]
        private void OnPhysicsProcess(double delta)
        {
            switch (MotionMode)
            {
                case MotionModeEnum.Grounded:
                    GroundedMotion.OnPhysicsProcess(this, (float)delta);
                    break;
                case MotionModeEnum.Floating:
                    FloatingMotion.OnPhysicsProcess(this, (float)delta);
                    break;
            }
        }

        public override partial void _PhysicsProcess(double delta);
    }
}
