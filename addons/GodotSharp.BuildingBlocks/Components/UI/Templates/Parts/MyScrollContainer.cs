using Godot;

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class MyScrollContainer : ScrollContainer
    {
        [GodotOverride]
        private void OnReady()
        {
            InitMargin();
            InitContent();

            void InitMargin()
            {
                var vScroll = GetVScrollBar();
                var hScroll = GetHScrollBar();

                SetMargin();
                vScroll.VisibilityChanged += SetMargin;
                hScroll.VisibilityChanged += SetMargin;

                void SetMargin()
                {
                    var vMargin = vScroll.Visible ? vScroll.Size.X : 0;
                    var hMargin = hScroll.Visible ? hScroll.Size.Y : 0;
                    _.Margin.SetMargin(0, 0, (int)vMargin, (int)hMargin);
                }
            }

            void InitContent()
            {
                FitContent();
                _.Margin.Resized += FitContent;

                void FitContent()
                {
                    CustomMinimumSize = default;

                    var screenSize = GetViewportRect().Size;
                    var parentSize = GetOwner<Control>().Size;
                    CustomMinimumSize = _.Margin.Size.Clamp(screenSize - parentSize);
                }
            }
        }

        public override partial void _Ready();
    }
}
