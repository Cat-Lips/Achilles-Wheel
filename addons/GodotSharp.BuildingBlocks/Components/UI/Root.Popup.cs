using System.Diagnostics;
using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class Root
    {
        [Export] private bool Popup { get; set; }

        private void InitPopup()
        {
            InitPopup();
            InitInput();

            void InitPopup()
            {
                if (!Popup) return;

                Root[] visibleItems = null;
                VisibilityChanged += OnVisibilityChanged;

                void OnVisibilityChanged()
                {
                    if (IsVisibleInTree()) OnPopupOpen();
                    else OnPopupClose();

                    void OnPopupOpen()
                    {
                        visibleItems = GetVisibleItems();
                        visibleItems.ForEach(x => x.Hide());

                        Root[] GetVisibleItems()
                        {
                            return GetParent()
                                .GetChildren<Root>()
                                .Except(new[] { this })
                                .Where(x => x.IsVisibleInTree()).ToArray();
                        }
                    }

                    void OnPopupClose()
                    {
                        visibleItems.ForEach(x => x.Show());
                        visibleItems = null;
                    }
                }
            }

            void InitInput()
            {
                SetProcessUnhandledInput(false);

                if (!Popup) return;
                VisibilityChanged += () => SetProcessUnhandledInput(IsVisibleInTree());
            }
        }

        [GodotOverride]
        private void OnUnhandledInput(InputEvent e)
        {
            Debug.Assert(Popup);
            Debug.Assert(IsVisibleInTree());

            ConsumeAllInput();
            if (CriticalKeyHit()) Hide();

            void ConsumeAllInput()
                => GetViewport().SetInputAsHandled();

            bool CriticalKeyHit()
            {
                return e.IsActionPressed("ui_cancel") ||
                    e is InputEventMouseButton mouse && mouse.ButtonMask.HasFlag(MouseButtonMask.Left);
            }
        }

        public override partial void _UnhandledInput(InputEvent e);
    }
}
