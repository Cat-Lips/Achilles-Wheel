using Godot;

namespace GodotSharp.BuildingBlocks.Terrain1
{
    [Tool, SceneTree]
    public partial class Terrain : Node
    {
        private TerrainData _config;

        [Export] public Camera3D Camera { get; set; }
        [Export] public TerrainData Config { get => _config; set => this.OnSet(ref _config, value, onSet: ClearChunks, onChanged: () => redraw = true); }

        [GodotOverride]
        private void OnProcess(double _)
        {
            if (Config is null) return;
            if (RedrawRequired(out var center, out var refresh))
                UpdateTerrainChunks(center, refresh);
        }

        private bool redraw;
        private Vector2I lastCenter;
        private bool RedrawRequired(out Vector2I center, out bool refresh)
        {
            refresh = redraw;
            redraw = false;

            if (Camera is null)
            {
                center = lastCenter;
                return refresh;
            }

            center = GetCell(CameraXZ());
            if (refresh || lastCenter != center)
            {
                lastCenter = center;
                return true;
            }

            return false;

            Vector2 CameraXZ()
                => new(Camera.Position.X, Camera.Position.Z);

            Vector2I GetCell(in Vector2 xz)
            {
                var chunkSize = Config.ChunkSize * Config.ChunkScale;
                var chunkOffset = chunkSize * .5f * xz.Sign();
                return (Vector2I)((xz + chunkOffset) / chunkSize);
            }
        }

        private readonly Dictionary<Vector2I, Node> chunks = [];
        private void UpdateTerrainChunks(in Vector2I center, bool refresh)
        {
            var chunksToRemove = new HashSet<Vector2I>(chunks.Keys);
            foreach (var (cell, ring) in center.Spiral(Config.ChunkRadius))
            {
                var lod = ring is 0 ? 0 : ring - 1;
                if (chunks.TryGetValue(cell, out var chunk))
                {
                    if (refresh) Config.Redraw(chunk, cell, lod);
                    else Config.SetLod(chunk, lod);
                    chunksToRemove.Remove(cell);
                    continue;
                }

                AddChunk(cell, lod);
            }

            chunksToRemove.ForEach(RemoveChunk);

            void AddChunk(in Vector2I cell, int lod)
            {
                var chunk = Config.CreateChunk(cell, lod);
                chunks.Add(cell, chunk);
                AddChild(chunk);
            }

            void RemoveChunk(Vector2I cell)
            {
                chunks.Remove(cell, out var chunk);
                this.RemoveChild(chunk, free: true);
            }
        }

        private void ClearChunks()
        {
            chunks.Clear();
            this.RemoveChildren(free: true);
        }

        public override partial void _Process(double _);
    }
}
