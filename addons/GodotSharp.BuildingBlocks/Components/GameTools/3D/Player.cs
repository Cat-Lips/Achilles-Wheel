// https://github.com/godotengine/godot/pull/73873/
// https://www.youtube.com/watch?v=A3HLeyaBCq4

using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class Player : CharacterBody3D, IActive
    {
        public bool Active { get; set; }
    }
}
