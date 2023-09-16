using System.Diagnostics;
using Godot;
using GodotSharp.BuildingBlocks;

namespace Game
{
    [SceneTree]
    public partial class Main : Game3D
    {
        [GodotOverride]
        private void OnReady()
        {
            InitPlayer();
            InitVehicles();

            void InitPlayer()
            {
                MainMenu.PlayerNameChanged += MainPlayer.SetPlayerName;
                MainMenu.PlayerColorChanged += MainPlayer.SetPlayerColor;
            }

            void InitVehicles()
                => Vehicles.Initialise(Terrain, Camera);

        }

        [GodotOverride]
        private void OnUnhandledKeyInput(InputEvent e)
            => HandleDebugInput(e as InputEventKey);

        [Conditional("DEBUG")]
        private void HandleDebugInput(InputEventKey e)
        {
            if (this.Handle(e, Key.F12, ToggleTerrainMesh)) return;

            void ToggleTerrainMesh()
                => Terrain.Mesh.Visible = !Terrain.Mesh.Visible;
        }

        public override partial void _Ready();
        public override partial void _UnhandledKeyInput(InputEvent e);
    }
}
