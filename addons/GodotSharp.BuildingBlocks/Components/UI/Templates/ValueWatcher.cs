using Godot;
using WatchTarget = (Godot.Label Label, Godot.RichTextLabel Value, System.Func<Godot.Variant> GetValue);

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class ValueWatcher : Root
    {
        private Dictionary<string, WatchTarget> WatchTargets { get; } = [];

        public static ValueWatcher Instance { get; private set; }

        public ValueWatcher()
        {
            TreeEntered += () => Instance = this;
            TreeExiting += () => Instance = null;
        }

        public void Add(string key, Func<Variant> valueProvider)
        {
            var label = new Label { Name = $"{key}Label", Text = key };
            var value = new RichTextLabel { Name = $"{key}Value", FitContent = true, AutowrapMode = TextServer.AutowrapMode.Off };
            value.Resized += () => value.CustomMinimumSize = MathV.Max(value.CustomMinimumSize, value.Size);

            Visible = true;
            Content.AddChild(label);
            Content.AddChild(value);
            WatchTargets.Add(key, (label, value, valueProvider));
        }

        public void Remove(string key)
        {
            if (WatchTargets.Remove(key, out var entry))
            {
                Visible = WatchTargets.Count is not 0;
                Content.RemoveChild(entry.Label, free: true);
                Content.RemoveChild(entry.Value, free: true);
            }
        }

        public void Clear()
        {
            while (WatchTargets.Count is not 0)
                Remove(WatchTargets.Keys.First());
        }

        [GodotOverride]
        private void OnProcess(double _)
            => WatchTargets.Values.ForEach(x => x.Value.Text = $"{x.GetValue()}");

        public override partial void _Process(double _);
    }
}
