using System.Runtime.CompilerServices;
using FastEnumUtility;
using Godot;
using GodotSharp.BuildingBlocks.Terrain2.Internal;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    public interface ITerrain
    {
        Node3D Mesh { get; }
        StaticBody3D Body { get; }

        float GetHeight(in Vector2 vector2);
    }

    public partial class TerrainData
    {
        private static readonly PackedScene Chunk = App.LoadScene<TerrainChunk>();
        private static TerrainChunk NewChunk() => Chunk.Instantiate<TerrainChunk>();

        private readonly ShaderMaterial SharedShader = Default.ShaderMaterial();

        private ITerrain terrain;

        private readonly AutoAction ResetChunks = new();
        private readonly AutoAction ResetShader = new();
        private readonly AutoAction UpdateNoise = new();
        private readonly AutoAction ResetRegions = new();
        private readonly AutoAction UpdateOther = new();

        private bool UseNoise => Noise is not null;
        private bool UseHeightMap => Noise is null;
        private bool UseNormalMap => NormalMap is not null;

        public void Init(ITerrain terrain)
        {
            if (Regions.IsNullOrEmpty())
            {
                Regions = null;
                HeightCurve = null;
            }

            HeightCurve ??= null;

            this.terrain = terrain;

            this.ResetChunks.Action = ResetChunks;
            this.ResetShader.Action = ResetShader;
            this.UpdateNoise.Action = UpdateNoise;
            this.ResetRegions.Action = ResetRegions;
            this.UpdateOther.Action = UpdateOther;

            this.ResetChunks.Run();
            this.ResetShader.Run();
            this.UpdateNoise.Run();
            this.ResetRegions.Run();
            this.UpdateOther.Run();

            void ResetChunks()
            {
                ClearChunks();
                CreateChunks();

                void ClearChunks()
                    => terrain.Mesh.RemoveChildren<TerrainChunk>(free: true);

                void CreateChunks()
                {
                    this.terrain.Mesh.Scale = this.terrain.Body.Scale = Vector3.One * ChunkScale;

                    var count = ChunkRadius - 1;
                    foreach (var (x, z) in Utils.RangeGrid(-count, count))
                    {
                        var chunk = NewChunk();
                        chunk.Name = $"Chunk ({x}, {z})";
                        chunk.Position = new Vector3(x, 0, z) * ChunkSize;
                        chunk.Material = SharedShader;
                        chunk.ExtraCullMargin = Amplitude;

                        var lod = Math.Max(Math.Abs(x), Math.Abs(z));
                        chunk.SetSize(ChunkSize, lod * LodStep);

                        terrain.Mesh.AddChild(chunk);
                    }
                }
            }

            void ResetShader()
            {
                SharedShader.Shader.Code = string.Join("\n", Parts());

                IEnumerable<string> Parts()
                {
                    yield return "shader_type spatial;\n";

                    if (UseHeightMap)
                    {
                        yield return "#define USE_HEIGHT_MAP";

                        if (UseNormalMap)
                            yield return "#define USE_NORMAL_MAP";
                    }
                    else
                    {
                        yield return Def(Noise.NoiseType, "FNL_NOISE");
                        yield return Def(Noise.FractalType, "FNL_FRACTAL");
                    }

                    if (EnableBlending) yield return "#define USE_BLENDING";
                    if (UseHeightCurve) yield return "#define USE_HEIGHT_CURVE";

                    yield return $"\n#include \"{App.GetShaderIncPath<TerrainChunk>()}\"";

                    static string Def<T>(T value, string prefix = null) where T : struct, Enum
                    {
                        return $"#define {prefix ?? Part(typeof(T).Name)}_{Part(FastEnum.ToString(value))}";

                        static string Part(string v)
                            => v.ToSnakeCase().ToUpper();
                    }
                }
            }

            void UpdateNoise()
            {
                if (!UseNoise) return;

                Set(Noise.Gain);
                Set(Noise.Seed);
                Set(Noise.Octaves);
                Set(Noise.Frequency);
                Set(Noise.Lacunarity);
                Set(Noise.WeightedStrength);
                Set(Noise.PingPongStrength);

                void Set(Variant value, [CallerArgumentExpression(nameof(value))] string name = null)
                    => this.Set(value, "_fnl_", name.TrimPrefix("Noise."));
            }

            void ResetRegions()
            {
                var regions = Regions.Where(x => x is not null).ToArray();

                Set(regions.Length, name: "layer_count");
                Set(regions.Select(x => x.Tint).ToArray(), name: "tints");
                Set(regions.Select(x => x.Texture).ToArray(), name: "textures");
                Set(regions.Select(x => x.Gradient).ToArray(), name: "gradients");
                Set(regions.Select(x => x.MinSlope).ToArray(), name: "min_slopes");
                Set(regions.Select(x => x.MaxSlope).ToArray(), name: "max_slopes");
                Set(regions.Select(x => x.MinHeight).ToArray(), name: "min_heights");
                Set(regions.Select(x => x.MaxHeight).ToArray(), name: "max_heights");
                Set(regions.Select(x => x.TextureScale).ToArray(), name: "texture_scales");
                Set(regions.Select(x => x.TintStrength).ToArray(), name: "tint_strengths");
                Set(regions.Select(x => x.BlendStrength).ToArray(), name: "blend_strengths");
            }

            void UpdateOther()
            {
                Set(Offset);
                Set(Amplitude);

                Set(LodStep);
                Set(ChunkSize);
                Set(ChunkScale);
                //Set(ChunkRadius);

                if (UseHeightMap) Set(HeightMap);
                if (UseNormalMap) Set(NormalMap);
                if (UseHeightCurve) Set(HeightCurve);
            }
        }

        private void InitNoise(ShaderNoiseLite oldValue, ShaderNoiseLite newValue)
        {
            ResetShader.Run();
            UpdateNoise.Run();

            if (oldValue is not null)
            {
                oldValue.TypesChanged -= ResetShader.Run;
                oldValue.ValuesChanged -= UpdateNoise.Run;
            }

            if (newValue is not null)
            {
                newValue.TypesChanged += ResetShader.Run;
                newValue.ValuesChanged += UpdateNoise.Run;
            }
        }

        private void Set(Curve value, string prefix = null, [CallerArgumentExpression(nameof(value))] string name = null)
            => Set(Default.CurveTexture(value), prefix, name);

        private void Set(Variant value, string prefix = null, [CallerArgumentExpression(nameof(value))] string name = null)
            => SharedShader.SetShaderParameter($"{prefix}{name.ToSnakeCase()}", value);
    }
}
