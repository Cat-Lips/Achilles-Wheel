using Godot;

namespace GodotSharp.BuildingBlocks
{
    [GlobalClass]
    public partial class PlayerDataFloating : PlayerData
    {
        [Export] private float WalkAcceleration { get; set; } = DefaultWalkAcceleration;
        [Export] private float MaxWalkAcceleration { get; set; } = DefaultMaxWalkAcceleration;

        [Export] private float RunAcceleration { get; set; } = DefaultWalkAcceleration * DefaultRunMultiplier;
        [Export] private float MaxRunAcceleration { get; set; } = DefaultMaxWalkAcceleration * DefaultRunMultiplier;

        [Export] private float Friction { get; set; } = DefaultFriction;

        [Export(PropertyHint.Range, "0,1")] public float MouseSensitivity { get; set; } = DefaultMouseSensitivity;

        private Vector3 currentRotation;
        private float currentAcceleration;

        protected override void UpdateOrientation(Player body, float delta)
        {
            var (x, y) = body.GetOrientation();
            var z = body.GetRotation();

            currentRotation.X += x * MouseSensitivity * delta;
            currentRotation.Y += y * MouseSensitivity * delta;
            currentRotation.Z += z * MouseSensitivity * delta;

            var transform = body.Transform;
            transform.Basis = Basis.Identity;
            body.Transform = transform;

            body.RotateY(currentRotation.X);
            body.RotateX(currentRotation.Y);
            body.RotateZ(currentRotation.Z);
        }

        protected override Vector3 GetVelocity(Player body, float delta)
        {
            var movement = body.GetMovement();

            return movement == Vector3.Zero
                ? ApplyFriction()
                : ApplyAcceleration();

            Vector3 ApplyFriction()
            {
                currentAcceleration = Mathf.MoveToward(currentAcceleration, 0, Friction * delta);
                return body.Velocity.MoveToward(Vector3.Zero, Friction * delta);
            }

            Vector3 ApplyAcceleration()
            {
                var isRunning = body.IsRunning();
                var acceleration = isRunning ? RunAcceleration : WalkAcceleration;
                var maxAcceleration = isRunning ? MaxRunAcceleration : MaxWalkAcceleration;

                currentAcceleration += acceleration;
                currentAcceleration = Math.Clamp(currentAcceleration, 0, maxAcceleration);

                var direction = (body.Transform.Basis * movement).Normalized();
                return currentAcceleration * direction * delta;
            }
        }
    }
}
