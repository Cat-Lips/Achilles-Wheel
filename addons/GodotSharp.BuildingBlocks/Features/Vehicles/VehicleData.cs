using Godot;

namespace GodotSharp.BuildingBlocks
{
    public enum WheelAction
    {
        All,
        Back,
        Front,
    }

    public partial class VehicleData : CustomResource
    {
        #region Defaults

        private const float DefaultAccel = 10;
        private const float DefaultDecel = 20;
        private const float DefaultTurbo = 30;
        private const float DefaultBrake = 40;
        private const float DefaultBounce = 10;
        private const float DefaultReverse = 10;
        private const float DefaultSteerSpeed = 5;

        private const float DefaultMaxAccel = 1000;
        private const float DefaultMaxTurbo = 2000;
        private const float DefaultMaxSteer = .4f;
        private const float DefaultMaxReverse = 100;

        private const WheelAction DefaultDriveType = WheelAction.Back;
        private const WheelAction DefaultSteerType = WheelAction.Front;

        private const float DefaultWheelRollInfluence = 0;  //
        private const float DefaultWheelRestLength = 0;     //0.1
        private const float DefaultWheelFrictionSlip = 0;   //
        private const float DefaultSuspensionTravel = 0;    //0.2
        private const float DefaultSuspensionStiffness = 0; //5
        private const float DefaultSuspensionMaxForce = 0;  //
        private const float DefaultDampingCompression = 0;  //0.3
        private const float DefaultDampingRelaxation = 0;   //0.5

        #endregion

        #region Private

        private WheelAction _driveType = DefaultDriveType;
        private WheelAction _steerType = DefaultSteerType;

        private float _WheelRollInfluence = DefaultWheelRollInfluence;
        private float _WheelRestLength = DefaultWheelRestLength;
        private float _WheelFrictionSlip = DefaultWheelFrictionSlip;
        private float _SuspensionTravel = DefaultSuspensionTravel;
        private float _SuspensionStiffness = DefaultSuspensionStiffness;
        private float _SuspensionMaxForce = DefaultSuspensionMaxForce;
        private float _DampingCompression = DefaultDampingCompression;
        private float _DampingRelaxation = DefaultDampingRelaxation;

        #endregion

        #region Export

        [ExportGroup("Physics")]
        [Export] public float Accel { get; set; } = DefaultAccel;
        [Export] public float Decel { get; set; } = DefaultDecel;
        [Export] public float Turbo { get; set; } = DefaultTurbo;
        [Export] public float Brake { get; set; } = DefaultBrake;
        [Export] public float Bounce { get; set; } = DefaultBounce;
        [Export] public float Reverse { get; set; } = DefaultReverse;
        [Export] public float SteerSpeed { get; set; } = DefaultSteerSpeed;

        [ExportSubgroup("MaxValues")]
        [Export] public float MaxAccel { get; set; } = DefaultMaxAccel;
        [Export] public float MaxTurbo { get; set; } = DefaultMaxTurbo;
        [Export] public float MaxSteer { get; set; } = DefaultMaxSteer;
        [Export] public float MaxReverse { get; set; } = DefaultMaxReverse;

        [ExportGroup("Features")]
        [Export] public WheelAction DriveType { get => _driveType; set => this.Set(ref _driveType, value); }
        [Export] public WheelAction SteerType { get => _steerType; set => this.Set(ref _steerType, value); }

        [ExportGroup("WheelMechanics")]
        [Export] public float WheelRollInfluence { get => _WheelRollInfluence; set => this.Set(ref _WheelRollInfluence, value); }
        [Export] public float WheelRestLength { get => _WheelRestLength; set => this.Set(ref _WheelRestLength, value); }
        [Export] public float WheelFrictionSlip { get => _WheelFrictionSlip; set => this.Set(ref _WheelFrictionSlip, value); }
        [Export] public float SuspensionTravel { get => _SuspensionTravel; set => this.Set(ref _SuspensionTravel, value); }
        [Export] public float SuspensionStiffness { get => _SuspensionStiffness; set => this.Set(ref _SuspensionStiffness, value); }
        [Export] public float SuspensionMaxForce { get => _SuspensionMaxForce; set => this.Set(ref _SuspensionMaxForce, value); }
        [Export] public float DampingCompression { get => _DampingCompression; set => this.Set(ref _DampingCompression, value); }
        [Export] public float DampingRelaxation { get => _DampingRelaxation; set => this.Set(ref _DampingRelaxation, value); }

        #endregion
    }
}
