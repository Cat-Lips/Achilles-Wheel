using Godot;

namespace GodotSharp.BuildingBlocks
{
    [Tool]
    public partial class CustomResource : Resource
    {
        public new event Action Changed;
        private readonly AutoAction OnChanged = new();

        public bool HasChanged => OnChanged.Triggered;

        public CustomResource()
        {
            base.Changed += OnChanged.Run;
            OnChanged.Action = () => Changed?.Invoke();
        }
    }

    //[Tool]
    //public partial class CustomResource : Resource
    //{
    //    private bool changed = true;

    //    public CustomResource()
    //        => Changed += () => changed = true;

    //    public bool HasChanged(bool reset = false)
    //        => reset ? Reset() : changed;

    //    public bool Reset()
    //    {
    //        var ret = changed;
    //        changed = false;
    //        return ret;
    //    }
    //}
}
