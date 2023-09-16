using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class ScrollContainerExtensions
    {
        public static void ScrollToEnd(this ScrollContainer source)
            => source.CallDeferred(() => source.ScrollVertical = (int)source.GetVScrollBar().MaxValue);
    }
}
