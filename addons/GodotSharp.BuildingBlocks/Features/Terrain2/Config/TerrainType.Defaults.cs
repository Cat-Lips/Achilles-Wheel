using Godot;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    public enum DefaultTerrainTypes
    {
        Mixed,
        Slope,
        Height,
    }

    public enum TintFromTexture
    {
        None,
        Average,
        MostCommon,
    }

    public partial class TerrainType
    {
        internal static DefaultTerrainTypes DefaultTerrainTypes = DefaultTerrainTypes.Height;
        internal static TintFromTexture TintFromTexture = TintFromTexture.None;

        internal static float DefaultTextureScale = 25;
        internal static float DefaultTintStrength = 0;
        internal static float DefaultBlendStrength = .5f;

        internal static TerrainType[] Defaults()
        {
            return Defaults().ForAll(x =>
            {
                x.Texture = LoadTexture(x.Name);
                x.TextureScale = DefaultTextureScale;
                x.TintStrength = DefaultTintStrength;
                x.BlendStrength = DefaultBlendStrength;
                ApplyTintFromTexture(x, TintFromTexture);
            }).ToArray();

            static IEnumerable<TerrainType> Defaults()
            {
                //const float Flat = 0;
                const float Gentle = .1f;
                const float Medium = .25f;
                const float Steep = 1;

                return DefaultTerrainTypes switch
                {
                    DefaultTerrainTypes.Mixed => MixedDefaults(),
                    DefaultTerrainTypes.Slope => SlopeDefaults(),
                    DefaultTerrainTypes.Height => HeightDefaults(),
                    _ => throw new NotImplementedException(),
                };

                static IEnumerable<TerrainType> MixedDefaults()
                {
                    yield return new() { Name = "DeepWater", Tint = Colors.DarkBlue, Gradient = Steep, MinSlope = 0, MaxSlope = 1, MinHeight = .0f, MaxHeight = .3f };
                    yield return new() { Name = "ShallowWater", Tint = Colors.LightSeaGreen, Gradient = Gentle, MinSlope = 0, MaxSlope = 1, MinHeight = .3f, MaxHeight = .4f };

                    yield return new() { Name = "Sand", Tint = Colors.SandyBrown, Gradient = Gentle, MinSlope = 0, MaxSlope = .3f, MinHeight = .4f, MaxHeight = .5f };
                    yield return new() { Name = "Grass", Tint = Colors.LawnGreen, Gradient = Gentle, MinSlope = 0, MaxSlope = .5f, MinHeight = .5f, MaxHeight = .7f };
                    yield return new() { Name = "Forest", Tint = Colors.ForestGreen, Gradient = Medium, MinSlope = .5f, MaxSlope = .7f, MinHeight = .4f, MaxHeight = .7f };
                    yield return new() { Name = "LowRock", Tint = Colors.SaddleBrown, Gradient = Medium, MinSlope = .7f, MaxSlope = 1, MinHeight = .4f, MaxHeight = .7f };

                    yield return new() { Name = "HighRock", Tint = Colors.SaddleBrown.Darkened(.3f), Gradient = Steep, MinSlope = 0, MaxSlope = 1, MinHeight = .7f, MaxHeight = .8f };
                    yield return new() { Name = "Snow", Tint = Colors.Snow, Gradient = Steep, MinSlope = 0, MaxSlope = 1, MinHeight = .8f, MaxHeight = 1 };
                }

                static IEnumerable<TerrainType> SlopeDefaults()
                {
                    yield return new() { Name = "DeepWater", Tint = Colors.DarkBlue, Gradient = Steep, MinSlope = 0, MaxSlope = .3f };
                    yield return new() { Name = "ShallowWater", Tint = Colors.LightSeaGreen, Gradient = Gentle, MinSlope = .3f, MaxSlope = .4f };

                    yield return new() { Name = "Sand", Tint = Colors.SandyBrown, Gradient = Gentle, MinSlope = .4f, MaxSlope = .45f };
                    yield return new() { Name = "Grass", Tint = Colors.LawnGreen, Gradient = Gentle, MinSlope = .45f, MaxSlope = .55f };
                    yield return new() { Name = "Forest", Tint = Colors.ForestGreen, Gradient = Medium, MinSlope = .55f, MaxSlope = .6f };
                    yield return new() { Name = "LowRock", Tint = Colors.SaddleBrown, Gradient = Medium, MinSlope = .6f, MaxSlope = .7f };

                    yield return new() { Name = "HighRock", Tint = Colors.SaddleBrown.Darkened(.3f), Gradient = Steep, MinSlope = .7f, MaxSlope = .8f };
                    yield return new() { Name = "Snow", Tint = Colors.Snow, Gradient = Steep, MinSlope = .8f, MaxSlope = 1 };
                }

                static IEnumerable<TerrainType> HeightDefaults()
                {
                    yield return new() { Name = "DeepWater", Tint = Colors.DarkBlue, Gradient = Steep, MinHeight = .0f, MaxHeight = .3f };
                    yield return new() { Name = "ShallowWater", Tint = Colors.LightSeaGreen, Gradient = Gentle, MinHeight = .3f, MaxHeight = .4f };

                    yield return new() { Name = "Sand", Tint = Colors.SandyBrown, Gradient = Gentle, MinHeight = .4f, MaxHeight = .45f };
                    yield return new() { Name = "Grass", Tint = Colors.LawnGreen, Gradient = Gentle, MinHeight = .45f, MaxHeight = .55f };
                    yield return new() { Name = "Forest", Tint = Colors.ForestGreen, Gradient = Medium, MinHeight = .55f, MaxHeight = .6f };
                    yield return new() { Name = "LowRock", Tint = Colors.SaddleBrown, Gradient = Medium, MinHeight = .6f, MaxHeight = .7f };

                    yield return new() { Name = "HighRock", Tint = Colors.SaddleBrown.Darkened(.3f), Gradient = Steep, MinHeight = .7f, MaxHeight = .8f };
                    yield return new() { Name = "Snow", Tint = Colors.Snow, Gradient = Steep, MinHeight = .8f, MaxHeight = 1 };
                }
            }

            static void ApplyTintFromTexture(TerrainType x, TintFromTexture tintType)
            {
                if (x.Texture is null) return;
                if (tintType is TintFromTexture.None) return;

                var image = x.Texture.GetImage();
                image.Decompress();

                if (tintType is TintFromTexture.Average) x.Tint = image.Colors().Average();
                if (tintType is TintFromTexture.MostCommon) x.Tint = image.Colors().MostCommon();
            }

            static Texture2D LoadTexture(string name, string ext = "png")
                => GD.Load<Texture2D>(App.GetScriptDir<TerrainType>().PathJoin($"../Textures/{name}.{ext}"));
        }
    }
}
