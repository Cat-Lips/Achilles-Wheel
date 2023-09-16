using Godot;

// https://www.youtube.com/watch?v=Hgv9iAdazKg
// https://www.youtube.com/watch?v=jDM0m4WuBAg
// https://www.youtube.com/watch?v=izsMr5Pyk2g
// https://www.youtube.com/watch?v=x7ti3AEhV18

namespace GodotSharp.BuildingBlocks.Terrain2
{
    [Tool, SceneTree]
    public partial class Terrain : Node, ITerrain
    {
        private TerrainData _config;

        [Export] private Camera3D Camera { get; set; }
        [Export] private TerrainData Config { get => _config; set => this.OnSet(ref _config, value, () => Config?.Init(this)); }
        [Export] private PhysicsBody3D[] Colliders { get; set; }

        public float GetHeight(in Vector2 xz) => Config.GetHeight(xz);
        public void AddCollider(PhysicsBody3D source) => Config.AddCollider(source);

        [GodotOverride]
        private void OnReady()
        {
            Config ??= new();
            Colliders.ForEach(Config.AddCollider);
        }

        [GodotOverride]
        private void OnPhysicsProcess(double delta)
        {
            if (Camera is null) return;
            if (!Camera.IsInsideTree()) return;

            TerrainPosition.Set(Mesh.Position = Camera.Position.XZ(0).Snapped(Vector3.One * Config.GetScaledChunkSize()));
        }

        public override partial void _Ready();
        public override partial void _PhysicsProcess(double delta);
    }
}
