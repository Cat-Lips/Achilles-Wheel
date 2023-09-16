using System.Diagnostics;
using Godot;
using Array = Godot.Collections.Array;

namespace GodotSharp.BuildingBlocks
{
    public static partial class Default
    {
        #region LagueTerrain

        public static class LagueTerrain
        {
            public const int MaxLod = 6;
            public const int ChunkSize = 240; // {24,48,72,96,120,144,168,192,216,240,...}
            public const int NoiseSize = 241;

            public const int BorderedSize = 241;
            public const int SeamlessSize = 238;

            private static readonly Noise PerlinNoise = new FastNoiseLite
            {
                NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
                FractalType = FastNoiseLite.FractalTypeEnum.None,
            };

            public static float[,] HeightMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2I offset, float? globalNormal = null, float[,] overlay = null)
            {
                Debug.Assert(overlay is null || overlay.GetLength(0) == width && overlay.GetLength(1) == height);

                if (scale is 0) scale = float.Epsilon;

                var halfWidth = width * .5f;
                var halfHeight = height * .5f;
                var maxLocalHeight = float.MinValue;
                var minLocalHeight = float.MaxValue;
                var maxGlobalHeight = 0f;

                var heightMap = new float[width, height];

                CreateHeightMap();
                NormaliseHeightMap();

                return heightMap;

                void CreateHeightMap()
                {
                    RandomiseOctaveOffsets(out var octaveOffsets);

                    for (var x = 0; x < width; ++x)
                    {
                        for (var y = 0; y < height; ++y)
                        {
                            float amplitude = 1;
                            float frequency = 1;
                            float noiseHeight = 0;

                            for (var i = 0; i < octaves; ++i)
                            {
                                var octaveOffset = octaveOffsets[i];
                                var sx = (x - halfWidth + octaveOffset.X) / scale * frequency;
                                var sy = (y - halfHeight + octaveOffset.Y) / scale * frequency;
                                var perlin = PerlinNoise.GetNoise2D(sx, sy);
                                Debug.Assert(perlin is >= -1 and <= 1);
                                noiseHeight += perlin * amplitude;

                                amplitude *= persistance;
                                frequency *= lacunarity;
                            }

                            if (noiseHeight > maxLocalHeight) maxLocalHeight = noiseHeight;
                            if (noiseHeight < minLocalHeight) minLocalHeight = noiseHeight;

                            heightMap[x, y] = overlay is null ? noiseHeight : noiseHeight - overlay[x, y];
                        }
                    }

                    void RandomiseOctaveOffsets(out Vector2I[] octaveOffsets)
                    {
                        const int RandRange = 100000;

                        float amplitude = 1;
                        var rng = new Random(seed);
                        octaveOffsets = new Vector2I[octaves];
                        for (var i = 0; i < octaves; ++i)
                        {
                            var offsetX = rng.Next(-RandRange, RandRange) + offset.X;
                            var offsetY = rng.Next(-RandRange, RandRange) + offset.Y;
                            octaveOffsets[i] = new(offsetX, offsetY);

                            maxGlobalHeight += amplitude;
                            amplitude *= persistance;
                        }
                    }
                }

                void NormaliseHeightMap()
                {
                    if (globalNormal is null)
                        Normalise(minLocalHeight, maxLocalHeight);
                    else
                        NormaliseGlobal(maxGlobalHeight * globalNormal.Value);

                    void Normalise(float minHeight, float maxHeight)
                    {
                        for (var x = 0; x < width; ++x)
                        {
                            for (var y = 0; y < height; ++y)
                            {
                                heightMap[x, y] = Mathf.InverseLerp(minHeight, maxHeight, heightMap[x, y]);
                            }
                        }
                    }

                    void NormaliseGlobal(float adjustedHeight)
                        => Normalise(-adjustedHeight, adjustedHeight);
                }
            }

            public static Image HeightMap(float[,] heightMap, Func<float, Color> getColor = null)
            {
                var width = heightMap.GetLength(0);
                var height = heightMap.GetLength(1);
                var image = Image.Create(width, height, false, Image.Format.Rgb8);

                getColor ??= GetNoiseColor;
                for (var x = 0; x < width; ++x)
                {
                    for (var y = 0; y < height; ++y)
                    {
                        var weight = heightMap[x, y];
                        var color = getColor(weight);
                        image.SetPixel(x, y, color);
                    }
                }

                return image;

                static Color GetNoiseColor(float weight)
                    => Colors.White.Lerp(Colors.Black, weight);
            }

            public static float[,] IslandOverlay(int width, int height, float a = 3, float b = 2.2f)
            {
                var heightMap = new float[width, height];

                for (var x = 0; x < width; ++x)
                {
                    for (var y = 0; y < height; ++y)
                    {
                        var value = Math.Max(
                            Math.Abs(x / (float)width * 2 - 1),
                            Math.Abs(y / (float)height * 2 - 1));
                        heightMap[x, y] = EvaluateDropOff(value);
                    }
                }

                return heightMap;

                float EvaluateDropOff(float value)
                {
                    var v = Mathf.Pow(value, a);
                    return v / (v + Mathf.Pow(b - b * value, a));
                }
            }

            public static float GetHeight(float value, float amplitude, Curve heightCurve)
            {
                return value > heightCurve.MaxValue
                    ? value *= amplitude
                    : heightCurve.Sample(value) * amplitude;
            }

            public static Mesh Terrain(float[,] heightMap, float amplitude, Curve heightCurve, int lod, Material material, bool seamless = false, bool flatShaded = false, bool useGodotNormals = false)
            {
                lod = Math.Clamp(lod, 0, MaxLod);
                lod = lod is 0 ? 1 : lod * 2;

                return seamless
                    ? Seamless() // Broken
                    : Original();

                Mesh Seamless()
                {
                    var borderedSize = heightMap.GetLength(0);
                    Debug.Assert(heightMap.GetLength(1) == borderedSize);

                    var meshSize = borderedSize - 2;
                    var meshSizeLod = borderedSize - 2 * lod;

                    var topLeft = (meshSize - 1) * -.5f;
                    var lodCount = (meshSizeLod - 1) / lod + 1;

                    var meshVertices = new Vector3[lodCount * lodCount];
                    var meshTriangles = new int[(lodCount - 1) * (lodCount - 1) * 6];
                    var borderVertices = new Vector3[lodCount * 4 + 4];
                    var borderTriangles = new int[lodCount * 24];
                    var uvs = new Vector2[meshVertices.Length];

                    InitVertexIndexLookup(out var vertexIndexLookup);

                    var meshTriangleIndex = -1;
                    var borderTriangleIndex = -1;
                    for (var y = 0; y < borderedSize; y += lod)
                    {
                        for (var x = 0; x < borderedSize; x += lod)
                        {
                            AddVertex(x, y, GetHeight(heightMap[x, y], amplitude, heightCurve));

                            if (x < borderedSize - 1 && y < borderedSize - 1)
                            {
                                var a = vertexIndexLookup[x, y];
                                var b = vertexIndexLookup[x + lod, y];
                                var c = vertexIndexLookup[x, y + lod];
                                var d = vertexIndexLookup[x + lod, y + lod];

                                AddTriangle(a, d, c);
                                AddTriangle(d, a, b);
                            }
                        }
                    }

                    if (flatShaded) ApplyFlatShading(ref meshVertices, ref meshTriangles, ref uvs);
                    return CreateMesh(meshVertices, meshTriangles, uvs, useGodotNormals ? null : CalculateNormals(), material);

                    bool IsBorderIndex(int x) => x < 0;
                    int GetBorderIndex(int x) => -x - 1;
                    bool IsBorderTriangle(int a, int b, int c) => a < 0 || b < 0 || c < 0;
                    void InitVertexIndexLookup(out int[,] lookup)
                    {
                        lookup = new int[borderedSize, borderedSize];

                        var meshIndex = 0;
                        var borderIndex = -1;
                        for (var x = 0; x < borderedSize; x += lod)
                        {
                            for (var y = 0; y < borderedSize; y += lod)
                            {
                                var isBorderVertex =
                                    x == 0 || x == borderedSize - 1 ||
                                    y == 0 || y == borderedSize - 1;

                                lookup[x, y] = isBorderVertex
                                    ? borderIndex--
                                    : meshIndex++;
                            }
                        }
                    }

                    void AddVertex(int x, int y, float terrainHeight)
                    {
                        var uv = new Vector2((x - lod) / (float)meshSizeLod, (y - lod) / (float)meshSizeLod);
                        var vertex = new Vector3(topLeft + uv.X * meshSize, terrainHeight, topLeft + uv.Y * meshSize);

                        var vertexIndex = vertexIndexLookup[x, y];

                        if (IsBorderIndex(vertexIndex))
                        {
                            var borderIndex = GetBorderIndex(vertexIndex);
                            borderVertices[borderIndex] = vertex;
                        }
                        else
                        {
                            uvs[vertexIndex] = uv;
                            meshVertices[vertexIndex] = vertex;
                        }
                    }

                    void AddTriangle(int a, int b, int c)
                    {
                        if (IsBorderTriangle(a, b, c))
                        {
                            borderTriangles[++borderTriangleIndex] = a;
                            borderTriangles[++borderTriangleIndex] = b;
                            borderTriangles[++borderTriangleIndex] = c;
                        }
                        else
                        {
                            meshTriangles[++meshTriangleIndex] = a;
                            meshTriangles[++meshTriangleIndex] = b;
                            meshTriangles[++meshTriangleIndex] = c;
                        }
                    }

                    Vector3[] CalculateNormals()
                    {
                        var normals = new Vector3[meshVertices.Length];

                        Calculate();
                        Normalise();

                        return normals;

                        void Calculate()
                        {
                            for (var i = 0; i < meshTriangles.Length; i += 3)
                            {
                                var indexA = meshTriangles[i];
                                var indexB = meshTriangles[i + 1];
                                var indexC = meshTriangles[i + 2];

                                var normal = GetNormal(indexA, indexB, indexC);

                                normals[indexA] += normal;
                                normals[indexB] += normal;
                                normals[indexC] += normal;
                            }

                            for (var i = 0; i < borderTriangles.Length; i += 3)
                            {
                                var indexA = borderTriangles[i];
                                var indexB = borderTriangles[i + 1];
                                var indexC = borderTriangles[i + 2];

                                var normal = GetNormal(indexA, indexB, indexC);

                                if (!IsBorderIndex(indexA)) normals[indexA] += normal;
                                if (!IsBorderIndex(indexB)) normals[indexB] += normal;
                                if (!IsBorderIndex(indexC)) normals[indexC] += normal;
                            }

                            Vector3 GetNormal(int indexA, int indexB, int indexC)
                            {
                                var pointA = IsBorderIndex(indexA) ? borderVertices[GetBorderIndex(indexA)] : meshVertices[indexA];
                                var pointB = IsBorderIndex(indexB) ? borderVertices[GetBorderIndex(indexB)] : meshVertices[indexB];
                                var pointC = IsBorderIndex(indexC) ? borderVertices[GetBorderIndex(indexC)] : meshVertices[indexC];

                                var sideAB = pointB - pointA;
                                var sideAC = pointC - pointA;

                                return sideAC.Cross(sideAB).Normalized();
                            }
                        }

                        void Normalise()
                        {
                            for (var i = 0; i < normals.Length; ++i)
                                normals[i] = normals[i].Normalized();
                        }
                    }
                }

                Mesh Original()
                {
                    var width = heightMap.GetLength(0);
                    var height = heightMap.GetLength(1);

                    var topLeftX = (width - 1) * -.5f;
                    var topLeftZ = (height - 1) * -.5f;
                    var lodWidth = (width - 1) / lod + 1;
                    var lodHeight = (height - 1) / lod + 1;

                    var vertices = new Vector3[lodWidth * lodHeight];
                    var triangles = new int[(lodWidth - 1) * (lodHeight - 1) * 6];
                    var uvs = new Vector2[vertices.Length];

                    var vertexIndex = -1;
                    var triangleIndex = -1;
                    for (var y = 0; y < height; y += lod)
                    {
                        for (var x = 0; x < width; x += lod)
                        {
                            AddVertex(x, y, GetHeight(heightMap[x, y], amplitude, heightCurve));

                            if (x < width - 1 && y < height - 1)
                            {
                                var a = vertexIndex;
                                var b = vertexIndex + 1;
                                var c = vertexIndex + lodWidth;
                                var d = vertexIndex + lodWidth + 1;

                                AddTriangle(a, d, c);
                                AddTriangle(d, a, b);
                            }
                        }
                    }

                    if (flatShaded) ApplyFlatShading(ref vertices, ref triangles, ref uvs);
                    return CreateMesh(vertices, triangles, uvs, useGodotNormals ? null : CalculateNormals(), material);

                    void AddVertex(int x, int y, float terrainHeight)
                    {
                        uvs[++vertexIndex] = new(x / (float)width, y / (float)height);
                        vertices[vertexIndex] = new(topLeftX + x, terrainHeight, topLeftZ + y);
                    }

                    void AddTriangle(int a, int b, int c)
                    {
                        triangles[++triangleIndex] = a;
                        triangles[++triangleIndex] = b;
                        triangles[++triangleIndex] = c;
                    }

                    Vector3[] CalculateNormals()
                    {
                        var normals = new Vector3[vertices.Length];

                        Calculate();
                        Normalise();

                        return normals;

                        void Calculate()
                        {
                            for (var i = 0; i < triangles.Length; i += 3)
                            {
                                var indexA = triangles[i];
                                var indexB = triangles[i + 1];
                                var indexC = triangles[i + 2];

                                var normal = GetNormal(indexA, indexB, indexC);

                                normals[indexA] += normal;
                                normals[indexB] += normal;
                                normals[indexC] += normal;
                            }

                            Vector3 GetNormal(int indexA, int indexB, int indexC)
                            {
                                var pointA = vertices[indexA];
                                var pointB = vertices[indexB];
                                var pointC = vertices[indexC];

                                var sideAB = pointB - pointA;
                                var sideAC = pointC - pointA;

                                return sideAC.Cross(sideAB).Normalized();
                            }
                        }

                        void Normalise()
                        {
                            for (var i = 0; i < normals.Length; ++i)
                                normals[i] = normals[i].Normalized();
                        }
                    }
                }

                void ApplyFlatShading(ref Vector3[] vertices, ref int[] triangles, ref Vector2[] uvs)
                {
                    var flatShadedUVs = new Vector2[triangles.Length];
                    var flatShadedVertices = new Vector3[triangles.Length];

                    for (var i = 0; i < triangles.Length; ++i)
                    {
                        flatShadedVertices[i] = vertices[triangles[i]];
                        flatShadedUVs[i] = uvs[triangles[i]];
                        triangles[i] = i;
                    }

                    uvs = flatShadedUVs;
                    vertices = flatShadedVertices;
                }
            }

            private static Mesh CreateMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs, Vector3[] normals = null, Material material = null)
            {
                var arrays = new Array();
                arrays.Resize((int)Mesh.ArrayType.Max);
                arrays[(int)Mesh.ArrayType.Vertex] = vertices;
                arrays[(int)Mesh.ArrayType.Index] = triangles;
                arrays[(int)Mesh.ArrayType.Normal] = normals ?? new Vector3[vertices.Length];
                arrays[(int)Mesh.ArrayType.TexUV] = uvs;

                var mesh = new ArrayMesh();
                mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
                if (material is not null) mesh.SurfaceSetMaterial(0, material);
                if (normals is null) mesh.RegenNormalMaps();
                return mesh;
            }
        }

        #endregion
    }
}
