using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class ImageExtensions
    {
        public static IEnumerable<Color> Colors(this Image source)
        {
            var bmp = new Bitmap();
            bmp.CreateFromImageAlpha(source);
            var size = bmp.GetSize();

            for (var x = 0; x < size.X; ++x)
            {
                for (var y = 0; y < size.Y; ++y)
                {
                    if (bmp.GetBit(x, y))
                        yield return source.GetPixel(x, y);
                }
            }
        }
    }
}
