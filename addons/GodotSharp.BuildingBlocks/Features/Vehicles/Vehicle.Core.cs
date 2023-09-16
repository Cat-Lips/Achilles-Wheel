using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class Vehicle
    {
        private float curSteer;
        private float curDrive;
        private float curBrake;

        [GodotOverride]
        private void OnPhysicsProcess(double _delta)
        {
            var delta = (float)_delta;
            var isOnFloor = IsOnFloor();

            Help();
            Steer();
            Drive();
            Bounce();

            void Help()
            {
                if (isOnFloor && Input.Help())
                {
                    var transform = Transform;
                    transform.Basis = Basis.Identity;
                    Transform = transform;
                }
            }

            void Bounce()
            {
                if (isOnFloor && Input.Bounce())
                    SetAxisVelocity(Vector3.Up * Config.Bounce * Mass);
            }

            void Steer()
            {
                UpdateSteer(Input.GetSteer());
                SteerWheels.ForEach(wheel => wheel.Steering = curSteer);

                void UpdateSteer(float input)
                {
                    var steerTarget = input * Config.MaxSteer;
                    curSteer = Mathf.Lerp(curSteer, steerTarget, Config.SteerSpeed * delta);
                }
            }

            void Drive()
            {
                UpdateDrive(Input.GetDrive());
                DriveWheels.ForEach(wheel =>
                {
                    wheel.Brake = curBrake;
                    wheel.EngineForce = curDrive;
                });

                void UpdateDrive(float input)
                {
                    if (input is 0) Decelerate();
                    else AccelerateBrakeReverse();

                    void Decelerate()
                    {
                        curBrake = 0;
                        curDrive = Mathf.MoveToward(curDrive, 0, Config.Decel * delta);
                    }

                    void AccelerateBrakeReverse()
                    {
                        var turbo = Input.Turbo();
                        var min = -Config.MaxReverse;
                        var max = turbo ? Config.MaxTurbo : Config.MaxAccel;

                        if (curDrive >= 0 && input > 0) { Accelerate(); return; }
                        if (curDrive >= 0 && input < 0) { Brake(); return; }
                        if (curDrive <= 0 && input < 0) { Reverse(); return; }
                        if (curDrive <= 0 && input > 0) { Brake(); return; }

                        void Accelerate()
                        {
                            var acceleration = input * (turbo ? Config.Turbo : Config.Accel);

                            curDrive += acceleration;
                            curDrive = Math.Clamp(curDrive, min, max);

                        }

                        void Reverse()
                        {
                            curDrive -= input * Config.Reverse;
                            curDrive = Math.Clamp(curDrive, min, max);
                        }

                        void Brake()
                        {
                            curBrake = input * Config.Brake;
                            curDrive = Mathf.MoveToward(curDrive, 0, Config.Brake * delta);
                        }
                    }
                }
            }
        }

        public override partial void _PhysicsProcess(double _delta);
    }
}
