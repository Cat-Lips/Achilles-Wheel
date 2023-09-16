using Godot;
using GodotSharp.BuildingBlocks.Terrain2.Internal;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    [Tool, GlobalClass]
    public partial class TerrainData : CustomResource
    {
        private Texture2D _heightMap;
        private Texture2D _normalMap;
        private ShaderNoiseLite _noise;
        private float _amplitude = 25;
        private Vector2 _offset;

        private TerrainType[] _regions;
        private bool _enableBlending;
        private bool _useHeightCurve;
        private Curve _heightCurve;

        private int _lodStep = 1;
        private int _chunkSize = 512;
        private int _chunkRadius = 10;
        private float _chunkScale = 1;

        private int _colliderSize = 8;

        [ExportGroup("Terrain")]
        [Export] private Texture2D HeightMap { get => _heightMap; set => this.Set(ref _heightMap, value, notify: true, () => Set(HeightMap), ResetShader.Run, OnHeightMapChanged); }
        [Export] private Texture2D NormalMap { get => _normalMap; set => this.Set(ref _normalMap, value, notify: true, () => Set(NormalMap), ResetShader.Run); }
        [Export] private ShaderNoiseLite Noise { get => _noise; set => this.Set(ref _noise, value, notify: true, InitNoise); }
        [Export] private float Amplitude { get => _amplitude; set => this.Set(ref _amplitude, value, () => Set(Amplitude)); }
        [Export(PropertyHint.Link)] private Vector2 Offset { get => _offset; set => this.Set(ref _offset, value, () => Set(Offset)); }

        [ExportGroup("Regions")]
        [Export] private TerrainType[] Regions { get => _regions; set => this.Set(ref _regions, value ?? TerrainType.Defaults(), ResetRegions.Run); }
        [Export] private bool EnableBlending { get => _enableBlending; set => this.Set(ref _enableBlending, value, ResetShader.Run); }
        [Export] private bool UseHeightCurve { get => _useHeightCurve; set => this.Set(ref _useHeightCurve, value, ResetShader.Run); }
        [Export] private Curve HeightCurve { get => _heightCurve; set => this.Set(ref _heightCurve, value ?? DefaultHeightCurve(), () => Set(HeightCurve)); }

        [ExportSubgroup("Defaults", "Region")]
        [Export] private float RegionTextureScale { get => TerrainType.DefaultTextureScale; set => this.Set(ref TerrainType.DefaultTextureScale, value, () => Regions = null); }
        [Export(PropertyHint.Range, "0,1")] private float RegionGradients { get => Regions.AverageOrDefault(x => x.Gradient); set => Regions.ForEach(x => x.Gradient = value); }
        [Export(PropertyHint.Range, "0,1")] private float RegionTintStrength { get => TerrainType.DefaultTintStrength; set => this.Set(ref TerrainType.DefaultTintStrength, value, () => Regions = null); }
        [Export(PropertyHint.Range, "0,1")] private float RegionBlendStrength { get => TerrainType.DefaultBlendStrength; set => this.Set(ref TerrainType.DefaultBlendStrength, value, () => Regions = null); }
        [Export] private DefaultTerrainTypes RegionDefaultType { get => TerrainType.DefaultTerrainTypes; set => this.Set(ref TerrainType.DefaultTerrainTypes, value, () => Regions = null); }
        [Export] private TintFromTexture RegionTintFromTexture { get => TerrainType.TintFromTexture; set => this.Set(ref TerrainType.TintFromTexture, value, () => Regions = null); }

        [ExportGroup("Chunks")]
        [Export] private int LodStep { get => _lodStep; set => this.Set(ref _lodStep, Math.Max(value, 0), () => Set(LodStep), ResetChunks.Run); }
        [ExportGroup("Chunks", "Chunk")]
        [Export] private int ChunkSize { get => _chunkSize; set => this.Set(ref _chunkSize, Utils.NextPo2(value, _chunkSize), () => Set(ChunkSize), ResetChunks.Run); }
        [Export] private int ChunkRadius { get => _chunkRadius; set => this.Set(ref _chunkRadius, Math.Max(value, 0)/*, () => Set(ChunkRadius)*/, ResetChunks.Run); }
        [Export] private float ChunkScale { get => _chunkScale; set => this.Set(ref _chunkScale, Math.Max(value, 0), () => Set(ChunkScale), ResetChunks.Run); }

        [ExportGroup("Colliders")]
        [Export] private int DefaultSize { get => _colliderSize; set => this.Set(ref _colliderSize, Math.Max(value, 0)); }

        public float GetScaledChunkSize() => ChunkSize * ChunkScale;
        public float GetScaledTerrainSize() => GetScaledChunkSize() * ChunkRadius * 2;
    }
}
