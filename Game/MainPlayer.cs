using Godot;
using GodotSharp.BuildingBlocks;

namespace Game
{
    [SceneTree]
    public partial class MainPlayer : Player
    {
        public void SetPlayerName(string x) => Name = x;
        public void SetPlayerColor(Color x) => ((StandardMaterial3D)_.MeshInstance3D.GetActiveMaterial(0)).AlbedoColor = x;
    }
}
