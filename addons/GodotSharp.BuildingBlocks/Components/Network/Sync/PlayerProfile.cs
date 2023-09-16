using Godot;

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class PlayerProfile : Node
    {
        public string PlayerName { get; set; }
        public Color PlayerColor { get; set; }

        public void OnReady(Action action)
            => _.Sync.OnReady(action);

        [GodotOverride]
        private void OnReady()
        {
            _.Sync.Add(PlayerName);
            _.Sync.Add(PlayerColor);
        }

        public override partial void _Ready();
    }
}
