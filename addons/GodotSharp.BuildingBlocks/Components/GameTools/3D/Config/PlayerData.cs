using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class PlayerData : CustomResource
    {
        protected const float DefaultWalkAcceleration = 15;
        protected const float DefaultMaxWalkAcceleration = 300;

        protected const float DefaultRunMultiplier = 1.5f;

        protected const float DefaultFriction = 200;
        protected const float DefaultJumpVelocity = 5;

        protected const float DefaultMouseSensitivity = .5f;
        protected const float DefaultMinVerticalAngle = -40; //-10 //-40
        protected const float DefaultMaxVerticalAngle = 60; //30 //60

        internal void OnPhysicsProcess(Player player, float delta)
        {
            if (!player.Active) return;
            UpdateOrientation(player, delta);
            player.Velocity = GetVelocity(player, delta);
            player.MoveAndSlide();
        }

        protected virtual void UpdateOrientation(Player player, float delta) { }
        protected virtual Vector3 GetVelocity(Player player, float delta) => default;
    }
}
