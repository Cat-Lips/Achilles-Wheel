using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class Vehicle
    {
        private VehicleData _config = new();
        private VehicleInput _input = new();

        private List<VehicleWheel3D> DriveWheels { get; } = [];
        private List<VehicleWheel3D> SteerWheels { get; } = [];

        [Export] public VehicleData Config { get => _config; set => this.Set(ref _config, value ?? new(), InitWheels); }
        [Export] public VehicleInput Input { get => _input; set => this.Set(ref _input, value ?? new()); }

        public Vehicle()
        {
            ContactMonitor = true;
            MaxContactsReported = 1;
        }

        private void InitWheels()
        {
            DriveWheels.Clear();
            SteerWheels.Clear();

            foreach (var wheel in this.GetChildren<VehicleWheel3D>())
            {
                wheel.WheelRollInfluence = Config.WheelRollInfluence;
                wheel.WheelRestLength = Config.WheelRestLength;
                wheel.WheelFrictionSlip = Config.WheelFrictionSlip;
                wheel.SuspensionTravel = Config.SuspensionTravel;
                wheel.SuspensionStiffness = Config.SuspensionStiffness;
                wheel.SuspensionMaxForce = Config.SuspensionMaxForce;
                wheel.DampingCompression = Config.DampingCompression;
                wheel.DampingRelaxation = Config.DampingRelaxation;

                if (wheel.UseAsTraction = wheel.Is(Config.DriveType)) DriveWheels.Add(wheel);
                if (wheel.UseAsSteering = wheel.Is(Config.SteerType)) SteerWheels.Add(wheel);
            }
        }

        private bool IsOnFloor()
            => GetCollidingBodies().Count > 0;
    }
}
