using Godot;
using static Godot.FastNoiseLite;
using static Godot.MultiMesh;

namespace GodotSharp.BuildingBlocks
{
    public static partial class Default
    {
        public static PlaneMesh PlaneMesh(int size = 0, Material material = null) => new()
        {
            Size = Vector2.One * size,
            SubdivideDepth = size - 1,
            SubdivideWidth = size - 1,
            Material = material,
        };

        public static FastNoiseLite Noise(int seed = 0) => new()
        {
            Seed = seed,
            NoiseType = NoiseTypeEnum.Perlin,
            FractalType = FractalTypeEnum.Ridged,
        };

        public static Gradient Gradient() => new()
        {
        };

        public static NoiseTexture2D NormalTexture(Noise noise, int size, Gradient gradient = default) => NoiseTexture(noise, size, gradient, normal: true);
        public static NoiseTexture2D NoiseTexture(Noise noise, int size, Gradient gradient = default, bool normal = false) => new()
        {
            Noise = noise,
            Width = size,
            Height = size,
            Normalize = false,
            ColorRamp = gradient,
            AsNormalMap = normal,
            GenerateMipmaps = false,
        };

        public static StandardMaterial3D Material(Image texture, bool blend = true, bool enhanced = false) => Material(ImageTexture.CreateFromImage(texture), null, blend, enhanced);
        public static StandardMaterial3D Material(Image texture, Image normals, bool blend = true, bool enhanced = false) => Material(ImageTexture.CreateFromImage(texture), ImageTexture.CreateFromImage(normals), blend, enhanced);
        public static StandardMaterial3D Material(Texture2D texture, Texture2D normals = null, bool blend = true, bool enhanced = false) => new()
        {
            TextureRepeat = false,
            AlbedoTexture = texture,
            NormalTexture = normals,
            TextureFilter = blend
                ? enhanced ? BaseMaterial3D.TextureFilterEnum.LinearWithMipmapsAnisotropic : BaseMaterial3D.TextureFilterEnum.LinearWithMipmaps
                : enhanced ? BaseMaterial3D.TextureFilterEnum.NearestWithMipmapsAnisotropic : BaseMaterial3D.TextureFilterEnum.NearestWithMipmaps,
        };

        //public static Texture2DArray TextureArray(IEnumerable<Texture2D> textures) => TextureArray(textures.Select(x => x.GetImage()));
        //public static Texture2DArray TextureArray(IEnumerable<Image> images)
        //{
        //    //Debug.Assert(images.DistinctBy(x => x.GetSize()).Count() is 1);
        //    //Debug.Assert(images.DistinctBy(x => x.GetFormat()).Count() is 1);

        //    var ret = new Texture2DArray();
        //    ret.CreateFromImages(new Array<Image>(images)).Throw();
        //    return ret;
        //}

        public static MultiMesh MultiMesh(Mesh source, int count, bool data = false, bool colors = false, int visible = -1) => new()
        {
            Mesh = source,
            UseColors = colors,
            UseCustomData = data,
            TransformFormat = TransformFormatEnum.Transform3D,
            VisibleInstanceCount = visible,
            InstanceCount = count,
        };

        public static ShaderMaterial ShaderMaterial<T>(string part = null) where T : GodotObject => new()
        {
            Shader = GD.Load<Shader>(App.GetShaderPath<T>(part)),
        };

        public static ShaderMaterial ShaderMaterial(string code = "") => new()
        {
            Shader = new Shader { Code = code },

        };
    }
}
