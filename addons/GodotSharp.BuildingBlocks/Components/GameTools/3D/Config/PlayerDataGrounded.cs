using Godot;

namespace GodotSharp.BuildingBlocks
{
    [GlobalClass]
    public partial class PlayerDataGrounded : PlayerData
    {
        [Export] private float WalkAcceleration { get; set; } = DefaultWalkAcceleration;
        [Export] private float MaxWalkAcceleration { get; set; } = DefaultMaxWalkAcceleration;

        [Export] private float RunAcceleration { get; set; } = DefaultWalkAcceleration * DefaultRunMultiplier;
        [Export] private float MaxRunAcceleration { get; set; } = DefaultMaxWalkAcceleration * DefaultRunMultiplier;

        [Export] private float Friction { get; set; } = DefaultFriction;
        [Export] private float JumpVelocity { get; set; } = DefaultJumpVelocity;
        [Export] private float SuperJumpVelocity { get; set; } = DefaultJumpVelocity * DefaultRunMultiplier;

        [Export(PropertyHint.Range, "0,1")] public float MouseSensitivity { get; set; } = DefaultMouseSensitivity;

        private Vector3 currentRotation;
        private float currentAcceleration;
        private readonly float gravity = App.DefaultGravity;

        protected override void UpdateOrientation(Player body, float delta)
        {
            var (x, _) = body.GetOrientation();

            currentRotation.X += x * MouseSensitivity * delta;

            var transform = body.Transform;
            transform.Basis = Basis.Identity;
            body.Transform = transform;

            body.RotateY(currentRotation.X);
        }

        protected override Vector3 GetVelocity(Player body, float delta)
        {
            var velocity = body.Velocity;
            var isOnFloor = body.IsOnFloor();
            var isRunning = body.IsRunning();

            AddJump();
            AddGravity();
            AddMovement();

            return velocity;

            void AddJump()
            {
                if (isOnFloor && body.IsJumping())
                    velocity.Y = isRunning ? SuperJumpVelocity : JumpVelocity;
            }

            void AddGravity()
            {
                if (!isOnFloor)
                    velocity.Y -= gravity * delta;
            }

            void AddMovement()
            {
                var movement = body.GetMovement();

                if (movement == Vector3.Zero)
                    ApplyFriction();
                else
                    ApplyAcceleration();

                void ApplyFriction()
                {
                    currentAcceleration = Mathf.MoveToward(currentAcceleration, 0, Friction * delta);
                    velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * delta);
                    velocity.Z = Mathf.MoveToward(velocity.Z, 0, Friction * delta);
                }

                void ApplyAcceleration()
                {
                    var acceleration = isRunning ? RunAcceleration : WalkAcceleration;
                    var maxAcceleration = isRunning ? MaxRunAcceleration : MaxWalkAcceleration;

                    currentAcceleration += acceleration;
                    currentAcceleration = Math.Clamp(currentAcceleration, 0, maxAcceleration);

                    var direction = (body.Transform.Basis * movement).Normalized();
                    velocity.X = currentAcceleration * direction.X * delta;
                    velocity.Z = currentAcceleration * direction.Z * delta;
                }
            }
        }
    }
}
