using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static partial class App
    {
        public static string GetResPath<T>(string ext) where T : GodotObject => GetResPath<T>(null, ext);
        public static string GetResPath<T>(string part, string ext) where T : GodotObject
            => GetScriptPath<T>().Replace(".cs", part is null ? $".{ext}" : $".{part}.{ext}");

        public static string GetShaderPath<T>(string part = null) where T : GodotObject
            => GetResPath<T>(part, "gdshader");

        public static string GetShaderIncPath<T>(string part = null) where T : GodotObject
            => GetResPath<T>(part, "gdshaderinc");
    }
}
