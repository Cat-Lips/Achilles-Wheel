using Godot;
using Array = Godot.Collections.Array;

namespace GodotSharp.BuildingBlocks
{
    public static partial class Default
    {
        public static Mesh Grass(Material material = null)
        {
            var verts = new Vector3[]
            {
                new(-.5f, 0, 0),
                new(.5f, 0, 0),
                new(0, 1, 0)
            };

            var uv = new Vector2[]
            {
                new(0, 0),
                new(0, 0),
                new(1, 1)
            };

            var arrays = new Array();
            arrays.Resize((int)Mesh.ArrayType.Max);
            arrays[(int)Mesh.ArrayType.Vertex] = verts;
            arrays[(int)Mesh.ArrayType.TexUV2] = uv;

            var mesh = new ArrayMesh();
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
            if (material is not null) mesh.SurfaceSetMaterial(0, material);
            mesh.CustomAabb = new(new(-.5f, 0, -.5f), new(1, 1, 1));
            return mesh;
        }
    }
}
