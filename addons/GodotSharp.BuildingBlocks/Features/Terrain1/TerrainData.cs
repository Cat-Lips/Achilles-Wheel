using Godot;

namespace GodotSharp.BuildingBlocks.Terrain1
{
    [Tool, GlobalClass]
    public abstract partial class TerrainData : CustomResource
    {
        private const int MinChunkSize = 1;
        private const int MinChunkScale = 0;
        private const int MinChunkRadius = 0;

        private int _chunkSize = 16;
        private int _chunkRadius = 2;
        private float _chunkScale = 1;

        [ExportGroup("Chunks")]
        [ExportCategory("Chunks")]
        [Export] public virtual int ChunkSize { get => _chunkSize; set => this.Set(ref _chunkSize, value < MinChunkSize ? MinChunkSize : value); }
        [Export] public virtual float ChunkScale { get => _chunkScale; set => this.Set(ref _chunkScale, value < MinChunkScale ? MinChunkScale : value); }
        [Export] public virtual int ChunkRadius { get => _chunkRadius; set => this.Set(ref _chunkRadius, value < MinChunkRadius ? MinChunkRadius : value); }

        public abstract Node CreateChunk(in Vector2I cell, int lod);
        public abstract void Redraw(Node chunk, in Vector2I cell, int lod);
        public abstract void SetLod(Node chunk, int lod);
    }
}
