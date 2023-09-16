using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class Camera : Camera3D
    {
        private const string Active = nameof(IActive.Active);

        private Node3D _target = null;
        private bool _selectMode = true;

        private float _nearClip = 10;
        private float sqrNearClip = 100;

        [Export] public bool SelectMode { get => _selectMode; set => this.Set(ref _selectMode, value, OnSelectModeChanged); }

        [ExportGroup("Camera")]
        [Export] public float Speed { get; set; } = 3;

        [ExportGroup("Mouse")]
        [Export(PropertyHint.Range, "0,1,exp")] public float MouseSensitvity { get; set; } = .005f;
        [Export(PropertyHint.Range, "-45,0")] public float MinVerticalAngle { get; set; } = -10;
        [Export(PropertyHint.Range, "0,45")] public float MaxVerticalAngle { get; set; } = 30;

        [ExportGroup("Targeting")]
        [Export] public Node3D Target { get => _target; set => this.Set(ref _target, value, OnTargetChanged); }
        [Export] public Vector3 Offset { get; set; } = new Vector3(0, 3, 5);

        [ExportGroup("Tracking")]
        [Export] public float NearClip { get => _nearClip; set => this.Set(ref _nearClip, Math.Max(value, 0), OnNearClipChanged); }

        public event Action SelectModeChanged;
        public event Action<CollisionObject3D> ItemSelected;

        private void OnTargetChanged(Node3D oldTarget, Node3D newTarget)
        {
            oldTarget?.Set(Active, false);
            newTarget?.Set(Active, true);

            if (Target is not null)
                SelectMode = false;
        }

        private void OnSelectModeChanged()
        {
            if (SelectMode)
            {
                Target?.Set(Active, false);
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                Target?.Set(Active, true);
                GetViewport()?.GuiReleaseFocus();
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }

            SelectModeChanged?.Invoke();
        }

        private void OnNearClipChanged()
            => sqrNearClip = NearClip * NearClip;

        [GodotOverride]
        private void OnProcess(double _)
            => this.OnTrackingUpdate(sqrNearClip);

        [GodotOverride]
        private void OnPhysicsProcess(double delta)
        {
            if (!SelectMode)
            {
                if (Target is null)
                    this.OnFreeLookUpdate(Speed, (float)delta);
                else if (Target is RigidBody3D body)
                    this.OnChaseCamUpdate(body, Offset, Speed, (float)delta);
                else
                    this.OnFollowCamUpdate(Target, Offset, Speed, (float)delta);
            }
            else
            {
                if (Input.IsActionJustPressed(MyInput.Select))
                    this.OnSelect(ItemSelected);
            }
        }

        [GodotOverride]
        private void OnUnhandledInput(InputEvent e)
        {
            this.Handle(e, MyInput.Cancel, OnCancelToggled);

            if (!SelectMode)
            {
                if (Target is null)
                    this.OnFreeLookUpdate(e, MouseSensitvity, MinVerticalAngle, MaxVerticalAngle);
                else if (Target is RigidBody3D body)
                    this.OnChaseCamUpdate(e, body, MouseSensitvity, MinVerticalAngle, MaxVerticalAngle);
                else
                    this.OnFollowCamUpdate(e, Target, MouseSensitvity, MinVerticalAngle, MaxVerticalAngle);
            }

            void OnCancelToggled()
            {
                Target = null;
                SelectMode = !SelectMode;
            }
        }

        public override partial void _Process(double _);
        public override partial void _PhysicsProcess(double delta);
        public override partial void _UnhandledInput(InputEvent e);
    }
}
