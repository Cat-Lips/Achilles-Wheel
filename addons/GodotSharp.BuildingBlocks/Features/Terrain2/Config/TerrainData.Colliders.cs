using Godot;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    public partial class TerrainData
    {
        public void AddCollider(PhysicsBody3D source, int size = -1)
        {
            if (source is null) return;
            if (Engine.IsEditorHint()) return;

            size = GetSize();
            TerrainCollider collider = null;

            if (source.IsInsideTree()) Add();
            source.Connect("tree_entered", Add);
            source.Connect("tree_exiting", Remove);

            void Add() => terrain.Body.AddChild(collider = TerrainCollider.Instantiate(terrain, source, size));
            void Remove() => terrain.Body.RemoveChild(collider, free: true);

            int GetSize()
            {
                return size > 0 ? size
                     : size < 0 ? AutoSize()
                     : DefaultSize;

                int AutoSize()
                {
                    var minSize = (int)Math.Ceiling(source.GetAabb().GetLongestAxisSize());
                    return minSize is 0 ? DefaultSize : minSize + 2;
                }
            }
        }
    }
}
