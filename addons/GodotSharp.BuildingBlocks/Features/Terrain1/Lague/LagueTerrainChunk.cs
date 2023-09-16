using System.Diagnostics;
using Godot;
using CTS = System.Threading.CancellationTokenSource;

namespace GodotSharp.BuildingBlocks.Terrain1.Lague
{
    [Tool]
    public partial class LagueTerrainChunk : CustomMesh
    {
        [OnInstantiate]
        public void Initialise(LagueTerrain data, in Vector2I cell, int lod)
        {
            var chunkPos = cell * data.ChunkSize;
            var offset = chunkPos + data.Offset;

            Name = $"Chunk {cell}";
            Scale = Vector3.One * data.ChunkScale;
            Position = new Vector3(chunkPos.X, 0, chunkPos.Y) * data.ChunkScale;

            CreateChunkAsync(data, offset, lod);
        }

        private int lod;
        private Material material;
        private float[,] heightMap;

        private CTS ctsHeightMap = new();
        private CTS ctsTerrainMesh = new();

        private void CreateChunkAsync(LagueTerrain data, Vector2I offset, int lod)
        {
            this.lod = lod;

            if (Engine.IsEditorHint())
                data = (LagueTerrain)data.Duplicate(true);

            GenerateHeightMapAsync(data, offset);
        }

        private void GenerateHeightMapAsync(LagueTerrain data, Vector2I offset)
        {
            ctsHeightMap.Cancel();
            ctsTerrainMesh.Cancel();
            ctsHeightMap = this.RunAsync(ct =>
            {
                var heightMap = GetHeightMap();
                var material = GetMaterial(heightMap);

                this.CallDeferred(ct, () =>
                {
                    this.heightMap = heightMap;
                    this.material = material;

                    GenerateTerrainMeshAsync(data);
                });
            });

            float[,] GetHeightMap()
            {
                return data.DrawMode is DrawMode.IslandOverlay ? data.IslandOverlay
                    : Default.LagueTerrain.HeightMap(data.NoiseSize, data.NoiseSize,
                        data.Seed, data.Scale, data.Octaves, data.Persistence, data.Lacunarity, offset,
                        data.NormaliseMode is NormaliseMode.Global ? data.GlobalNormalMultiplier : null,
                        data.Islands ? data.IslandOverlay : null);
            }

            Material GetMaterial(float[,] heightMap)
            {
                var image = data.DrawMode is DrawMode.Mesh or DrawMode.ColorMap
                    ? Default.LagueTerrain.HeightMap(heightMap, GetRegionColor)
                    : Default.LagueTerrain.HeightMap(heightMap);

                return
                    data.MaterialOverride is not null ? data.MaterialOverride :
                    data.DrawMode is DrawMode.ColorShader ? CreateColorShader() :
                    data.DrawMode is DrawMode.TextureShader ? CreateTextureShader() :
                    Default.Material(image, blend: data.Blending);

                Color GetRegionColor(float height)
                    => data.Regions.FirstOrDefault(x => height <= x.Height)?.Tint ?? data.Regions.LastOrDefault()?.Tint ?? TerrainType.Default().Tint;

                Material CreateColorShader()
                {
                    Debug.Assert(data.Regions.Length <= 8);

                    var shader = Default.ShaderMaterial<LagueTerrain>(data.Blending ? "ColorShaderWithBlending" : "ColorShader");

                    shader.SetShaderParameter("layer_count", data.Regions.Length);
                    shader.SetShaderParameter("colors", data.Regions.Select(x => x.Tint).ToArray());
                    shader.SetShaderParameter("heights", data.Regions.Select(x => x.ShaderHeight).ToArray());
                    shader.SetShaderParameter("min_height", Default.LagueTerrain.GetHeight(0, data.Amplitude, data.HeightCurve));
                    shader.SetShaderParameter("max_height", Default.LagueTerrain.GetHeight(1, data.Amplitude, data.HeightCurve));
                    if (data.Blending) shader.SetShaderParameter("blend_strengths", data.Regions.Select(x => x.BlendStrength).ToArray());

                    return shader;
                }

                Material CreateTextureShader()
                {
                    Debug.Assert(data.Regions.Length <= 8);

                    var shader = Default.ShaderMaterial<LagueTerrain>(data.Blending ? "TextureShaderWithBlending" : "TextureShader");
                    shader.SetShaderParameter("layer_count", data.Regions.Length);
                    shader.SetShaderParameter("colors", data.Regions.Select(x => x.Tint).ToArray());
                    shader.SetShaderParameter("heights", data.Regions.Select(x => x.ShaderHeight).ToArray());
                    shader.SetShaderParameter("textures", data.Regions.Select(x => x.Texture).ToArray());
                    shader.SetShaderParameter("texture_scales", data.Regions.Select(x => x.TextureScale).ToArray());
                    shader.SetShaderParameter("color_strengths", data.Regions.Select(x => x.TintStrength).ToArray());
                    shader.SetShaderParameter("min_height", Default.LagueTerrain.GetHeight(0, data.Amplitude, data.HeightCurve));
                    shader.SetShaderParameter("max_height", Default.LagueTerrain.GetHeight(1, data.Amplitude, data.HeightCurve));
                    if (data.Blending) shader.SetShaderParameter("blend_strengths", data.Regions.Select(x => x.BlendStrength).ToArray());

                    return shader;
                }
            }
        }

        private void GenerateTerrainMeshAsync(LagueTerrain data)
        {
            ctsTerrainMesh.Cancel();
            ctsTerrainMesh = this.RunAsync(ct =>
            {
                var terrainMesh = GetTerrainMesh();
                this.CallDeferred(ct, () => Mesh = terrainMesh);
            });

            Mesh GetTerrainMesh()
            {
                return data.DrawMode is DrawMode.ColorMap or DrawMode.NoiseMap or DrawMode.IslandOverlay ? Default.PlaneMesh(data.ChunkSize, material)
                    : Default.LagueTerrain.Terrain(heightMap, data.Amplitude, data.HeightCurve, lod, material, data.Seamless, data.FlatShaded, data.GodotNormals);
            }
        }

        public void SetLod(LagueTerrain data, int lod)
        {
            if (this.lod == lod) return;

            this.lod = lod;

            if (heightMap is null) return;
            GenerateTerrainMeshAsync(data);
        }
    }
}
