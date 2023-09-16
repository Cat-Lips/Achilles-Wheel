using System.Diagnostics;
using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class PhysicsSync : MultiplayerSynchronizer
    {
        // If client requires more control than input, server must validate position
        public event Action ServerValidatePosition;

        [Export] private Vector3 Position { get; set; }
        [Export] private Vector3 Rotation { get; set; }
        [Export] private Vector3 LinearVelocity { get; set; }
        [Export] private Vector3 AngularVelocity { get; set; }

        private bool IsSyncd { get; set; }
        private bool IsUpdated { get; set; }

        public virtual void IntegrateForces(PhysicsDirectBodyState3D state)
        {
            if (IsMultiplayerAuthority())
            {
                Position = state.Transform.Origin;
                Rotation = state.Transform.Basis.GetEuler();
                LinearVelocity = state.LinearVelocity;
                AngularVelocity = state.AngularVelocity;
            }
            else if (IsSyncd)
            {
                LogLag(state);

                state.LinearVelocity = LinearVelocity;
                state.AngularVelocity = AngularVelocity;
                state.Transform = new Transform3D(new Basis(Quaternion.FromEuler(Rotation)), Position);

                IsUpdated = true;
            }
        }

        [Conditional("LogLag")]
        private void LogLag(PhysicsDirectBodyState3D state)
        {
            if (IsUpdated)
            {
                var lag = GetLag();
                if (lag.Any()) GD.Print($"*** LAG *** {GetParent().Name}:{GetMultiplayerAuthority()} [{string.Join(", ", lag)}]");
            }

            IEnumerable<string> GetLag()
            {
                var posLag = (Position - state.Transform.Origin).Length();
                var rotLag = (Rotation - state.Transform.Basis.GetEuler()).Length();
                var lvLag = (LinearVelocity - state.LinearVelocity).Length();
                var avLag = (AngularVelocity - state.AngularVelocity).Length();

                const float threshold = 1;
                if (posLag > threshold) yield return $"Pos: {posLag}";
                if (rotLag > threshold) yield return $"Rot: {rotLag}";
                if (lvLag > threshold) yield return $"LV: {lvLag}";
                if (avLag > threshold) yield return $"AV: {avLag}";
            }
        }

        [GodotOverride]
        private void OnReady()
        {
            Synchronized += () =>
            {
                IsSyncd = true;

                if (this.IsMultiplayerServer())
                    ServerValidatePosition?.Invoke();
            };
        }

        public override partial void _Ready();
    }
}
