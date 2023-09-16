using Godot;

// https://www.youtube.com/watch?v=usdwhhZWIJ4

namespace GodotSharp.BuildingBlocks.Grass2
{
    [Tool]
    public partial class Grass : MultiMeshInstance3D
    {
        private float _span = 5;
        private int _count = 1000;
        private Vector2 _width = new(.1f, .2f);
        private Vector2 _height = new(.4f, .8f);
        private Vector2I _sway_yaw = new(0, 10);
        private Vector2I _sway_pitch = new(0, 10);

        [Export] public float Span { get => _span; set => this.Set(ref _span, value, Rebuild.Run); }
        [Export] public int Count { get => _count; set => this.Set(ref _count, value, Rebuild.Run); }
        [Export] public Vector2 Width { get => _width; set => this.Set(ref _width, value, Rebuild.Run); }
        [Export] public Vector2 Height { get => _height; set => this.Set(ref _height, value, Rebuild.Run); }
        [Export] public Vector2I SwayYaw { get => _sway_yaw; set => this.Set(ref _sway_yaw, value, Rebuild.Run); }
        [Export] public Vector2I SwayPitch { get => _sway_pitch; set => this.Set(ref _sway_pitch, value, Rebuild.Run); }

        private readonly AutoAction Rebuild = new();

        [GodotOverride]
        private void OnReady()
        {
            RebuildGrass();
            Rebuild.Action = RebuildGrass;

            void RebuildGrass()
            {
                var mesh = Default.Grass(Default.ShaderMaterial<Grass>());
                Multimesh = Default.MultiMesh(mesh, Count, data: true);

                for (var i = 0; i < Count; ++i)
                {
                    var x = (float)GD.RandRange(-Span, Span);
                    var z = (float)GD.RandRange(-Span, Span);
                    var angle = Mathf.DegToRad(GD.RandRange(0, 359));
                    var width = (float)GD.RandRange(Width.X, Width.Y);
                    var height = (float)GD.RandRange(Height.X, Height.Y);
                    var swayYaw = Mathf.DegToRad(GD.RandRange(SwayYaw.X, SwayYaw.Y));
                    var swayPitch = Mathf.DegToRad(GD.RandRange(SwayPitch.X, SwayPitch.Y));

                    var pos = new Vector3(x, 0, z);
                    var rot = new Basis(Vector3.Up, angle);
                    Multimesh.SetInstanceTransform(i, new(rot, pos));
                    Multimesh.SetInstanceCustomData(i, new(width, height, swayYaw, swayPitch));
                }
            }
        }

        public override partial void _Ready();
    }
}
