using Godot;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    public static class TerrainPosition
    {
        private static readonly GlobalShaderParam<Vector3> param = new();

        public static void Add() => param.Add("vec3");
        public static void Remove() => param.Remove();
        public static void Set(in Vector3 pos) => param.Set(pos);
    }
}
