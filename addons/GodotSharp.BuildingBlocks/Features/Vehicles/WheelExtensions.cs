using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static partial class WheelExtensions
    {
        public static bool IsBack(this VehicleWheel3D x) => x.Position.Z >= 0;
        public static bool IsFront(this VehicleWheel3D x) => x.Position.Z <= 0;
        public static bool Is(this VehicleWheel3D x, WheelAction type) => type switch
        {
            WheelAction.All => true,
            WheelAction.Back => x.IsBack(),
            WheelAction.Front => x.IsFront(),
            _ => throw new NotImplementedException(),
        };
    }
}
