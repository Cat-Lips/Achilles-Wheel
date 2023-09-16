using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static partial class App
    {
        public static readonly string Name
            = ProjectSettings.GetSetting("application/config/name").AsString();

        public static readonly int Hash = GD.Hash(Name);

        public static readonly float DefaultGravity
            = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    }
}
