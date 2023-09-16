using Godot;

namespace GodotSharp.BuildingBlocks
{
    [Tool, SceneTree]
    public partial class AccordionTitle : HBoxContainer
    {
        [GodotOverride]
        private void OnReady()
            => Title.Text = $">>> {Name.ToString().Capitalize()}";

        public override partial void _Ready();
    }
}
