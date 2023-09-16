using Godot;

// https://www.youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3

namespace GodotSharp.BuildingBlocks.Terrain1.Lague
{
    [Tool, GlobalClass]
    public partial class LagueTerrain : TerrainData
    {
        public override int ChunkSize { get => Default.LagueTerrain.ChunkSize; set { } }
        public int NoiseSize => Seamless ? Default.LagueTerrain.BorderedSize : Default.LagueTerrain.NoiseSize;

        private int _seed;
        private float _scale = .3f;
        private Vector2I _offset;
        private int _octaves = 4;
        private float _persistence = .5f;
        private float _lacunarity = 2;

        private int _amplitude = 25;
        private TerrainType[] _regions;
        private Curve _heightCurve;

        private bool _islands;
        private float _isleSlope = 3;
        private float _isleWater = 2.2f;

        private DrawMode _drawMode = DrawMode.Mesh;
        private NormaliseMode _normaliseMode = NormaliseMode.Global;
        private float _globalNormalMultiplier = .5f;
        private bool _blending = true;
        private bool _seamless = false; // Broken
        private bool _flatShaded = false;
        private bool _godotNormals = false; // ArrayMesh.RegenNormalMaps
        private Material _materialOverride;

        [ExportGroup("Noise")]
        [ExportCategory("Noise")]
        [Export] public int Seed { get => _seed; set => this.Set(ref _seed, value); }
        [Export] public float Scale { get => _scale; set => this.Set(ref _scale, value); }
        [Export(PropertyHint.Link)] public Vector2I Offset { get => _offset; set => this.Set(ref _offset, value); }
        [Export(PropertyHint.Range, "1,10")] public int Octaves { get => _octaves; set => this.Set(ref _octaves, value); }
        [Export(PropertyHint.Range, "0,1")] public float Persistence { get => _persistence; set => this.Set(ref _persistence, value); }
        [Export(PropertyHint.Range, "1,10")] public float Lacunarity { get => _lacunarity; set => this.Set(ref _lacunarity, value); }

        [ExportGroup("Terrain")]
        [ExportCategory("Terrain")]
        [Export] public int Amplitude { get => _amplitude; set => this.Set(ref _amplitude, value); }
        [Export] public TerrainType[] Regions { get => _regions; set => this.Set(ref _regions, value.IsNullOrEmpty() ? TerrainType.DefaultTerrainTypes() : value); }
        [Export] public Curve HeightCurve { get => _heightCurve; set => this.Set(ref _heightCurve, value ?? TerrainType.CreateTerrainCurve(Regions)); }

        [ExportGroup("Islands", "Isl")]
        [ExportCategory("Islands")]
        [Export] public bool Islands { get => _islands; set => this.Set(ref _islands, value, notify: true); }
        [Export(PropertyHint.Range, "1,10")] public float IsleSlope { get => _isleSlope; set => this.Set(ref _isleSlope, value, GenerateIslandOverlay); }
        [Export(PropertyHint.Range, "1,10")] public float IsleWater { get => _isleWater; set => this.Set(ref _isleWater, value, GenerateIslandOverlay); }

        [ExportGroup("Features")]
        [ExportCategory("Features")]
        [Export] public DrawMode DrawMode { get => _drawMode; set => this.Set(ref _drawMode, value); }
        [Export] public NormaliseMode NormaliseMode { get => _normaliseMode; set => this.Set(ref _normaliseMode, value, notify: true); }
        [Export] public float GlobalNormalMultiplier { get => _globalNormalMultiplier; set => this.Set(ref _globalNormalMultiplier, value); }
        [Export] public bool Blending { get => _blending; set => this.Set(ref _blending, value); }
        [Export] public bool Seamless { get => _seamless; set => this.Set(ref _seamless, value); }
        [Export] public bool FlatShaded { get => _flatShaded; set => this.Set(ref _flatShaded, value); }
        [Export] public bool GodotNormals { get => _godotNormals; set => this.Set(ref _godotNormals, value); }
        [Export] public Material MaterialOverride { get => _materialOverride; set => this.Set(ref _materialOverride, value); }

        public float[,] IslandOverlay { get; private set; }
        private void GenerateIslandOverlay() => IslandOverlay = Default.LagueTerrain.IslandOverlay(NoiseSize, NoiseSize, IsleSlope, IsleWater);

        public LagueTerrain()
        {
            Regions = null;
            HeightCurve = null;
            GenerateIslandOverlay();
        }

        public override Node CreateChunk(in Vector2I cell, int lod)
            => LagueTerrainChunk.Instantiate(this, cell, lod);

        public override void Redraw(Node chunk, in Vector2I cell, int lod)
            => ((LagueTerrainChunk)chunk).Initialise(this, cell, lod);

        public override void SetLod(Node chunk, int lod)
            => ((LagueTerrainChunk)chunk).SetLod(this, lod);
    }
}
