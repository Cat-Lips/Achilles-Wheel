using Godot;

// https://gamedevacademy.org/vehiclewheel3d-in-godot-complete-guide/

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class Vehicle : VehicleBody3D, IActive
    {
        public bool Active { get; set; }
    }
}
