using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class CameraTracking
    {
        public enum Scope { Tree, Camera };

        private static readonly Dictionary<Camera3D, Dictionary<Node3D, (Control Icon, Vector2 Inset)>> TrackedCameraItems = [];

        public static void Track(this Camera3D camera, Node3D item)
            => camera.Track(item, UI.Label(item.Name));

        public static void Track(this Camera3D camera, Node3D item, in Color color)
        {
            var icon = UI.Label(item.Name);
            icon.SetFontColor(color);
            camera.Track(item, icon);
        }

        public static void Track(this Camera3D camera, Node3D item, Control icon, Scope scope = Scope.Tree)
        {
            if (!TrackedCameraItems.TryGetValue(camera, out var items))
                TrackedCameraItems.Add(camera, items = []);

            icon.ResetSize();
            var inset = icon.Size * .5f;
            icon = Center(icon);

            SetScope();
            SetVisibility();
            item.VisibilityChanged += SetVisibility;

            void SetScope()
            {
                switch (scope)
                {
                    case Scope.Tree:
                        item.TreeExiting += () => { HideIcon(); icon.QueueFree(); };
                        break;
                    case Scope.Camera:
                        item.TreeEntered += ShowIcon;
                        item.TreeExiting += HideIcon;
                        if (item.IsInsideTree()) ShowIcon();
                        camera.TreeExiting += icon.QueueFree;
                        break;
                }
            }

            void SetVisibility() { if (item.IsVisibleInTree()) ShowIcon(); else HideIcon(); }
            void ShowIcon() { items.Add(item, (icon, inset)); camera.AddChild(icon); }
            void HideIcon() { items.Remove(item); camera.RemoveChild(icon); }
        }

        public static void OnTrackingUpdate(this Camera3D camera, float sqrNearClip)
        {
            if (!TrackedCameraItems.TryGetValue(camera, out var items))
                return;

            var bounds = camera.GetViewport().GetVisibleRect();

            foreach (var kvp in items)
            {
                var item = kvp.Key;

                var pos = item.Position;
                if (!camera.IsPositionBehind(pos))
                {
                    var icon = kvp.Value.Icon;
                    var inset = kvp.Value.Inset;
                    var clamp = bounds.GrowIndividual(-inset.X, -inset.Y, -inset.X, -inset.Y);

                    icon.Position = camera
                        .UnprojectPosition(pos)
                        .Clamp(clamp.Position, clamp.End);

                    icon.Visible = camera.Position.DistanceSquaredTo(pos) > sqrNearClip;
                }
            }
        }

        private static CenterContainer Center(Control icon)
        {
            var container = new CenterContainer { UseTopLeft = true };
            container.AddChild(icon);
            return container;
        }
    }
}
