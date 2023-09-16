using Godot;
using GodotSharp.BuildingBlocks;
using GodotSharp.BuildingBlocks.Terrain2;

namespace Game
{
    [SceneTree]
    public partial class Vehicles : Node
    {
        public void Initialise(Terrain terrain, Camera camera)
        {
            InitCamera();
            InitVehicles();

            void InitCamera()
            {
                camera.ItemSelected += OnItemSelected;

                void OnItemSelected(CollisionObject3D target)
                {
                    if (target.GetParent() == this)
                        camera.Target = target;
                }
            }

            void InitVehicles()
            {
                this.ForEachChild<PhysicsBody3D>(InitVehicle);

                void InitVehicle(PhysicsBody3D vehicle)
                    => terrain.AddCollider(vehicle);
            }
        }
    }
}
