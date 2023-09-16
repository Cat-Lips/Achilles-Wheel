using Godot;

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class FlowMenu : Container
    {
        private Control Parent;
        private Popup[] PopupMenus;
        private FlowMenu[] SisterMenus;

        public void ShowMenuItems()
        {
            MenuItems.Visible = true;
            SisterMenus.ForEach(x => x.HideMenuItems());
        }

        public void HideMenuItems()
            => MenuItems.Visible = false;

        public bool IsShowingMenuItems()
            => MenuItems.Visible;

        [GodotOverride]
        private void OnReady()
        {
            HideMenuItems();

            Parent = GetParent<Control>();
            PopupMenus = MenuItems.GetChildren()
                .Where(x => x.HasMethod("get_popup"))
                .Select(x => (Popup)x.Call("get_popup"))
                .ToArray();
            SisterMenus = Parent.GetChildren<FlowMenu>()
                .Where(x => x != this)
                .ToArray();
        }

        [GodotOverride]
        private void OnProcess(double _)
        {
            if (IsShowingMenuItems())
                HideMenuItemsOnMouseExit();
            else
                ShowMenuItemsOnMouseOver();

            void HideMenuItemsOnMouseExit()
            {
                if (Parent.IsMouseOver()) return;
                if (PopupMenus.Any(x => x.IsMouseOver())) return;
                if (this.RecurseChildren<Control>().Any(x => x.HasFocus())) return;

                HideMenuItems();
            }

            void ShowMenuItemsOnMouseOver()
            {
                if (MenuLabel.IsMouseOver())
                    ShowMenuItems();
            }
        }

        public override partial void _Ready();
        public override partial void _Process(double _);
    }
}
