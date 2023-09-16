using Godot;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    public partial class TerrainCollider : CollisionShape3D
    {
        private Node3D source;
        private Vector3 halfSize;
        private Action UpdateShape;

        private Vector3 GetSourcePosition()
            => source.Position.TrueRound().XZ(0);

        [OnInstantiate]
        private void Initialise(ITerrain terrain, Node3D source, int size)
        {
            this.source = source;
            Name = $"Collider ({source.Name})";

            Ready += InitSourceHeight;

            //if (InitHeightMap(Shape as HeightMapShape3D)) return; // Broken? (heights don't match terrain)
            if (InitPolygon(Shape as ConcavePolygonShape3D)) return;

            var defaultShape = new ConcavePolygonShape3D();
            InitPolygon(defaultShape);
            Shape = defaultShape;

            //bool InitHeightMap(HeightMapShape3D shape)
            //{
            //    if (shape is null) return false;

            //    halfSize = MathV.Vec3(size / 2);
            //    var heights = new float[size * size];
            //    shape.MapWidth = shape.MapDepth = size;

            //    this.UpdateShape = UpdateShape;

            //    return true;

            //    void UpdateShape()
            //    {
            //        var gpos = Position;

            //        for (var x = 0; x < size; ++x)
            //        {
            //            for (var y = 0; y < size; ++y)
            //            {
            //                var hpos = new Vector2(gpos.X + x, gpos.Y + y);
            //                heights[y * size + x] = terrain.GetHeight(hpos);
            //            }
            //        }

            //        shape.MapData = heights;
            //    }
            //}

            bool InitPolygon(ConcavePolygonShape3D shape)
            {
                if (shape is null) return false;

                var mesh = Default.PlaneMesh(size);
                var vertices = mesh.GetFaces();

                this.UpdateShape = UpdateShape;

                return true;

                void UpdateShape()
                {
                    var gpos = Position;

                    for (var i = 0; i < vertices.Length; ++i)
                    {
                        var hpos = (gpos + vertices[i]).XZ();
                        vertices[i].Y = terrain.GetHeight(hpos);
                    }

                    shape.Data = vertices;
                }
            }

            void InitSourceHeight()
            {
                var pos = source.Position;
                var size = source.GetAabb().Size.Y;
                var height = terrain.GetHeight(pos.XZ());
                if (pos.Y - size < height)
                {
                    pos.Y = height + size;
                    source.Position = pos;
                }
            }
        }

        [GodotOverride]
        private void OnReady()
        {
            Position = GetSourcePosition();
            UpdateShape();
        }

        [GodotOverride]
        private void OnPhysicsProcess(double delta)
        {
            var gpos = GetSourcePosition();

            if (Position != gpos)
            {
                Position = gpos;
                UpdateShape();
            }
        }

        public override partial void _Ready();
        public override partial void _PhysicsProcess(double delta);
    }
}
