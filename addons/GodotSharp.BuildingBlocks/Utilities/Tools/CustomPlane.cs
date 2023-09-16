using Godot;

namespace GodotSharp.BuildingBlocks
{
    [Tool]
    public partial class CustomPlane : CustomMesh
    {
        private int _size = 0;

        private new PlaneMesh Mesh => (PlaneMesh)base.Mesh;
        protected CustomPlane() => base.Mesh = Default.PlaneMesh(Size);

        [Export]
        private int Size { get => _size; set => SetSize(value); }

        [Export]
        public Material Material { get => Mesh.Material; set => Mesh.Material = value; }

        public void SetSize(int size, int lod = 0)
        {
            _size = Mathf.NearestPo2(size);
            Mesh.Size = Vector2.One * Size;

            SetLod(lod);
        }

        public void SetLod(int lod)
        {
            lod = lod <= 0
                ? Size - 1
                : Size / (int)Math.Pow(2, lod) - 1;

            Mesh.SubdivideDepth = lod;
            Mesh.SubdivideWidth = lod;
        }
    }
}
