using Godot;

namespace GodotSharp.BuildingBlocks
{
    [Tool, SceneTree]
    public partial class ResourceEditor : Accordion
    {
        [Notify]
        public Resource Resource
        {
            get => _resource.Get();
            set => _resource.Set(value);
        }

        [GodotOverride]
        private void OnReady()
        {
            InitEditor();
            _resource.Changed += InitEditor;

            void InitEditor()
            {
                Clear();
                CreateContent(Resource);

                void CreateContent(Resource resource)
                {
                    if (resource is null) return;

                    AddGroup(resource.ResourceName, resource.GetEditControls().ToArray());

                    foreach (var subResource in resource.GetSubResources())
                        CreateContent(subResource);
                }
            }
        }

        public override partial void _Ready();
    }
}
