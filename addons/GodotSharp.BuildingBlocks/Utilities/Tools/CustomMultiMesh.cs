using Godot;

namespace GodotSharp.BuildingBlocks
{
    [Tool]
    public partial class CustomMultiMesh : MultiMeshInstance3D
    {
        //private PackedScene[] _sources = [];

        //[Export] private PackedScene[] Sources { get => _sources; set => this.Set(ref _sources, value ?? [], UpdateScene.Run); }

        //private readonly AutoAction UpdateScene = new();

        //public override void _Ready()
        //{
        //    UpdateScene();
        //    this.UpdateScene.Action = UpdateScene;

        //    void UpdateScene()
        //    {
        //        this.RemoveChildren(free: true);

        //        var surface = GetSurface();
        //        if (surface is null) return;

        //        foreach (var mesh in GetSources())
        //        {
        //            var multiMesh = Default.MultiMesh(surface, mesh);
        //            AddChild(multiMesh);
        //        }

        //        MeshInstance3D GetSurface()
        //        {
        //            return GetParentOrNull<MeshInstance3D>() ?? GetEditorSurface();

        //            MeshInstance3D GetEditorSurface()
        //            {
        //                if (!Engine.IsEditorHint()) return null;

        //                var surface = new MeshInstance3D
        //                {
        //                    Mesh = Default.PlaneMesh(10)
        //                };

        //                AddChild(surface);
        //                return surface;
        //            }
        //        }

        //        IEnumerable<Mesh> GetSources()
        //            => Sources.SelectMany(x => x.Instantiate<Node3D>().GetChildren<MeshInstance3D>()).Select(x => x.Mesh);
        //    }
        //}
    }
}
